using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AspDotNet.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AspDotNet.Controllers
{
    [Route( "ws" )]
    public class ChatController : Controller
    {
        // Shared static list for all connected clients
        private static readonly List<WebSocket> _clients = new();
        private static readonly object _lock = new(); // for thread-safe access

        [HttpGet]
        public async Task Get()
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                HttpContext.Response.StatusCode = 400;
                return;
            }

            // Accept WebSocket connection
            var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

            // Add client safely
            lock (_lock)
            {
                _clients.Add( webSocket );
                Console.WriteLine( $"✅ Client connected. Total clients: {_clients.Count}" );
            }

            var buffer = new byte[1024 * 4];

            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult result;

                    try
                    {
                        result = await webSocket.ReceiveAsync( new ArraySegment<byte>( buffer ), CancellationToken.None );
                    }
                    catch
                    {
                        break;
                    }

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync( WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None );
                        break;
                    }

                    var messageJson = Encoding.UTF8.GetString( buffer, 0, result.Count );

                    ChatMessageModel chatMessage;
                    try
                    {
                        chatMessage = JsonConvert.DeserializeObject<ChatMessageModel>( messageJson );
                        if (chatMessage == null) continue;
                    }
                    catch
                    {
                        Console.WriteLine( "⚠️ JSON Error: Failed to parse message" );
                        continue;
                    }

                    var broadcastBytes = Encoding.UTF8.GetBytes( JsonConvert.SerializeObject( chatMessage ) );

                    // Copy clients safely for broadcasting
                    List<WebSocket> clientsCopy;
                    lock (_lock)
                    {
                        clientsCopy = _clients.ToList();
                    }

                    var disconnectedClients = new List<WebSocket>();

                    // Broadcast outside lock
                    foreach (var client in clientsCopy)
                    {
                        if (client.State != WebSocketState.Open)
                        {
                            disconnectedClients.Add( client );
                            continue;
                        }

                        try
                        {
                            await client.SendAsync(
                                new ArraySegment<byte>( broadcastBytes ),
                                WebSocketMessageType.Text,
                                true,
                                CancellationToken.None
                            );
                        }
                        catch
                        {
                            disconnectedClients.Add( client );
                        }
                    }

                    // Remove disconnected clients safely
                    lock (_lock)
                    {
                        foreach (var dc in disconnectedClients)
                        {
                            _clients.Remove( dc );
                            Console.WriteLine( "❌ Removed disconnected client" );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine( $"⚠️ WebSocket Error: {ex.Message}" );
                HttpContext.Response.StatusCode = 500;
            }
            finally
            {
                // Remove this client on disconnect
                lock (_lock)
                {
                    _clients.Remove( webSocket );
                    Console.WriteLine( $"❌ Client disconnected. Total clients: {_clients.Count}" );
                }
            }
        }
    }
}

using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;

namespace AspDotNet.Services
{
    public class EmailService
    {
        private readonly string _email = "devairdit@gmail.com";
        private readonly string _password = "wshm ajtv cjnj tsdx";
        private readonly string _imapServer = "imap.gmail.com";
        private readonly int _imapPort = 993;

        public async Task<List<MimeMessage>> FetchRecentEmailsAsync(int count = 10)
        {
            var emails = new List<MimeMessage>();

            using var client = new ImapClient();

            // 1. Connect & Authenticate
            await client.ConnectAsync(_imapServer, _imapPort, SecureSocketOptions.SslOnConnect);
            await client.AuthenticateAsync(_email, _password);
            await client.Inbox.OpenAsync(MailKit.FolderAccess.ReadOnly);

            // 2. Get total messages and compute range to fetch only 'count' latest
            int total = client.Inbox.Count;
            int start = Math.Max(0, total - count);

            // 3. Fetch only required message summaries (just headers to sort)
            var summaries = await client.Inbox.FetchAsync(start, -1, MailKit.MessageSummaryItems.Envelope | MailKit.MessageSummaryItems.UniqueId);

            // 4. Sort by Date descending and get top 'count' UniqueIds
            var sortedUids = summaries
                .OrderByDescending(m => m.Envelope.Date)
                .Take(count)
                .Select(m => m.UniqueId)
                .ToList();

            // 5. Now fetch full message bodies for those UIDs
            foreach (var uid in sortedUids)
            {
                var message = await client.Inbox.GetMessageAsync(uid);
                emails.Add(message);
            }

            // 6. Disconnect and return
            await client.DisconnectAsync(true);
            return emails;
        }


        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Airdit Software", "devairdit@gmail.com"));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_email, _password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}

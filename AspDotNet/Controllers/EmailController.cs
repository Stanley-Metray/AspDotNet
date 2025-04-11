using System;
using AspDotNet.Services;
using Microsoft.AspNetCore.Mvc;
using AspDotNet.Models;


namespace AspDotNet.Controllers
{
    [Route("email")]
    public class EmailController : Controller
    {
        private readonly EmailService _emailService;

        public EmailController()
        {
            _emailService = new EmailService();
        }

        [HttpGet("inbox")]
        public async Task<IActionResult> InboxPage()
        {
            var emails = await _emailService.FetchRecentEmailsAsync();
            var result = emails.Select(e => new EmailModel
            {
                From = e.From.ToString(),
                Subject = e.Subject,
                Body = e.TextBody ?? e.HtmlBody,
                Date = e.Date.LocalDateTime.ToString()
            }).ToList();

            return View(result);
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestEmail()
        {
            var emails = await _emailService.FetchRecentEmailsAsync(1);
            var latest = emails.FirstOrDefault();

            if (latest == null) return NoContent();

            return Ok(new
            {
                from = latest.From.ToString(),
                subject = latest.Subject,
                body = latest.TextBody ?? latest.HtmlBody,
                date = latest.Date.LocalDateTime.ToString()
            });
        }


        [HttpGet("send")]
        public IActionResult SendEmailPage()
        {
            return View();
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromForm] string to, [FromForm] string subject, [FromForm] string body)
        {
            try
            {
                await _emailService.SendEmailAsync(to, subject, body);
                return Ok(new { success = true, message = "Email sent successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }

}
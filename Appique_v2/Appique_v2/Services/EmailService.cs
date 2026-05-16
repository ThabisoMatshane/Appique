using System.Net;
using System.Net.Mail;
using Appique_v2.Models;
using Microsoft.Extensions.Options;

namespace Appique_v2.Services
{
    public interface IEmailService
    {
        Task SendContactEmailAsync(ContactFormModel form);
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task SendContactEmailAsync(ContactFormModel form)
        {
            var subject = $"New enquiry from {form.Name}" +
                          (string.IsNullOrWhiteSpace(form.Company) ? "" : $", {form.Company}");

            var body = BuildEmailBody(form);

            using var message = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            // Send to the Appique inbox
            message.To.Add(new MailAddress(_settings.SenderEmail, _settings.SenderName));

            // Reply-To so we can reply directly to the enquirer
            message.ReplyToList.Add(new MailAddress(form.Email, form.Name));

            using var client = new SmtpClient(_settings.SmtpServer, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                EnableSsl = true,   // STARTTLS on port 587
                DeliveryMethod = SmtpDeliveryMethod.Network,
            };

            await client.SendMailAsync(message);
            _logger.LogInformation("Contact email sent from {Email}", form.Email);
        }

        private static string BuildEmailBody(ContactFormModel form)
        {
            var rows = new List<(string Label, string? Value)>
            {
                ("Name",    form.Name),
                ("Email",   form.Email),
                ("Phone",   form.Phone),
                ("Company", form.Company),
                ("Service", form.Service),
            };

            var detailsHtml = string.Join("", rows
                .Where(r => !string.IsNullOrWhiteSpace(r.Value))
                .Select(r => $"""
                    <tr>
                        <td style="padding:6px 12px;font-weight:600;color:#556280;width:100px;
                                   font-family:'DM Sans',Arial,sans-serif;font-size:13px;
                                   border-right:2px solid #e2d9c5;">{r.Label}</td>
                        <td style="padding:6px 12px;color:#0d1f3c;
                                   font-family:'DM Sans',Arial,sans-serif;font-size:13px;">{System.Web.HttpUtility.HtmlEncode(r.Value)}</td>
                    </tr>
                """));

            return $"""
                <!DOCTYPE html>
                <html lang="en">
                <head><meta charset="utf-8"/></head>
                <body style="margin:0;padding:0;background:#f0e9d8;font-family:'DM Sans',Arial,sans-serif;">
                  <table width="100%" cellpadding="0" cellspacing="0" style="background:#f0e9d8;padding:40px 20px;">
                    <tr><td align="center">
                      <table width="600" cellpadding="0" cellspacing="0"
                             style="max-width:600px;background:#fffcf7;border-radius:16px;
                                    overflow:hidden;border:1px solid #e2d9c5;">

                        <!-- Header -->
                        <tr>
                          <td style="background:#07204a;padding:28px 32px;">
                            <span style="font-family:Georgia,serif;font-size:22px;font-weight:700;color:#fff;">
                              Appique
                            </span>
                            <span style="font-size:13px;color:rgba(255,255,255,0.6);margin-left:12px;">
                              New website enquiry
                            </span>
                          </td>
                        </tr>

                        <!-- Sender details -->
                        <tr>
                          <td style="padding:28px 32px 0;">
                            <p style="margin:0 0 16px;font-size:14px;color:#556280;">
                              You have received a new project enquiry via the Appique website.
                            </p>
                            <table width="100%" cellpadding="0" cellspacing="0"
                                   style="border:1px solid #e2d9c5;border-radius:10px;overflow:hidden;">
                              {detailsHtml}
                            </table>
                          </td>
                        </tr>

                        <!-- Message -->
                        <tr>
                          <td style="padding:24px 32px;">
                            <p style="margin:0 0 8px;font-size:12px;font-weight:600;
                                      color:#556280;text-transform:uppercase;letter-spacing:0.1em;">
                              Project details
                            </p>
                            <div style="background:#faf6ee;border:1px solid #e2d9c5;border-radius:10px;
                                        padding:16px;font-size:14px;color:#0d1f3c;line-height:1.7;
                                        white-space:pre-wrap;">{System.Web.HttpUtility.HtmlEncode(form.Message)}</div>
                          </td>
                        </tr>

                        <!-- CTA -->
                        <tr>
                          <td style="padding:0 32px 28px;">
                            <a href="mailto:{form.Email}"
                               style="display:inline-block;background:linear-gradient(135deg,#c9973a,#e4b96b);
                                      color:#07204a;padding:12px 24px;border-radius:100px;
                                      font-size:14px;font-weight:600;text-decoration:none;">
                              Reply to {System.Web.HttpUtility.HtmlEncode(form.Name)} ↗
                            </a>
                          </td>
                        </tr>

                        <!-- Footer -->
                        <tr>
                          <td style="background:#f0e9d8;padding:16px 32px;
                                     border-top:1px solid #e2d9c5;font-size:11px;color:#a8b0c2;">
                            Appique (Pty) Ltd · Johannesburg, South Africa ·
                            <a href="https://appique.co.za" style="color:#c9973a;text-decoration:none;">
                              appique.co.za
                            </a>
                          </td>
                        </tr>

                      </table>
                    </td></tr>
                  </table>
                </body>
                </html>
            """;
        }
    }
}

using System.Globalization;
using System.Net.Mail;
using System.Net;
using TechGear.AuthService.Interfaces;

namespace TechGear.AuthService.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendOtpEmailAsync(string toEmail, int otp)
        {
            string from = "vixtruong1001@gmail.com";
            string pass = "hktd tdau doec jjdh";
            string to = toEmail;

            string messageBody = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
                            <div style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 10px; overflow: hidden;'>
                                <div style='background-color: #000000; color: white; padding: 20px; text-align: center;'>
                                    <h1>Tech Gear OTP</h1>
                                </div>
                                <div style='padding: 20px; text-align: center;'>
                                    <h2 style='color: #000000;'>{otp}</h2>
                                </div>
                            </div>
                        </body>
                        </html>";


            var message = new MailMessage
            {
                From = new MailAddress(from, "TechGear"),
                Subject = "TechGear - OTP",
                Body = messageBody,
                IsBodyHtml = true
            };
            message.To.Add(to);

            var smtp = new SmtpClient("smtp.gmail.com")
            {
                EnableSsl = true,
                Port = 587,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(from, pass)
            };

            try
            {
                await smtp.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to send email: " + ex.Message);
            }
        }
    }
}

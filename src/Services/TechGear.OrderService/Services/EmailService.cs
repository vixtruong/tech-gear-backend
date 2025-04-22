using System.Net.Mail;
using System.Net;
using TechGear.OrderService.Data;
using TechGear.OrderService.DTOs;
using TechGear.OrderService.Interfaces;
using System.Globalization;

namespace TechGear.OrderService.Services
{
    public class EmailService(TechGearOrderServiceContext context, IHttpClientFactory httpClientFactory)
        : IEmailService
    {
        public async Task SendOrderConfirmationEmailAsync(OrderEmailDto order)
        {
            string from = "vixtruong1001@gmail.com";
            string pass = "hktd tdau doec jjdh";
            string to = order.Email;

            var culture = new CultureInfo("vi-VN");

            string messageBody = $@"
                <div style='font-family: Arial, sans-serif; background-color: #1c1c1c; color: #ffffff; padding: 20px;'>
                    <h2 style='color: #00c9a7;'>TechGear - Order Confirmation #{order.OrderId}</h2>
                    <p>Hello <strong>{order.CustomerName}</strong>,</p>
                    <p>Thank you for placing your order at <strong>TechGear</strong>!</p>

                    <p><em>If this is your first purchase, your <strong>login information</strong> has been automatically created using <strong>your email</strong> and <strong>your phone number</strong> as the default password. Please log in and change your password as soon as possible.</em></p>

                    <h3 style='color: #00c9a7;'>Order Information</h3>
                    <ul>
                        <li><strong>Order Date:</strong> {order.OrderDate:dd/MM/yyyy HH:mm}</li>
                        <li><strong>Payment Method:</strong> {order.PaymentMethod}</li>
                        <li><strong>Shipping Address:</strong> {order.Address}</li>
                        <li><strong>Phone Number:</strong> {order.PhoneNumber}</li>
                        {(string.IsNullOrWhiteSpace(order.Note) ? "" : $"<li><strong>Note:</strong> {order.Note}</li>")}
                    </ul>

                    <h3 style='color: #00c9a7;'>Order Details</h3>
                    <table style='width: 100%; border-collapse: collapse; margin-top: 10px;'>
                        <thead>
                            <tr style='background-color: #333; color: #ffffff;'>
                                <th style='padding: 10px; border: 1px solid #444;'>Image</th>
                                <th style='padding: 10px; border: 1px solid #444;'>Product</th>
                                <th style='padding: 10px; border: 1px solid #444;'>SKU</th>
                                <th style='padding: 10px; border: 1px solid #444;'>Qty</th>
                                <th style='padding: 10px; border: 1px solid #444;'>Unit Price</th>
                                <th style='padding: 10px; border: 1px solid #444;'>Total</th>
                            </tr>
                        </thead>
                        <tbody>";

                        foreach (var item in order.Items)
                        {
                            messageBody += $@"
                            <tr style='background-color: #2a2a2a;'>
                                <td style='text-align: center; border: 1px solid #444; padding: 6px;'>
                                    <img src='{item.ImageUrl}' width='70' height='70' style='object-fit: contain; border-radius: 4px;' />
                                </td>
                                <td style='border: 1px solid #444; padding: 10px;'>{item.ProductName}</td>
                                <td style='border: 1px solid #444; padding: 10px;'>{item.Sku}</td>
                                <td style='border: 1px solid #444; padding: 10px;'>{item.Quantity}</td>
                                <td style='border: 1px solid #444; padding: 10px;'>{item.UnitPrice.ToString("C0", culture)}</td>
                                <td style='border: 1px solid #444; padding: 10px;'>{item.TotalPrice.ToString("C0", culture)}</td>
                            </tr>";
                        }

                        messageBody += $@"
                        </tbody>
                    </table>

                    <h3 style='color: #00c9a7;'>Payment Summary</h3>
                    <ul>
                        <li><strong>Original Amount:</strong> {order.OriginalAmount.ToString("C0", culture)}</li>
                        <li><strong>Discount:</strong> {order.DiscountValue.ToString("C0", culture)}</li>
                        <li><strong>Used Points:</strong> {(order.UsedPoints * 1000).ToString("C0", culture)}</li>
                        <li><strong>Total Payable:</strong> <span style='color: #00ffb7; font-size: 18px;'>{order.FinalAmount.ToString("C0", culture)}</span></li>
                    </ul>

                    <p style='margin-top: 30px;'>If you have any questions, feel free to contact our support team.</p>
                    <p style='font-size: 13px; color: #888;'>This is an automated message. Please do not reply to this email.</p>
                    <p style='color: #00c9a7; font-weight: bold;'>— TechGear Team</p>
                </div>";

            var message = new MailMessage
            {
                From = new MailAddress(from, "TechGear"),
                Subject = $"TechGear - Order Confirmation #{order.OrderId}",
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

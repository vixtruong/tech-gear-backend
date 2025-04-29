namespace TechGear.AuthService.Interfaces
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(string to, int otp);
    }
}

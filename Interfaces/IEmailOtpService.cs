namespace OtpDemo.Interfaces
{
    public interface IEmailOtpService
    {
        string GenerateOtp(string email);
        bool ValidateOtp(string email, string otp);
        void SendOtpEmail(string email, string otp);
    }
}
using OtpDemo.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace OtpDemo.Repository
{
    public class EmailOtpService : IEmailOtpService
    {
        private readonly Dictionary<string, (string Otp, DateTime GeneratedAt)> _otpStorage = new();

        public string GenerateOtp(string email)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            _otpStorage[email] = (Otp: otp, GeneratedAt: DateTime.UtcNow);
            return otp;
        }

        public bool ValidateOtp(string email, string otp)
        {
            if (_otpStorage.ContainsKey(email))
            {
                var (storedOtp, generatedAt) = _otpStorage[email];
                if (storedOtp == otp && DateTime.UtcNow <= generatedAt.AddMinutes(5))
                {
                    _otpStorage.Remove(email); // Remove OTP after successful validation
                    return true;
                }
            }
            return false;
        }

        public void SendOtpEmail(string email, string otp)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Demo App", "jaypmodi999@gmail.com")); // Updated sender email
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Your OTP Code";
            message.Body = new TextPart("plain")
            {
                Text = $"Your OTP code is: {otp}. It is valid for 5 minutes."
            };

            using (var client = new SmtpClient())
            {
                // For testing only; remove in production
                client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                try
                {
                    client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls); 
                    client.Authenticate("jaypmodi999@gmail.com", "yptckgzdzgasebhy"); 
                    client.Send(message);
                    Console.WriteLine("OTP email sent successfully."+otp);
                    client.Disconnect(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    client.Disconnect(true);
                }
                finally
                {
                    client.Disconnect(true);
                }
            }
        }

    }
}
using Microsoft.AspNetCore.Mvc;
using OtpDemo.Interfaces;
using OtpDemo.Models;

[ApiController]
[Route("api/[controller]")]
public class EmailOtpController : ControllerBase
{
    private readonly IEmailOtpService _emailOtpService;

    public EmailOtpController(IEmailOtpService emailOtpService)
    {
        _emailOtpService = emailOtpService;
    }

    [HttpPost("generate-otp")]
    public IActionResult GenerateOtp([FromBody] otpEmailRequest request)
    {
        var otp = _emailOtpService.GenerateOtp(request.Email);
        _emailOtpService.SendOtpEmail(request.Email, otp);
        return Ok("OTP sent to email successfully");
    }

    [HttpPost("validate-otp")]
    public IActionResult ValidateOtp([FromBody] verifyOtpMailRequest request)
    {
        var isValid = _emailOtpService.ValidateOtp(request.Email, request.OTP);
        if (isValid)
        {
            return Ok("OTP validated successfully");
        }
        return Unauthorized("Invalid OTP or OTP expired");
    }
}

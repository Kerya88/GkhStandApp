namespace GkhStandApp.Services
{
    public interface IEmailService
    {
        public Task SendVerificationCodeAsync(string email);
    }
}

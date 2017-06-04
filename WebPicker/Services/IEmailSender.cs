using System.Threading.Tasks;

namespace WebPicker.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}

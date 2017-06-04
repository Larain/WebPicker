using System.Threading.Tasks;

namespace WebPicker.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}

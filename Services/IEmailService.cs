using System.Threading.Tasks;

namespace Services
{
    public interface IEmailService
    {
        Task SendAsync(string email, string name, string to, string subject, string body, string file = null);
    }
}

using System.Threading.Tasks;

namespace BloodBond.BLL.Service
{
    
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}

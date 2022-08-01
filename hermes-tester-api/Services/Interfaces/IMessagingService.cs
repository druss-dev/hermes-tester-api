using System.Threading.Tasks;
using hermes_tester_api.Models;

namespace hermes_tester_api.Services.Interfaces;

public interface IMessagingService
{
    Task<string> DispatchSmsMessageAsync(SmsRecipient recipientInformation);
    Task<string> DispatchEmailMessageAsync(EmailRecipient recipientInformation);
}
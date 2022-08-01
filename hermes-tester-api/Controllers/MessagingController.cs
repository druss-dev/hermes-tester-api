using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using hermes_tester_api.Models;
using hermes_tester_api.Services.Interfaces;

namespace hermes_tester_api.Controllers;

[Route("api/message")]
public class MessagingController
{
    private readonly IMessagingService _messagingService;

    public MessagingController(IMessagingService messagingService)
    {
        _messagingService = messagingService;
    }

    [HttpPost]
    [Route("dispatchSms")]
    public async Task<string> DispatchSmsMessageAsync([FromBody] SmsRecipient recipientInformation)
    {
        return await _messagingService.DispatchSmsMessageAsync(recipientInformation);
    }
    
    [HttpPost]
    [Route("dispatchMultipleSms")]
    public async Task<List<string>> DispatchMultipleSmsMessagesAsync([FromBody] List<SmsRecipient> recipients)
    {
        var response = new List<string>();
        foreach (var recipient in recipients)
        {
            response.Add(await _messagingService.DispatchSmsMessageAsync(recipient));
        }
        return response;
    }
    
    [HttpPost]
    [Route("dispatchEmail")]
    public async Task<string> DispatchEmailMessageAsync([FromBody] EmailRecipient recipientInformation)
    {
        return await _messagingService.DispatchEmailMessageAsync(recipientInformation);
    }
}
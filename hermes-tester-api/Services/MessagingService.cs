using Hermes.Client.Models;
using Hermes.Client;
using Hermes.Shared.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using UrlShortener.Client;
using hermes_tester_api.Models.Configuration;
using hermes_tester_api.Models;
using hermes_tester_api.Services.Interfaces;

namespace hermes_tester_api.Services;

public class MessagingService : IMessagingService
{
    private readonly EnvironmentOptions _environmentOptions;
    private readonly IHermesClient _hermesClient;
    private readonly IUrlShortenerClient _urlShortenerClient;
    
    public MessagingService(IHermesClient hermesClient, IUrlShortenerClient urlShortenerClient)
    {
        _environmentOptions = new EnvironmentOptions();
        _hermesClient = hermesClient;
        _urlShortenerClient = urlShortenerClient;
    }

    public async Task<string> DispatchSmsMessageAsync(SmsRecipient recipientInformation)
    {
        /* 
         Currently the Caerus webhook does not respond to the text message if a patient workflow is not found.
         Replying with "1" after the text will not send a response back because of this, but will show a message in the logs.
        */
        //TODO: grab existing patient workflow to actually send a valid loyal unsub response? (this may have consequences)
        
        // build the envelope
        var envelope = BuildEnvelope(recipientInformation);
        
        // ship it
        await _hermesClient.Send(envelope.Result);

        return $"Processed recipient {envelope.Result.MessageChainId}.";
    }
    
    public async Task<string> DispatchEmailMessageAsync(EmailRecipient recipientInformation)
    {
        recipientInformation.Body = recipientInformation.Body.Replace("\n", "<br />");
        await _hermesClient.EnqueueEmailAsync(new EmailPayload
        {
            ClientId = recipientInformation.ClientId,
            FromAddress = _environmentOptions.DefaultEmailFromAddress,
            To = new[]{ new EmailAddress { Email = recipientInformation.ToAddress } },
            Subject = recipientInformation.Subject,
            Body = recipientInformation.Body,
            Purpose = "Test",
            OnBehalfOf = "Hermes-Tester"
        });
                
        return $"Processed email recipient.";
    }

    private async Task<Envelope> BuildEnvelope(SmsRecipient recipientInformation)
    {
        // create new message chain id if one is not given
        var messageChainId = $"hermes-tester-{Guid.NewGuid().ToString()}";
        
        // builds the text for the sms message and calls url shortener
        var message = await BuildMessageAndHandleUrl(recipientInformation);
        
        // assure the phone number is in the format hermes expects
        recipientInformation.RecipientPhoneNumber = FormatPhoneNumber(recipientInformation.RecipientPhoneNumber);

        // create envelope for hermes to process
        var envelope = _hermesClient.Compose(recipientInformation.ShortenedUrlPayload.ClientId, 0)
            .To(recipientInformation.RecipientPhoneNumber, MessagingChannel.TextMessage)
            .FromBot("Hermes-Tester")
            .ForConversation(messageChainId)
            .WithText(message)
            .Build();
        
        // designate the sending phone number (twilio number)
        envelope.Sender = new SenderInformation { Identifier = _environmentOptions.TwilioPhoneNumber };

        return envelope;
    }
    
    // this is a temporary extension for importers to use to format a phone number to be compatible with Hermes and
    // it's blacklist process. this only works on north american phone numbers
    private string FormatPhoneNumber(string rawPhoneNumber)
    {
        // strip non-numeric values
        var strippedNumber = string.Concat(rawPhoneNumber
            .Where(char.IsDigit)
            .ToArray());

        return strippedNumber.Length switch
        {
            10 => $"+1{strippedNumber}",
            11 => $"+{strippedNumber}",
            _ => null
        };
    }

    private async Task<string> BuildMessageAndHandleUrl(SmsRecipient recipientInformation)
    {

        // add a url to the message if a long url was specified
        var message = new StringBuilder();
        message.Append($"Hi {recipientInformation.FirstName}, thanks for using my janky hermes client testing api. ");

        // shorten the long url if true and add to the message
        if (recipientInformation.ShouldShortenUrl && recipientInformation.ShortenedUrlPayload is not null)
        {
            var shortenedResult = await _urlShortenerClient.GetShortenedUrlAsync(recipientInformation.ShortenedUrlPayload);
            message.Append($"Here is your shortened url.. click at your own risk: {shortenedResult.Url.ShortUrl}");
        }
        else if (recipientInformation.ShortenedUrlPayload is not null)
        {
            message.Append($"Here is your url that you refused to shorten... tsk tsk: {recipientInformation.ShortenedUrlPayload.Link}");
        }

        return message.ToString();
    }
}
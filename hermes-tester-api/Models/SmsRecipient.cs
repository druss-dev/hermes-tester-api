using UrlShortener.Client.Models;

namespace hermes_tester_api.Models;

public class SmsRecipient
{
    public ShortenedUrlPayload? ShortenedUrlPayload { get; set; }
    public bool ShouldShortenUrl { get; set; }
    public string FirstName { get; set; } = "";
    public string RecipientPhoneNumber { get; set; } = "";
}

  
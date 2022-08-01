using System;

namespace hermes_tester_api.Models.Configuration;

public class EnvironmentOptions
{
    public string AllowSelfSignedCertificates { get; set; }
    public string AspNetCoreEnvironment { get; set; }
    public string AspNetCoreUrls { get; set; }
    public string DefaultEmailFromAddress { get; set; }
    public string HermesUri { get; set; }
    public string ServicePassword { get; set; }
    public string ServiceUsername { get; set; }
    public string TwilioPhoneNumber { get; set; }
    public string UrlShortenerUri { get; set; }

    public EnvironmentOptions()
    {
        AllowSelfSignedCertificates = Environment.GetEnvironmentVariable("ALLOW_SELF_SIGNED_CERTIFICATES");
        AspNetCoreEnvironment= Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        AspNetCoreUrls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
        DefaultEmailFromAddress = Environment.GetEnvironmentVariable("DEFAULT_EMAIL_FROM_ADDRESS");
        HermesUri = Environment.GetEnvironmentVariable("HERMES_URI");
        ServicePassword = Environment.GetEnvironmentVariable("SERVICE_PASSWORD");
        ServiceUsername = Environment.GetEnvironmentVariable("SERVICE_USERNAME");
        TwilioPhoneNumber = Environment.GetEnvironmentVariable("URL_SHORTENER_URI");
        UrlShortenerUri = Environment.GetEnvironmentVariable("URL_SHORTENER_URI");
    }
}
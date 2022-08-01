namespace hermes_tester_api.Models;

public class EmailRecipient
{
    public string ClientId { get; set; } = "";
    public string ToAddress { get; set; } = "";
    public string Subject { get; set; } = "";
    public string Body { get; set; } = "";
}
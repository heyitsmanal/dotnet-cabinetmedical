using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

public class FakeEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Log the email details for debugging purposes (optional)
        Console.WriteLine($"Email sent to {email}: {subject}\n{htmlMessage}");

        // Return a completed task to satisfy the IEmailSender interface
        return Task.CompletedTask;
    }
}

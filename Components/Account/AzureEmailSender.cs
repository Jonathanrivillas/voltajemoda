using Azure.Communication.Email;
using Microsoft.AspNetCore.Identity;
using VoltajeModa.Data;

namespace VoltajeModa.Components.Account;

internal sealed class AzureEmailSender(EmailClient emailClient, IConfiguration configuration, ILogger<AzureEmailSender> logger)
    : IEmailSender<ApplicationUser>
{
    private readonly string senderAddress = configuration["AzureCommunicationServices:SenderAddress"]
        ?? throw new InvalidOperationException("Falta configurar AzureCommunicationServices:SenderAddress.");

    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
        SendAsync(email, "Confirma tu cuenta de Voltaje Moda",
            $"<p>Gracias por registrarte en Voltaje Moda. Confirma tu cuenta haciendo <a href='{confirmationLink}'>click aquí</a>.</p>");

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
        SendAsync(email, "Restablece tu contraseña de Voltaje Moda",
            $"<p>Restablece tu contraseña haciendo <a href='{resetLink}'>click aquí</a>. Si no solicitaste esto, ignora este correo.</p>");

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
        SendAsync(email, "Restablece tu contraseña de Voltaje Moda",
            $"<p>Tu código para restablecer la contraseña es: <strong>{resetCode}</strong></p>");

    private async Task SendAsync(string toAddress, string subject, string htmlBody)
    {
        try
        {
            await emailClient.SendAsync(Azure.WaitUntil.Started, senderAddress, toAddress, subject, htmlBody);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "No se pudo enviar el correo '{Subject}' a {ToAddress}", subject, toAddress);
            throw;
        }
    }
}

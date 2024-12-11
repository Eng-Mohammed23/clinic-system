using Microsoft.AspNetCore.SignalR;

namespace ClinicSystem.Api.Hubs;

public class ChatHub : Hub
{
    // إرسال رسالة إلى العملاء المتصلين
    public async Task SendMessage(string sender, string receiver, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", sender, receiver, message);
    }
}

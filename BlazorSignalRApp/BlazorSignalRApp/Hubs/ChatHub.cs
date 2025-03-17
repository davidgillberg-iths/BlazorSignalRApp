using Microsoft.AspNetCore.SignalR;

namespace BlazorSignalRApp.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            Console.WriteLine("Call to SendMessage with values, user: " + user + " and message: " + message);
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}

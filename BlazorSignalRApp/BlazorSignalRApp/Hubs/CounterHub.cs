using Microsoft.AspNetCore.SignalR;

namespace BlazorSignalRApp.Hubs
{
    public class CounterHub : Hub
    {
        public async Task AddToTotal(string user, int value)
        {
            return Clients.All.SendAsync("CounterIncremented", user, value);
        }
    }
}

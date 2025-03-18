using Microsoft.AspNetCore.SignalR;

namespace BlazorSignalRApp.Hubs
{
    public class CounterHub : Hub
    {
        public async Task AddToTotal(int value)
        {
            await Clients.All.SendAsync("CounterIncremented", value);
        }
    }
}

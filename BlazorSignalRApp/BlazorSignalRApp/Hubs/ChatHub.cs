using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public class ChatHub : Hub
{
    // Dictionary för att lagra användarnamn och deras ConnectionId (trådsäker)
    private static ConcurrentDictionary<string, string> Users = new ConcurrentDictionary<string, string>();

    // Användare registrerar sig med sitt namn vid anslutning
    public async Task<bool> RegisterUser(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            await Clients.Caller.SendAsync("RegistrationFailed", "Användarnamnet får inte vara tomt.");
            return false;
        }

        if (Users.ContainsKey(username))
        {
            await Clients.Caller.SendAsync("RegistrationFailed", "Användarnamnet är redan taget. Vänligen välj ett annat.");
            return false;
        }

        // Lägg till användaren i listan
        if (Users.TryAdd(username, Context.ConnectionId))
        {
            await Clients.Caller.SendAsync("RegistrationSuccess", username);
            await Clients.All.SendAsync("UserJoined", username);
            return true;
        }

        await Clients.Caller.SendAsync("RegistrationFailed", "Ett oväntat fel uppstod. Försök igen.");
        return false;
    }

    // Skicka privat meddelande till en användare baserat på deras namn
    public async Task SendPrivateMessage(string toUsername, string fromUsername, string message)
    {
        if (Users.TryGetValue(toUsername, out string receiverConnectionId))
        {
            await Clients.Client(receiverConnectionId).SendAsync("ReceivePrivateMessage", fromUsername, message);
            await Clients.Caller.SendAsync("ReceivePrivateMessage", "-me-", message);
        }
        else
        {
            await Clients.Caller.SendAsync("ReceivePrivateMessage", "System", $"Användaren {toUsername} är inte online.");
        }
    }

    public async Task SendPublicMessage(string fromUser, string message)
    {
        await Clients.All.SendAsync("ReceivePublicMessage", fromUser, message);
    }

    // När en användare kopplas från, ta bort dem från listan
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var user = Users.FirstOrDefault(x => x.Value == Context.ConnectionId);
        if (!string.IsNullOrEmpty(user.Key))
        {
            Users.TryRemove(user.Key, out _);
            await Clients.All.SendAsync("UserLeft", user.Key);
        }

        await base.OnDisconnectedAsync(exception);
    }
}

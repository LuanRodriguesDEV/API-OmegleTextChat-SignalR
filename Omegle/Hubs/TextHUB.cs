using Microsoft.AspNetCore.SignalR;
using Omegle.Model;

namespace Omegle.Hubs
{
    public class TextHUB : Hub
    {
        protected List<User> WaitingUsers = new();
        protected List<InChannel> InChannelUsers = new();

        public async Task SearchChat()
        {
            var userId = Context.ConnectionId;
            var usuario = new User()
            {
                ConnectionID = userId,
            };

            if (WaitingUsers.Count > 0)
            {
                var usuario1 = WaitingUsers[0];
                var usuario2 = new User() { ConnectionID = userId};

                WaitingUsers.Remove(usuario1);

                var newGroup = new InChannel()
                {
                    Users = new List<User> { usuario1, usuario2 }
                };

                InChannelUsers.Add(newGroup);

                await Clients.Client(usuario1.ConnectionID).SendAsync("ChatIniciado", usuario2.ConnectionID);
                await Clients.Client(usuario2.ConnectionID).SendAsync("ChatIniciado", usuario1.ConnectionID);
            }
            else
            {
                WaitingUsers.Add(usuario);
            }   
        }
        public async Task SendMessage(Message message)
        {
            await Clients.Client(message.ChatId).SendAsync(message.UserName, message.Description);
            await Clients.Caller.SendAsync(message.UserName, message.Description);
        }
        public async Task StopChat()
        {
            var userId = Context.ConnectionId;
            var find = InChannelUsers.FirstOrDefault(channel => channel.Users.Any(user => user.ConnectionID == userId));
            if(find != null)
            {
                InChannelUsers.Remove(find);
                await Clients.Client(find.Users[0].ConnectionID).SendAsync("Chat Terminado", find.Users[1].ConnectionID);
                await Clients.Client(find.Users[1].ConnectionID).SendAsync("Chat Terminado", find.Users[0].ConnectionID);
            }
        }
        public async Task StopSearch()
        {
            var userId = Context.ConnectionId;
            var desconectedUser = WaitingUsers.FirstOrDefault(u => u.ConnectionID == userId);
            if (desconectedUser != null)
            {
                WaitingUsers.Remove(desconectedUser);
            }
            await Clients.Caller.SendAsync("Busca Terminada");
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.ConnectionId;
            var desconectedUser = WaitingUsers.FirstOrDefault(u => u.ConnectionID == userId);
            if (desconectedUser != null)
            {
                WaitingUsers.Remove(desconectedUser);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}

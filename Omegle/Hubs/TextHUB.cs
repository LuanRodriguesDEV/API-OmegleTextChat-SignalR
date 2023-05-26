using Microsoft.AspNetCore.SignalR;
using Omegle.Model;
using Omegle.VOs.Input;
using System.Text.RegularExpressions;

namespace Omegle.Hubs
{
    public class TextHUB : Hub
    {
        public static List<User> WaitingUsers = new List<User>();
        public static List<InChannel> InChannelUsers = new List<InChannel>();

        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            await Clients.Caller.SendAsync("Connected", connectionId);
            
        }
        public async Task SearchChat(string userName)
        {
            var userId = Context.ConnectionId;
            var usuario = new User()
            {
                UserName = userName,
                ConnectionID = userId,
            };

            if (WaitingUsers.Count > 0)
            {
                var usuario1 = WaitingUsers[0];
                var GroupID = usuario.ConnectionID + usuario1.ConnectionID;
                WaitingUsers.Remove(usuario1);

                var newGroup = new InChannel()
                {
                    ChatID = GroupID,
                    Users = new List<User> { usuario1, usuario }
                };

                foreach (var user in newGroup.Users)
                {
                    await Groups.AddToGroupAsync(user.ConnectionID, newGroup.ChatID);
                }

                InChannelUsers.Add(newGroup);
                await Clients.Group(newGroup.ChatID).SendAsync("StartChat", newGroup);
            }
            else
            {
                var verify = WaitingUsers.FirstOrDefault(x => x.ConnectionID == userId);
                if (verify == null)
                    WaitingUsers.Add(usuario);
            }   
        }
        public async Task SendMessage(MessageVOInput message)
        {
            var newMessage = new Message()
            {
                ChatID = message.ConnectionID,
                Description = message.Description,
                UserName = message.UserName,
            };
            await Clients.Group(message.ChatId).SendAsync("SendMessage", newMessage);
        }
        public async Task StopChat()
        {
            var userId = Context.ConnectionId;
            var find = InChannelUsers.FirstOrDefault(channel => channel.Users.Any(user => user.ConnectionID == userId));
            if(find != null)
            {
                InChannelUsers.Remove(find);
                
                await Clients.Group(find.ChatID).SendAsync("StopChat");
                await DeleteGroup(find);
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
            await Clients.Caller.SendAsync("StopSearch");
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.ConnectionId;
            var desconectedUser = WaitingUsers.FirstOrDefault(u => u.ConnectionID == userId);

            if (desconectedUser != null)
            {
                WaitingUsers.Remove(desconectedUser);
            }
            var find = InChannelUsers.FirstOrDefault(channel => channel.Users.Any(user => user.ConnectionID == userId));
            if (find != null)
            {
                await Clients.Group(find.ChatID).SendAsync("StopChat");
                await DeleteGroup(find);
            }
            await base.OnDisconnectedAsync(exception);
        }

        private async Task DeleteGroup (InChannel channel)
        {
            foreach (var user in channel.Users)
            {
                await Groups.RemoveFromGroupAsync(user.ConnectionID, channel.ChatID);
            }
        }
    }
}

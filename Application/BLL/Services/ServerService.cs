
using BLL.Abstractions;
using BLL.Abstractions.Interfaces;
using Core.Models;
using DAL.Abstractions.Interfaces;
using DAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ServerService : IServerService
    {
        private readonly IGenericRepository<Server> _serverRepository;
        private readonly IGenericRepository<Chat> _chatRepository;
        private readonly ServerInvitationService _serverInvitationService;

        public ServerService(IGenericRepository<Server> serverReposirory,
            IGenericRepository<Chat> chatRepository,
            ServerInvitationService serverInvitationService)
        {
            _serverRepository = serverReposirory;
            _chatRepository = chatRepository;
            _serverInvitationService = serverInvitationService;
        }

        public bool CreateServer(string name)
        {
            if (name == null)
            {
                return false;
            }

            var server = new Server();
            server.Name = name;

            // checking if server with this name already exists
            if (!Equals(_serverRepository.FindByCondition(s => s.Name == server.Name), Enumerable.Empty<Server>()))
            {
                return false;
            }

            server.DateCreated = DateTime.Now;
            server.Chats = new List<Chat>();
            server.Users = new List<User>();
            // adding roles

            // creating a server
            _serverRepository.CreateAsync(server);
            return true;
        }

        public bool DeleteServer(Server server)
        {
            if (server == null)
            {
                throw new ArgumentNullException(nameof(server));
            }

            // checking if such server exists
            if (Equals(_serverRepository.FindByCondition(s => s.Id == server.Id), Enumerable.Empty<Server>()))
            {
                return false;
            }

            // checking if you have particular roles to delete the server ...

            // deleting all chats from the server
            foreach (var chat in server.Chats)
            {
                _chatRepository.DeleteAsync(chat);
            }

            // deleting all users from the server
            foreach (var user in server.Users)
            {
                user.Servers.Remove(server);
            }

            _serverRepository.DeleteAsync(server);
            return true;
        }

        public bool AddChat(Server server, Chat chat)
        {
            if (server == null || chat == null)
            {
                return false;
            }

            // checking if this server exists
            if (Equals(_serverRepository.FindByCondition(s => s == server), Enumerable.Empty<Server>()))
            {
                return false;
            }

            // checking if this chat exists
            if (Equals(_chatRepository.FindByCondition(c => c == chat), Enumerable.Empty<Chat>()))
            {
                return false;
            }

            // checking if this chat already belongs to this or another server
            if (Equals(_serverRepository.FindByCondition(s => s.Chats.Contains(chat)), Enumerable.Empty<Server>()))
            {
                return false;
            }

            server.Chats.Add(chat);
            chat.Server = server;

            _serverRepository.UpdateAsync(server);

            return true;
        }

        public bool DeleteChat(Server server, Chat chat)
        {
            if (server == null || chat == null)
            {
                return false;
            }

            // checking if you have particular roles to delete the chat ...

            // checking if this chat is in this server
            if (server.Chats.FirstOrDefault(c => c == chat) == null)
            {
                return false;
            }

            // deleting the chat
            _chatRepository.DeleteAsync(chat); // if we delete a chat from the server than we delete it everywhere
            server.Chats.Remove(chat);

            _serverRepository.UpdateAsync(server);

            return true;
        }

        public bool AddUser(Server server, User user)
        {
            if (user == null || server == null)
            {
                return false;
            }

            // checking if this user is alreadly in this server
            if (server.Users.FirstOrDefault(u => u == user) != null)
            {
                return false;
            }

            server.Users.Add(user);

            // adding this user to all chats that are in this server ...

            user.Servers.Add(server);

            _serverRepository.UpdateAsync(server);

            return true;
        }

        public bool AddUsers(Server server,IEnumerable<User> users)
        {
            if (server == null)
            {
                return false;
            }

            foreach(var user in users)
            {
                if (user != null && server.Users.FirstOrDefault(u => u.Id == user.Id) == null)
                {
                    server.Users.Add(user);
                    user.Servers.Add(server);
                    // adding all chats that are in this server in this user's chats
                }
            }

            _serverRepository.UpdateAsync(server);

            return true;
        }

        public bool DeleteUser(Server server, User user)
        {
            if (user == null || server == null)
            {
                return false;
            }
            
            // checking if this user is in this server
            if (server.Users.FirstOrDefault(u => u == user) == null)
            {
                return false;
            }

            // checking if you have particular roles to delete users from the server ... 

            user.Servers.Remove(server);
            server.Users.Remove(user);

            _serverRepository.UpdateAsync(server);

            return true;
        }

        public bool DeleteUsers(Server server, IEnumerable<User> users)
        {
            if (server == null)
            {
                return false;
            }

            foreach (var user in users)
            {
                if (user != null && server.Users.FirstOrDefault(u => u.Id == user.Id) != null)
                {
                    server.Users.Remove(user);
                    user.Servers.Remove(server);
                    // deleting all chats that are in this server in this user's chats
                }
            }

            _serverRepository.UpdateAsync(server);

            return true;
        }

        public async Task SendInvitation(Server server, User user)
        {
            await _serverInvitationService.InviteByEmailAsync(server, user);

            await _serverRepository.UpdateAsync(server);
        }
    }
}

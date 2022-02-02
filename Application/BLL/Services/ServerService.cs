
using BLL.Abstractions;
using BLL.Abstractions.Interfaces;
using Core.Models;
using DAL.Abstractions.Interfaces;
using DAL.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ServerService : IServerService
    {
        private readonly IGenericRepository<Server> _serverRepository;
        private readonly IGenericRepository<Chat> _chatRepository;
        private readonly EmailNotificationService _emailNotificationService;

        public ServerService(IGenericRepository<Server> serverRepository,
            IGenericRepository<Chat> chatRepository,
            EmailNotificationService emailNotificationService)
        {
            _serverRepository = serverRepository;
            _chatRepository = chatRepository;
            _emailNotificationService = emailNotificationService;
        }

        public async Task<bool> CreateServerAsync(string name)
        {
            var server = new Server
            {
                Name = name
            };

            // checking if server with this name already exists
            if (_serverRepository.Any(s => s.Name == server.Name))
            {
                return false;
            }

            server.DateCreated = DateTime.Now;
            server.Chats = new List<Chat>();
            server.Users = new List<User>();
            // adding roles

            // creating a server
            await _serverRepository.CreateAsync(server);
            return true;
        }

        public async Task<bool> DeleteServerAsync(Server server)
        {
            // checking if such server exists
            if (!_serverRepository.Any(s => s.Id == server.Id))
            {
                return false;
            }

            // checking if you have particular roles to delete the server ...

            // deleting all chats from the server
            foreach (var chat in server.Chats)
            {
                await _chatRepository.DeleteAsync(chat);
            }

            // deleting all users from the server
            foreach (var user in server.Users)
            {
                user.Servers.Remove(server);
            }

            await _serverRepository.DeleteAsync(server);
            return true;
        }

        public async Task<bool> AddUserAsync(Server server, User user)
        {
            // checking if this user is already in this server
            if (server.Users.FirstOrDefault(u => u == user) != null)
            {
                return false;
            }

            server.Users.Add(user);

            // adding this user to all chats that are in this server ...

            user.Servers.Add(server);

            await _serverRepository.UpdateAsync(server);

            return true;
        }

        public async Task<bool> AddUsersAsync(Server server, IEnumerable<User> users)
        {
            foreach(var user in users)
            {
                if (user != null && server.Users.FirstOrDefault(u => u.Id == user.Id) == null)
                {
                    server.Users.Add(user);
                    user.Servers.Add(server);
                    // adding all chats that are in this server in this user's chats
                }
            }

            await _serverRepository.UpdateAsync(server);

            return true;
        }

        public async Task<bool> DeleteUserAsync(Server server, User user)
        {
            // checking if this user is in this server
            if (server.Users.FirstOrDefault(u => u == user) == null)
            {
                return false;
            }

            // checking if you have particular roles to delete users from the server ... 

            user.Servers.Remove(server);
            server.Users.Remove(user);

            await _serverRepository.UpdateAsync(server);

            return true;
        }

        public async Task<bool> DeleteUsersAsync(Server server, IEnumerable<User> users)
        {
            foreach (var user in users)
            {
                if (user != null && server.Users.FirstOrDefault(u => u.Id == user.Id) != null)
                {
                    server.Users.Remove(user);
                    user.Servers.Remove(server);
                    // deleting all chats that are in this server in this user's chats
                }
            }

            await _serverRepository.UpdateAsync(server);

            return true;
        }

        public async Task SendInvitationAsync(Server server, User user)
        {
            await _emailNotificationService.InviteByEmailAsync(server, user);

            await _serverRepository.UpdateAsync(server);
        }
    }
}

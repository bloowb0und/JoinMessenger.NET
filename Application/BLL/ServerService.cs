
using BLL.Abstractions;
using Core.Models;
using DAL.Abstractions.Interfaces;
using DAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ServerService : IServerService
    {
        private readonly IGenericRepository<Server> _serverRepository;
        private readonly IGenericRepository<Chat> _chatRepository;

        public ServerService(IGenericRepository<Server> serverReposirory,
            IGenericRepository<Chat> chatRepository)
        {
            _serverRepository = serverReposirory;
            _chatRepository = chatRepository;
        }

        public bool CreateServer(string name)
        {
            if (name is null)
            {
                return false;
            }

            var server = new Server();
            server.Name = name;

            // checking if this server already exists
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
            if (server is null)
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
            if (server is null || chat is null)
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
            if (server is null || chat is null)
            {
                return false;
            }

            // checking if you have particular roles to delete the chat ...

            // checking if this chat is in this server
            if (server.Chats.FirstOrDefault(c => c == chat) is null)
            {
                return false;
            }

            // deleting the chat
            chat.Server = null;
            server.Chats.Remove(chat);

            _serverRepository.UpdateAsync(server);

            return true;
        }

        public bool AddUser(Server server, User user)
        {
            if (user is null || server is null)
            {
                return false;
            }

            // checking if this user is alreadly in this server
            if (server.Users.FirstOrDefault(u => u == user) is not null)
            {
                return false;
            }

            server.Users.Add(user);

            // adding this user to all chats that are in this server ...

            user.Servers.Add(server);

            _serverRepository.UpdateAsync(server);

            return true;
        }

        public bool DeleteUser(Server server, User user)
        {
            if (user is null || server is null)
            {
                return false;
            }
            
            // checking if this user is in this server
            if (server.Users.FirstOrDefault(u => u == user) is null)
            {
                return false;
            }

            // checking if you have particular roles to delete users from the server ... 

            user.Servers.Remove(server);
            server.Users.Remove(user);

            _serverRepository.UpdateAsync(server);

            return true;
        }
    }
}

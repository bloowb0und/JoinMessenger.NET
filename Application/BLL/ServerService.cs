
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
                throw new ArgumentNullException(nameof(name));
            }

            var server = new Server();
            server.Name = name;

            // checking if this server already exists
            if (!Equals(_serverRepository.FindByCondition(s => s.Name == server.Name), Enumerable.Empty<Server>()))
            {
                throw new ArgumentException($"Server with this name ({name}) already exists");
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

            if (Equals(_serverRepository.FindByCondition(s => s.Id == server.Id), Enumerable.Empty<Server>()))
            {
                throw new ArgumentException("There is no such server");
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

                throw new ArgumentNullException($"{nameof(server)} or {nameof(chat)} was/were null");
            }

            if (Equals(_serverRepository.FindByCondition(s => s == server), Enumerable.Empty<Server>()))
            {
                throw new ArgumentException($"Server {nameof(server)} doesn't exist");
            }

            if (Equals(_chatRepository.FindByCondition(c => c == chat), Enumerable.Empty<Chat>()))
            {
                throw new ArgumentException($"Chat {nameof(chat)} doesn't exist");
            }

            if (Equals(_serverRepository.FindByCondition(s => s.Chats.Contains(chat)), Enumerable.Empty<Server>()))
            {
                throw new ArgumentException($"{nameof(server)} already belongs to this or another server");
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
                throw new ArgumentNullException($"{nameof(server)} or {nameof(chat)} was/were null");
            }

            // checking if you have particular roles to delete the chat ...

            if (server.Chats.FirstOrDefault(c => c == chat) is null)
            {
                throw new ArgumentException($"{nameof(chat)} is not in the ${nameof(server)}");
            }

            // deleting the chat
            chat.Server = null;
            server.Chats.Remove(chat);
            return true;
        }

        public bool AddUser(Server server, User user)
        {
            if (user is null || server is null)
            {
                throw new ArgumentNullException($"{nameof(server)} or {nameof(user)} was/were null");
            }

            if (server.Users.FirstOrDefault(u => u == user) is not null)
            {
                throw new ArgumentException($"{nameof(user)} is already in the ${nameof(server)}");
            }

            server.Users.Add(user);

            // adding all chats that are in this server to the user's list of chats

            user.Servers.Add(server);

            _serverRepository.UpdateAsync(server);

            return true;
        }

        public bool DeleteUser(Server server, User user)
        {
            if (user is null || server is null)
            {
                throw new ArgumentNullException($"{nameof(server)} or {nameof(user)} was/were null");
            }

            if (server.Users.FirstOrDefault(u => u == user) is null)
            {
                throw new ArgumentException($"There is no {nameof(user)} in the ${nameof(server)}");
            }

            // checking if you have particular roles to delete users from the server

            user.Servers.Remove(server);
            server.Users.Remove(user);

            _serverRepository.UpdateAsync(server);

            return true;
        }
    }
}


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
    public class ServerService 
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

    }
}

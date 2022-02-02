﻿
using BLL.Abstractions;
using BLL.Abstractions.Interfaces;
using Core.Models;
using DAL.Abstractions.Interfaces;
using DAL.Database;
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
        private readonly EmailNotificationService _emailNotificationService;
        private readonly AppDbContext _context;

        public ServerService(AppDbContext context,
            EmailNotificationService emailNotificationService)
        {
            _context = context;
            _emailNotificationService = emailNotificationService;
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
            if (_context.Servers.Any(s => s.Name == server.Name))
            {
                return false;
            }

            server.DateCreated = DateTime.Now;
            server.Chats = new List<Chat>();
            
            // adding roles

            // creating a server
            _context.Servers.Add(server);
            _context.SaveChanges();
            return true;
        }

        public bool DeleteServer(Server server)
        {
            if (server == null)
            {
                return false;
            }

            // checking if such server exists
            if (!_context.Servers.Any(s => s.Id == server.Id))
            {
                return false;
            }

            // checking if you have particular roles to delete the server ...

            // deleting all chats from the server
            foreach (var chat in server.Chats)
            {
                _context.Chats.Remove(chat);
            }

            _context.Servers.Remove(server);
            _context.SaveChanges();
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

            _context.Servers.Update(server);
            _context.SaveChanges();

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

            _context.Servers.Update(server);
            _context.SaveChanges();

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

            _context.Servers.Update(server);
            _context.SaveChanges();

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

            _context.Servers.Update(server);
            _context.SaveChanges();

            return true;
        }

        public async Task SendInvitation(Server server, User user)
        {
            await _emailNotificationService.InviteByEmailAsync(server, user);

            _context.Servers.Update(server);
        }
    }
}

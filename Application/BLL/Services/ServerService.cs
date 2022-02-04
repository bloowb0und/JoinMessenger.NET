
using BLL.Abstractions;
using BLL.Abstractions.Interfaces;
using Core.Models;
using DAL.Abstractions.Interfaces;
using DAL.Contexts;
using DAL.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models.ServiceMethodsModels;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class ServerService : IServerService
    {
        private readonly IEmailNotificationService _emailNotificationService;
        private readonly AppDbContext _context;

        public ServerService(AppDbContext context,
            IEmailNotificationService emailNotificationService)
        {
            _context = context;
            _emailNotificationService = emailNotificationService;
        }

        public async Task<bool> CreateServerAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            var server = new Server
            {
                Name = name
            };

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
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteServerAsync(Server server)
        {
            // checking if such server exists
            if (!_context.Servers.Any(s => s.Id == server.Id))
            {
                return false;
            }

            // checking if you have particular roles to delete the server ...

            // deleting all chats from the server

            _context.Servers.Remove(server);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AddUserAsync(Server server, User user)
        {
            var list = _context.UserServers.ToList();

            if (server == null || user == null)
            {
                return false;
            }

            // checking if this user is already in this server
            if (list.Any(us => us.UserId == user.Id && us.ServerId == server.Id))
            {
                return false;
            }

            server.Users.Add(user);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AddUsersAsync(Server server,IEnumerable<User> users)
        {
            if (server == null)
            {
                return false;
            }

            var list = await _context.UserServers.ToListAsync();

            foreach(var user in users)
            {
                if (user != null && !list.Any(us => us.UserId == user.Id && us.ServerId == server.Id))
                {
                    server.Users.Add(user);
                }
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteUserAsync(Server server, User user)
        {
            if (user == null || server == null)
            {
                return false;
            }

            var list = _context.UserServers.ToList();

            // checking if this user is in this server
            if (!list.Any(us => us.UserId == user.Id && us.ServerId == server.Id))
            {
                return false;
            }

            // checking if you have particular roles to delete users from the server ... 

            server.Users.Remove(user);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteUsersAsync(Server server, IEnumerable<User> users)
        {
            if (server == null)
            {
                return false;
            }

            var list = _context.UserServers.ToList();

            foreach (var user in users)
            {
                if (user != null && list.Any(us => us.UserId == user.Id && us.ServerId == server.Id))
                {
                    user.Servers.Remove(server);
                }
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task SendInvitationAsync(Server server, User user)
        {
            await _emailNotificationService.InviteByEmailAsync(server, user);

            _context.Servers.Update(server);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> EditServerAsync(Server server, ServerServiceEditServer newServer)
        {
            if (_context.Servers.Any(s => s.Name == newServer.ServerName))
            {
                return false;
            }

            server.Name = newServer.ServerName;

            await _context.SaveChangesAsync();
            
            return true;
        }
    }
}

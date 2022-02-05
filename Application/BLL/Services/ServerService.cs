
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
        private readonly UnitOfWork _unitOfWork;

        public ServerService(IEmailNotificationService emailNotificationService,
            UnitOfWork unitOfWork)
        {
            _emailNotificationService = emailNotificationService;
            _unitOfWork = unitOfWork;
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

            if (_unitOfWork.ServerRepository.Any(s => s.Name == name))
            {
                return false;
            }

            server.DateCreated = DateTime.Now;
            
            var newChat = new Chat()
            {
                Name = "general",
                Server = server,
                Type = ChatType.Text,
            };
            await _unitOfWork.ChatRepository.Create(newChat);

            // create everyone and owner roles in RoleService

            // creating a server
            await _unitOfWork.ServerRepository.Create(server);

            return true;
        }

        public async Task<bool> DeleteServerAsync(Server server)
        {
            if (server == null)
            {
                return false;
            }

            // checking if such server exists
            if (!_unitOfWork.ServerRepository.Any(s => s.Id == server.Id))
            {
                return false;
            }

            // checking if you have particular roles to delete the server ...

            // deleting all chats from the server

            await _unitOfWork.ServerRepository.Delete(server);

            return true;
        }

        public async Task<bool> AddUserAsync(Server server, User user)
        {
            if (server == null || user == null)
            {
                return false;
            }

            // checking if this user is already in this server
            if (_unitOfWork.UserRepository.Any(us =>
                    us.UserServers.Any(u => u.User.Id == user.Id && u.Server.Id == server.Id)))
            {
                return false;
            }

            // server.Users.Add(user);

            await _unitOfWork.Save();

            return true;
        }

        public async Task<bool> AddUsersAsync(Server server,IEnumerable<User> users)
        {
            if (server == null)
            {
                return false;
            }

            foreach (var user in users)
            {
                if (user != null && !_unitOfWork.UserRepository.Any(us =>
                        us.UserServers.Any(u => u.User.Id == user.Id && u.Server.Id == server.Id)))
                {
                    server.UserServers.Add(new UserServer()
                    {
                        User = user,
                        Server = server,
                    });
                }
            }
            
            await _unitOfWork.ServerRepository.Update(server);

            return true;
        }

        public async Task<bool> DeleteUserAsync(Server server, User user)
        {
            if (server == null || user == null)
            {
                return false;
            }

            // checking if this user is in this server
            if (_unitOfWork.UserRepository.Any(us =>
                    us.UserServers.Any(u => u.User.Id == user.Id && u.Server.Id == server.Id)))
            {
                return false;
            }

            // checking if you have particular roles to delete users from the server ... 
            if (!user.UserServers.Any(us => us.UserServerRoles.Any(usr =>
                    usr.Role.ServerPermissionRoles.Any(spr =>
                        spr.ServerPermission.Id == 0 && spr.ServerPermissionStatus)))) // change spr.ServerPermission.Id to particular in ServerPermissions table
            {
                return false;
            }
            
            server.UserServers.Remove(
                server.UserServers.FirstOrDefault(us => us.User.Id == user.Id && us.Server.Id == server.Id));

            await _unitOfWork.ServerRepository.Update(server);

            return true;
        }

        public async Task<bool> DeleteUsersAsync(Server server, IEnumerable<User> users)
        {
            if (server == null)
            {
                return false;
            }

            var dbServer = _unitOfWork.ServerRepository.FirstOrDefault(s => s.Id == server.Id);

            foreach (var user in users)
            {
                if (user != null && _unitOfWork.UserRepository.Any(us =>
                        us.UserServers.Any(u => u.User.Id == user.Id && u.Server.Id == server.Id)))
                {
                    _unitOfWork.UserRepository.Any(u =>
                        u.UserServers.Remove(u.UserServers.FirstOrDefault(us =>
                            us.User.Id == user.Id && us.Server.Id == server.Id)));
                }
            }

            await _unitOfWork.ServerRepository.Update(server);

            return true;
        }

        // not done yet
        public async Task SendInvitationAsync(Server server, User user)
        {
            await _emailNotificationService.InviteByEmailAsync(server, user);

            await _unitOfWork.ServerRepository.Update(server);
        }

        public async Task<bool> EditServerAsync(Server server, ServerServiceEditServer newServer)
        {
            if (_unitOfWork.ServerRepository.Any(s => s.Name == newServer.ServerName))
            {
                return false;
            }

            server.Name = newServer.ServerName;

            await _unitOfWork.ServerRepository.Update(server);
            
            return true;
        }
    }
}

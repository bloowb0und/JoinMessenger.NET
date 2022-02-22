
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
        private readonly IUnitOfWork _unitOfWork;

        public ServerService(IEmailNotificationService emailNotificationService,
            IUnitOfWork unitOfWork)
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

            if (await _unitOfWork.ServerRepository.Any(s => s.Name == name))
            {
                return false;
            }

            server.DateCreated = DateTime.Now;
            
            // creating a server

            // create everyone and owner roles in RoleService
            var newChat = new Chat()
            {
                Name = "general",
                Server = server,
                Type = ChatType.Text,
            };

            using (_unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await _unitOfWork.ServerRepository.CreateAsync(server);
                    await _unitOfWork.SaveAsync();
                    
                    await _unitOfWork.ChatRepository.CreateAsync(newChat); // call chat service
                    await _unitOfWork.SaveAsync();
                    
                    await _unitOfWork.CommitTransactionAsync();
                }
                catch 
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
            }

            return true;
        }

        public async Task<bool> DeleteServerAsync(Server server)
        {
            if (server == null)
            {
                return false;
            }

            // checking if such server exists
            if (!await _unitOfWork.ServerRepository.Any(s => s.Id == server.Id))
            {
                return false;
            }

            // checking if you have particular roles to delete the server ...

            // deleting all chats from the server

            using (_unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    _unitOfWork.ServerRepository.Delete(server);
                    await _unitOfWork.SaveAsync();

                    await _unitOfWork.CommitTransactionAsync();
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
            }

            return true;
        }

        public async Task<bool> AddUserAsync(Server server, User user)
        {
            if (server == null || user == null)
            {
                return false;
            }

            if (Equals(await _unitOfWork.ServerRepository.Get(
                    u => u.UserServers.Any(us => us.User.Id == user.Id && us.Server.Id == server.Id), null,
                    "UserServers"), Enumerable.Empty<Server>()))
            {
                return false;
            }

            using (_unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    (await _unitOfWork.ServerRepository.Get(s => s.Id == server.Id, null, "UserServers"))
                        .FirstOrDefault()?.UserServers.Add(new UserServer()
                        {
                            User = user,
                            Server = server,
                            DateEntered = DateTime.Now
                        });
                    await _unitOfWork.SaveAsync();

                    await _unitOfWork.CommitTransactionAsync();
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
            }

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
                if (user != null && !await _unitOfWork.UserRepository.Any(us =>
                        us.UserServers.Any(u => u.User.Id == user.Id && u.Server.Id == server.Id)))
                {
                    server.UserServers.Add(new UserServer()
                    {
                        User = user,
                        Server = server,
                    });
                }
            }
            
            using (_unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    _unitOfWork.ServerRepository.Update(server);
                    await _unitOfWork.SaveAsync();

                    await _unitOfWork.CommitTransactionAsync();
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
            }

            return true;
        }

        public async Task<bool> DeleteUserAsync(Server server, User user)
        {
            if (server == null || user == null)
            {
                return false;
            }

            // checking if this user is in this server
            if (await _unitOfWork.UserRepository.Any(us =>
                    us.UserServers.Any(u => u.User.Id == user.Id && u.Server.Id == server.Id)))
            {
                return false;
            }

            // checking if you have particular roles to delete users from the server ... 
            if (!user.UserServers.Any(us => us.UserServerRoles.Any(usr =>
                    usr.Role.ServerPermissionRoles.Any(spr =>
                        spr.ServerPermission.Id == 0 && spr.Status)))) // change spr.ServerPermission.Id to particular in ServerPermissions table
            {
                return false;
            }
            
            server.UserServers.Remove(
                server.UserServers.FirstOrDefault(us => us.User.Id == user.Id && us.Server.Id == server.Id));

            using (_unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    _unitOfWork.ServerRepository.Update(server);
                    await _unitOfWork.SaveAsync();

                    await _unitOfWork.CommitTransactionAsync();
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
            }

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
                if (user != null && await _unitOfWork.UserRepository.Any(us =>
                        us.UserServers.Any(u => u.User.Id == user.Id && u.Server.Id == server.Id)))
                {
                    _unitOfWork.UserRepository.Any(u =>
                        u.UserServers.Remove(u.UserServers.FirstOrDefault(us =>
                            us.User.Id == user.Id && us.Server.Id == server.Id)));
                }
            }
            
            using (_unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    _unitOfWork.ServerRepository.Update(server);
                    await _unitOfWork.SaveAsync();

                    await _unitOfWork.CommitTransactionAsync();
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
            }

            return true;
        }

        public Server GetServerByName(string name)
        {
            return _unitOfWork.ServerRepository.FirstOrDefault(s => s.Name == name);
        }

        // not done yet
        public async Task SendInvitationAsync(Server server, User user)
        {
            await _emailNotificationService.InviteByEmailAsync(server, user);
            
            using (_unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    _unitOfWork.ServerRepository.Update(server);
                    await _unitOfWork.SaveAsync();

                    await _unitOfWork.CommitTransactionAsync();
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
            }
        }

        public async Task<bool> EditServerAsync(Server server, ServerServiceEditServer newServer)
        {
            if (await _unitOfWork.ServerRepository.Any(s => s.Name == newServer.ServerName))
            {
                return false;
            }

            server.Name = newServer.ServerName;
            
            using (_unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    _unitOfWork.ServerRepository.Update(server);
                    await _unitOfWork.SaveAsync();

                    await _unitOfWork.CommitTransactionAsync();
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
            }
            
            return true;
        }
    }
}

using BLL.Abstractions.Interfaces;
using Core.Models;
using DAL.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models.ServiceMethodsModels;
using FluentResults;

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

        public async Task<Result> CreateServerAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Result.Fail("Server name is empty.");
            }

            var server = new Server
            {
                Name = name
            };

            if (await _unitOfWork.ServerRepository.Any(s => s.Name == name))
            {
                return Result.Fail("Server already exists.");
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

            return Result.Ok();
        }

        public async Task<Result> DeleteServerAsync(Server server)
        {
            // checking if such server exists
            if (!await _unitOfWork.ServerRepository.Any(s => s.Id == server.Id))
            {
                return Result.Fail("Server doesn't exist.");
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

            return Result.Ok();
        }

        public async Task<Result> AddUserAsync(Server server, User user)
        {
            if ((await _unitOfWork.ServerRepository.Get(
                    u => u.UserServers.Any(us => us.User.Id == user.Id && us.Server.Id == server.Id), null,
                    "UserServers")).Count() != 0)
            {
                return Result.Fail("User is already a member of this server.");
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

            return Result.Ok();
        }

        public async Task<Result> AddUsersAsync(Server server,IEnumerable<User> users)
        {
            foreach (var user in users)
            {
                if (!await _unitOfWork.UserRepository.Any(us =>
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

            return Result.Ok();
        }

        public async Task<Result> DeleteUserAsync(Server server, User user)
        {
            // checking if this user is in this server
            if (await _unitOfWork.UserRepository.Any(us =>
                    us.UserServers.Any(u => u.User.Id == user.Id && u.Server.Id == server.Id)))
            {
                return Result.Fail("User is not a member of this server");
            }

            // checking if you have particular roles to delete users from the server ... 
            if (!user.UserServers.Any(us => us.UserServerRoles.Any(usr =>
                    usr.Role.ServerPermissionRoles.Any(spr =>
                        spr.ServerPermission.Id == 0 && spr.Status)))) // change spr.ServerPermission.Id to particular in ServerPermissions table
            {
                return Result.Fail("No permission to delete users from server.");
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

            return Result.Ok();
        }

        public async Task<Result> DeleteUsersAsync(Server server, IEnumerable<User> users)
        {
            foreach (var user in users)
            {
                if (await _unitOfWork.UserRepository.Any(us =>
                        us.UserServers.Any(u => u.User.Id == user.Id && u.Server.Id == server.Id)))
                {
                    await _unitOfWork.UserRepository.Any(u =>
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

            return Result.Ok();
        }

        public Result<Server> GetServerByName(string name)
        {
            var foundServer = _unitOfWork.ServerRepository.FirstOrDefault(s => s.Name == name);

            return foundServer == null ? Result.Fail("Server with such name doesn't exist.") : Result.Ok(foundServer);
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

        public async Task<Result> EditServerAsync(Server server, ServerServiceEditServer newServer)
        {
            if (await _unitOfWork.ServerRepository.Any(s => s.Name == newServer.ServerName))
            {
                return Result.Fail("Server with this name already exists.");
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
            
            return Result.Ok();
        }
    }
}

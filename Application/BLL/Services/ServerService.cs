using BLL.Abstractions.Interfaces;
using Core.Models;
using DAL.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models.DTO;
using Core.Models.ServiceMethodsModels;
using FluentResults;

namespace BLL.Services
{
    public class ServerService : IServerService
    {
        private readonly IEmailNotificationService _emailNotificationService;
        private readonly IRoleService _roleService;
        private readonly IChatService _chatService;
        private readonly IUnitOfWork _unitOfWork;

        public ServerService(IEmailNotificationService emailNotificationService,
            IRoleService roleService,
            IChatService chatService,
            IUnitOfWork unitOfWork)
        {
            _emailNotificationService = emailNotificationService;
            _roleService = roleService;
            _chatService = chatService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ServerDto>> CreateServerAsync(string name, User user)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Result.Fail("Server name is empty.");
            }

            if (await _unitOfWork.ServerRepository.Any(s => s.Name == name))
            {
                return Result.Fail("Server already exists.");
            }
            
            var server = new Server
            {
                Name = name,
                DateCreated = DateTime.Now
            };

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

                    var addUserRes = await AddUserAsync(server, user);
                    if (addUserRes.IsFailed)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                    }
                    
                    var createdOwnerRole = await _roleService.CreateRoleAsync(new RoleServiceEditRole
                    {
                        RoleName = "owner",
                        RoleServer = server
                    });
                    
                    if (createdOwnerRole.IsFailed)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                    }

                    await _roleService.AttachUserToRoleAsync(createdOwnerRole.Value, user);
                    
                    await _unitOfWork.CommitTransactionAsync();
                }
                catch 
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
            }

            return Result.Ok(new ServerDto{Id = server.Id, Name = server.Name, DateCreated = server.DateCreated});
        }

        public async Task<Result> DeleteServerAsync(Server server, User user)
        {
            // checking if such server exists
            if (!await _unitOfWork.ServerRepository.Any(s => s.Id == server.Id))
            {
                return Result.Fail("Server doesn't exist.");
            }

            // checking if you have particular roles to delete the server
            var hasOwnerRole = await _unitOfWork.UserRepository
                .Any(u => u.UserServers.Any(us => us.UserServerRoles.Any(usr => usr.UserServer == u.UserServers
                    .FirstOrDefault(uss => uss.User.Id == user.Id 
                                          && uss.Server.Id == server.Id) && usr.Role.Name == "owner")), "UserServers,UserServerRoles");

            if (!hasOwnerRole) // user has no owner role
            {
                return Result.Fail("User must have 'owner' role to delete a Server.");
            }
            
            // deleting all chats from the server
            var chats = await _unitOfWork.ChatRepository.Get(x => x.Server.Id == server.Id, null, null);
            foreach (var chat in chats)
            {
                var delRes = await _chatService.DeleteChatAsync(chat);
                if (delRes.IsFailed)
                {
                    return Result.Fail(delRes.Errors.FirstOrDefault());
                }
            }

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

        public Result<Server> GetServerById(int id)
        {
            var foundServer = _unitOfWork.ServerRepository.FirstOrDefault(s => s.Id == id);

            return foundServer == null
                ? Result.Fail("Server with such Id doesn't exist.")
                : Result.Ok(foundServer);
        }
        
        public Result<Server> GetServerByName(string name)
        {
            var foundServer = _unitOfWork.ServerRepository.FirstOrDefault(s => s.Name == name);

            return foundServer == null
                ? Result.Fail("Server with such name doesn't exist.")
                : Result.Ok(foundServer);
        }

        public async Task<Result<IEnumerable<ServerDto>>> GetServersForUser(User user)
        {
            var foundServers = (await _unitOfWork.UserServerRepository
                    .Get(filter: us => us.User.Id == user.Id,
                        includeProperties: "Server"))
                .Select(us => new ServerDto
                    {Id = us.Server.Id, Name = us.Server.Name, DateCreated = us.Server.DateCreated});

            if (foundServers == null)
            {
                return Result.Fail("Server array was null.");
            }
            
            return Result.Ok(foundServers);
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

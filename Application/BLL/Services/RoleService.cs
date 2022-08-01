using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core.Models;
using Core.Models.ServiceMethodsModels;
using DAL.Abstractions.Interfaces;
using FluentResults;

namespace BLL.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async Task<Result<Role>> CreateRoleAsync(RoleServiceEditRole role)
        {
            var createdRole = new Role
            {
                Name = role.RoleName,
                Server = role.RoleServer
            };
            
            if (await _unitOfWork.RoleRepository.Any(r=> (r.Id == createdRole.Id) 
                                                         || (r.Name == createdRole.Name 
                                                             && r.Server == createdRole.Server)))
            {
                return Result.Fail("Such role already exists!");
            }
            
            using (_unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await _unitOfWork.RoleRepository.CreateAsync(createdRole);
                    
                    await _unitOfWork.SaveAsync();

                    await _unitOfWork.CommitTransactionAsync();
                }
                catch 
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
            }
            
            return Result.Ok(createdRole);
        }

        public async Task<Result> DeleteRoleAsync(Role role)
        {
            if (!await _unitOfWork.RoleRepository.Any(r=> (r.Id == role.Id)
                                                          || (r.Name == role.Name 
                                                              && r.Server == role.Server)))
            {
                return Result.Fail("Such role doesn't exist!");
            }

            using (_unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    _unitOfWork.RoleRepository.Delete(role);

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

        public async Task<Result> AttachUserToRoleAsync(Role role, User user)
        {
            if (!await _unitOfWork.RoleRepository.Any(r=> (r.Id == role.Id) 
                                                          || (r.Name == role.Name 
                                                              && r.Server == role.Server)))
            {
                return Result.Fail("Such role doesn't exist!");
            }

            if ((await _unitOfWork.RoleRepository.Get(r => r.Id == role.Id
                                                           && r.UserServerRoles
                                                               .Any(usr => usr.UserServer.User.Id == user.Id
                                                                           && usr.Role.Id == role.Id
                                                                           && usr.UserServer.Server.Id ==
                                                                           role.Server.Id),
                    null, "UserServerRoles")).Any())
            {
                return Result.Fail("User with same role already exists!");
            }

            using (_unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var userServer = user.UserServers.FirstOrDefault(us => us.Server.Id == role.Server.Id);
                    if (userServer == null)
                    {
                        return Result.Fail("User was not found on current server.");
                    }
                    
                    (await _unitOfWork.RoleRepository.Get(r => r.Id == role.Id, null, "UserServerRoles"))
                        .FirstOrDefault()?.UserServerRoles.Add(new UserServerRole
                        {
                            UserServer = userServer,
                            Role = role,
                            DateApplied = DateTime.Now
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

        public async Task<Result> AttachUsersToRoleAsync(Role role, IEnumerable<User> users)
        {
            foreach (var user in users)
            {
                if (!await _unitOfWork.RoleRepository.Any(r=> (r.Id == role.Id) 
                                                              || (r.Name == role.Name 
                                                                  && r.Server == role.Server)))
                {
                    return Result.Fail("Such role doesn't exist!");
                }

                if ((await _unitOfWork.RoleRepository.Get(r=>r.Id == role.Id 
                                                                   && r.UserServerRoles
                                                                       .Any(usr=>usr.UserServer.User.Id == user.Id 
                                                                           && usr.Role.Id == role.Id 
                                                                           && usr.UserServer.Server.Id == role.Server.Id),null,"UserServerRoles")).Any())
                {
                    return Result.Fail("User with same role already exists!");
                }
                
                var curUserServer = user.UserServers.FirstOrDefault(us => us.Server.Id == role.Server.Id);
                
                if (curUserServer == null)
                {
                    return Result.Fail($"User {user.Login} can not be found on the server.");
                }
                
                (await _unitOfWork.RoleRepository.Get(r => r.Id == role.Id, null, "UserServerRoles"))
                    .FirstOrDefault()?.UserServerRoles.Add(new UserServerRole
                    {
                        UserServer = curUserServer,
                        Role = role,
                        DateApplied = DateTime.Now
                    });
            }
            
            using (_unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    _unitOfWork.RoleRepository.Update(role);
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

        public async Task<Result> RemoveUserFromRoleAsync(Role role, User user)
        {
            if (!await _unitOfWork.RoleRepository.Any(r=> (r.Id == role.Id) 
                                                          || (r.Name == role.Name 
                                                              && r.Server == role.Server)))
            {
                return Result.Fail("Such role doesn't exist!");
            }
            
            if (!(await _unitOfWork.RoleRepository.Get(r=>r.Id == role.Id 
                                                          && r.UserServerRoles
                                                                   .Any(usr=>usr.UserServer.User.Id == user.Id 
                                                                             && usr.Role.Id == role.Id 
                                                                             && usr.UserServer.Server.Id == role.Server.Id),null,"UserServerRoles")).Any())
            {
                return Result.Fail("User with same role doesn't exist!");
            }
            
            if (!user.UserServers.Any(us => us.UserServerRoles.Any(usr =>
                    usr.Role.ServerPermissionRoles.Any(spr =>
                        spr.ServerPermission.Id == 0 && spr.Status)))) // change spr.ServerPermission.Id to particular in ServerPermissions table
            {
                return Result.Fail("Your privilege isn't enough to do this action!");
            }

            role.UserServerRoles.Remove(user.UserServers
                .FirstOrDefault(us => us.Server.Id == role.Server.Id)
                ?.UserServerRoles
                    .FirstOrDefault(usr => usr.Role.Id == role.Id));
            
            using (_unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    _unitOfWork.RoleRepository.Update(role);
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

        public async Task<Result> RemoveUsersFromRoleAsync(Role role, IEnumerable<User> users)
        {
            foreach (var user in users)
            {
                if (!await _unitOfWork.RoleRepository.Any(r => (r.Id == role.Id) 
                                                               || (r.Name == role.Name 
                                                                   && r.Server == role.Server)))
                {
                    return Result.Fail("Such role doesn't exist!");
                }

                if ((await _unitOfWork.RoleRepository.Get(r => r.Id == role.Id 
                                                                      && r.UserServerRoles
                                                                          .Any(usr =>
                                                                              usr.UserServer.User.Id == user.Id 
                                                                              && usr.Role.Id == role.Id 
                                                                              && usr.UserServer.Server.Id ==
                                                                              role.Server.Id), null, "UserServerRoles")).Any())
                {
                    return Result.Fail("User with same role already exists!");
                }

                if (!user.UserServers.Any(us => us.UserServerRoles.Any(usr =>
                        usr.Role.ServerPermissionRoles.Any(spr =>
                            spr.ServerPermission.Id == 0 
                            && spr.Status)))) // change spr.ServerPermission.Id to particular in ServerPermissions table
                {
                    return Result.Fail("Your privilege isn't enough to do this action!");
                }

                role.UserServerRoles.Remove(user.UserServers
                    .FirstOrDefault(us => us.Server.Id == role.Server.Id)?.UserServerRoles
                    .FirstOrDefault(usr => usr.Role.Id == role.Id));
            }

            using (_unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    _unitOfWork.RoleRepository.Update(role);
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

        public async Task<Result> EditRoleAsync(Role role, RoleServiceEditRole newRole)
        {
            if (!await _unitOfWork.RoleRepository.Any(r => (r.Id == role.Id) 
                                                           || (r.Name == role.Name 
                                                               && r.Server == role.Server)))
            {
                return Result.Fail("Such role doesn't exist!");
            }

            role.Name = newRole.RoleName;
            
            using (_unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    _unitOfWork.RoleRepository.Update(role);
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
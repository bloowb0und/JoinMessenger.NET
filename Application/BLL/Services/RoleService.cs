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
        
        public async Task<Result> CreateRoleAsync(RoleServiceEditRole role)
        {
            var _role = new Role
            {
                Name = role.RoleName,
                Server = role.RoleServer
            };
            
            if (await _unitOfWork.RoleRepository.Any(r=> (r.Id == _role.Id) ||
                                                         (r.Name == _role.Name &&
                                                         r.Server == _role.Server)))
            {
                return Result.Fail("Such role already exists!");
            }
            
            using (_unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await _unitOfWork.RoleRepository.CreateAsync(_role);
                    
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

        public async Task<Result> DeleteRoleAsync(Role role)
        {
            if (!await _unitOfWork.RoleRepository.Any(r=> (r.Id == role.Id) ||
                                                         (r.Name == role.Name &&
                                                          r.Server == role.Server)))
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
            if (!await _unitOfWork.RoleRepository.Any(r=> (r.Id == role.Id) ||
                                                          (r.Name == role.Name &&
                                                           r.Server == role.Server)))
            {
                return Result.Fail("Such role doesn't exist!");
            }

            if (Equals(await _unitOfWork.RoleRepository.Get(r=>r.Id == role.Id &&
                                                               r.UserServerRoles
                                                                   .Any(usr=>usr.UserServer.User.Id == user.Id &&
                                                                             usr.Role.Id == role.Id &&
                                                                             usr.UserServer.Server.Id == role.Server.Id),null,"UserServerRoles"),Enumerable.Empty<Role>()) )
            {
                return Result.Fail("User with same role already exists!");
            }
            
            using (_unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var parametr = user.UserServers.FirstOrDefault(us => us.Server.Id == role.Server.Id);
                    if (parametr==null)
                    {
                        throw new ArgumentNullException();
                    }
                    (await _unitOfWork.RoleRepository.Get(r => r.Id == role.Id, null, "UserServerRoles"))
                        .FirstOrDefault()?.UserServerRoles.Add(new UserServerRole
                        {
                            UserServer = parametr,
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
                if (!await _unitOfWork.RoleRepository.Any(r=> (r.Id == role.Id) ||
                                                              (r.Name == role.Name &&
                                                               r.Server == role.Server)))
                {
                    return Result.Fail("Such role doesn't exist!");
                }

                if (Equals(await _unitOfWork.RoleRepository.Get(r=>r.Id == role.Id &&
                                                                   r.UserServerRoles
                                                                       .Any(usr=>usr.UserServer.User.Id == user.Id &&
                                                                           usr.Role.Id == role.Id &&
                                                                           usr.UserServer.Server.Id == role.Server.Id),null,"UserServerRoles"),Enumerable.Empty<Role>()) )
                {
                    return Result.Fail("User with same role already exists!");
                }
                
                var parametr = user.UserServers.FirstOrDefault(us => us.Server.Id == role.Server.Id);
                
                if (parametr==null)
                {
                    throw new ArgumentNullException();
                }
                
                (await _unitOfWork.RoleRepository.Get(r => r.Id == role.Id, null, "UserServerRoles"))
                    .FirstOrDefault()?.UserServerRoles.Add(new UserServerRole
                    {
                        UserServer = parametr,
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
            if (!await _unitOfWork.RoleRepository.Any(r=> (r.Id == role.Id) ||
                                                          (r.Name == role.Name &&
                                                           r.Server == role.Server)))
            {
                return Result.Fail("Such role doesn't exist!");
            }
            
            if (!Equals(await _unitOfWork.RoleRepository.Get(r=>r.Id == role.Id &&
                                                               r.UserServerRoles
                                                                   .Any(usr=>usr.UserServer.User.Id == user.Id &&
                                                                             usr.Role.Id == role.Id &&
                                                                             usr.UserServer.Server.Id == role.Server.Id),null,"UserServerRoles"),Enumerable.Empty<Role>()) )
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
                if (!await _unitOfWork.RoleRepository.Any(r => (r.Id == role.Id) ||
                                                               (r.Name == role.Name &&
                                                                r.Server == role.Server)))
                {
                    return Result.Fail("Such role doesn't exist!");
                }

                if (!Equals(await _unitOfWork.RoleRepository.Get(r => r.Id == role.Id &&
                                                                      r.UserServerRoles
                                                                          .Any(usr =>
                                                                              usr.UserServer.User.Id == user.Id &&
                                                                              usr.Role.Id == role.Id &&
                                                                              usr.UserServer.Server.Id ==
                                                                              role.Server.Id), null, "UserServerRoles"),
                        Enumerable.Empty<Role>()))
                {
                    return Result.Fail("User with same role already exists!");
                }

                if (!user.UserServers.Any(us => us.UserServerRoles.Any(usr =>
                        usr.Role.ServerPermissionRoles.Any(spr =>
                            spr.ServerPermission.Id == 0 &&
                            spr.Status)))) // change spr.ServerPermission.Id to particular in ServerPermissions table
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
            if (!await _unitOfWork.RoleRepository.Any(r => (r.Id == role.Id) ||
                                                           (r.Name == role.Name &&
                                                            r.Server == role.Server)))
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
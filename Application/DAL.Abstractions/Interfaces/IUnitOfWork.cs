using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DAL.Abstractions.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IDbGenericRepository<Server> ServerRepository { get; }
        IDbGenericRepository<User> UserRepository { get; }
        IDbGenericRepository<Chat> ChatRepository { get; }
        IDbGenericRepository<Role> RoleRepository { get; }
        IDbGenericRepository<Message> MessageRepository { get; }
        IDbGenericRepository<ChatPermission> ChatPermissionRepository { get; }
        IDbGenericRepository<ServerPermission> ServerPermissionRepository { get; }
        
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
        Task<int> SaveAsync(CancellationToken cancellationToken = default);
    }
}
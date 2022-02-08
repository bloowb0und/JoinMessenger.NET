using Core.Models;
using DAL.Contexts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DAL.Abstractions.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DAL.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        
        private IDbContextTransaction _transaction;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IDbGenericRepository<Server> ServerRepository => new DbGenericRepository<Server>(_context);
        public IDbGenericRepository<User> UserRepository => new DbGenericRepository<User>(_context);
        public IDbGenericRepository<Chat> ChatRepository => new DbGenericRepository<Chat>(_context);
        public IDbGenericRepository<Role> RoleRepository => new DbGenericRepository<Role>(_context);
        public IDbGenericRepository<Message> MessageRepository => new DbGenericRepository<Message>(_context);
        public IDbGenericRepository<ChatPermission> ChatPermissionRepository => new DbGenericRepository<ChatPermission>(_context);
        public IDbGenericRepository<ServerPermission> ServerPermissionRepository => new DbGenericRepository<ServerPermission>(_context);

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }
        
        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _transaction.CommitAsync(cancellationToken);
        }
        
        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
        }
        
        public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        private bool disposed = false;

        protected virtual async Task Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    await _context.DisposeAsync();
                }
            }

            this.disposed = true;
        }

        public async void Dispose()
        {
            await Dispose(true);
            GC.SuppressFinalize(this);
        }
    } 
}

using Core.Models;
using DAL.Contexts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class UnitOfWork : IDisposable
    {
        private readonly AppDbContext _context;
        private DbGenericRepository<Server> _serverRepository;
        private DbGenericRepository<User> _userRepository;
        private DbGenericRepository<Chat> _chatRepository;
        private DbGenericRepository<Role> _roleRepository;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public DbGenericRepository<Server> ServerRepository
        {
            get
            {
                if (_serverRepository == null)
                {
                    _serverRepository = new DbGenericRepository<Server>(_context);
                }

                return _serverRepository;
            }
        }

        public DbGenericRepository<User> UserRepository
        {
            get
            {
                if (_userRepository == null)
                {
                    _userRepository = new DbGenericRepository<User>(_context);
                }
                return _userRepository;
            }
        }

        public DbGenericRepository<Chat> ChatRepository
        {
            get
            {
                if (_chatRepository == null)
                {
                    _chatRepository = new DbGenericRepository<Chat>(_context);
                }

                return _chatRepository;
            }
        }
        
        public DbGenericRepository<Role> RoleRepository
        {
            get
            {
                if (_roleRepository == null)
                {
                    _roleRepository = new DbGenericRepository<Role>(_context);
                }

                return _roleRepository;
            }
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
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

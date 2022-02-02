using Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Server> Servers { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //Many to Many (Server & User)
            builder.Entity<Server>()
            .HasMany(p => p.Users)
            .WithMany(p => p.Servers)
            .UsingEntity<UserServer>(
                j => j
                    .HasOne(pt => pt.User)
                    .WithMany(t => t.UserServers)
                    .HasForeignKey(pt => pt.UserId),
                j => j
                    .HasOne(pt => pt.Server)
                    .WithMany(p => p.UserServers)
                    .HasForeignKey(pt => pt.ServerId),
                j =>
                {
                    j.Property(pt => pt.DateEntered).HasDefaultValueSql("CURRENT_TIMESTAMP");
                    j.HasKey(t => new { t.UserId, t.ServerId });
                    j.ToTable("UserServer");
                });

            // Many to Many (Chat & Role)
            builder.Entity<ChatRole>()
                .HasKey(cr => new { cr.ChatId, cr.RoleId });

            builder.Entity<ChatRole>()
                .HasOne(x => x.Chat)
                .WithMany(c => c.ChatRoles)
                .HasForeignKey(f => f.ChatId);

            builder.Entity<ChatRole>()
                .HasOne(x => x.Role)
                .WithMany(r => r.ChatRoles)
                .HasForeignKey(f => f.RoleId);

            // Many to Many (Server & Role)
            builder.Entity<ServerRole>()
                .HasKey(sr => new { sr.ServerId, sr.RoleId });

            builder.Entity<ServerRole>()
                .HasOne(sr => sr.Server)
                .WithMany(s => s.ServerRoles)
                .HasForeignKey(f => f.ServerId);

            builder.Entity<ServerRole>()
                .HasOne(sr => sr.Role)
                .WithMany(r => r.ServerRoles)
                .HasForeignKey(f => f.RoleId);

            // Role
            builder.Entity<Role>()
                .Property(r => r.RoleType)
                .HasConversion<string>()
                .HasMaxLength(30);

            // Chat
            builder.Entity<Chat>()
                .Property(c => c.Type)
                .HasConversion<string>()
                .HasMaxLength(30);

            // Message
            builder.Entity<Message>()
                .HasOne(m => m.Server)
                .WithMany(s => s.Messages)
                .HasForeignKey(m => m.ServerId)
                .OnDelete(DeleteBehavior.NoAction);

            // Many to Many (ChatRole & Permission
            builder.Entity<ChatRolePermission>()
                .HasKey(k => new { k.ChatRoleId, k.PermissionId });

            builder.Entity<ChatRolePermission>()
                .HasOne(cr => cr.ChatRole)
                .WithMany(r => r.ChatRolePermissions)
                .HasForeignKey(f => f.ChatRoleId)
                .HasPrincipalKey(x => x.Id);

            builder.Entity<ChatRolePermission>()
                .HasOne(cr => cr.Permission)
                .WithMany(p => p.ChatRolePermissions)
                .HasForeignKey(f => f.PermissionId);

            // Many to Many (ServerRole & Permission)
            builder.Entity<ServerRolePermission>()
                .HasKey(srp => new { srp.ServerRoleId, srp.PermissionId });

            builder.Entity<ServerRolePermission>()
                .HasOne(srp => srp.ServerRole)
                .WithMany(sr => sr.ServerRolePermissions)
                .HasForeignKey(f => f.ServerRoleId)
                .HasPrincipalKey(pk => pk.Id);

            builder.Entity<ServerRolePermission>()
                .HasOne(srp => srp.Permission)
                .WithMany(p => p.ServerRolePermissions)
                .HasForeignKey(f => f.PermissionId);
        }
    }

}

using ClickCounter.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClickCounter.Infrastructure.Context;

public partial class ClickCounterDbContext : DbContext {
    public ClickCounterDbContext() { }

    public ClickCounterDbContext(DbContextOptions<ClickCounterDbContext> options) : base(options) { }

    public virtual DbSet<Counter> Counters { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserToken> UserTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source=../app.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Counter>(entity => {
            entity.HasIndex(e => e.CounterId, "IX_Counters_CounterId").IsUnique();
        });

        modelBuilder.Entity<Role>(entity => {
            entity.HasIndex(e => e.RoleId, "IX_Roles_RoleId").IsUnique();
        });

        modelBuilder.Entity<User>(entity => {
            entity.HasIndex(e => e.UserId, "IX_Users_UserId").IsUnique();
        });

        modelBuilder.Entity<UserRole>(entity => {
            entity.HasIndex(e => e.UserRoleId, "IX_UserRoles_UserRoleId").IsUnique();

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles).HasForeignKey(d => d.RoleId);

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<UserToken>(entity => {
            entity.HasIndex(e => e.UserTokenId, "IX_UserTokens_UserTokenId").IsUnique();

            entity.HasOne(d => d.User).WithMany(p => p.UserTokens).HasForeignKey(d => d.UserId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

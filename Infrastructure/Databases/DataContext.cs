﻿using System.Linq.Expressions;
using OrderService.Models;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Infrastructure.Databases
{
    public partial class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
    {
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Set default max length column for string data type
            modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetProperties())
                .Where(p => p.ClrType == typeof(string) && p.GetMaxLength() == null)
                .ToList()
                .ForEach(p => p.SetMaxLength(255));

            GenerateUuid<Order>(modelBuilder, "Id");
            SoftDelete<Order>(modelBuilder);

            GenerateUuid<OrderDetail>(modelBuilder, "Id");
            SoftDelete<OrderDetail>(modelBuilder);

            modelBuilder.Entity<Notification>()
                .Property(n => n.IsRead)
                .HasDefaultValue(false);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderNumber)
                .IsUnique();

            modelBuilder.Entity<Order>()
                .Property(o => o.SubTotalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderDetail>()
                .HasIndex(o => new { o.OrderId, o.ProductId, o.CategoryId, o.ExProductCode, o.ExCategoryKey });

            modelBuilder.Entity<OrderDetail>()
                .HasOne(o => o.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(o => o.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderDetail>()
                .Property(o => o.ExUnitPrice)
                .HasPrecision(18, 2);
            
            modelBuilder.Entity<OrderDetail>()
                .Property(o => o.TotalPrice)
                .HasPrecision(18, 2);
        }

        public override int SaveChanges()
        {
            var currentTime = DateTime.Now;

            var entries = ChangeTracker
                .Entries<BaseModel>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                var entity = entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = currentTime;
                    entity.UpdatedAt = currentTime;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entity.UpdatedAt = currentTime;
                    entry.Property(nameof(entity.CreatedAt)).IsModified = false;
                }
            }

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var currentTime = DateTime.Now;

            var entries = ChangeTracker
                .Entries<BaseModel>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                var entity = entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = currentTime;
                    entity.UpdatedAt = currentTime;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entity.UpdatedAt = currentTime;
                    entry.Property(nameof(entity.CreatedAt)).IsModified = false;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        /*=================================== Service Support ===========================================*/

        private static void GenerateUuid<T>(ModelBuilder modelBuilder, string column) where T : class
        {
            modelBuilder.Entity<T>()
                .HasIndex(CreateExpression<T>(column));

            modelBuilder.Entity<T>()
                .Property(CreateExpression<T>(column))
                .HasDefaultValueSql("NEWID()");
        }

        private static void SoftDelete<T>(ModelBuilder modelBuilder) where T : class
        {
            modelBuilder.Entity<T>()
                .HasQueryFilter(u => EF.Property<DateTime?>(u, "DeletedAt") == null);
        }

        private static Expression<Func<T, object>> CreateExpression<T>(string uuid) where T : class
        {
            var type = typeof(T);
            var property = type.GetProperty(uuid);
            var parameter = Expression.Parameter(type);
            var access = Expression.Property(parameter, property);
            var convert = Expression.Convert(access, typeof(object));
            var function = Expression.Lambda<Func<T, object>>(convert, parameter);

            return function;
        }
    }
}

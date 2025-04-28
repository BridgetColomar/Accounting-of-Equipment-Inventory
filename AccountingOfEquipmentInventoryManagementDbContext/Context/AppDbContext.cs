using AccountingOfEquipmentInventoryManagementLib.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AccountingOfEquipmentInventoryManagementDbContext.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options)
        : base(options)
        {
        }

        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<EquipmentCategory> EquipmentCategories { get; set; }
        public DbSet<InventoryRecord> InventoryRecords { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Конфигурация Equipment
            modelBuilder.Entity<Equipment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.SerialNumber).HasMaxLength(100);
                // Связь с категорией (опционально, если оборудование может не иметь категории)
                entity.HasOne(e => e.Category)
                      .WithMany()
                      .HasForeignKey("CategoryId")
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Конфигурация EquipmentCategory
            modelBuilder.Entity<EquipmentCategory>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired();
            });

            // Конфигурация InventoryRecord
            modelBuilder.Entity<InventoryRecord>(entity =>
            {
                entity.HasKey(ir => ir.Id);
                entity.HasOne(ir => ir.Equipment)
                      .WithMany() // Если требуется, можно добавить коллекцию InventoryRecords в Equipment
                      .HasForeignKey("EquipmentId")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Конфигурация Employee
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(emp => emp.Id);
                entity.Property(emp => emp.FullName).IsRequired();
                entity.Property(emp => emp.Username).IsRequired();
                entity.Property(emp => emp.PasswordHash).IsRequired();
            });
        }
    }
}

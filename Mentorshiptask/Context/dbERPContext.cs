﻿using Mentorshiptask.Model;
//using Mentrship_task1.Model;
using Microsoft.EntityFrameworkCore;

public class dbERPContext : DbContext
{
    public dbERPContext(DbContextOptions<dbERPContext> options) : base(options) { }

    public DbSet<Product> TblProducts { get; set; }
    public DbSet<Order> TblOrders { get; set; }

   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // One-to-Many: Product has many Orders
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Product)
            .WithMany(p => p.Orders)
            .HasForeignKey(o => o.ProductId) // Use Guid for foreign key
            .OnDelete(DeleteBehavior.Cascade); // Adjust delete behavior as needed

        // Optional: Set table names if needed
        modelBuilder.Entity<Product>().ToTable("Products");
        modelBuilder.Entity<Order>().ToTable("Orders");

        //  modelBuilder.Entity<Product>().HasData(
        //    new Product
        //    {
        //        IntProductId = 1,
        //        StrProductName = "Laptop",
        //        NumUnitPrice = 1200.50m,
        //        NumStock = 50
        //    },
        //    new Product
        //    {
        //        IntProductId = 2,
        //        StrProductName = "Smartphone",
        //        NumUnitPrice = 800.00m,
        //        NumStock = 100
        //    }
        //);


        //  modelBuilder.Entity<Order>().HasData(
        //      new Order
        //      {
        //          IntOrderId = 1,
        //          IntProductId = 1,
        //          StrCustomerName = "John Doe",
        //          NumQuantity = 2,
        //          DtOrderDate = DateTime.Now.AddDays(-10)
        //      },
        //      new Order
        //      {
        //          IntOrderId = 2,
        //          IntProductId = 2,
        //          StrCustomerName = "Jane Smith",
        //          NumQuantity = 1,
        //          DtOrderDate = DateTime.Now.AddDays(-5)
        //      }
        //);
    }
}
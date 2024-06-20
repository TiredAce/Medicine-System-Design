using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace MVC_Test.Models;

public partial class DrugSystemContext : DbContext
{
    public DrugSystemContext()
    {
    }

    public DrugSystemContext(DbContextOptions<DrugSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Medicine> Medicines { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("persist security info=True;data source=localhost;port=3306;initial catalog=drug_system;user id=root;password=123456", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.37-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Cid).HasName("PRIMARY");

            entity.ToTable("customer");

            entity.HasIndex(e => e.Telephone, "telephone").IsUnique();

            entity.Property(e => e.Cid).HasMaxLength(20);
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.Cname).HasMaxLength(100);
            entity.Property(e => e.DetailedAddress)
                .HasMaxLength(255)
                .HasColumnName("detailed_address");
            entity.Property(e => e.District)
                .HasMaxLength(100)
                .HasColumnName("district");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Province)
                .HasMaxLength(100)
                .HasColumnName("province");
            entity.Property(e => e.Telephone)
                .HasMaxLength(20)
                .HasColumnName("telephone");
        });

        modelBuilder.Entity<Medicine>(entity =>
        {
            entity.HasKey(e => e.Mid).HasName("PRIMARY");

            entity.ToTable("medicine");

            entity.HasIndex(e => e.Sid, "Sid");

            entity.Property(e => e.Mid).HasMaxLength(20);
            entity.Property(e => e.Details)
                .HasColumnType("text")
                .HasColumnName("details");
            entity.Property(e => e.Mname).HasMaxLength(100);
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Sid).HasMaxLength(20);

            entity.HasOne(d => d.SidNavigation).WithMany(p => p.Medicines)
                .HasForeignKey(d => d.Sid)
                .HasConstraintName("medicine_ibfk_1");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Oid).HasName("PRIMARY");

            entity.ToTable("order");

            entity.HasIndex(e => e.Cid, "Cid");

            entity.HasIndex(e => e.Mid, "Mid");

            entity.HasIndex(e => e.Sid, "Sid");

            entity.Property(e => e.Cid).HasMaxLength(20);
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.DetailedAddress)
                .HasMaxLength(255)
                .HasColumnName("detailed_address");
            entity.Property(e => e.District)
                .HasMaxLength(100)
                .HasColumnName("district");
            entity.Property(e => e.Mid).HasMaxLength(20);
            entity.Property(e => e.Mname).HasMaxLength(100);
            entity.Property(e => e.Number).HasColumnName("number");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.Province)
                .HasMaxLength(100)
                .HasColumnName("province");
            entity.Property(e => e.Sid).HasMaxLength(20);
            entity.Property(e => e.State)
                .HasColumnType("enum('Pending payment','Pending delivery','Transitting delivery','Received delivery')")
                .HasColumnName("state");
            entity.Property(e => e.Time)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("time");

            entity.HasOne(d => d.CidNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Cid)
                .HasConstraintName("order_ibfk_1");

            entity.HasOne(d => d.MidNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Mid)
                .HasConstraintName("order_ibfk_2");

            entity.HasOne(d => d.SidNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Sid)
                .HasConstraintName("order_ibfk_3");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Sid).HasName("PRIMARY");

            entity.ToTable("supplier");

            entity.HasIndex(e => e.Telephone, "telephone").IsUnique();

            entity.Property(e => e.Sid).HasMaxLength(20);
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            entity.Property(e => e.ContactPerson)
                .HasMaxLength(100)
                .HasColumnName("contact_person");
            entity.Property(e => e.DetailedAddress)
                .HasMaxLength(255)
                .HasColumnName("detailed_address");
            entity.Property(e => e.District)
                .HasMaxLength(50)
                .HasColumnName("district");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Province)
                .HasMaxLength(50)
                .HasColumnName("province");
            entity.Property(e => e.Sname).HasMaxLength(100);
            entity.Property(e => e.Telephone)
                .HasMaxLength(20)
                .HasColumnName("telephone");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

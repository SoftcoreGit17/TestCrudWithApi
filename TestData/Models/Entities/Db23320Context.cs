using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TestData.Models.Entities;

public partial class Db23320Context : DbContext
{
    public Db23320Context()
    {
    }

    public Db23320Context(DbContextOptions<Db23320Context> options)
        : base(options)
    {
    }

    public virtual DbSet<ContactU> ContactUs { get; set; }

    public virtual DbSet<CustomerRe> CustomerRes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=db23320.public.databaseasp.net;Database=db23320;User Id=db23320;Password=X-g7oB4!N8%j;Encrypt=True;TrustServerCertificate=True;Command Timeout=180;MultipleActiveResultSets=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ContactU>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ContactU__3213E83F7E953C50");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Message).HasMaxLength(250);
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<CustomerRe>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Customer__3213E83F4E3449DE");

            entity.ToTable("CustomerRe");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.CustomerName).HasMaxLength(200);
            entity.Property(e => e.Password).HasMaxLength(200);
            entity.Property(e => e.RefreshToken).HasMaxLength(200);
            entity.Property(e => e.RefreshTokenExpiry).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

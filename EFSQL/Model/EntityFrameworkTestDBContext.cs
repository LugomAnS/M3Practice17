using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFSQL.Model
{
    public partial class EntityFrameworkTestDBContext : DbContext
    {
        public EntityFrameworkTestDBContext()
        {
        }

        public EntityFrameworkTestDBContext(DbContextOptions<EntityFrameworkTestDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Clients> Clients { get; set; }

        //"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=EntityFrameworkTestDB"

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=EntityFrameworkTestDB;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Clients>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ClientName)
                    .IsRequired()
                    .HasColumnName("clientName")
                    .HasMaxLength(15);

                entity.Property(e => e.ClientPatronymic)
                    .IsRequired()
                    .HasColumnName("clientPatronymic")
                    .HasMaxLength(15);

                entity.Property(e => e.ClientSurname)
                    .IsRequired()
                    .HasColumnName("clientSurname")
                    .HasMaxLength(15);

                entity.Property(e => e.EMail)
                    .IsRequired()
                    .HasColumnName("eMail")
                    .HasMaxLength(20);

                entity.Property(e => e.Phone)
                    .HasColumnName("phone")
                    .HasMaxLength(15);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

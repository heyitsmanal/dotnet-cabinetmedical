using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CabinetMedical.Models;

namespace cabinetMedical.Models
{
    public partial class MedicalContext : IdentityDbContext
    {
        public MedicalContext()
        {
        }

        public MedicalContext(DbContextOptions<MedicalContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Consultation> Consultations { get; set; }
        public virtual DbSet<DossierMedical> DossiersMedicals { get; set; }
        public virtual DbSet<Facture> Factures { get; set; }
        public virtual DbSet<Infirmiere> Infirmieres { get; set; }
        public virtual DbSet<Medecin> Medecins { get; set; }
        public virtual DbSet<Medicament> Medicaments { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<Planning> Plannings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
            => optionsBuilder.UseSqlServer("Server=DESKTOP-R0V0R37;Database=medical;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True;");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Admin>().ToTable("Admin");
            modelBuilder.Entity<Consultation>().ToTable("Consultation");
            modelBuilder.Entity<DossierMedical>().ToTable("DossierMedical");
            modelBuilder.Entity<Facture>().ToTable("Factures");
            modelBuilder.Entity<Infirmiere>().ToTable("Infirmiere");
            modelBuilder.Entity<Medecin>().ToTable("Medecin");
            modelBuilder.Entity<Medicament>().ToTable("Medicament");
            modelBuilder.Entity<Notification>().ToTable("Notifications");
            modelBuilder.Entity<Patient>().ToTable("Patient");
            modelBuilder.Entity<Planning>().ToTable("Planning");

            modelBuilder.Entity<Facture>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_Factures");

                entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.Date).HasColumnType("datetime");
                entity.Property(e => e.PatientId).HasMaxLength(450);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_Notifications");

                entity.Property(e => e.Date).HasColumnType("datetime");
                entity.Property(e => e.MedecinId).HasMaxLength(450);
                entity.Property(e => e.Message).HasDefaultValue("Votre rendez-vous de (date) est à venir.");
                entity.Property(e => e.PatientId).HasMaxLength(450);
            });

            modelBuilder.Entity<DossierMedical>().HasKey(e => e.Id);
            modelBuilder.Entity<Medicament>().HasKey(e => e.Id);
            modelBuilder.Entity<Planning>().HasKey(e => e.Id);
            modelBuilder.Entity<Patient>().HasKey(e => e.Id);
            modelBuilder.Entity<Medecin>().HasKey(e => e.Id);
            modelBuilder.Entity<Infirmiere>().HasKey(e => e.Id);
            modelBuilder.Entity<Admin>().HasKey(e => e.Id);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

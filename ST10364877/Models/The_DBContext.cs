using Microsoft.EntityFrameworkCore;
using ST10364877.Models;

namespace ST10364877
{
    public class The_DBContext : DbContext
    {
        public The_DBContext(DbContextOptions<The_DBContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<IncidentReport> IncidentReports { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User table configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.Property(u => u.Username)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(255);
                entity.Property(u => u.PasswordHash)
                    .IsRequired();
                entity.Property(u => u.FirstName)
                    .HasMaxLength(50);
                entity.Property(u => u.LastName)
                    .HasMaxLength(50);
                entity.Property(u => u.PhoneNumber)
                    .HasMaxLength(20);
                entity.Property(u => u.CreatedAt)
                    .IsRequired();
                entity.Property(u => u.IsActive)
                    .IsRequired();

                // Unique constraints
                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();
            });

            // IncidentReport table configuration
            modelBuilder.Entity<IncidentReport>(entity =>
            {
                entity.HasKey(ir => ir.IncidentId);
                entity.Property(ir => ir.Title)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(ir => ir.Description)
                    .IsRequired();
                entity.Property(ir => ir.Location)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(ir => ir.IncidentDate)
                    .IsRequired();
                entity.Property(ir => ir.DisasterType)
                    .HasMaxLength(50);
                entity.Property(ir => ir.AffectedAreas)
                    .HasMaxLength(500);
                entity.Property(ir => ir.UrgencyLevel)
                    .HasMaxLength(20);
                entity.Property(ir => ir.ReportedAt)
                    .IsRequired();
                entity.Property(ir => ir.Status)
                    .HasMaxLength(50)
                    .HasDefaultValue("Pending");

                // Relationships
                entity.HasOne(ir => ir.User)
                    .WithMany(u => u.IncidentReports)
                    .HasForeignKey(ir => ir.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Contribution table configuration
            modelBuilder.Entity<Donation>(entity =>
            {
                entity.HasKey(d => d.DonationId);
                entity.Property(d => d.DonationType)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(d => d.ItemDescription)
                    .HasMaxLength(500);
                entity.Property(d => d.Quantity)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");
                entity.Property(d => d.Unit)
                    .HasMaxLength(50);
                entity.Property(d => d.TargetArea)
                    .HasMaxLength(100);
                entity.Property(d => d.DonationDate)
                    .IsRequired();
                entity.Property(d => d.Status)
                    .HasMaxLength(50)
                    .HasDefaultValue("Pending");
                entity.Property(d => d.SpecialInstructions)
                    .HasMaxLength(1000);

                // Relationships
                entity.HasOne(d => d.User)
                    .WithMany(u => u.Donations)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Volunteer table configuration
            modelBuilder.Entity<Volunteer>(entity =>
            {
                entity.HasKey(v => v.VolunteerId);
                entity.Property(v => v.Skills)
                    .HasMaxLength(500);
                entity.Property(v => v.Availability)
                    .HasMaxLength(50);
                entity.Property(v => v.PreferredLocation)
                    .HasMaxLength(100);
                entity.Property(v => v.EmergencyContact)
                    .HasMaxLength(200);
                entity.Property(v => v.RegisteredAt)
                    .IsRequired();
                entity.Property(v => v.Status)
                    .HasMaxLength(50)
                    .HasDefaultValue("Active");

                // Relationships
                entity.HasOne(v => v.User)
                    .WithMany(u => u.VolunteerRegistrations)
                    .HasForeignKey(v => v.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Ensure one user can only have one volunteer registration
                entity.HasIndex(v => v.UserId).IsUnique();
            });

            // Configure cascading behaviors and additional constraints
            ConfigureCascadingBehaviors(modelBuilder);
        }

        private void ConfigureCascadingBehaviors(ModelBuilder modelBuilder)
        {
            // Set default delete behavior to Restrict to prevent accidental data loss
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
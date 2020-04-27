using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

using Cnf.Finance.Entity;

namespace Cnf.Finance.Api.Models
{
    public partial class FinanceContext : DbContext
    {
        public FinanceContext()
        {
        }

        public FinanceContext(DbContextOptions<FinanceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AnnualBalance> AnnualBalance { get; set; }
        public virtual DbSet<Organization> Organization { get; set; }
        public virtual DbSet<Perform> Perform { get; set; }
        public virtual DbSet<PerformTerms> PerformTerms { get; set; }
        public virtual DbSet<Plan> Plan { get; set; }
        public virtual DbSet<PlanTerms> PlanTerms { get; set; }
        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<Terms> Terms { get; set; }
        public virtual DbSet<Users> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnnualBalance>(entity =>
            {
                entity.HasIndex(e => new { e.Year, e.ProjectId })
                    .HasName("IX_AnnualBalance")
                    .IsUnique(false);

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Balance).HasColumnType("money");

                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.AnnualBalance)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AB_Proj");
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Perform>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Incoming).HasColumnType("money");

                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.Property(e => e.Retrieve).HasColumnType("money");

                entity.Property(e => e.Settlement).HasColumnType("money");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Perform)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_Perform_Proj");
            });

            modelBuilder.Entity<PerformTerms>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.PerformId).HasColumnName("PerformID");

                entity.Property(e => e.TermsId).HasColumnName("TermsID");

                entity.HasOne(d => d.Perform)
                    .WithMany(p => p.PerformTerms)
                    .HasForeignKey(d => d.PerformId)
                    .HasConstraintName("FK_PerformTerms_Perform");

                entity.HasOne(d => d.Terms)
                    .WithMany(p => p.PerformTerms)
                    .HasForeignKey(d => d.TermsId)
                    .HasConstraintName("FK_PerformTerms_Terms");
            });

            modelBuilder.Entity<Plan>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Incoming).HasColumnType("money");

                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.Property(e => e.Retrieve).HasColumnType("money");

                entity.Property(e => e.Settlement).HasColumnType("money");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Plan)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_Plan_Proj");
            });

            modelBuilder.Entity<PlanTerms>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.PlanId).HasColumnName("PlanID");

                entity.Property(e => e.TermsId).HasColumnName("TermsID");

                entity.HasOne(d => d.Plain)
                    .WithMany(p => p.PlanTerms)
                    .HasForeignKey(d => d.PlanId)
                    .HasConstraintName("FK_PlanTerms_Plan");

                entity.HasOne(d => d.Terms)
                    .WithMany(p => p.PlanTerms)
                    .HasForeignKey(d => d.TermsId)
                    .HasConstraintName("FK_PlanTerms_Terms");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.Property(e => e.CloseDate).HasColumnType("date");

                entity.Property(e => e.ContractAmount).HasColumnType("money");

                entity.Property(e => e.ContractCode)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");

                entity.Property(e => e.ProjectManager)
                    .IsRequired()
                    .HasColumnName("PM")
                    .HasMaxLength(20);

                entity.Property(e => e.ShortName)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Project)
                    .HasForeignKey(d => d.OrganizationId)
                    .HasConstraintName("FK_Proj_Org");
            });

            modelBuilder.Entity<Terms>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.Property(e => e.TargetAmount).HasColumnType("money");

                entity.Property(e => e.TargetDate).HasColumnType("date");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Terms)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_Terms_Proj");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(u => u.UserId);

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.OrganizationId)
                    .HasConstraintName("FK_Users_Org");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

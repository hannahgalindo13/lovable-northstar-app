using System;
using System.Collections.Generic;
using Backend.Models;
using Backend.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public partial class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Donation> Donations { get; set; }

    public virtual DbSet<DonationAllocation> DonationAllocations { get; set; }

    public virtual DbSet<EducationRecord> EducationRecords { get; set; }

    public virtual DbSet<HealthWellbeingRecord> HealthWellbeingRecords { get; set; }

    public virtual DbSet<HomeVisitation> HomeVisitations { get; set; }

    public virtual DbSet<InKindDonationItem> InKindDonationItems { get; set; }

    public virtual DbSet<IncidentReport> IncidentReports { get; set; }

    public virtual DbSet<InterventionPlan> InterventionPlans { get; set; }

    public virtual DbSet<Partner> Partners { get; set; }

    public virtual DbSet<PartnerAssignment> PartnerAssignments { get; set; }

    public virtual DbSet<ProcessRecording> ProcessRecordings { get; set; }

    public virtual DbSet<PublicImpactSnapshot> PublicImpactSnapshots { get; set; }

    public virtual DbSet<Resident> Residents { get; set; }

    public virtual DbSet<Safehouse> Safehouses { get; set; }

    public virtual DbSet<SafehouseMonthlyMetric> SafehouseMonthlyMetrics { get; set; }

    public virtual DbSet<SocialMediaPost> SocialMediaPosts { get; set; }

    public virtual DbSet<Supporter> Supporters { get; set; }

    public virtual DbSet<__EFMigrationsLock> __EFMigrationsLocks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Donation>(entity =>
        {
            entity.HasIndex(e => e.SupporterId, "IX_Donations_SupporterId");

            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.EstimatedValue).HasColumnType("decimal(18,2)");

            entity.HasOne(d => d.Supporter).WithMany(p => p.Donations)
                .HasForeignKey(d => d.SupporterId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DonationAllocation>(entity =>
        {
            entity.HasKey(e => e.AllocationId);

            entity.HasIndex(e => e.DonationId, "IX_DonationAllocations_DonationId");

            entity.HasIndex(e => e.SafehouseId, "IX_DonationAllocations_SafehouseId");

            entity.Property(e => e.AmountAllocated).HasColumnType("decimal(18,2)");

            entity.HasOne(d => d.Donation).WithMany(p => p.DonationAllocations)
                .HasForeignKey(d => d.DonationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Safehouse).WithMany(p => p.DonationAllocations)
                .HasForeignKey(d => d.SafehouseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EducationRecord>(entity =>
        {
            entity.HasIndex(e => e.ResidentId, "IX_EducationRecords_ResidentId");

            entity.Property(e => e.AttendanceRate).HasColumnType("decimal(5,4)");
            entity.Property(e => e.ProgressPercent).HasColumnType("decimal(5,2)");

            entity.HasOne(d => d.Resident).WithMany(p => p.EducationRecords).HasForeignKey(d => d.ResidentId);
        });

        modelBuilder.Entity<HealthWellbeingRecord>(entity =>
        {
            entity.HasKey(e => e.HealthRecordId);

            entity.HasIndex(e => e.ResidentId, "IX_HealthWellbeingRecords_ResidentId");

            entity.Property(e => e.Bmi).HasColumnType("decimal(4,2)");
            entity.Property(e => e.EnergyLevelScore).HasColumnType("decimal(3,2)");
            entity.Property(e => e.GeneralHealthScore).HasColumnType("decimal(3,2)");
            entity.Property(e => e.HeightCm).HasColumnType("decimal(5,2)");
            entity.Property(e => e.NutritionScore).HasColumnType("decimal(3,2)");
            entity.Property(e => e.SleepQualityScore).HasColumnType("decimal(3,2)");
            entity.Property(e => e.WeightKg).HasColumnType("decimal(5,2)");

            entity.HasOne(d => d.Resident).WithMany(p => p.HealthWellbeingRecords).HasForeignKey(d => d.ResidentId);
        });

        modelBuilder.Entity<HomeVisitation>(entity =>
        {
            entity.HasKey(e => e.VisitationId);

            entity.HasIndex(e => e.ResidentId, "IX_HomeVisitations_ResidentId");

            entity.Property(e => e.CoordinationKind).HasDefaultValue("HomeVisit");

            entity.HasOne(d => d.Resident).WithMany(p => p.HomeVisitations).HasForeignKey(d => d.ResidentId);
        });

        modelBuilder.Entity<InKindDonationItem>(entity =>
        {
            entity.HasKey(e => e.ItemId);

            entity.HasIndex(e => e.DonationId, "IX_InKindDonationItems_DonationId");

            entity.Property(e => e.EstimatedUnitValue).HasColumnType("decimal(18,2)");

            entity.HasOne(d => d.Donation).WithMany(p => p.InKindDonationItems).HasForeignKey(d => d.DonationId);
        });

        modelBuilder.Entity<IncidentReport>(entity =>
        {
            entity.HasKey(e => e.IncidentId);

            entity.HasIndex(e => e.ResidentId, "IX_IncidentReports_ResidentId");

            entity.HasIndex(e => e.SafehouseId, "IX_IncidentReports_SafehouseId");

            entity.HasOne(d => d.Resident).WithMany(p => p.IncidentReports)
                .HasForeignKey(d => d.ResidentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Safehouse).WithMany(p => p.IncidentReports)
                .HasForeignKey(d => d.SafehouseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<InterventionPlan>(entity =>
        {
            entity.HasKey(e => e.PlanId);

            entity.HasIndex(e => e.ResidentId, "IX_InterventionPlans_ResidentId");

            entity.Property(e => e.TargetValue).HasColumnType("decimal(10,2)");

            entity.HasOne(d => d.Resident).WithMany(p => p.InterventionPlans).HasForeignKey(d => d.ResidentId);
        });

        modelBuilder.Entity<PartnerAssignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId);

            entity.HasIndex(e => e.PartnerId, "IX_PartnerAssignments_PartnerId");

            entity.HasIndex(e => e.SafehouseId, "IX_PartnerAssignments_SafehouseId");

            entity.HasOne(d => d.Partner).WithMany(p => p.PartnerAssignments).HasForeignKey(d => d.PartnerId);

            entity.HasOne(d => d.Safehouse).WithMany(p => p.PartnerAssignments)
                .HasForeignKey(d => d.SafehouseId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ProcessRecording>(entity =>
        {
            entity.HasKey(e => e.RecordingId);

            entity.HasIndex(e => e.ResidentId, "IX_ProcessRecordings_ResidentId");

            entity.HasOne(d => d.Resident).WithMany(p => p.ProcessRecordings).HasForeignKey(d => d.ResidentId);
        });

        modelBuilder.Entity<PublicImpactSnapshot>(entity =>
        {
            entity.HasKey(e => e.SnapshotId);
        });

        modelBuilder.Entity<Resident>(entity =>
        {
            entity.HasIndex(e => e.SafehouseId, "IX_Residents_SafehouseId");

            entity.HasOne(d => d.Safehouse).WithMany(p => p.Residents).HasForeignKey(d => d.SafehouseId);
        });

        modelBuilder.Entity<SafehouseMonthlyMetric>(entity =>
        {
            entity.HasKey(e => e.MetricId);

            entity.HasIndex(e => e.SafehouseId, "IX_SafehouseMonthlyMetrics_SafehouseId");

            entity.Property(e => e.AvgEducationProgress).HasColumnType("decimal(5,2)");
            entity.Property(e => e.AvgHealthScore).HasColumnType("decimal(3,2)");

            entity.HasOne(d => d.Safehouse).WithMany(p => p.SafehouseMonthlyMetrics).HasForeignKey(d => d.SafehouseId);
        });

        modelBuilder.Entity<SocialMediaPost>(entity =>
        {
            entity.HasKey(e => e.PostId);

            entity.Property(e => e.BoostBudgetPhp).HasColumnType("decimal(10,2)");
            entity.Property(e => e.EngagementRate).HasColumnType("decimal(8,6)");
            entity.Property(e => e.EstimatedDonationValuePhp).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<__EFMigrationsLock>(entity =>
        {
            entity.ToTable("__EFMigrationsLock");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

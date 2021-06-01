﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace CIS655Project.Models
{
    public partial class Team116dbContext : DbContext
    {
        public Team116dbContext()
        {
        }

        public Team116dbContext(DbContextOptions<Team116dbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Business> Businesses { get; set; }
        public virtual DbSet<BusinessRole> BusinessRoles { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<JobApplication> JobApplications { get; set; }
        public virtual DbSet<JobCategory> JobCategories { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Business>(entity =>
            {
                entity.Property(e => e.City).IsUnicode(false);

                entity.Property(e => e.Country).IsUnicode(false);

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.State).IsUnicode(false);

                entity.Property(e => e.Street).IsUnicode(false);
            });

            modelBuilder.Entity<BusinessRole>(entity =>
            {
                entity.HasKey(e => e.BusId)
                    .HasName("PK__Business__6A0F60B5682AF51C");

                entity.Property(e => e.BusId).ValueGeneratedNever();

                entity.HasOne(d => d.Bus)
                    .WithOne(p => p.BusinessRole)
                    .HasForeignKey<BusinessRole>(d => d.BusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Table_Business");
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.Property(e => e.JobType).IsUnicode(false);

                entity.HasOne(d => d.Bus)
                    .WithMany(p => p.Jobs)
                    .HasForeignKey(d => d.BusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Job_Business");

                entity.HasOne(d => d.JobCat)
                    .WithMany(p => p.Jobs)
                    .HasForeignKey(d => d.JobCatId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Job_JobCategory");
            });

            modelBuilder.Entity<JobApplication>(entity =>
            {
                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobApplications)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_JobApplication_Job");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.JobApplications)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_JobApplication_User");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.City).IsUnicode(false);

                entity.Property(e => e.Country).IsUnicode(false);

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.State).IsUnicode(false);

                entity.Property(e => e.Street).IsUnicode(false);
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__UserRole__1788CC4CC611AC79");

                entity.Property(e => e.UserId).ValueGeneratedNever();

                entity.HasOne(d => d.User)
                    .WithOne(p => p.UserRole)
                    .HasForeignKey<UserRole>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Table_Users");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
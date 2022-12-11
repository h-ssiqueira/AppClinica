﻿using Contracts.Entities.Attributes;
using Microsoft.EntityFrameworkCore;
using Contracts.Entities;
using Repository.Utills;

namespace Repository.Context
{
    public class TemplateDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Patient> Patients { get; set; }
        //public DbSet<PatientOrder> PatientOrders { get; set; }
        public DbSet<PatientRequest> PatientRequests { get; set; }
        public DbSet<Professor> Professors { get; set; }
        public DbSet<ScheduleProfessor> ScheduleProfessors { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Student> Students { get; set; }

        public TemplateDbContext(DbContextOptions<TemplateDbContext> options) : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    if (property.PropertyInfo != null)
                    {
                        var attributes = property
                        .PropertyInfo
                        .GetCustomAttributes(typeof(SensitiveDataAttribute), false);

                        if (attributes.Length > 0)
                        {
                            property.SetValueConverter(new DataProtectionConverter());
                        }
                    }
                }
            }
            //Appointment
            modelBuilder.Entity<Appointment>()
                .HasOne(s => s.Student)
                .WithMany(a => a.Appointments);
            modelBuilder.Entity<Appointment>()
                .HasOne(e => e.Employee)
                .WithMany(a => a.Appointments);
            modelBuilder.Entity<Appointment>()
                .HasOne(p => p.Patient)
                .WithMany(a => a.Appointments);
            modelBuilder.Entity<Appointment>()
                .HasOne(p => p.Schedule)
                .WithMany(a => a.Appointment);

            //Notification
            modelBuilder.Entity<Notification>()
                .HasOne(a => a.Appointment)
                .WithMany(b => b.Notifications);
            modelBuilder.Entity<Notification>()
                .HasOne(a => a.PatientRequest)
                .WithMany(b => b.Notifications);

            //PatientRequest
            modelBuilder.Entity<PatientRequest>()
                .HasOne(ss => ss.Student)
                .WithMany(b => b.PatientRequests)
                .HasForeignKey(sp => sp.StudentId);
            modelBuilder.Entity<PatientRequest>()
                .HasMany(s => s.Notifications)
                .WithOne(pr => pr.PatientRequest);

            // Schedule Professor
            modelBuilder.Entity<ScheduleProfessor>()
                .HasKey(bc => new { bc.ScheduleId, bc.ProfessorId });
            modelBuilder.Entity<ScheduleProfessor>()
                .HasOne(sp => sp.Professor)
                .WithMany(p => p.ScheduleProfessors)
                .HasForeignKey(sp => sp.ProfessorId);
            modelBuilder.Entity<ScheduleProfessor>()
                .HasOne(sp => sp.Schedule)
                .WithMany(p => p.ScheduleProfessors)
                .HasForeignKey(sp => sp.ScheduleId);

            modelBuilder.Entity<Appointment>()
                .Property(e => e.Status)
                .HasConversion<string>();

        }
    }
}

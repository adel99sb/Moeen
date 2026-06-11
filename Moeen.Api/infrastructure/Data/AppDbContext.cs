using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Entities;

namespace Moeen.Api.infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Mosque> Mosques { get; set; }
        public DbSet<MosqueUser> MosqueUsers { get; set; }
        public DbSet<ParentStudent> ParentStudents { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<ProgressEntry> ProgressEntries { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<MosqueUser>()
                            .HasKey(mu => new { mu.MosqueId, mu.UserId });

            modelBuilder.Entity<ParentStudent>()
                .HasKey(ps => new { ps.ParentId, ps.StudentId });

            modelBuilder.Entity<ParentStudent>()
                .HasOne(ps => ps.Parent)
                .WithMany(u => u.AsParentStudents)
                .HasForeignKey(ps => ps.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ParentStudent>()
                .HasOne(ps => ps.Student)
                .WithMany(u => u.AsChildStudents)
                .HasForeignKey(ps => ps.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProgressEntry>()
                .HasOne(p => p.Student)
                .WithMany()
                .HasForeignKey(p => p.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProgressEntry>()
                .HasOne(p => p.Teacher)
                .WithMany()
                .HasForeignKey(p => p.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Exam>()
                .HasOne(e => e.Student)
                .WithMany()
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Exam>()
                .HasOne(e => e.Teacher)
                .WithMany()
                .HasForeignKey(e => e.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
            //foreach (var foreignKey in modelBuilder.Model
            //    .GetEntityTypes()
            //    .SelectMany(e => e.GetForeignKeys()))
            //{
            //    foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            //}            
        }
    }
}
//Add-Migration InitialCreate -OutputDir infrastructure/Data/Migrations

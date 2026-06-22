using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Enities;
using Moeen.Api.Core.Entities;

namespace Moeen.Api.infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Fouj> Foujs { get; set; }
        public DbSet<Halqa> Halqas { get; set; }
        public DbSet<HalqaSession> HalqaSessions { get; set; }
        public DbSet<Mosque> Mosques { get; set; }
        public DbSet<PdfFile> PdfFiles { get; set; }
        public DbSet<PosInteraction> PosInteractions { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<ProgressEntry> ProgressEntries { get; set; }
        public DbSet<SaturdayHalqa> SaturdayHalqes { get; set; }
        public DbSet<ParentSudent> ParentSudents { get; set; }
        public DbSet<WeeklyLesson> WeeklyLessons { get; set; }

        public DbSet<Student> Students { get; set; }
        public DbSet<Supervisor> Supervisors { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public new DbSet<User> Users { get; set; }
        public DbSet <TeacherExam> TeacherExams { get; set; }
        public DbSet<ExamTeacherHalqa> ExamTeacherHalqa { get; set; }
        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Student>().ToTable("Students");
            modelBuilder.Entity<Teacher>().ToTable("Teachers");
            modelBuilder.Entity<Supervisor>().ToTable("Supervisors");

            foreach (var foreignKey in modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }            
        }
    }
}
//Add-Migration InitialCreate -OutputDir infrastructure/Data/Migrations

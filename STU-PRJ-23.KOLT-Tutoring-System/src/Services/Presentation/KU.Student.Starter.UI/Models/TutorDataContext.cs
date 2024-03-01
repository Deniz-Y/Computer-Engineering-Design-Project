using Microsoft.EntityFrameworkCore;

namespace KU.Student.Starter.UI.Models
{
    public class TutorDataContext : DbContext
    {
        public TutorDataContext(DbContextOptions<TutorDataContext> options) :
          base(options)
        {
        }
        public DbSet<Tutor> Tutors { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Period> Periods { get; set; }
        public DbSet<CourseTutor> TutorCourses { get; set; }
        public DbSet<PeriodTutor> TutorPeriods { get; set; }
        public DbSet<PeriodCubicle> PeriodCubicles { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<Cubicle> Cubicles { get; set; }
        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<EditScheduleTexts> EditScheduleTexts { get; set; }
        public DbSet<HeadTutor> HeadTutors { get; set; }
        public DbSet<HeadTutorConnection> HeadTutorConnections { get; set; }
        public DbSet<TutorApplication> TutorApplications { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseSerialColumns();
             modelBuilder.Entity<CourseTutor>()
                  .HasKey(tc => new { tc.TutorId, tc.CourseId });

              modelBuilder.Entity<CourseTutor>()
                  .HasOne(tc => tc.Tutor)
                  .WithMany(t => t.Courses)
                  .HasForeignKey(tc => tc.TutorId);

              modelBuilder.Entity<CourseTutor>()
                  .HasOne(tc => tc.Course)
                  .WithMany(c => c.Tutors)
                  .HasForeignKey(tc => tc.CourseId);


            modelBuilder.Entity<PeriodTutor>()
                 .HasKey(tc => new { tc.PeriodTutorId });

            modelBuilder.Entity<PeriodTutor>()
                .HasOne(tc => tc.Tutor)
                .WithMany(t => t.Periods)
                .HasForeignKey(tc => tc.TutorId);

            modelBuilder.Entity<PeriodTutor>()
                .HasOne(tc => tc.Period)
                .WithMany(c => c.Tutors)
                .HasForeignKey(tc => tc.PeriodId);


            modelBuilder.Entity<PeriodCubicle>()
                 .HasKey(pc => new {pc.PeriodCubicleId });

            modelBuilder.Entity<PeriodCubicle>()
                .HasOne(pc => pc.PeriodTutor)
                .WithMany(c => c.Cubicles)
                .HasForeignKey(pc => pc.PeriodTutorId);

            modelBuilder.Entity<PeriodCubicle>()
                .HasOne(pc => pc.Cubicle)
                .WithMany(p => p.PeriodTutors)
                .HasForeignKey(pc => pc.CubicleId);

            modelBuilder.Entity<HeadTutorConnection>()
                 .HasKey(pc => new { pc.Id });

            modelBuilder.Entity<HeadTutorConnection>()
                .HasOne(ht=>ht.HeadTutor)
                .WithMany(h => h.Tutors)
                .HasForeignKey(ht => ht.HeadTutorId);

            modelBuilder.Entity<HeadTutorConnection>()
                .HasMany(ht => ht.Tutors)
                .WithOne(t => t.HeadTutor)
                .HasForeignKey(ht => ht.Id);


            modelBuilder.Entity<Course>()
                .HasIndex(tc => tc.Code)
                .IsUnique();

            modelBuilder.Entity<Tutor>()
                .HasIndex(tc => tc.Email)
                .IsUnique();

            modelBuilder.Entity<TutorApplication>()
                .HasIndex(tc => tc.Email)
                .IsUnique();

            modelBuilder.Entity<AdminUser>()
                .HasIndex(tc => tc.Email)
                .IsUnique();

            modelBuilder.Entity<Period>()
              .HasIndex(p => new { p.Day, p.StartHour, p.EndHour })
              .IsUnique()
              .HasName("IX_UniquePeriod");

        }
    }
}

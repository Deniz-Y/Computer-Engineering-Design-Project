﻿// <auto-generated />
using KU.Student.Starter.UI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KU.Student.Starter.UI.Migrations
{
    [DbContext(typeof(TutorDataContext))]
    [Migration("20230505121854_Configuration1")]
    partial class Configuration1
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseSerialColumns(modelBuilder);

            modelBuilder.Entity("KU.Student.Starter.UI.Models.AdminUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("AdminUsers");
                });

            modelBuilder.Entity("KU.Student.Starter.UI.Models.Configuration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("Id"));

                    b.Property<string>("Date")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("NumberOfCubicles")
                        .HasColumnType("integer");

                    b.Property<bool>("PublishSchedule")
                        .HasColumnType("boolean");

                    b.Property<int>("ScheduleSplitCount")
                        .HasColumnType("integer");

                    b.Property<string>("Time")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Configurations");
                });

            modelBuilder.Entity("KU.Student.Starter.UI.Models.Course", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("KU.Student.Starter.UI.Models.CourseTutor", b =>
                {
                    b.Property<int>("TutorId")
                        .HasColumnType("integer");

                    b.Property<int>("CourseId")
                        .HasColumnType("integer");

                    b.HasKey("TutorId", "CourseId");

                    b.HasIndex("CourseId");

                    b.ToTable("TutorCourses");
                });

            modelBuilder.Entity("KU.Student.Starter.UI.Models.Cubicle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("Id"));

                    b.Property<string>("CubicleName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CubicleNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CubiclePlace")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Cubicles");
                });

            modelBuilder.Entity("KU.Student.Starter.UI.Models.EditScheduleTexts", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("Id"));

                    b.Property<string>("BelowTable")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("GoToTop")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("MainTitle")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("NotPublishedText")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PageTitle")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Place")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SubTitle")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("EditScheduleTexts");
                });

            modelBuilder.Entity("KU.Student.Starter.UI.Models.Period", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("Id"));

                    b.Property<string>("DayHour")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Periods");
                });

            modelBuilder.Entity("KU.Student.Starter.UI.Models.PeriodTutor", b =>
                {
                    b.Property<int>("TutorId")
                        .HasColumnType("integer");

                    b.Property<int>("PeriodId")
                        .HasColumnType("integer");

                    b.HasKey("TutorId", "PeriodId");

                    b.HasIndex("PeriodId");

                    b.ToTable("TutorPeriods");
                });

            modelBuilder.Entity("KU.Student.Starter.UI.Models.Tutor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("WeeklyHour")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Tutors");
                });

            modelBuilder.Entity("KU.Student.Starter.UI.Models.CourseTutor", b =>
                {
                    b.HasOne("KU.Student.Starter.UI.Models.Course", "Course")
                        .WithMany("Tutors")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KU.Student.Starter.UI.Models.Tutor", "Tutor")
                        .WithMany("Courses")
                        .HasForeignKey("TutorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("Tutor");
                });

            modelBuilder.Entity("KU.Student.Starter.UI.Models.PeriodTutor", b =>
                {
                    b.HasOne("KU.Student.Starter.UI.Models.Period", "Period")
                        .WithMany("Tutors")
                        .HasForeignKey("PeriodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KU.Student.Starter.UI.Models.Tutor", "Tutor")
                        .WithMany("Periods")
                        .HasForeignKey("TutorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Period");

                    b.Navigation("Tutor");
                });

            modelBuilder.Entity("KU.Student.Starter.UI.Models.Course", b =>
                {
                    b.Navigation("Tutors");
                });

            modelBuilder.Entity("KU.Student.Starter.UI.Models.Period", b =>
                {
                    b.Navigation("Tutors");
                });

            modelBuilder.Entity("KU.Student.Starter.UI.Models.Tutor", b =>
                {
                    b.Navigation("Courses");

                    b.Navigation("Periods");
                });
#pragma warning restore 612, 618
        }
    }
}

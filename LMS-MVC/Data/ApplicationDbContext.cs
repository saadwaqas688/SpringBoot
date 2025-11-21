using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LMS_MVC.Models;

namespace LMS_MVC.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Course> Courses { get; set; }
    public DbSet<UserCourse> UserCourses { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<LessonContent> LessonContents { get; set; }
    public DbSet<DiscussionPost> DiscussionPosts { get; set; }
    public DbSet<LessonProgress> LessonProgresses { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure UserCourse composite key
        builder.Entity<UserCourse>()
            .HasKey(uc => new { uc.UserId, uc.CourseId });

        builder.Entity<UserCourse>()
            .HasOne(uc => uc.User)
            .WithMany(u => u.EnrolledCourses)
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<UserCourse>()
            .HasOne(uc => uc.Course)
            .WithMany(c => c.UserCourses)
            .HasForeignKey(uc => uc.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Course
        builder.Entity<Course>()
            .HasOne(c => c.Creator)
            .WithMany(u => u.CreatedCourses)
            .HasForeignKey(c => c.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Lesson
        builder.Entity<Lesson>()
            .HasOne(l => l.Course)
            .WithMany(c => c.Lessons)
            .HasForeignKey(l => l.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure LessonContent
        builder.Entity<LessonContent>()
            .HasOne(lc => lc.Lesson)
            .WithMany(l => l.Contents)
            .HasForeignKey(lc => lc.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure DiscussionPost
        builder.Entity<DiscussionPost>()
            .HasOne(dp => dp.ParentPost)
            .WithMany(dp => dp.Replies)
            .HasForeignKey(dp => dp.ParentPostId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<DiscussionPost>()
            .HasOne(dp => dp.User)
            .WithMany(u => u.Posts)
            .HasForeignKey(dp => dp.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<DiscussionPost>()
            .HasOne(dp => dp.Lesson)
            .WithMany()
            .HasForeignKey(dp => dp.LessonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<DiscussionPost>()
            .HasOne(dp => dp.LessonContent)
            .WithMany(lc => lc.DiscussionPosts)
            .HasForeignKey(dp => dp.ContentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure LessonProgress composite key
        builder.Entity<LessonProgress>()
            .HasKey(lp => new { lp.UserId, lp.LessonId });

        builder.Entity<LessonProgress>()
            .HasOne(lp => lp.User)
            .WithMany(u => u.LessonProgresses)
            .HasForeignKey(lp => lp.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<LessonProgress>()
            .HasOne(lp => lp.Lesson)
            .WithMany(l => l.LessonProgresses)
            .HasForeignKey(lp => lp.LessonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<LessonProgress>()
            .HasOne(lp => lp.Course)
            .WithMany()
            .HasForeignKey(lp => lp.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}


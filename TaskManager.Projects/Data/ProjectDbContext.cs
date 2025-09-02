using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using TaskManager.Projects.Entities;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace TaskManager.Projects.Data
{
    public class ProjectDbContext : DbContext
    {
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options) { }

        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<ProjectMember>()
        //        .HasOne(pm => pm.Project)
        //        .WithMany(p => p.Members)
        //        .HasForeignKey(pm => pm.ProjectId);
        //}
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using University.Portal.Data.Data.Models;

namespace University.Portal.Data.Data
{
	public class AppDBContext : DbContext
	{
		public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }		
		public DbSet<AppUser> AppUser { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<AppUserRole> AppUserRole { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Student> Student { get; set; }
        public DbSet<UniversityMaster> University { get; set; }
        public DbSet<FeeDetails> FeeDetails { get; set; }
        public DbSet<FeeMaster> FeeMaster { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<FeePayment> FeePayment { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>()
                .HasOne(p => p.AppUserRole)
                .WithOne(b => b.AppUser);

            modelBuilder.Entity<AppUser>()
                .HasOne(p => p.University);

            modelBuilder.Entity<AppUser>()
                .HasOne(p => p.Student);

            modelBuilder.Entity<AppUserRole>()
                .HasOne(p => p.Role);                

            modelBuilder.Entity<AppUserRole>()
                .HasOne(p => p.AppUser);

            modelBuilder.Entity<Student>()
                .HasOne(p => p.Department);

            modelBuilder.Entity<Student>()
                .HasOne(p => p.University);

            modelBuilder.Entity<FeeDetails>()
                .HasOne(p => p.University);
            
            modelBuilder.Entity<FeeDetails>()
                .HasOne(p => p.Department);
            
            modelBuilder.Entity<FeeDetails>()
                .HasOne(p => p.FeeMaster);

            modelBuilder.Entity<Notification>()
                .HasOne(p => p.Student);
            
            modelBuilder.Entity<Notification>()
                .HasOne(p => p.University);

            modelBuilder.Entity<FeePayment>()
                .HasOne(p => p.FeeDetails);
            
            modelBuilder.Entity<FeePayment>()
                .HasOne(p => p.Student);
        }
    }
}

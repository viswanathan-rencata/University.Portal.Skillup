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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>()
                .HasOne(p => p.AppUserRole)
                .WithOne(b => b.AppUser);

            modelBuilder.Entity<AppUser>()
                .HasOne(p => p.University);

            modelBuilder.Entity<AppUserRole>()
                .HasOne(p => p.Role);                

            modelBuilder.Entity<AppUserRole>()
                .HasOne(p => p.AppUser);
        }
    }
}

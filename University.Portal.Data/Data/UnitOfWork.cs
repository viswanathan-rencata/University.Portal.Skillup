using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using University.Portal.Data.Data.Models;
using University.Portal.Data.Interface;

namespace University.Portal.Data.Data
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly AppDBContext _context;		
		public UnitOfWork(AppDBContext context)
		{
			_context = context;			
		}

		public IRepository<AppUser> UserRepository => new Repository<AppUser>(_context);
        public IRepository<Role> RoleRepository => new Repository<Role>(_context);

        public bool Complete()
		{
			return _context.SaveChanges() > 0;
		}
		public async Task<bool> CompleteAsync()
		{
			return await _context.SaveChangesAsync() > 0;
		}
		public bool HasChanges()
		{
			return _context.ChangeTracker.HasChanges();
		}
	}
}

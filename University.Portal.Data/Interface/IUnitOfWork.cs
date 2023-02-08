using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using University.Portal.Data.Data;

namespace University.Portal.Data.Interface
{
	public interface IUnitOfWork
	{
		IRepository<AppUser> UserRepository { get; }
		Task<bool> CompleteAsync();
		bool Complete();
		bool HasChanges();
	}
}

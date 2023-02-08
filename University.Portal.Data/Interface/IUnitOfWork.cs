﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using University.Portal.Data.Data;
using University.Portal.Data.Data.Models;

namespace University.Portal.Data.Interface
{
	public interface IUnitOfWork
	{
		IRepository<AppUser> UserRepository { get; }
        IRepository<Role> RoleRepository { get; }
        Task<bool> CompleteAsync();
		bool Complete();
		bool HasChanges();
	}
}

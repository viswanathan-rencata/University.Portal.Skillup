using Microsoft.EntityFrameworkCore;
using System;
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
        IRepository<UniversityMaster> UniversityRepository { get; }
        IRepository<AppUserRole> AppUserRoleRepository { get; }
        IRepository<Department> DepartmentRepository { get; }
        IRepository<Student> StudentRepository { get; }
        IRepository<TutionFeeDetails> TutionFeeDetailsRepository { get; }
        Task<bool> CompleteAsync();
		bool Complete();
		bool HasChanges();
	}
}

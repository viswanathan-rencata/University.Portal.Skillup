using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Interface
{
	public interface IRepository<TEntity> where TEntity : class
	{
		void Add(TEntity entity);
		
		void AddAsync(TEntity entity);

		void Update(TEntity entity);

		void Remove(TEntity entity);

		TEntity Get(int Id);
		
		Task<TEntity> GetAsync(int Id);
		
		List<TEntity> GetByFilter(Expression<Func<TEntity, bool>> FilterBy, string IncludeStr = null);
		
		Task<List<TEntity>> GetByFilterAsync(Expression<Func<TEntity, bool>> FilterBy, string IncludeStr = null);

		List<TEntity> GetAll(string IncludeStr = null);
		
		Task<List<TEntity>> GetAllAsync(string IncludeStr = null);

	}
}

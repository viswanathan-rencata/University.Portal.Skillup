using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using University.Portal.Data.Interface;
using Microsoft.EntityFrameworkCore;

namespace University.Portal.Data.Data
{
	public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
	{

		private readonly AppDBContext _context;
		public Repository(AppDBContext context)
		{
			_context = context;
		}

		public void Add(TEntity entity)
		{
			_context.Set<TEntity>().Add(entity);
		}

		public void AddAsync(TEntity entity)
		{
			_context.Set<TEntity>().AddAsync(entity);
		}

		public void Remove(TEntity entity)
		{
			_context.Set<TEntity>().Remove(entity);
		}		

		public TEntity Get(int Id)
		{
			return _context.Set<TEntity>().Find(Id);
		}

		public async Task<TEntity> GetAsync(int Id)
		{
			return await _context.Set<TEntity>().FindAsync(Id);
		}

		public TEntity Get(Expression<Func<TEntity, bool>> FilterBy)
		{
			return _context.Set<TEntity>().Where(FilterBy).FirstOrDefault();
		}

		public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> FilterBy)
		{
			return await _context.Set<TEntity>().Where(FilterBy).FirstOrDefaultAsync();
		}

		public List<TEntity> GetAll()
		{
			return _context.Set<TEntity>().ToList();
		}
		public async Task<List<TEntity>> GetAllAsync()
		{
			return await _context.Set<TEntity>().ToListAsync();
		}

		public List<TEntity> GetByFilter(Expression<Func<TEntity, bool>> FilterBy)
		{
			return _context.Set<TEntity>().Where(FilterBy).ToList();
		}

		public async Task<List<TEntity>> GetByFilterAsync(Expression<Func<TEntity, bool>> FilterBy)
		{
			return await _context.Set<TEntity>().Where(FilterBy).ToListAsync();
		}

		public void Update(TEntity entity)
		{
			_context.Set<TEntity>().Update(entity);
		}		
	}
}

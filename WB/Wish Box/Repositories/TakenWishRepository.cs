using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Wish_Box.Repositories;

namespace Wish_Box.Models
{
    public class TakenWishRepository : IRepository<TakenWish>
	{
		private AppDbContext db;

		public TakenWishRepository(AppDbContext context)
		{
			db = context;
		}

		public IEnumerable<TakenWish> GetAll()
		{
			return db.TakenWishes;
		}

		public async Task<TakenWish> Get(int id)
		{
			return await db.TakenWishes.FirstOrDefaultAsync(p => p.Id == id);
		}

		public async Task Create(TakenWish TakenWish)
		{
			db.TakenWishes.Add(TakenWish);
			await db.SaveChangesAsync();
		}

		public async Task Update(TakenWish TakenWish)
		{
			db.Entry(TakenWish).State = EntityState.Modified;
			await db.SaveChangesAsync();
		}

		public async Task<IEnumerable<TakenWish>> Find(Func<TakenWish, Boolean> predicate)
		{
			return await Task.FromResult(db.TakenWishes.Where(predicate).ToList());
		}

		public async Task Delete(int id)
		{
			TakenWish TakenWish = db.TakenWishes.Find(id);

			if (TakenWish != null)
				db.Entry(TakenWish).State = EntityState.Deleted;
			await db.SaveChangesAsync();
		}


		public async Task<TakenWish> FindFirstOrDefault(Expression<Func<TakenWish, bool>> predicate)
		{
			return await db.TakenWishes.FirstOrDefaultAsync(predicate);
		}

	}
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

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

		public TakenWish Get(int id)
		{
			return db.TakenWishes.Find(id);
		}

		public void Create(TakenWish TakenWish)
		{
			db.TakenWishes.Add(TakenWish);
		}

		public void Update(TakenWish TakenWish)
		{
			db.Entry(TakenWish).State = EntityState.Modified;
		}

		public IEnumerable<TakenWish> Find(Func<TakenWish, Boolean> predicate)
		{
			return db.TakenWishes.Where(predicate).ToList();
		}

		public void Delete(int id)
		{
			TakenWish TakenWish = db.TakenWishes.Find(id);

			if (TakenWish != null)
				db.Entry(TakenWish).State = EntityState.Deleted;
		}
	}
}

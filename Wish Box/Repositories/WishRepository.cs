using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wish_Box.Models;

namespace Wish_Box.Repositories
{
    public class WishRepository : IRepository<Wish>
    {
        private AppDbContext db;
        public WishRepository(AppDbContext context)
        {
            db = context;
        }
        public async Task Create(Wish item)
        {
            db.Wishes.Add(item);
            await db.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            Wish wish = db.Wishes.Find(id);

            if (wish != null)
                db.Wishes.Remove(wish);
            await db.SaveChangesAsync();
        }

        public IEnumerable<Wish> Find(Func<Wish, bool> predicate)
        {
            return db.Wishes.Where(predicate).ToList();
        }

        public async Task<Wish> FindFirstOrDefault(System.Linq.Expressions.Expression<Func<Wish, bool>> predicate)
        {
            return await db.Wishes.FirstOrDefaultAsync(predicate);
        }

        public async Task<Wish> Get(int id)
        {
            return await db.Wishes.FirstOrDefaultAsync(p => p.Id == id);
        }

        public IEnumerable<Wish> GetAll()
        {
            return db.Wishes;
        }

        public async Task Update(Wish item)
        {
            db.Entry(item).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }

        
    }
}

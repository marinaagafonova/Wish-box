using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wish_Box.Models;

namespace Wish_Box.Repositories
{
    public class FollowingsRepository : IRepository<Following>
    {
        private AppDbContext db;

        public FollowingsRepository(AppDbContext context)
        {
            db = context;
        }
        public async Task Create(Following item)
        {
            db.Followings.Add(item);
            await db.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            Following following = db.Followings.Find(id);

            if (following != null)
                db.Followings.Remove(following);

            await db.SaveChangesAsync();
        }

        public IEnumerable<Following> Find(Func<Following, bool> predicate)
        {
            return db.Followings.Where(predicate);
        }

        public async Task<Following> FindFirstOrDefault(System.Linq.Expressions.Expression<Func<Following, bool>> predicate)
        {
            return await db.Followings.FirstOrDefaultAsync(predicate, System.Threading.CancellationToken.None);
        }

        public async Task<Following> Get(int id)
        {
            var item = await db.Followings.FindAsync(id);
            return item;
        }

        public IEnumerable<Following> GetAll()
        {
            var list = db.Followings.ToList();

            return list ?? new List<Following>();
        }

        public async Task Update(Following item)
        {
            db.Entry(item).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }
    }
}

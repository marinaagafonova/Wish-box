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
        public async void Create(Following item)
        {
            db.Followings.Add(item);
            await db.SaveChangesAsync();
        }

        public async void Delete(int id)
        {
            Following following = db.Followings.Find(id);

            if (following != null)
                db.Followings.Remove(following);

            await db.SaveChangesAsync();
        }

        public IEnumerable<Following> Find(Func<Following, bool> predicate)
        {
            return db.Followings.Where(predicate).ToList();
        }

        public Following Get(int id)
        {
            Following item = db.Followings.Find(id);
            return item;
        }

        public IEnumerable<Following> GetAll()
        {
            var list = db.Followings.ToList();

            return list ?? new List<Following>();
        }

        public async void Update(Following item)
        {
            db.Entry(item).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }
    }
}

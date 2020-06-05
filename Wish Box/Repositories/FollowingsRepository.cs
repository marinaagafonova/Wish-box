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
        public void Create(Following item)
        {
            db.Followings.Add(item);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            Following item = db.Followings.Find(id);
            db.Followings.Remove(item);
            db.SaveChanges();
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

        public void Update(Following item)
        {
            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}

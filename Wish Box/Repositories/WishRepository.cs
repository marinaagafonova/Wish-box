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
        public void Create(Wish item)
        {
            db.Wishes.Add(item);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            Wish wish = db.Wishes.Find(id);

            if (wish != null)
                db.Wishes.Remove(wish);
            db.SaveChanges();
        }

        public IEnumerable<Wish> Find(Func<Wish, bool> predicate)
        {
            return db.Wishes.Where(predicate).ToList();
        }

        public Wish Get(int id)
        {
            return db.Wishes.Find(id);
        }

        public IEnumerable<Wish> GetAll()
        {
            return db.Wishes;
        }

        public void Update(Wish item)
        {
            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}

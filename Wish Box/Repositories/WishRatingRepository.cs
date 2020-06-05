using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wish_Box.Models;

namespace Wish_Box.Repositories
{
    public class WishRatingRepository : IRepository<WishRating>
    {
        private AppDbContext db;
        public WishRatingRepository(AppDbContext context)
        {
            db = context;
        }
        public void Create(WishRating item)
        {
            db.WishRatings.Add(item);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            WishRating wishRating = db.WishRatings.Find(id);

            if (wishRating != null)
                db.WishRatings.Remove(wishRating);
            db.SaveChanges();
        }

        public IEnumerable<WishRating> Find(Func<WishRating, bool> predicate)
        {
            return db.WishRatings.Where(predicate).ToList();
        }

        public WishRating Get(int id)
        {
            return db.WishRatings.Find(id);
        }

        public IEnumerable<WishRating> GetAll()
        {
            return db.WishRatings;
        }

        public void Update(WishRating item)
        {
            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}

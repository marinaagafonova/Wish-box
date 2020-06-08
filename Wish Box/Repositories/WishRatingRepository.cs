using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        public async Task Create(WishRating item)
        {
            db.WishRatings.Add(item);
            await db.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            WishRating wishRating = db.WishRatings.Find(id);

            if (wishRating != null)
                db.WishRatings.Remove(wishRating);
            await db.SaveChangesAsync();
        }

        public IEnumerable<WishRating> Find(Func<WishRating, bool> predicate)
        {
            return db.WishRatings.Where(predicate).ToList();
        }

        public async Task<WishRating> FindFirstOrDefault(Expression<Func<WishRating, bool>> predicate)
        {
            return await db.WishRatings.FirstOrDefaultAsync(predicate);
        }

        public async Task<WishRating> Get(int id)
        {
            return await db.WishRatings.FirstOrDefaultAsync(p => p.Id == id);
        }

        public IEnumerable<WishRating> GetAll()
        {
            return db.WishRatings;
        }

        public async Task Update(WishRating item)
        {
            db.Entry(item).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }

       

        Task<WishRating> IRepository<WishRating>.Get(int id)
        {
            throw new NotImplementedException();
        }

    }
}

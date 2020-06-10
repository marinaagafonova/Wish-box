using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wish_Box.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Wish_Box.Repositories
{
    public class UserRepository : IRepository<User>
    {
        AppDbContext db;
        public UserRepository(AppDbContext context)
        {
            db = context;
        }
        public async Task Create(User item)
        {
            db.Users.Add(item);
            await db.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            User item = db.Users.Find(id);
            if (item != null)
                db.Users.Remove(item);
            await db.SaveChangesAsync();
        }

        public IEnumerable<User> Find(Func<User, bool> predicate)
        {
            return db.Users.Where(predicate).ToList();
        }

        public async Task<User> FindFirstOrDefault(Expression<Func<User, bool>> predicate)
        {
            return await db.Users.FirstOrDefaultAsync(predicate, System.Threading.CancellationToken.None);
        }

        public async Task<User> Get(int id)
        {
            var item = await db.Users.FindAsync(id);
            return item;
        }

        public IEnumerable<User> GetAll()
        {
            var list = db.Users.ToList();

            return list ?? new List<User>();
        }

        public async Task Update(User item)
        {
            db.Entry(item).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }
    }
}

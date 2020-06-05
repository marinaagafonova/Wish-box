using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wish_Box.Models;
using Microsoft.EntityFrameworkCore;

namespace Wish_Box.Repositories
{
    public class UserRepository : IRepository<User>
    {
        AppDbContext db;
        public UserRepository(AppDbContext context)
        {
            db = context;
        }
        public async void Create(User item)
        {
            db.Users.Add(item);
            await db.SaveChangesAsync();
        }

        public async void Delete(int id)
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

        public User Get(int id)
        {
            User item = db.Users.Find(id);
            return item;
        }

        public IEnumerable<User> GetAll()
        {
            var list = db.Users.ToList();

            return list ?? new List<User>();
        }

        public async void Update(User item)
        {
            db.Entry(item).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }
    }
}

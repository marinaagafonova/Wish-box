using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Wish_Box.Repositories;

namespace Wish_Box.Models
{
    public class CommentRepository : IRepository<Comment>
    {
		private AppDbContext db;

		public CommentRepository(AppDbContext context)
		{
			db = context;
		}

		public IEnumerable<Comment> GetAll()
		{
			return db.Comments;
		}

		public async Task<Comment> Get(int id)
		{
			return await db.Comments.FirstOrDefaultAsync(p => p.Id == id);
		}

		public async Task Create(Comment Comment)
		{
			db.Comments.Add(Comment);
			await db.SaveChangesAsync();
		}

		public async Task Update(Comment Comment)
		{
			db.Entry(Comment).State = EntityState.Modified;
			await db.SaveChangesAsync();
		}

		public async Task<IEnumerable<Comment>> Find(Func<Comment, Boolean> predicate)
		{
			return await Task.FromResult(db.Comments.Where(predicate).ToList());
		}

		public async Task Delete(int id)
		{
			Comment Comment = db.Comments.Find(id);

			if (Comment != null)
				db.Entry(Comment).State = EntityState.Deleted;
			await db.SaveChangesAsync();
		}


		public async Task<Comment> FindFirstOrDefault(Expression<Func<Comment, bool>> predicate)
		{
			return await db.Comments.FirstOrDefaultAsync(predicate);
		}
	}
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

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

			public Comment Get(int id)
			{
				return db.Comments.Find(id);
			}

			public void Create(Comment Comment)
			{
				db.Comments.Add(Comment);
			}

			public void Update(Comment Comment)
			{
				db.Entry(Comment).State = EntityState.Modified;
			}

			public IEnumerable<Comment> Find(Func<Comment, Boolean> predicate)
			{
				return db.Comments.Where(predicate).ToList();
			}

			public void Delete(int id)
			{
				Comment Comment = db.Comments.Find(id);

				if (Comment != null)
					db.Entry(Comment).State = EntityState.Deleted;
			}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wish_Box.Repositories
{
	public interface IRepository<T> where T : class
	{
		IEnumerable<T> GetAll();
		Task<T> Get(int id);
		IEnumerable<T> Find(Func<T, Boolean> predicate);
		Task<T> FindFirstOrDefault(System.Linq.Expressions.Expression<Func<T, Boolean>> predicate);


		Task Create(T item);
		Task Update(T item);
		Task Delete(int id);
	}
}

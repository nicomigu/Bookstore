using Bookstore.Models;

namespace Bookstore.DataAccess.Repositories.Interfaces
{
    public interface IBookRepository : IRepository<Book>
    {
        void Update(Book book);
    }
}

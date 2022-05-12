using Bookstore.DataAccess.Repositories.Interfaces;
using Bookstore.Models;

namespace Bookstore.DataAccess.Repositories
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        private ApplicationDbContext _context;
        public BookRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Book book)
        {
            var obj = _context.Books.FirstOrDefault(book => book.Id == book.Id);
            if (obj != null)
            {
                obj.Title = book.Title;
                obj.CategoryId = book.CategoryId;
                obj.CoverId = book.CoverId;
                obj.Price = book.Price;
                obj.Price100 = book.Price100;
                obj.Price200 = book.Price200;
                obj.Author = book.Author;
                obj.ISBN = book.ISBN;
                if (book.ImageUrl != null)
                {
                    obj.ImageUrl = book.ImageUrl;
                }
            }
        }
    }
}

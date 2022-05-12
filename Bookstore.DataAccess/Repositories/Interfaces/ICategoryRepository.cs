using Bookstore.Models;

namespace Bookstore.DataAccess.Repositories.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category category);
    }
}

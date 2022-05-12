using Bookstore.Models;

namespace Bookstore.DataAccess.Repositories.Interfaces
{
    public interface ICoverRepository : IRepository<Cover>
    {
        void Update(Cover cover);
    }
}

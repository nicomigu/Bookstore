using Bookstore.Models;

namespace Bookstore.DataAccess.Repositories.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        void Update(Order order);
    }
}

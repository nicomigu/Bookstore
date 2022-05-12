using Bookstore.DataAccess.Repositories.Interfaces;
using Bookstore.Models;

namespace Bookstore.DataAccess.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private ApplicationDbContext _context;
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Order order)
        {
            _context.Orders.Update(order);
        }
    }
}

using Bookstore.DataAccess.Repositories.Interfaces;
using Bookstore.Models;

namespace Bookstore.DataAccess.Repositories
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private ApplicationDbContext _context;
        public OrderHeaderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(OrderHeader orderHeader)
        {
            _context.OrderHeaders.Update(orderHeader);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var order = _context.OrderHeaders.FirstOrDefault(x => x.Id == id);
            if (order != null)
            {
                order.OrderStatus = orderStatus;
                if (paymentStatus != null)
                {
                    order.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
        {
            var order = _context.OrderHeaders.FirstOrDefault(x => x.Id == id);
            order.PaymentDate = DateTime.Now;
            order.SessionId = sessionId;
            order.PaymentIntentId = paymentIntentId;
        }
    }
}

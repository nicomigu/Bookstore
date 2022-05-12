namespace Bookstore.Models.ViewModel
{
    public class OrderVM
    {
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<Order> Order { get; set; }
    }
}

namespace Bookstore.Models.ViewModel
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> CartList { get; set; }

        public OrderHeader OrderHeader { get; set; }
    }
}

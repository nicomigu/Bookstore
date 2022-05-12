using Bookstore.Models;

namespace Bookstore.DataAccess.Repositories.Interfaces
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        int IncreaseBookCount(ShoppingCart shoppingCart, int count);
        int DecreaseBookCount(ShoppingCart shoppingCart, int count);
    }
}

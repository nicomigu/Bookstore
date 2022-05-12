namespace Bookstore.DataAccess.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepository { get; }
        ICoverRepository CoverRepository { get; }
        IBookRepository BookRepository { get; }
        ICompanyRepository CompanyRepository { get; }
        IShoppingCartRepository ShoppingCartRepository { get; }
        IAppUserRepository AppUserRepository { get; }

        IOrderRepository OrderRepository { get; }
        IOrderHeaderRepository OrderHeaderRepository { get; }
        void Save();
    }
}

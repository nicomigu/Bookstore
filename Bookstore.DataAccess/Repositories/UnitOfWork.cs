using Bookstore.DataAccess.Repositories.Interfaces;

namespace Bookstore.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;


        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            CategoryRepository = new CategoryRepository(_context);
            CoverRepository = new CoverRepository(_context);
            BookRepository = new BookRepository(_context);
            CompanyRepository = new CompanyRepository(_context);
            ShoppingCartRepository = new ShoppingCartRepository(_context);
            AppUserRepository = new AppUserRepository(_context);
            OrderRepository = new OrderRepository(_context);
            OrderHeaderRepository = new OrderHeaderRepository(_context);
        }
        public ICategoryRepository CategoryRepository { get; private set; }
        public ICoverRepository CoverRepository { get; private set; }

        public IBookRepository BookRepository { get; private set; }
        public ICompanyRepository CompanyRepository { get; private set; }
        public IShoppingCartRepository ShoppingCartRepository { get; private set; }
        public IAppUserRepository AppUserRepository { get; private set; }
        public IOrderHeaderRepository OrderHeaderRepository { get; private set; }
        public IOrderRepository OrderRepository { get; private set; }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}

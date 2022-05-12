using Bookstore.DataAccess.Repositories.Interfaces;
using Bookstore.Models;

namespace Bookstore.DataAccess.Repositories
{
    public class AppUserRepository : Repository<AppUser>, IAppUserRepository
    {
        private ApplicationDbContext _context;
        public AppUserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}

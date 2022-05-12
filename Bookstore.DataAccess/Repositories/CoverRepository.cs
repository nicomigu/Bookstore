using Bookstore.DataAccess.Repositories.Interfaces;
using Bookstore.Models;

namespace Bookstore.DataAccess.Repositories
{
    public class CoverRepository : Repository<Cover>, ICoverRepository
    {
        private ApplicationDbContext _context;
        public CoverRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(Cover cover)
        {
            _context.Covers.Update(cover);
        }
    }
}

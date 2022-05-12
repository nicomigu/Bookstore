﻿using Bookstore.Models;

namespace Bookstore.DataAccess.Repositories.Interfaces
{
    public interface ICompanyRepository : IRepository<Company>
    {
        void Update(Company company);
    }
}

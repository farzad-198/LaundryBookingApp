using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class UserAccountRepository : GenericRepository<UserAccount>, IUserAccountRepository
    {
        public UserAccountRepository(AppDbContext context) : base(context)
        {

        }
        public async Task<UserAccount?> GetByUsernameAsync(string username)
        {
            return await _dbSet
                // load person data too
                .Include(account => account.Person)
                .FirstOrDefaultAsync(account => account.Username == username);
        }
        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _dbSet.AnyAsync(account => account.Username == username);
        }

    }
}

using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories
{
    public class PersonRepository :GenericRepository<Person>, IPersonRepository
    {
        public PersonRepository(AppDbContext context) : base(context)
        {

        }

        public async Task<Person?> GetByFullNameAndAddressAsync(string fullName, string addressOrDepartment)
        { 
            // same person check
            return await _dbSet.FirstOrDefaultAsync(person => person.FullName == fullName && 
            person.AddressOrDepartment == addressOrDepartment);
        
        }


    }
}

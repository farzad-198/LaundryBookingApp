using Domain.Entities;


namespace Application.Interfaces.Repositories
{
    public interface IPersonRepository : IGenericRepository<Person>
    {
        Task<Person?> GetByFullNameAndAddressAsync(string fullName, string addressOrDepartment);
    }
}

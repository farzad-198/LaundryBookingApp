using Domain.Entities;

namespace Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<UserAccount> RegisterResidentAsync(
            string fullName,
            string addressOrDepartment,
            string username,
            string password);

        Task<UserAccount?> LoginAsync(string username, string password);
    }
}
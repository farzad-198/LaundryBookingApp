using Application.Helpers;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IPersonRepository _personRepository;

        public AuthService(
            IUserAccountRepository userAccountRepository,
            IPersonRepository personRepository)
        {
            _userAccountRepository = userAccountRepository;
            _personRepository = personRepository;
        }

        public async Task<UserAccount> RegisterResidentAsync(
            string fullName,
            string addressOrDepartment,
            string username,
            string password)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                throw new ArgumentException("Full name is required.", nameof(fullName));
            }

            if (string.IsNullOrWhiteSpace(addressOrDepartment))
            {
                throw new ArgumentException("Address or department is required.", nameof(addressOrDepartment));
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username is required.", nameof(username));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password is required.", nameof(password));
            }

            string normalizedUsername = username.Trim();
            bool usernameExists = await _userAccountRepository.UsernameExistsAsync(normalizedUsername);

            if (usernameExists)
            {
                throw new InvalidOperationException("This username already exists.");
            }

            Person person = new Person
            {
                Id = Guid.NewGuid(),
                FullName = fullName.Trim(),
                AddressOrDepartment = addressOrDepartment.Trim()
            };

            await _personRepository.AddAsync(person);
            await _personRepository.SaveChangesAsync();

            UserAccount account = new UserAccount
            {
                Id = Guid.NewGuid(),
                Username = normalizedUsername,
                PasswordHash = PasswordHelper.HashPassword(password),
                Role = UserRole.Resident,
                PersonId = person.Id
            };

            await _userAccountRepository.AddAsync(account);
            await _userAccountRepository.SaveChangesAsync();

            return account;
        }

        public async Task<UserAccount?> LoginAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            UserAccount? account = await _userAccountRepository.GetByUsernameAsync(username.Trim());

            if (account is null)
            {
                return null;
            }

            if (!PasswordHelper.IsPasswordMatch(account.PasswordHash, password))
            {
                return null;
            }

            return account;
        }
    }
}

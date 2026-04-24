using Domain.Entities;

namespace Application.Interfaces.Services
{
    public interface IAdminService
    {
        Task<Person> CreatePersonAsync(Person person);
        Task<LaundryRoom> CreateLaundryRoomAsync(LaundryRoom laundryRoom);
        Task<Person?> GetPersonByFullNameAndAddressAsync(string fullName, string addressOrDepartment);

        Task<IEnumerable<Person>> GetAllPersonsAsync();
        Task<IEnumerable<LaundryRoom>> GetAllLaundryRoomsAsync();

        Task<bool> UpdatePersonAsync(Guid personId, string fullName, string addressOrDepartment);
        Task<bool> UpdateLaundryRoomAsync(Guid laundryRoomId, string name, string location,
            int capacity, bool hasDryer, bool hasIron, string description);

        Task<bool> DeletePersonAsync(Guid personId);
        Task<bool> DeleteLaundryRoomAsync(Guid laundryRoomId);
    }
}
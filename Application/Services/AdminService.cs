using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;

namespace Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IPersonRepository _personRepository;
        private readonly ILaundryRoomRepository _laundryRoomRepository;
        private readonly IBookingRepository _bookingRepository;

        public AdminService(
            IPersonRepository personRepository,
            ILaundryRoomRepository laundryRoomRepository,
            IBookingRepository bookingRepository)
        {
            _personRepository = personRepository;
            _laundryRoomRepository = laundryRoomRepository;
            _bookingRepository = bookingRepository;
        }

        public async Task<Person> CreatePersonAsync(Person person)
        {
            if (person == null)
            {
                throw new ArgumentNullException(nameof(person), "Person cannot be null");
            }

            person.Id = Guid.NewGuid();
            await _personRepository.AddAsync(person);
            await _personRepository.SaveChangesAsync();

            return person;
        }

        public async Task<LaundryRoom> CreateLaundryRoomAsync(LaundryRoom laundryRoom)
        {
            if (laundryRoom == null)
            {
                throw new ArgumentNullException(nameof(laundryRoom), "Laundry room cannot be null");
            }

            laundryRoom.Id = Guid.NewGuid();
            await _laundryRoomRepository.AddAsync(laundryRoom);
            await _laundryRoomRepository.SaveChangesAsync();

            return laundryRoom;
        }

        public async Task<Person?> GetPersonByFullNameAndAddressAsync(string fullName, string addressOrDepartment)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                throw new ArgumentException("Full name cannot be null or whitespace", nameof(fullName));
            }
            
            if (string.IsNullOrWhiteSpace(addressOrDepartment))
            {
                throw new ArgumentException("Address or department cannot be null or whitespace", nameof(addressOrDepartment));
            }
            return await _personRepository.GetByFullNameAndAddressAsync(
                  fullName.Trim(),
                  addressOrDepartment.Trim());
        }

        public async Task<IEnumerable<Person>> GetAllPersonsAsync()
        {
            return await _personRepository.GetAllAsync();
        }

        public async Task<IEnumerable<LaundryRoom>> GetAllLaundryRoomsAsync()
        {
            return await _laundryRoomRepository.GetAllAsync();
        }

        public async Task<bool> UpdatePersonAsync(Guid personId, string fullName, string addressOrDepartment)
        {
            Person? person = await _personRepository.GetByIdAsync(personId);

            if (person is null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(addressOrDepartment))
            {
                throw new ArgumentException("Full name and address/department are required.");
            }

            person.FullName = fullName.Trim();
            person.AddressOrDepartment = addressOrDepartment.Trim();

            _personRepository.Update(person);
            await _personRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateLaundryRoomAsync(Guid laundryRoomId, string name, string location, int capacity, bool hasDryer, bool hasIron, string description)
        {
            LaundryRoom? room = await _laundryRoomRepository.GetByIdAsync(laundryRoomId);

            if (room is null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(location))
            {
                throw new ArgumentException("Room name and location are required.");
            }

            room.Name = name.Trim();
            room.Location = location.Trim();
            room.Capacity = capacity;
            room.HasDryer = hasDryer;
            room.HasIron = hasIron;
            room.Description = description ?? string.Empty;

            _laundryRoomRepository.Update(room);
            await _laundryRoomRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeletePersonAsync(Guid personId)
        {
            Person? person = await _personRepository.GetByIdAsync(personId);

            if (person is null)
            {
                return false;
            }

            IEnumerable<Booking> personBookings = await _bookingRepository.GetByPersonIdAsync(personId);

            // delete bookings first
            foreach (Booking booking in personBookings)
            {
                _bookingRepository.Delete(booking);
            }

            _personRepository.Delete(person);
            await _personRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteLaundryRoomAsync(Guid laundryRoomId)
        {
            LaundryRoom? room = await _laundryRoomRepository.GetByIdAsync(laundryRoomId);

            if (room is null)
            {
                return false;
            }

            IEnumerable<Booking> roomBookings = await _bookingRepository.GetByLaundryRoomIdAsync(laundryRoomId);

            // delete bookings first
            foreach (Booking booking in roomBookings)
            {
                _bookingRepository.Delete(booking);
            }

            _laundryRoomRepository.Delete(room);
            await _laundryRoomRepository.SaveChangesAsync();

            return true;
        }
    }
}

using Application.Helpers;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeding
{
    public static class LaundryBookingSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            await context.Database.MigrateAsync();
            await SeedPersonsAsync(context);
            await SeedLaundryRoomsAsync(context);
            await SeedUserAccountsAsync(context);
            await SeedBookingsAsync(context);
        }

        private static async Task SeedPersonsAsync(AppDbContext context)
        {
            if (await context.Persons.AnyAsync())
            {
                return;
            }

            List<Person> persons = new List<Person>
            {
                new Person
                {
                    Id = Guid.NewGuid(),
                    FullName = "Emma Johnson",
                    AddressOrDepartment = "Building A - Unit 101"
                },
                new Person
                {
                    Id = Guid.NewGuid(),
                    FullName = "Liam Smith",
                    AddressOrDepartment = "Building B - Unit 202"
                },
                new Person
                {
                    Id = Guid.NewGuid(),
                    FullName = "Olivia Brown",
                    AddressOrDepartment = "Building C - Unit 303"
                },
                new Person
                {
                    Id = Guid.NewGuid(),
                    FullName = "System Admin",
                    AddressOrDepartment = "Administration"
                }
            };

            await context.Persons.AddRangeAsync(persons);
            await context.SaveChangesAsync();
        }

        private static async Task SeedLaundryRoomsAsync(AppDbContext context)
        {
            if (await context.LaundryRooms.AnyAsync())
            {
                return;
            }

            List<LaundryRoom> rooms = new List<LaundryRoom>
            {
                new LaundryRoom
                {
                    Id = Guid.NewGuid(),
                    Name = "Laundry Room 1",
                    Location = "Basement A",
                    Capacity = 2,
                    HasDryer = true,
                    HasIron = false,
                    Description = "Two washing machines and one dryer"
                },
                new LaundryRoom
                {
                    Id = Guid.NewGuid(),
                    Name = "Laundry Room 2",
                    Location = "Basement B",
                    Capacity = 3,
                    HasDryer = true,
                    HasIron = true,
                    Description = "Three washing machines, one dryer and one iron"
                },
                new LaundryRoom
                {
                    Id = Guid.NewGuid(),
                    Name = "Laundry Room 3",
                    Location = "Basement C",
                    Capacity = 2,
                    HasDryer = false,
                    HasIron = true,
                    Description = "Two washing machines and one iron"
                }
            };

            await context.LaundryRooms.AddRangeAsync(rooms);
            await context.SaveChangesAsync();
        }

        private static async Task SeedUserAccountsAsync(AppDbContext context)
        {
            if (await context.UserAccounts.AnyAsync())
            {
                return;
            }

            List<Person> persons = await context.Persons.ToListAsync();

            Person? adminPerson = persons.FirstOrDefault(person => person.FullName == "System Admin");
            Person? emma = persons.FirstOrDefault(person => person.FullName == "Emma Johnson");
            Person? liam = persons.FirstOrDefault(person => person.FullName == "Liam Smith");
            Person? olivia = persons.FirstOrDefault(person => person.FullName == "Olivia Brown");

            if (adminPerson is null || emma is null || liam is null || olivia is null)
            {
                return;
            }

            List<UserAccount> accounts = new List<UserAccount>
            {
                // account password is hash
                new UserAccount
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    PasswordHash = PasswordHelper.HashPassword("admin123"),
                    Role = UserRole.Admin,
                    PersonId = adminPerson.Id
                },
                new UserAccount
                {
                    Id = Guid.NewGuid(),
                    Username = "emma",
                    PasswordHash = PasswordHelper.HashPassword("emma123"),
                    Role = UserRole.Resident,
                    PersonId = emma.Id
                },
                new UserAccount
                {
                    Id = Guid.NewGuid(),
                    Username = "liam",
                    PasswordHash = PasswordHelper.HashPassword("liam123"),
                    Role = UserRole.Resident,
                    PersonId = liam.Id
                },
                new UserAccount
                {
                    Id = Guid.NewGuid(),
                    Username = "olivia",
                    PasswordHash = PasswordHelper.HashPassword("olivia123"),
                    Role = UserRole.Resident,
                    PersonId = olivia.Id
                }
            };

            await context.UserAccounts.AddRangeAsync(accounts);
            await context.SaveChangesAsync();
        }

        private static async Task SeedBookingsAsync(AppDbContext context)
        {
            if (await context.Bookings.AnyAsync())
            {
                return;
            }

            List<Person> persons = await context.Persons.ToListAsync();
            List<LaundryRoom> rooms = await context.LaundryRooms.ToListAsync();

            Person? emma = persons.FirstOrDefault(person => person.FullName == "Emma Johnson");
            Person? liam = persons.FirstOrDefault(person => person.FullName == "Liam Smith");
            Person? olivia = persons.FirstOrDefault(person => person.FullName == "Olivia Brown");

            LaundryRoom? room1 = rooms.FirstOrDefault(room => room.Name == "Laundry Room 1");
            LaundryRoom? room2 = rooms.FirstOrDefault(room => room.Name == "Laundry Room 2");
            LaundryRoom? room3 = rooms.FirstOrDefault(room => room.Name == "Laundry Room 3");

            if (emma is null || liam is null || olivia is null ||
                room1 is null || room2 is null || room3 is null)
            {
                return;
            }

            DateOnly tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
            DateOnly dayAfterTomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
            DateOnly nextWeek = DateOnly.FromDateTime(DateTime.Today.AddDays(7));

            List<Booking> bookings = new List<Booking>
            {
                new Booking
                {
                    Id = Guid.NewGuid(),
                    PersonId = emma.Id,
                    LaundryRoomId = room1.Id,
                    BookingDate = tomorrow,
                    TimeSlot = TimeSlot.Slot09To12,
                    CreatedAt = DateTime.UtcNow
                },
                new Booking
                {
                    Id = Guid.NewGuid(),
                    PersonId = liam.Id,
                    LaundryRoomId = room1.Id,
                    BookingDate = tomorrow,
                    TimeSlot = TimeSlot.Slot12To15,
                    CreatedAt = DateTime.UtcNow
                },
                new Booking
                {
                    Id = Guid.NewGuid(),
                    PersonId = olivia.Id,
                    LaundryRoomId = room2.Id,
                    BookingDate = dayAfterTomorrow,
                    TimeSlot = TimeSlot.Slot15To18,
                    CreatedAt = DateTime.UtcNow
                },
                new Booking
                {
                    Id = Guid.NewGuid(),
                    PersonId = emma.Id,
                    LaundryRoomId = room3.Id,
                    BookingDate = nextWeek,
                    TimeSlot = TimeSlot.Slot09To12,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await context.Bookings.AddRangeAsync(bookings);
            await context.SaveChangesAsync();
        }
    }
}

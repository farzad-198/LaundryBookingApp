# Laundry Booking App

Laundry Booking App is a console-based booking system for shared laundry rooms in a building.

The application allows residents to register, log in, create and manage their own bookings, while administrators can manage persons, laundry rooms, bookings, and reports.

## Purpose

The purpose of this project is to build a simple but structured booking system where users can:

- see available laundry rooms
- book only free time slots
- manage bookings in a clear way
- avoid double-bookings

The project also demonstrates layered architecture, Entity Framework Core, SQL Server, LINQ, async programming, and role-based access.

## Roles

There are two roles in the system:

- **Admin**
- **Resident**

### Admin can:
- add, view, update, and delete persons
- add, view, update, and delete laundry rooms
- create, view, update, and delete bookings
- view reports

### Resident can:
- register and log in
- create own bookings
- view own bookings
- update own bookings
- delete own bookings
- check room schedule
- find available rooms

## Main Features

### Authentication
- register a new resident account
- log in with an existing account
- role-based redirection after login
- passwords are stored as hashes
- password input is hidden with `*`

### Booking
- select laundry room
- select booking date
- select one of three fixed time slots
- create booking only if the slot is available
- update or delete existing booking

### Room Availability
- view room schedule for a selected date
- view room schedule for a selected week
- find available rooms for a selected date and time slot

### Reports
- most booked laundry room
- top person by booking count
- occupancy rate for a selected date range
- weekly occupancy rate
- total occupancy rate

## Time Slots

The system uses 3 fixed booking intervals:

- `09:00 - 12:00`
- `12:00 - 15:00`
- `15:00 - 18:00`

## Project Structure

The project is divided into four layers:

### Domain
Contains the core models and enums:
- `Person`
- `UserAccount`
- `LaundryRoom`
- `Booking`
- `TimeSlot`
- `UserRole`

### Application
Contains:
- interfaces
- business logic
- services

Examples:
- `AuthService`
- `BookingService`
- `ReportService`

### Infrastructure
Contains:
- `AppDbContext`
- repositories
- entity configurations
- seeding
- dependency injection
- migrations

### Presentation
Contains:
- console menus
- startup flow
- helpers
- user interaction logic

## Database

The project uses:

- **Entity Framework Core**
- **SQL Server**

Main tables:
- `Persons`
- `UserAccounts`
- `LaundryRooms`
- `Bookings`

## Entity Relationships

- One **Person** can have many **Bookings**
- One **LaundryRoom** can have many **Bookings**
- Each **Booking** belongs to exactly one **Person**
- Each **Booking** belongs to exactly one **LaundryRoom**
- Each **UserAccount** is linked to one **Person**

## Person and Account Logic

In this project, `Person` and `UserAccount` are separated.

- If admin adds a person, it is stored only in the `Persons` table
- If a user registers, data is stored in both `Persons` and `UserAccounts`
- Login works only for records that exist in `UserAccounts`

This design makes it possible to separate personal information from authentication data.

## Double-Booking Protection

The system prevents double-booking in two ways:

1. **Business logic**
   - before creating or updating a booking, the system checks if the room is already booked for the selected date and time slot

2. **Database rule**
   - the database has a unique rule for:
     - `LaundryRoomId`
     - `BookingDate`
     - `TimeSlot`

This means the same room cannot be booked twice for the same date and time slot.

## Seed Data

The project includes seed data so the application can start in a ready-to-run state.

Seeded data includes:
- sample persons
- sample user accounts
- sample laundry rooms
- sample bookings

This makes the project easier to test and easier to demonstrate.

## Demo Accounts

Example accounts created by seed data:

- **Admin**: `admin / admin123`
- **Resident**: `emma / emma123`
- **Resident**: `liam / liam123`
- **Resident**: `olivia / olivia123`

## Technologies Used

- C#
- .NET
- Entity Framework Core
- SQL Server
- LINQ
- Async / Await
- Generic Repository Pattern
- Layered / Onion-inspired architecture

## How to Run

1. Set the connection string in **User Secrets** under `DefaultConnection`
2. Make sure SQL Server is running
3. Run the database update command
4. Start the `Presentation` project

### Update database
```powershell
Update-Database -Project Infrastructure -StartupProject Presentation

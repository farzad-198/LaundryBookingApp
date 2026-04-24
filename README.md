# Laundry Booking App

This project is a simple console application for laundry room booking in a building.

Users can register, login, and book laundry rooms for available time slots.
There are 2 roles in the system:

- Admin
- Resident

## What this project does

This system helps manage laundry room booking.
Resident can make booking for a room and time.
Admin can manage persons, laundry rooms, bookings, and reports.

## Main features

### Authentication
- User can login if account exists
- User can register
- User can login
- Password is saved as hash
- When user types password, it shows `*`

### Resident
- See own bookings
- Create booking
- Update booking
- Delete booking
- Check room schedule
- Find available rooms

### Admin
- Add, view, update, delete persons
- Add, view, update, delete laundry rooms
- Create, view, update, delete bookings
- See reports

### Reports
- Most booked laundry room
- Top person by booking count
- Occupancy rate for selected date range
- Weekly occupancy rate
- Total occupancy rate

## Project structure

This project uses simple layered architecture:

- `Domain`
  - entities and enums
- `Application`
  - interfaces and services
- `Infrastructure`
  - database, repositories, seeding, dependency injection
- `Presentation`
  - console menus and user interaction

## Person and account logic

- If admin adds a person, it is added only in `Persons` table
- If user registers, data is added in `Persons` table and `UserAccounts` table
- Login works only for users that have account in `UserAccounts` table

## Booking logic

- One room cannot be booked two times in same date and same time slot
- Past date is not allowed for booking
- User must choose valid room and valid time slot
- Admin can manage all bookings
- Resident can manage only own bookings
- If a person is deleted, related bookings are also deleted

## Database

Project uses Entity Framework Core with SQL Server.

Database has these main tables:
- Persons
- UserAccounts
- LaundryRooms
- Bookings

There is also seed data for:
- some persons
- some laundry rooms
- some user accounts
- some bookings

## Time slots

System uses 3 fixed time slots:

- 09:00 - 12:00
- 12:00 - 15:00
- 15:00 - 18:00

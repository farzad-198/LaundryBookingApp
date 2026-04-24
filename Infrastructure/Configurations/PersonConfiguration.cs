using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        { 
        builder.HasKey(Person=>Person.Id);

            builder.Property(Person => Person.FullName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(Person=> Person.AddressOrDepartment)
                .IsRequired()
                .HasMaxLength(200);
            builder.HasMany(Person=>Person.Bookings)
                .WithOne(Booking=>Booking.Person)
                .HasForeignKey(Booking=>Booking.PersonId)
                .OnDelete(DeleteBehavior.Restrict);
        }


    }
}

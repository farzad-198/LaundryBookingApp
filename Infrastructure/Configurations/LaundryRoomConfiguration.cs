using Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class LaundryRoomConfiguration : IEntityTypeConfiguration<LaundryRoom>
    {
        public void Configure(EntityTypeBuilder<LaundryRoom> builder)
        {
            builder.HasKey(LaundryRoom => LaundryRoom.Id);

            builder.Property(LaundryRoom => LaundryRoom.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(LaundryRoom => LaundryRoom.Location)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(LaundryRoom => LaundryRoom.Description)
                .HasMaxLength(250);

            builder.Property(LaundryRoom => LaundryRoom.Capacity)
                .IsRequired();

            builder.HasMany(LaundryRoom => LaundryRoom.Bookings)
                .WithOne(Booking => Booking.LaundryRoom)
                .HasForeignKey(Booking => Booking.LaundryRoomId)
                .OnDelete(DeleteBehavior.Restrict);

        }

    }
}

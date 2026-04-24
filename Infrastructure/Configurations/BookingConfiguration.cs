using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(booking => booking.Id);

            builder.Property(booking => booking.BookingDate)
            .IsRequired();

            builder.Property(booking => booking.BookingDate)
                .IsRequired();

            builder.Property(booking => booking.TimeSlot)
                .IsRequired();

            builder.Property(booking => booking.CreatedAt)
                .IsRequired();

            builder.HasIndex(booking=> new {booking.LaundryRoomId,booking.BookingDate, booking.TimeSlot })
                .IsUnique();

        }
    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class UserAccountConfiguration : IEntityTypeConfiguration<UserAccount>
    {
        public void Configure(EntityTypeBuilder<UserAccount> builder)
        {
            builder.HasKey(account => account.Id);

            builder.Property(account => account.Username)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(account => account.PasswordHash)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(account => account.Role)
                .IsRequired();

            builder.HasIndex(account => account.Username)
                .IsUnique();

            builder.HasOne(account => account.Person)
                .WithMany()
                .HasForeignKey(account => account.PersonId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
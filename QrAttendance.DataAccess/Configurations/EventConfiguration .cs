using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QrAttendanceSystem.Entities;

namespace QrAttendanceSystem.DataAccess.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("Events");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(1000);

        builder.Property(e => e.Date)
            .IsRequired();

        builder.Property(e => e.QrToken)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.QrExpire)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedByUserId)
            .IsRequired();

        // Event -> User (CreatedByUser)
        builder.HasOne(e => e.CreatedByUser)
            .WithMany(u => u.CreatedEvents)
            .HasForeignKey(e => e.CreatedByUserId)
            .OnDelete(DeleteBehavior.NoAction);
        // MSSQL multiple cascade paths hatasını engellemek için
        // User silinince Event’ler otomatik silinmesin

        // Event -> Attendances (one-to-many)
        builder.HasMany(e => e.Attendances)
            .WithOne(a => a.Event)
            .HasForeignKey(a => a.EventId);
        builder.Property(x => x.LocationPolygonJson)
    .HasColumnType("nvarchar(max)"); // PostgreSQL'de uzun JSON için uygun

    }
}

using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
    {
    }

    public DbSet<Notification> Notifications { get; set; }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<BookBorrow> BookBorrows { get; set; }
    public DbSet<BookRating> BookRatings { get; set; }
    public DbSet<DonationRequest> DonationRequests { get; set; }
    public DbSet<SystemSettings> SystemSettings { get; set; }
    public DbSet<FeaturedBook> FeaturedBooks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<FeaturedBook>(entity =>
        {
            entity.ToTable("featuredbooks");
            entity.HasKey(e => e.FeaturedBookId);

            entity.Property(e => e.FeaturedBookId)
                .ValueGeneratedOnAdd()
                .HasColumnName("FeaturedBookId");

            entity.Property(e => e.BookId)
                .IsRequired()
                .HasColumnName("BookId");

            entity.Property(e => e.FeaturedDate)
                .IsRequired()
                .HasColumnName("FeaturedDate")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasColumnName("IsActive")
                .HasDefaultValue(true);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("CreatedAt")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("UpdatedAt")
                .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            // Relationship
            entity.HasOne(fb => fb.Book)
                .WithMany()
                .HasForeignKey(fb => fb.BookId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure BookBorrow
        modelBuilder.Entity<BookBorrow>(entity =>
        {
            entity.HasKey(e => e.BorrowId);
            entity.HasOne(e => e.Book)
                  .WithMany(e => e.BookBorrows)
                  .HasForeignKey(e => e.BookId);
            entity.HasOne(e => e.User)
                  .WithMany(e => e.BookBorrows)
                  .HasForeignKey(e => e.UserId);
        });

        // Configure BookRating
        modelBuilder.Entity<BookRating>(entity =>
        {
            entity.HasKey(e => e.RatingId);
            entity.HasIndex(e => new { e.BookId, e.UserId }).IsUnique();
            entity.HasOne(e => e.Book)
                  .WithMany(e => e.BookRatings)
                  .HasForeignKey(e => e.BookId);
            entity.HasOne(e => e.User)
                  .WithMany(e => e.BookRatings)
                  .HasForeignKey(e => e.UserId);
        });

        // Configure DonationRequest
        modelBuilder.Entity<DonationRequest>(entity =>
        {
            entity.ToTable("donationrequests");
            entity.Property(e => e.RequestId).HasColumnName("RequestId");
            entity.Property(e => e.UserId).HasColumnName("UserId");
            entity.Property(e => e.BookTitle).HasColumnName("BookTitle");
            entity.Property(e => e.AuthorName).HasColumnName("AuthorName");
            entity.Property(e => e.Reason).HasColumnName("Reason");
            entity.Property(e => e.Status).HasColumnName("Status");
            entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt");
            entity.Property(e => e.ProcessedDate).HasColumnName("ProcessedDate");
            entity.Property(e => e.RequestDate).HasColumnName("RequestDate");
            entity.Property(e => e.AdminComments).HasColumnName("AdminComments");
            entity.Property(e => e.BrainStationId).HasColumnName("BrainStationId");
            entity.Property(e => e.PhoneNumber).HasColumnName("PhoneNumber");
            entity.Property(e => e.Address).HasColumnName("Address");
        });

        // Seed initial system settings
        modelBuilder.Entity<SystemSettings>().HasData(
            new SystemSettings { SettingId = 1 }
        );
    }
}
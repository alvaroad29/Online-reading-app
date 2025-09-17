using backend;
using backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // para identity

        modelBuilder.Entity<ReadingHistory>()
            .HasOne(c => c.Chapter)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ReadingHistory>()
            .HasOne(c => c.User)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        // CONFIGURACIÓN DE BOOKRATING (CAMBIOS NECESARIOS)
        modelBuilder.Entity<BookRating>(entity =>
        {
            // Índice único: un usuario no puede calificar dos veces el mismo libro
            entity.HasIndex(br => new { br.BookId, br.UserId })
                  .IsUnique();

            // Índice para búsquedas rápidas por libro
            entity.HasIndex(br => br.BookId);

            // Índice para búsquedas por usuario
            entity.HasIndex(br => br.UserId);

            // RELACIONES CON DELETE BEHAVIOR SEGURO
            entity.HasOne(br => br.Book)
                  .WithMany(b => b.Ratings)
                  .HasForeignKey(br => br.BookId)
                  .OnDelete(DeleteBehavior.ClientCascade);

            entity.HasOne(br => br.User)
                  .WithMany(u => u.BookRatings)
                  .HasForeignKey(br => br.UserId)
                  .OnDelete(DeleteBehavior.ClientCascade); 
            
             // Configurar clave compuesta para BookListBook
            modelBuilder.Entity<BookListBook>()
                .HasKey(blb => new { blb.BookListId, blb.BookId });

            modelBuilder.Entity<BookListBook>()
                .HasOne(blb => blb.BookList)
                .WithMany(bl => bl.BookListBooks)
                .HasForeignKey(blb => blb.BookListId)
                .OnDelete(DeleteBehavior.ClientCascade); // ← CAMBIAR A ClientCascade o Restrict

            modelBuilder.Entity<BookListBook>()
                .HasOne(blb => blb.Book)
                .WithMany()
                .HasForeignKey(blb => blb.BookId)
                .OnDelete(DeleteBehavior.ClientCascade); // ← CAMBIAR A ClientCascade o Restrict

            // Configurar clave compuesta para BookListFollower
            modelBuilder.Entity<BookListFollower>()
                .HasKey(blf => new { blf.BookListId, blf.UserId });

            modelBuilder.Entity<BookListFollower>()
                .HasOne(blf => blf.BookList)
                .WithMany(bl => bl.Followers)
                .HasForeignKey(blf => blf.BookListId)
                .OnDelete(DeleteBehavior.ClientCascade); // ← CAMBIAR

            modelBuilder.Entity<BookListFollower>()
                .HasOne(blf => blf.User)
                .WithMany(u => u.FollowedBookLists)
                .HasForeignKey(blf => blf.UserId)
                .OnDelete(DeleteBehavior.ClientCascade); // ← CAMBIAR
        });

        // Opcional: índice en Book.Score si haces queries por puntuación
        modelBuilder.Entity<Book>()
            .HasIndex(b => b.Score);
    }

    public DbSet<Genre> Genres { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Volume> Volumes { get; set; }
    public DbSet<Chapter> Chapters { get; set; }
    public DbSet<ReadingHistory> ReadingHistories { get; set; }
    public DbSet<Announcement> Announcements { get; set; }
    public DbSet<BookRating> BookRatings { get; set; }
    public DbSet<BookList> BookLists { get; set; }
    public DbSet<BookListBook> BookListBooks { get; set; }
    public DbSet<BookListFollower> BookListFollowers { get; set; }
}


using backend.Models;
using backend.Models.Dtos;
using backend.Models.Dtos.BookQueries;
namespace backend.Repository.IRepository;

public interface IBookRepository
{
    Task<PagedResponse<Book>> GetBooks(BookQueryParameters parameters);
    Task<Book?> GetBook(int id);
    Task<Book?> GetBookWithVolumesAndChapters(int bookId);
    Task<IEnumerable<Book>> GetPopularBooksAsync(DateTime startDate, int count);
    // bool BookExist(int id);
    Task<bool> BookExist(string name);
    Task<bool> CreateBook(Book book);
    // bool UpdateBook(Book book);
    // bool DeleteBook(Book book);
    Task<bool> Save();

    // rating
    Task<BookRating> RateBookAsync(int bookId, string userId, int rating);
    Task<int?> GetUserRatingAsync(int bookId, string userId);
    Task<bool> DeleteRatingAsync(int bookId, string userId);
    Task<bool> BookExistsAsync(int bookId);
    Task<PagedResponse<BookRating>> GetBookRatingsAsync(int bookId, RatingQueryParameters parameters);
    Task<PagedResponse<BookRating>> GetUserRatingsAsync(string userId, RatingQueryParameters parameters);

}
using backend.Models;
using backend.Models.Dtos;
using backend.Models.Dtos.BookQueries;
using backend.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace backend.Repository;

public class BookListRepository : IBookListRepository
{
    private readonly ApplicationDbContext _db;

    public BookListRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResponse<BookList>> GetAllAsync(BookListQueryParameters parameters, bool includePrivate = false)
    {
        var query = _db.BookLists
            .Include(bl => bl.Creator)
            .Include(bl => bl.Genres)
            .Include(bl => bl.BookListBooks)
            .ThenInclude(blb => blb.Book)
            .Where(bl => bl.IsActive)
            .AsNoTracking();

        // Filtrar listas privadas
        if (!includePrivate)
        {
            query = query.Where(bl => bl.IsPublic);
        }

        // Aplicar filtros
        query = query
            .ApplySearchFilter(parameters.SearchText)
            .ApplyGenreFilter(parameters.GenreIds);

        // Aplicar ordenamiento
        query = query.ApplySorting(parameters.SortBy, parameters.SortDescending);

        // Paginación
        var totalRecords = await query.CountAsync();
        var bookLists = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResponse<BookList>(bookLists, parameters.PageNumber, parameters.PageSize, totalRecords);
    }

    public async Task<BookList?> GetByIdAsync(int id, bool includePrivate = false)
    {
        var query = _db.BookLists
            .Include(bl => bl.Creator)
            .Include(bl => bl.Genres)
            .Include(bl => bl.BookListBooks)
            .ThenInclude(blb => blb.Book)
            .Where(bl => bl.Id == id && bl.IsActive);

        if (!includePrivate)
        {
            query = query.Where(bl => bl.IsPublic);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<BookList>> GetByUserIdAsync(string userId, bool includePrivate = false)
    {
        var query = _db.BookLists
            .Include(bl => bl.Creator)
            .Include(bl => bl.Genres)
            .Include(bl => bl.BookListBooks)
            .ThenInclude(blb => blb.Book)
            .Where(bl => bl.CreatorId == userId && bl.IsActive);

        if (!includePrivate)
        {
            query = query.Where(bl => bl.IsPublic);
        }

        return await query.ToListAsync();
    }

    public async Task<BookList> CreateAsync(BookList bookList)
    {
        _db.BookLists.Add(bookList);
        await _db.SaveChangesAsync();
        return bookList;
    }

    public async Task UpdateAsync(BookList bookList)
    {
        _db.BookLists.Update(bookList);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var bookList = await _db.BookLists.FindAsync(id);
        if (bookList == null)
            return false;

        bookList.IsActive = false;
        bookList.UpdateDate = DateTime.Now;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _db.BookLists
            .AnyAsync(bl => bl.Id == id && bl.IsActive);
    }

    public async Task<bool> IsOwnerAsync(int bookListId, string userId)
    {
        return await _db.BookLists
            .AnyAsync(bl => bl.Id == bookListId && bl.CreatorId == userId && bl.IsActive);
    }

    public async Task<bool> AddBookToListAsync(int bookListId, int bookId)
    {
        var exists = await _db.BookListBooks
            .AnyAsync(blb => blb.BookListId == bookListId && blb.BookId == bookId);

        if (exists)
            return false;

        var maxOrder = await _db.BookListBooks
            .Where(blb => blb.BookListId == bookListId)
            .MaxAsync(blb => (int?)blb.Order) ?? 0;

        var bookListBook = new BookListBook
        {
            BookListId = bookListId,
            BookId = bookId,
            AddedDate = DateTime.Now,
            Order = maxOrder + 1
        };

        _db.BookListBooks.Add(bookListBook);

        var bookList = await _db.BookLists.FindAsync(bookListId);
        if (bookList != null)
        {
            bookList.BookCount++;
            bookList.UpdateDate = DateTime.Now;
        }

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveBookFromListAsync(int bookListId, int bookId)
    {
        var bookListBook = await _db.BookListBooks
            .FirstOrDefaultAsync(blb => blb.BookListId == bookListId && blb.BookId == bookId);

        if (bookListBook == null)
            return false;

        _db.BookListBooks.Remove(bookListBook);

        var bookList = await _db.BookLists.FindAsync(bookListId);
        if (bookList != null)
        {
            bookList.BookCount--;
            bookList.UpdateDate = DateTime.Now;
        }

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> BookExistsInListAsync(int bookListId, int bookId)
    {
        return await _db.BookListBooks
            .AnyAsync(blb => blb.BookListId == bookListId && blb.BookId == bookId);
    }

    public async Task<int> GetUserBookListCountAsync(string userId)
    {
        return await _db.BookLists
            .CountAsync(bl => bl.CreatorId == userId && bl.IsActive);
    }

    public async Task<bool> CanUserCreateMoreListsAsync(string userId, int maxListsPerUser)
    {
        var currentCount = await GetUserBookListCountAsync(userId);
        return currentCount < maxListsPerUser;
    }
    
    public async Task<PagedResponse<BookListItemDto>> GetBooksByListIdAsync(int bookListId, BookListBooksQueryParameters parameters)
    {
        var query = _db.BookListBooks
            .Where(blb => blb.BookListId == bookListId)
            .Include(blb => blb.Book)
                .ThenInclude(b => b.Author)
            .Include(blb => blb.Book.Genres)
            .Select(blb => new BookListItemDto
            {
                Id = blb.Book.Id,
                Title = blb.Book.Title,
                ImageUrl = blb.Book.ImageUrl,
                Score = blb.Book.Score,
                AddedDate = blb.AddedDate,
                Order = blb.Order,
            })
            .AsNoTracking();

        // Aplicar ordenamiento
        query = parameters.SortBy?.ToLower() switch
        {
            "title" => parameters.SortDescending 
                ? query.OrderByDescending(b => b.Title) 
                : query.OrderBy(b => b.Title),
            "score" => parameters.SortDescending 
                ? query.OrderByDescending(b => b.Score) 
                : query.OrderBy(b => b.Score),
            "addeddate" => parameters.SortDescending 
                ? query.OrderByDescending(b => b.AddedDate) 
                : query.OrderBy(b => b.AddedDate),
            "order" => parameters.SortDescending 
                ? query.OrderByDescending(b => b.Order) 
                : query.OrderBy(b => b.Order),
            _ => query.OrderBy(b => b.Order) // Default: orden de agregado
        };

        // Paginación
        var totalRecords = await query.CountAsync();
        var books = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResponse<BookListItemDto>(books, parameters.PageNumber, parameters.PageSize, totalRecords);
    }
    }
using backend.Models;
using backend.Models.Dtos.BookQueries;
using Microsoft.EntityFrameworkCore; // Necesario para EF.Functions
public static class BookFilters
{
     public static IQueryable<Book> ApplySearchFilter(
        this IQueryable<Book> query, 
        string? searchText, 
        SearchInField? searchIn)
    {
        if (string.IsNullOrEmpty(searchText)) 
            return query;

        // Collation que ignora acentos y distinción de mayúsculas (SQL Server)
        const string accentInsensitiveCollation = "Latin1_General_CI_AI";

        return searchIn switch
        {
            SearchInField.Title => query.Where(b => 
                EF.Functions.Collate(b.Title, accentInsensitiveCollation)
                    .Contains(EF.Functions.Collate(searchText, accentInsensitiveCollation))),
            
            SearchInField.Author => query.Where(b => 
                EF.Functions.Collate(b.Author.DisplayName, accentInsensitiveCollation)
                    .Contains(EF.Functions.Collate(searchText, accentInsensitiveCollation))),
            
            SearchInField.Description => query.Where(b => 
                b.Description != null && 
                EF.Functions.Collate(b.Description, accentInsensitiveCollation)
                    .Contains(EF.Functions.Collate(searchText, accentInsensitiveCollation))),
            
            _ => query.Where(b => 
                EF.Functions.Collate(b.Title, accentInsensitiveCollation).Contains(searchText) ||
                (b.Description != null && 
                 EF.Functions.Collate(b.Description, accentInsensitiveCollation).Contains(searchText)) ||
                EF.Functions.Collate(b.Author.DisplayName, accentInsensitiveCollation).Contains(searchText))
        };
    }

    public static IQueryable<Book> ApplyStateFilter(
        this IQueryable<Book> query, 
        State? state)
    {
        return state.HasValue 
            ? query.Where(b => b.State == state.Value) 
            : query;
    }

    public static IQueryable<Book> ApplyGenreFilter(
        this IQueryable<Book> query, 
        List<int>? genreIds)
    {
        if (genreIds == null || !genreIds.Any()) 
            return query;

        foreach (var id in genreIds)
        {
            query = query.Where(b => b.Genres.Any(g => g.Id == id));
        }
        return query;
    }
}
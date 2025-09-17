using backend.Models;
using Microsoft.EntityFrameworkCore;

public static class BookListFilters
{
    public static IQueryable<BookList> ApplySearchFilter(
        this IQueryable<BookList> query, 
        string? searchText)
    {
        if (string.IsNullOrEmpty(searchText)) 
            return query;

        const string accentInsensitiveCollation = "Latin1_General_CI_AI";

        return query.Where(bl => 
            EF.Functions.Collate(bl.Title, accentInsensitiveCollation)
                .Contains(EF.Functions.Collate(searchText, accentInsensitiveCollation)) ||
            (bl.Description != null && 
             EF.Functions.Collate(bl.Description, accentInsensitiveCollation)
                .Contains(EF.Functions.Collate(searchText, accentInsensitiveCollation))));
    }

    public static IQueryable<BookList> ApplyGenreFilter(
        this IQueryable<BookList> query, 
        List<int>? genreIds)
    {
        if (genreIds == null || !genreIds.Any()) 
            return query;

        return query.Where(bl => bl.Genres.Any(g => genreIds.Contains(g.Id)));
    }
}


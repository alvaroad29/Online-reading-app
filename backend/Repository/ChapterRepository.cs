using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Models;
using backend.Models.Dtos;
using backend.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace backend.Repository;

public class ChapterRepository : IChapterRepository
{
    ApplicationDbContext _db;

    public ChapterRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Chapter?> GetChapterByIdAsync(int chapterId)
    {
        return await _db.Chapters.FindAsync(chapterId);
    }

    public async Task IncrementViewCountAsync(Chapter chapter)
    {
        chapter.ViewCount++;
        _db.Chapters.Update(chapter);
        await _db.SaveChangesAsync();
    }

    public async Task AddToReadingHistoryAsync(ReadingHistory history)
    {
        _db.ReadingHistories.Add(history);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> HasUserViewedChapterAsync(int chapterId, string userId)
    {
        return await _db.ReadingHistories
            .AnyAsync(r => r.ChapterId == chapterId && r.UserId == userId);
    }

    public async Task<Chapter> CreateChapterAsync(Chapter chapter)
    {
        await _db.Chapters.AddAsync(chapter);
        await _db.SaveChangesAsync();
        return chapter;
    }

    public async Task<Chapter?> GetPreviousChapterAsync(int volumeId, int currentOrder)
    {
        return await _db.Chapters
            .Where(c => c.VolumeId == volumeId && c.Order < currentOrder)
            .OrderByDescending(c => c.Order)
            .FirstOrDefaultAsync();
    }

    public async Task<Chapter?> GetNextChapterAsync(int volumeId, int currentOrder)
    {
        return await _db.Chapters
            .Where(c => c.VolumeId == volumeId && c.Order > currentOrder)
            .OrderBy(c => c.Order)
            .FirstOrDefaultAsync();
    }

    public async Task<int> GetNextOrderForVolumeAsync(int volumeId)
    {
        var lastOrder = await _db.Chapters
            .Where(c => c.VolumeId == volumeId)
            .OrderByDescending(c => c.Order)
            .Select(c => c.Order)
            .FirstOrDefaultAsync();
        return lastOrder + 1;
    }
    
}

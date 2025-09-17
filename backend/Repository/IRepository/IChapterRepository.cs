using backend.Models;
using backend.Models.Dtos;
using backend.Models.Dtos.BookQueries;
namespace backend.Repository.IRepository;

public interface IChapterRepository
{
    Task<Chapter?> GetChapterByIdAsync(int chapterId);
    Task IncrementViewCountAsync(Chapter chapter);
    Task AddToReadingHistoryAsync(ReadingHistory history);
    Task<bool> HasUserViewedChapterAsync(int chapterId, string userId);
    Task<Chapter> CreateChapterAsync(Chapter chapter);

    Task<Chapter?> GetPreviousChapterAsync(int volumeId, int currentOrder);
    Task<Chapter?> GetNextChapterAsync(int volumeId, int currentOrder);
    Task<int> GetNextOrderForVolumeAsync(int volumeId);
}
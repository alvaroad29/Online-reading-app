using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Models;
using backend.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace backend.Repository;

public class VolumeRepository : IVolumeRepository
{
    ApplicationDbContext _db;

    public VolumeRepository(ApplicationDbContext db)
    {
        _db = db;
    }


    // Obtener volúmenes con capítulos activos de un libro
    public async Task<IEnumerable<VolumeDto>> GetVolumesByBook(int bookId)
    {
        return await _db.Volumes
            .Where(v => v.BookId == bookId && v.IsActive)
            .OrderBy(v => v.Order)
            .Select(v => new VolumeDto
            {
                Id = v.Id,
                Title = v.Title,
                Order = v.Order,
                Chapters = v.Chapters
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.Order)
                    .Select(c => new ChapterDto
                    {
                        Id = c.Id,
                        Title = c.Title,
                        ViewCount = c.ViewCount,
                        PublishDate = c.PublishDate,
                        Order = c.Order
                    })
                    .ToList()
            })
            .ToListAsync();
    }

    // Crear volumen
    public async Task<Volume> CreateVolume(VolumeCreateDto volumeDto)
    {
        var volume = new Volume
        {
            Title = volumeDto.Title,
            Order = volumeDto.Order,
            BookId = volumeDto.BookId,
            IsActive = true
        };

        _db.Volumes.Add(volume);
        await _db.SaveChangesAsync();
        return volume;
    }

    // Actualizar volumen
    public async Task UpdateVolume(int volumeId, VolumeUpdateDto volumeDto)
    {
        var volume = await _db.Volumes.FindAsync(volumeId);
        if (volume == null) throw new KeyNotFoundException("Volume not found");

        volume.Title = volumeDto.Title ?? volume.Title;
        volume.Order = volumeDto.Order ?? volume.Order;
        volume.IsActive = volumeDto.IsActive ?? volume.IsActive;

        await _db.SaveChangesAsync();
    }

    // Eliminar volumen (soft delete)
    public async Task<bool> DeleteVolume(int volumeId)
    {
        var volume = await _db.Volumes.FindAsync(volumeId);
        if (volume == null) return false;

        volume.IsActive = false;
        await _db.SaveChangesAsync();
        return true;
    }
}

using System;
using backend.Models;
using backend.Models.Dtos;

namespace backend.Repository;

public interface IVolumeRepository
{
    // Task<VolumeDto> GetVolumeWithChapters(int volumeId);
    Task<IEnumerable<VolumeDto>> GetVolumesByBook(int bookId);
    Task<Volume> CreateVolume(VolumeCreateDto volumeDto);
    Task UpdateVolume(int volumeId, VolumeUpdateDto volumeDto);
    Task<bool> DeleteVolume(int volumeId);

}
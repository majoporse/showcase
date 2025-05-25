using BusinessLayer.DTOs;
using Infrastructure.QueryObjects;

namespace BusinessLayer.Services.Interfaces;

public interface IManufacturerService
{
    Task<ManufacturerDto?> CreateManufacturerAsync(ManufacturerDto manufacturerDto);
    Task<IEnumerable<ManufacturerDto>> GetAllManufacturersAsync();
    Task<FilteredResult<ManufacturerDto>> GetManufacturersAsync(ManufacturerFilterDto manufacturerFilterDto);
    Task<ManufacturerDto?> GetManufacturerByIdAsync(int id);
    Task<ManufacturerDto?> UpdateManufacturerAsync(ManufacturerDto manufacturerDto);
    Task<bool> DeleteManufacturerByIdAsync(int id);
}
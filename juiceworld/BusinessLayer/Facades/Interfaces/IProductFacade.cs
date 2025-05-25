using BusinessLayer.DTOs;
using Infrastructure.QueryObjects;

namespace BusinessLayer.Facades.Interfaces;

public interface IProductFacade
{
    Task<ProductDto?> CreateProductAsync(ProductImageDto productDto);
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<FilteredResult<ProductDto>> GetProductsFilteredAsync(ProductFilterDto productFilter);
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<ProductDetailDto?> GetProductDetailByIdAsync(int id);
    Task<ProductDto?> UpdateProductAsync(ProductImageDto productDto, int userId);
    Task<bool> DeleteProductByIdAsync(int id);
}

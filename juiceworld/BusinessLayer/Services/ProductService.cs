using System.Text.Json;
using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Services.Interfaces;
using Commons.Enums;
using Infrastructure.QueryObjects;
using Infrastructure.Repositories;
using JuiceWorld.Entities;
using JuiceWorld.UnitOfWork;
using Microsoft.Extensions.Caching.Memory;

namespace BusinessLayer.Services;

public class ProductService(
    IRepository<Product> productRepository,
    IMapper mapper,
    IMemoryCache memoryCache,
    IQueryObject<Product> queryObject,
    ProductUnitOfWork productUnitOfWork
) : IProductService
{
    private readonly string _cacheKeyPrefix = nameof(ProductService);
    private string CacheKeyFiltered(ProductFilterDto productFilter) => $"{_cacheKeyPrefix}-product{productFilter.GetHashCode()}";
    private string CacheKeyFilteredDetail(ProductFilterDto productFilter) => $"{_cacheKeyPrefix}-productDetail{productFilter.GetHashCode()}";
    private string CacheKeyProduct(int id) => $"{_cacheKeyPrefix}-product{id}";

    public async Task<ProductDto?> CreateProductAsync(ProductDto productDto)
    {
        var newProduct = await productRepository.CreateAsync(mapper.Map<Product>(productDto));
        return newProduct is null ? null : mapper.Map<ProductDto>(newProduct);
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await productRepository.GetAllAsync();
        return mapper.Map<List<ProductDto>>(products);
    }

    public async Task<FilteredResult<ProductDto>> GetProductsFilteredAsync(ProductFilterDto productFilter)
    {
        var cacheKey = CacheKeyFiltered(productFilter);
        if (!memoryCache.TryGetValue(cacheKey, out FilteredResult<Product>? value))
        {
            var query = GetQueryObject(productFilter);
            query.Include(nameof(Product.Tags), nameof(Product.Manufacturer), nameof(Product.Reviews));
            value = await query.ExecuteAsync();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(30));
            memoryCache.Set(cacheKey, value, cacheEntryOptions);
        }

        return new FilteredResult<ProductDto>
        {
            Entities = mapper.Map<List<ProductDto>>(value!.Entities),
            PageIndex = value.PageIndex,
            TotalPages = value.TotalPages
        };
    }

    public async Task<FilteredResult<ProductDetailDto>> GetProductDetailsFilteredAsync(ProductFilterDto productFilter)
    {
        var cacheKey = CacheKeyFilteredDetail(productFilter);
        if (!memoryCache.TryGetValue(cacheKey, out FilteredResult<Product>? value))
        {
            var query = GetQueryObject(productFilter);
            value = await query.ExecuteAsync();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(30));
            memoryCache.Set(cacheKey, value, cacheEntryOptions);
        }

        return new FilteredResult<ProductDetailDto>
        {
            Entities = mapper.Map<List<ProductDetailDto>>(value!.Entities),
            PageIndex = value.PageIndex,
            TotalPages = value.TotalPages
        };
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var cacheKey = CacheKeyProduct(id);
        if (!memoryCache.TryGetValue(cacheKey, out Product? value))
        {
            value = await productRepository.GetByIdAsync(id, nameof(Product.Tags));

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(30));
            memoryCache.Set(cacheKey, value, cacheEntryOptions);
        }

        return value is null ? null : mapper.Map<ProductDto>(value);
    }

    public async Task<ProductDetailDto?> GetProductDetailByIdAsync(int id)
    {
        var value = await productRepository.GetByIdAsync(id, nameof(Product.Manufacturer),
            nameof(Product.Reviews), $"{nameof(Product.Reviews)}.{nameof(Review.User)}", nameof(Product.Tags));

        if (value is null) return null;

        value.Reviews = value.Reviews.OrderByDescending(r => r.CreatedAt).ToList();

        return mapper.Map<ProductDetailDto>(value);
    }

    public async Task<ProductDto?> UpdateProductAsync(ProductDto productDto, int userId)
    {
        var cacheKey1 = CacheKeyProduct(productDto.Id);
        memoryCache.Remove(cacheKey1);
        var oldProduct = await productUnitOfWork.ProductRepository.GetByIdAsync(productDto.Id);
        if (oldProduct is null)
        {
            return null;
        }

        var product = mapper.Map<Product>(productDto);
        oldProduct.Tags.Clear();

        var tags = await productUnitOfWork.TagRepository.GetByIdRangeAsync(productDto.TagIds.Cast<object>());
        product.Tags = tags.ToList();

        var updatedProduct = await productUnitOfWork.ProductRepository.UpdateAsync(product, userId, false);
        await productUnitOfWork.Commit(userId);
        return updatedProduct is null ? null : mapper.Map<ProductDto>(updatedProduct);
    }

    public Task<bool> DeleteProductByIdAsync(int id)
    {
        var cacheKey1 = CacheKeyProduct(id);
        memoryCache.Remove(cacheKey1);
        return productRepository.DeleteAsync(id);
    }

    private IQueryObject<Product> GetQueryObject(ProductFilterDto productFilter)
    {
        Enum.TryParse<ProductCategory>(productFilter.Category, true, out var categoryEnum);

        return queryObject.Filter(p =>
                (productFilter.Category == null || p.Category == categoryEnum) &&
                (productFilter.PriceMax == null || p.Price <= productFilter.PriceMax) &&
                (productFilter.PriceMin == null || p.Price >= productFilter.PriceMin) &&
                (productFilter.ManufacturerId == null || p.ManufacturerId == productFilter.ManufacturerId) &&
                (productFilter.TagId == null || p.Tags.Any(t => t.Id == productFilter.TagId)) &&
                (p.Manufacturer == null || productFilter.NameQuery == null ||
                 p.Name.ToLower().Contains(productFilter.NameQuery.ToLower()) ||
                 p.Description.ToLower().Contains(productFilter.NameQuery.ToLower())))
            .OrderBy(p => p.Id)
            .Paginate(productFilter.PageIndex, productFilter.PageSize);
    }
}

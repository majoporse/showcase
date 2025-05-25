using System.Security.Claims;
using BusinessLayer.DTOs;
using BusinessLayer.Facades.Interfaces;
using Commons.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Customer))]
public class ProductController(IProductFacade productFacade) : ControllerBase
{
    private const string ApiBaseName = "Product";

    [HttpPost]
    [OpenApiOperation(ApiBaseName + nameof(CreateProduct))]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<ProductDto>> CreateProduct(ProductImageDto product)
    {
        var result = await productFacade.CreateProductAsync(product);
        return result == null ? Problem() : Ok(result);
    }

    [HttpGet]
    [OpenApiOperation(ApiBaseName + nameof(GetProductFiltered))]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductFiltered(
        [FromQuery] ProductFilterDto productFilter)
    {
        var result = await productFacade.GetProductsFilteredAsync(productFilter);
        return Ok(result);
    }

    [HttpGet("{productId:int}")]
    [OpenApiOperation(ApiBaseName + nameof(GetProduct))]
    public async Task<ActionResult<ProductDto>> GetProduct(int productId)
    {
        var result = await productFacade.GetProductByIdAsync(productId);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("{productId:int}/detail")]
    [OpenApiOperation(ApiBaseName + nameof(GetProductDetail))]
    public async Task<ActionResult<ProductDetailDto>> GetProductDetail(int productId)
    {
        var result = await productFacade.GetProductDetailByIdAsync(productId);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPut]
    [OpenApiOperation(ApiBaseName + nameof(UpdateProduct))]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<ProductDto>> UpdateProduct(ProductImageDto product)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.Sid) ?? "", out var userId))
        {
            return Unauthorized();
        }

        var result = await productFacade.UpdateProductAsync(product, userId);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{productId:int}")]
    [OpenApiOperation(ApiBaseName + nameof(DeleteProduct))]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<bool>> DeleteProduct(int productId)
    {
        var result = await productFacade.DeleteProductByIdAsync(productId);
        return result ? Ok() : NotFound();
    }
}

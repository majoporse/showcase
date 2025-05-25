using AutoMapper;
using System.Security.Claims;
using BusinessLayer.DTOs;
using BusinessLayer.Facades.Interfaces;
using BusinessLayer.Services.Interfaces;
using Infrastructure.QueryObjects;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Mvc.ActionFilters;
using PresentationLayer.Mvc.Areas.Admin.Models;
using PresentationLayer.Mvc.Models;

namespace PresentationLayer.Mvc.Areas.Admin.Controllers;

[Area(Constants.Areas.Admin)]
[RedirectIfNotAdminActionFilter]
public class ProductController(IProductFacade productFacade,
    IManufacturerService manufacturerService,
    IMapper mapper,
    ITagService tagService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] ProductFilterViewModel productFilter)
    {
        var products = await productFacade.GetProductsFilteredAsync(mapper.Map<ProductFilterDto>(productFilter));
        return View(mapper.Map<FilteredResult<ProductViewModel>>(products));
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var manufacturers = await manufacturerService.GetAllManufacturersAsync();
        var tags = await tagService.GetAllTagsAsync();
        var viewModel = new ProductEditViewModel
        {
            Product = new ProductImageViewModel(),
            AllManufacturers = mapper.Map<IEnumerable<ManufacturerViewModel>>(manufacturers),
            AllTags = mapper.Map<IEnumerable<TagViewModel>>(tags)
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductEditViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            viewModel.AllManufacturers = mapper.Map<IEnumerable<ManufacturerViewModel>>(await manufacturerService.GetAllManufacturersAsync());
            viewModel.AllTags = mapper.Map<IEnumerable<TagViewModel>>(await tagService.GetAllTagsAsync());
            return View(viewModel);
        }

        if (viewModel.ImageFile != null && viewModel.ImageFile.Length > 0)
        {
            using (var memoryStream = new MemoryStream())
            {
                await viewModel.ImageFile.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();
                viewModel.Product.ImageValue = Convert.ToBase64String(imageBytes);
            }
        }

        var createdProduct = await productFacade.CreateProductAsync(mapper.Map<ProductImageDto>(viewModel.Product));
        if (createdProduct == null)
        {
            ModelState.AddModelError("Id", "Failed to create product.");
            return View(viewModel);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await productFacade.GetProductByIdAsync(id);
        if (product == null)
        {
            ModelState.AddModelError("Id", "Product not found.");
            return View();
        }

        var manufacturers = await manufacturerService.GetAllManufacturersAsync();
        var tags = await tagService.GetAllTagsAsync();
        var viewModel = new ProductEditViewModel
        {
            Product = mapper.Map<ProductImageViewModel>(product),
            AllManufacturers = mapper.Map<IEnumerable<ManufacturerViewModel>>(manufacturers),
            AllTags = mapper.Map<IEnumerable<TagViewModel>>(tags)
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ProductEditViewModel viewModel)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.Sid) ?? "", out var userId))
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            viewModel.AllManufacturers = mapper.Map<IEnumerable<ManufacturerViewModel>>(await manufacturerService.GetAllManufacturersAsync());
            viewModel.AllTags = mapper.Map<IEnumerable<TagViewModel>>(await tagService.GetAllTagsAsync());
            return View(viewModel);
        }

        if (viewModel.ImageFile != null && viewModel.ImageFile.Length > 0)
        {
            using (var memoryStream = new MemoryStream())
            {
                await viewModel.ImageFile.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();
                viewModel.Product.ImageValue = Convert.ToBase64String(imageBytes);
            }
        }

        var updatedProduct = await productFacade.UpdateProductAsync(mapper.Map<ProductImageDto>(viewModel.Product), userId);
        if (updatedProduct == null)
        {
            ModelState.AddModelError("Id", "Failed to update product.");
            return View(viewModel);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await productFacade.DeleteProductByIdAsync(id);
        return result ? RedirectToAction(nameof(Index)) : View(Constants.Views.NotFound);
    }
}

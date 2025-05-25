using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Services.Interfaces;
using Infrastructure.QueryObjects;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Mvc.ActionFilters;
using PresentationLayer.Mvc.Areas.Admin.Models;
using PresentationLayer.Mvc.Models;

namespace PresentationLayer.Mvc.Areas.Admin.Controllers;

[Area(Constants.Areas.Admin)]
[RedirectIfNotAdminActionFilter]
public class ManufacturerController(IManufacturerService manufacturerService, IMapper mapper) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] ManufacturerFilterViewModel manufacturerFilterDto)
    {
        var manufacturers = await manufacturerService.GetManufacturersAsync(mapper.Map<ManufacturerFilterDto>(manufacturerFilterDto));
        return View(mapper.Map<FilteredResult<ManufacturerViewModel>>(manufacturers));
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new ManufacturerViewModel { Name = "" });
    }

    [HttpPost]
    public async Task<IActionResult> Create(ManufacturerViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var createdManufacturer = await manufacturerService.CreateManufacturerAsync(mapper.Map<ManufacturerDto>(viewModel));
        if (createdManufacturer == null)
        {
            ModelState.AddModelError("Id", "Failed to create manufacturer.");
            return View(viewModel);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var manufacturer = mapper.Map<ManufacturerViewModel>(await manufacturerService.GetManufacturerByIdAsync(id));
        if (manufacturer == null)
        {
            ModelState.AddModelError("Id", "Manufacturer not found.");
            return View(manufacturer);
        }

        return View(manufacturer);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ManufacturerViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var updatedManufacturer = await manufacturerService.UpdateManufacturerAsync(mapper.Map<ManufacturerDto>(viewModel));
        if (updatedManufacturer == null)
        {
            ModelState.AddModelError("Id", "Failed to update manufacturer.");
            return View(viewModel);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var isDeleted = await manufacturerService.DeleteManufacturerByIdAsync(id);
        if (!isDeleted)
        {
            ModelState.AddModelError("Id", "Failed to delete manufacturer.");
            return View(nameof(Index));
        }

        return RedirectToAction(nameof(Index));
    }
}

using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Services.Interfaces;
using Infrastructure.QueryObjects;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Mvc.ActionFilters;
using PresentationLayer.Mvc.Areas.Admin.Models;

namespace PresentationLayer.Mvc.Areas.Admin.Controllers;

[Area(Constants.Areas.Admin)]
[RedirectIfNotAdminActionFilter]
public class GiftCardController(IGiftCardService giftCardService, IMapper mapper) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] GiftCardFilterViewModel giftCardFilterViewModel)
    {
        var allGiftCards = await giftCardService.GetAllGiftCardsAsync(mapper.Map<GiftCardFilterDto>(giftCardFilterViewModel));
        return View(mapper.Map<FilteredResult<GiftCardEditViewModel>>(allGiftCards));
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new GiftCardCreateViewModel
        {
            Name = "",
            Discount = 0,
            CouponsCount = 0,
            StartDateTime = null,
            ExpiryDateTime = null
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(GiftCardCreateViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var createdManufacturer = await giftCardService.CreateGiftCardAsync(mapper.Map<GiftCardCreateDto>(viewModel));
        if (createdManufacturer == null)
        {
            ModelState.AddModelError("Id", "Failed to create gift-card.");
            return View(viewModel);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var giftCard = await giftCardService.GetGiftCardByIdAsync(id);
        if (giftCard == null)
        {
            ModelState.AddModelError("Id", "Manufacturer not found.");
            return View(new GiftCardEditViewModel());
        }

        return View(new GiftCardEditViewModel { Name = giftCard.Name });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(GiftCardEditViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var updatedManufacturer = await giftCardService.UpdateGiftCardAsync(mapper.Map<GiftCardEditDto>(viewModel));
        if (updatedManufacturer == null)
        {
            ModelState.AddModelError("Id", "Failed to update manufacturer.");
            return View(viewModel);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var giftCard = await giftCardService.GetGiftCardByIdAsync(id);
        if (giftCard == null)
        {
            ModelState.AddModelError("Id", "Gift-card not found.");
            return RedirectToAction(nameof(Index));
        }

        return View(mapper.Map<GiftCardDetailViewModel>(giftCard));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var isDeleted = await giftCardService.DeleteGiftCardByIdAsync(id);
        if (!isDeleted)
        {
            ModelState.AddModelError("Id", "Failed to delete manufacturer.");
            return View(nameof(Index));
        }

        return RedirectToAction(nameof(Index));
    }
}

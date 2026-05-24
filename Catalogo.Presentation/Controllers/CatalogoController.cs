using CatalogoApp.Domain.Models;
using Catalogo.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Catalogo.Presentation.Controllers;

public class CatalogoController : Controller
{
    private readonly ItemService _itemService;

    public CatalogoController(ItemService itemService)
    {
        _itemService = itemService;
    }

    public IActionResult Index()
    {
        var items = _itemService.GetAll();
        return View(items);
    }

    public IActionResult Detalle(int id)
    {
        var item = _itemService.GetById(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost]
    [Authorize]
    public IActionResult AgregarResena(int itemId, int rating, string comment)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var username = User.FindFirstValue(ClaimTypes.Name)!;

        var review = new Review
        {
            UserId = userId,
            Username = username,
            Rating = rating,
            Comment = comment
        };

        var (success, message) = _itemService.AddReview(itemId, review);
        TempData[success ? "Success" : "Error"] = message;

        return RedirectToAction("Detalle", new { id = itemId });
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exam.Data;
using Exam.Models;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

[Authorize]
public class VehicleController : Controller
{
    private readonly ApplicationDbContext _context;

    public VehicleController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(int? categoryId, bool showSold = false)//показать карту машины
    {
        ViewBag.Categories = _context.Categories.ToList();
        ViewBag.SelectedCategoryId = categoryId;
        ViewBag.ShowSold = showSold;

        var vehicles = _context.Vehicles
            .Include(v => v.Category)
            .AsQueryable();

        if (!showSold)
        {
            vehicles = vehicles.Where(v => !v.IsSold); // только не проданные
        }
        else
        {
            vehicles = vehicles.Where(v => v.IsSold); // только проданные
        }

        if (categoryId.HasValue)
        {
            vehicles = vehicles.Where(v => v.CategoryId == categoryId.Value); //фильтр категорий
        }

        if (User.Identity.IsAuthenticated)//зарегистрированный пользователь (списать эмэил)
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            ViewBag.UserEmail = email;
        }

        var cartJson = HttpContext.Session.GetString("Cart"); //для корзины (получение из сессии)
        List<CartItem> cart = new();

        if (!string.IsNullOrEmpty(cartJson))//подстраховка для карточек машин
        {
            cart = JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }
        ViewBag.CartProductIds = cart.Select(c => c.ProductId).ToList();

        return View(vehicles.ToList());
    }

}

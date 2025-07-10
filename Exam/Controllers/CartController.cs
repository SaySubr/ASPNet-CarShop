using Exam.Models;
using Microsoft.AspNetCore.Mvc;

public class CartController : Controller
{
    private readonly CartService _cartService;

    public CartController(CartService cartService)
    {
        _cartService = cartService;
    }

    [HttpPost]
    public IActionResult AddToCart(int productId, string productName, decimal price, int quantity)
    {
        var item = new CartItem
        {
            ProductId = productId,
            ProductName = productName,
            Price = price,
            Quantity = quantity
        };

        _cartService.AddItem(item);

        return RedirectToAction("Index", "Vehicle", new { id = productId });        //Здесь карточка товара
    }

    public IActionResult Index()
    {
        var cart = _cartService.GetCart();
        return View(cart); // передать корзину во View
    }

    public IActionResult Remove(int productId)
    {
        _cartService.RemoveItem(productId);
        return RedirectToAction("Index");
    }

    public IActionResult Clear()
    {
        _cartService.ClearCart();
        return RedirectToAction("Index");
    }
}

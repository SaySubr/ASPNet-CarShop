using Exam.Data;
using Exam.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exam.Models;

public class OrderController : Controller
{
    private readonly CartService _cartService;
    private readonly OrderService _orderService;
    private readonly ApplicationDbContext _DbContext;
    public OrderController(OrderService orderService, CartService cartService, ApplicationDbContext applicationDb)
    {
        _orderService = orderService;
        _cartService = cartService;
        _DbContext = applicationDb;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int id)
    {
        var vehicle = await _orderService.GetVehicleByIdAsync(id);

        if (vehicle == null)
            return NotFound();

        return View(vehicle);
    }


    [HttpPost]//заказать машину в модальном окне
    public async Task<IActionResult> Confirm(int vehicleId, string email, string phone, string cardNumber, string expiryDate, string cvv)
    {
       
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value; //ищем аккаунт

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var order = new Order //формируем заказ для базы данных
        {
            VehicleId = vehicleId,
            Email = email,
            Phone = phone,
            CardNumber = cardNumber,
            ExpiryDate = expiryDate,
            CVV = cvv,
            OrderDate = DateTime.Now,
            UserId = userId
        };

        var saved = await _orderService.SaveOrderAsync(order);//в случае если база данных не доступна
        if (!saved)
            return BadRequest("Не удалось сохранить заказ");

        var vehicle = await _orderService.GetVehicleByIdAsync(vehicleId);//в случае если база данных не доступна
        if (vehicle == null)
            return NotFound("Машина не найдена для удаления");

        vehicle.IsSold = true;//делаем машину проданной и обновляем базу данных
        _DbContext.Vehicles.Update(vehicle);
        await _DbContext.SaveChangesAsync();

        _cartService.RemoveItem(vehicleId);//убираем карточку из списка доступных машин

        TempData["SuccessMessage"] = $"Заказ на авто успешно оформлен!";
        return RedirectToAction("Index", "Vehicle");//счастливая надпись
    }

    [HttpPost]//заказать машину из корзины
    public async Task<IActionResult> ConfirmCart(string email, string phone, string cardNumber, string expiryDate, string cvv)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value; //ищем аккаунт

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var cartItems = _cartService.GetCart();//подстраховка если корзина не увидит карты
        if (cartItems == null || !cartItems.Any())
            return BadRequest("Корзина пуста");

        foreach (var item in cartItems)//формируем заказ
        {
            var order = new Order
            {
                VehicleId = item.ProductId,
                Email = email,
                Phone = phone,
                CardNumber = cardNumber,
                ExpiryDate = expiryDate,
                CVV = cvv,
                OrderDate = DateTime.Now,
                UserId = userId
            };

            var saved = await _orderService.SaveOrderAsync(order);//сохраняем все машины в корзине и отдаем в базу данных
            if (saved)
            {
                var vehicle = await _orderService.GetVehicleByIdAsync(item.ProductId);
                if (vehicle != null)
                {
                    vehicle.IsSold = true;
                    _DbContext.Vehicles.Update(vehicle);
                }
            }
        }

        await _DbContext.SaveChangesAsync();
        _cartService.ClearCart();//очищаем корзину

        TempData["SuccessMessage"] = "Ваш заказ успешно оформлен!";
        return RedirectToAction("Index", "Vehicle");
    }



    [HttpGet]
    public IActionResult Cart()//подстраховка для корзины (чтоб не путал карточки машин)
    {
        var cartItems = _cartService.GetCart();

        var viewModel = cartItems
            .Select(ci => new CartItemViewModel
            {
                Vehicle = _DbContext.Vehicles.FirstOrDefault(v => v.Id == ci.ProductId),
                Quantity = ci.Quantity
            })
            .Where(vm => vm.Vehicle != null)
            .ToList();

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult RemoveItem(int id)//удаление товара из корзины
    {
        _cartService.RemoveItem(id);
        return RedirectToAction("Cart");
    }


}

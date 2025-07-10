using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Exam.Models;

public class CartService
{
    private readonly IHttpContextAccessor _httpContextAccessor; //сессия аккаунта
    private const string SessionKey = "Cart"; //ключ сессии


    public CartService(IHttpContextAccessor httpContextAccessor) //конструктор
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ISession Session => _httpContextAccessor.HttpContext.Session; //получение сессии из контекста HTTP

    public List<CartItem> GetCart() //получение корзины из сессии
    {
        var cartJson = Session.GetString(SessionKey);
        if (string.IsNullOrEmpty(cartJson))
            return new List<CartItem>();

        return JsonSerializer.Deserialize<List<CartItem>>(cartJson);
    }

    public void SaveCart(List<CartItem> cart)// сохранение корзины в сессию 
    {
        var cartJson = JsonSerializer.Serialize(cart);
        Session.SetString(SessionKey, cartJson);
    }

    public void AddItem(CartItem item)// добавление товара в корзину 
    {
        var cart = GetCart();
        var existingItem = cart.FirstOrDefault(i => i.ProductId == item.ProductId);
        if (existingItem != null)
        {
            existingItem.Quantity += item.Quantity;
        }
        else
        {
            cart.Add(item);
        }
        SaveCart(cart);
    }


    public void RemoveItem(int productId) //удаление товара 
    {
        var cart = GetCart();
        var item = cart.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            cart.Remove(item);
            SaveCart(cart);
        }
    }

    public void ClearCart()//очищение товара (для админа
    {
        Session.Remove(SessionKey);
    }
}


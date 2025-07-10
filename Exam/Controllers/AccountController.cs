using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;// менеджер входа в систему
    private readonly UserManager<IdentityUser> _userManager;// менеджер пользователей

    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public IActionResult Login() => View();
    public IActionResult Register() => View();

    
    [HttpPost]
    public async Task<IActionResult> Register(string email, string password)// Регистрация пользователя
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))//Ошибка для хитрицов
        {
            ViewData["Message"] = "Email и пароль обязательны!";
            return View();
        }

        var user = new IdentityUser { UserName = email, Email = email };//формируем аккаунт
        var result = await _userManager.CreateAsync(user, password);//сбрасываем в базу данных

        if (result.Succeeded)//счастливое сообщение
        {
            await _userManager.AddToRoleAsync(user, "User");
            await _signInManager.SignInAsync(user, isPersistent: false);
            ViewData["Message"] = "Регистрация прошла успешно!";
            return RedirectToAction("Index", "Vehicle");
        }

        
        ViewData["Message"] = string.Join("<br>", result.Errors.Select(e => e.Description));// ошибка регистрации
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)//Вход пользователя
    {
        var result = await _signInManager.PasswordSignInAsync(email, password, false, false);//смотрим есть ли аккаунт

        if (result.Succeeded)
            return RedirectToAction("Index", "Vehicle");

        ViewData["Message"] = "Неверный логин или пароль";//ошибка

        return View();
    }

    public async Task<IActionResult> Logout()//выйти из аккаунта
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }
}

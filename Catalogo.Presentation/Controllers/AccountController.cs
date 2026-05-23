using System.Security.Claims;
using Catalogo.Application.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Catalogo.Presentation.Controllers;

public class AccountController : Controller
{
    private readonly UserService _userService;

    public AccountController(UserService userService)
    {
        _userService = userService;
    }

    // GET: /Account/Login
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Catalogo");
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    // POST: /Account/Login
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
    {
        if (!_userService.ValidateCredentials(email, password))
        {
            ViewBag.Error = "Correo o contraseña incorrectos.";
            return View();
        }

        var user = _userService.GetByEmail(email)!;
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        return Redirect(returnUrl ?? "/Catalogo");
    }

    // GET: /Account/Register
    public IActionResult Register() => View();

    // POST: /Account/Register
    [HttpPost]
    public IActionResult Register(string username, string email, string password, string confirmPassword)
    {
        if (password != confirmPassword)
        {
            ViewBag.Error = "Las contraseñas no coinciden.";
            return View();
        }

        var (success, message) = _userService.Register(username, email, password);
        if (!success)
        {
            ViewBag.Error = message;
            return View();
        }

        TempData["Success"] = "Cuenta creada. Ahora inicia sesión.";
        return RedirectToAction("Login");
    }

    // POST: /Account/Logout
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}
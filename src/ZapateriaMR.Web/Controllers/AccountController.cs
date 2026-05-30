using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ZapateriaMR.Application.DTOs.Auditoria;
using ZapateriaMR.Application.Interfaces;
using ZapateriaMR.Domain.Enums;
using ZapateriaMR.Infrastructure.Identity;
using ZapateriaMR.Web.ViewModels.Account;

namespace ZapateriaMR.Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuditoriaService _auditoriaService;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IAuditoriaService auditoriaService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _auditoriaService = auditoriaService;
    }

    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel
        {
            ReturnUrl = returnUrl
        });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var user = await _userManager.FindByEmailAsync(viewModel.Email);

        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
            return View(viewModel);
        }

        if (!user.Estado)
        {
            ModelState.AddModelError(string.Empty, "El usuario está inactivo. Contacte al administrador.");
            return View(viewModel);
        }

        var result = await _signInManager.PasswordSignInAsync(
            user,
            viewModel.Password,
            viewModel.RememberMe,
            lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
            return View(viewModel);
        }

        await _auditoriaService.RegistrarAsync(new RegistrarAuditoriaDto
        {
            UsuarioId = user.Id,
            NombreUsuario = $"{user.Nombre} {user.Apellido}".Trim(),
            Accion = TipoAccionAuditoria.IniciarSesion,
            EntidadAfectada = "Autenticación",
            RegistroId = user.Id,
            Detalle = $"El usuario '{user.Email}' inició sesión."
        });

        if (!string.IsNullOrWhiteSpace(viewModel.ReturnUrl) && Url.IsLocalUrl(viewModel.ReturnUrl))
        {
            return Redirect(viewModel.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is not null)
        {
            await _auditoriaService.RegistrarAsync(new RegistrarAuditoriaDto
            {
                UsuarioId = user.Id,
                NombreUsuario = $"{user.Nombre} {user.Apellido}".Trim(),
                Accion = TipoAccionAuditoria.CerrarSesion,
                EntidadAfectada = "Autenticación",
                RegistroId = user.Id,
                Detalle = $"El usuario '{user.Email}' cerró sesión."
            });
        }

        await _signInManager.SignOutAsync();

        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
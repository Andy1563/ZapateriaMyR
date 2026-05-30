using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ZapateriaMR.Application.DTOs.Auditoria;
using ZapateriaMR.Application.Interfaces;
using ZapateriaMR.Domain.Enums;
using ZapateriaMR.Infrastructure.Identity;
using ZapateriaMR.Web.ViewModels.Usuarios;

namespace ZapateriaMR.Web.Controllers;

[Authorize(Roles = "Administrador")]
public class UsuariosController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IAuditoriaService _auditoriaService;

    public UsuariosController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IAuditoriaService auditoriaService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _auditoriaService = auditoriaService;
    }

    public async Task<IActionResult> Index(string? busqueda)
    {
        var usuarios = _userManager.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var termino = busqueda.Trim();

            usuarios = usuarios.Where(u =>
                u.Email!.Contains(termino) ||
                u.Nombre.Contains(termino) ||
                u.Apellido.Contains(termino));
        }

        var lista = new List<UsuarioListadoViewModel>();

        foreach (var usuario in usuarios.OrderBy(u => u.Nombre).ToList())
        {
            var roles = await _userManager.GetRolesAsync(usuario);

            lista.Add(new UsuarioListadoViewModel
            {
                Id = usuario.Id,
                NombreCompleto = $"{usuario.Nombre} {usuario.Apellido}".Trim(),
                Email = usuario.Email ?? string.Empty,
                Estado = usuario.Estado,
                FechaCreacion = usuario.FechaCreacion,
                Roles = roles.Any() ? string.Join(", ", roles) : "Sin rol"
            });
        }

        return View(new UsuariosIndexViewModel
        {
            Busqueda = busqueda,
            Usuarios = lista
        });
    }

    public async Task<IActionResult> Create()
    {
        var viewModel = new UsuarioCreateViewModel();

        await CargarRolesAsync(viewModel);

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UsuarioCreateViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            await CargarRolesAsync(viewModel);
            return View(viewModel);
        }

        var emailExistente = await _userManager.FindByEmailAsync(viewModel.Email);

        if (emailExistente is not null)
        {
            ModelState.AddModelError(nameof(viewModel.Email), "Ya existe un usuario con este correo.");

            await CargarRolesAsync(viewModel);

            return View(viewModel);
        }

        var usuario = new ApplicationUser
        {
            UserName = viewModel.Email,
            Email = viewModel.Email,
            EmailConfirmed = true,
            Nombre = viewModel.Nombre.Trim(),
            Apellido = viewModel.Apellido.Trim(),
            Estado = true,
            FechaCreacion = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(usuario, viewModel.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            await CargarRolesAsync(viewModel);

            return View(viewModel);
        }

        await _userManager.AddToRoleAsync(usuario, viewModel.Rol);

        await _auditoriaService.RegistrarAsync(new RegistrarAuditoriaDto
        {
            UsuarioId = _userManager.GetUserId(User),
            NombreUsuario = User.Identity?.Name,
            Accion = TipoAccionAuditoria.Crear,
            EntidadAfectada = "Usuario",
            RegistroId = usuario.Id,
            Detalle = $"Se creó el usuario '{usuario.Email}' con rol '{viewModel.Rol}'."
        });

        TempData["Success"] = "Usuario creado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string id)
    {
        var usuario = await _userManager.FindByIdAsync(id);

        if (usuario is null)
        {
            return NotFound();
        }

        var rolesUsuario = await _userManager.GetRolesAsync(usuario);

        var viewModel = new UsuarioEditViewModel
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Email = usuario.Email ?? string.Empty,
            Estado = usuario.Estado,
            Rol = rolesUsuario.FirstOrDefault() ?? string.Empty
        };

        await CargarRolesAsync(viewModel);

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, UsuarioEditViewModel viewModel)
    {
        if (id != viewModel.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await CargarRolesAsync(viewModel);
            return View(viewModel);
        }

        var usuario = await _userManager.FindByIdAsync(id);

        if (usuario is null)
        {
            return NotFound();
        }

        var otroUsuarioConEmail = await _userManager.FindByEmailAsync(viewModel.Email);

        if (otroUsuarioConEmail is not null && otroUsuarioConEmail.Id != usuario.Id)
        {
            ModelState.AddModelError(nameof(viewModel.Email), "Ya existe otro usuario con este correo.");

            await CargarRolesAsync(viewModel);

            return View(viewModel);
        }

        usuario.Nombre = viewModel.Nombre.Trim();
        usuario.Apellido = viewModel.Apellido.Trim();
        usuario.Email = viewModel.Email.Trim();
        usuario.UserName = viewModel.Email.Trim();
        usuario.Estado = viewModel.Estado;

        var result = await _userManager.UpdateAsync(usuario);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            await CargarRolesAsync(viewModel);

            return View(viewModel);
        }

        var rolesActuales = await _userManager.GetRolesAsync(usuario);

        if (rolesActuales.Any())
        {
            await _userManager.RemoveFromRolesAsync(usuario, rolesActuales);
        }

        await _userManager.AddToRoleAsync(usuario, viewModel.Rol);

        await _auditoriaService.RegistrarAsync(new RegistrarAuditoriaDto
        {
            UsuarioId = _userManager.GetUserId(User),
            NombreUsuario = User.Identity?.Name,
            Accion = TipoAccionAuditoria.Editar,
            EntidadAfectada = "Usuario",
            RegistroId = usuario.Id,
            Detalle = $"Se editó el usuario '{usuario.Email}'. Rol asignado: '{viewModel.Rol}'. Estado activo: {usuario.Estado}."
        });

        TempData["Success"] = "Usuario actualizado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(string id)
    {
        var usuario = await _userManager.FindByIdAsync(id);

        if (usuario is null)
        {
            return NotFound();
        }

        usuario.Estado = !usuario.Estado;

        await _userManager.UpdateAsync(usuario);

        await _auditoriaService.RegistrarAsync(new RegistrarAuditoriaDto
        {
            UsuarioId = _userManager.GetUserId(User),
            NombreUsuario = User.Identity?.Name,
            Accion = TipoAccionAuditoria.Editar,
            EntidadAfectada = "Usuario",
            RegistroId = usuario.Id,
            Detalle = $"Se cambió el estado del usuario '{usuario.Email}' a {(usuario.Estado ? "Activo" : "Inactivo")}."
        });

        TempData["Success"] = "Estado del usuario actualizado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    private async Task CargarRolesAsync(UsuarioCreateViewModel viewModel)
    {
        var roles = _roleManager.Roles
            .OrderBy(r => r.Name)
            .ToList();

        viewModel.Roles = roles.Select(r => new SelectListItem
        {
            Value = r.Name,
            Text = r.Name
        });

        await Task.CompletedTask;
    }

    private async Task CargarRolesAsync(UsuarioEditViewModel viewModel)
    {
        var roles = _roleManager.Roles
            .OrderBy(r => r.Name)
            .ToList();

        viewModel.Roles = roles.Select(r => new SelectListItem
        {
            Value = r.Name,
            Text = r.Name
        });

        await Task.CompletedTask;
    }
}
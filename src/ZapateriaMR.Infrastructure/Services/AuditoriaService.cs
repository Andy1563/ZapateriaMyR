using Microsoft.EntityFrameworkCore;
using ZapateriaMR.Application.DTOs.Auditoria;
using ZapateriaMR.Application.Interfaces;
using ZapateriaMR.Domain.Entities;
using ZapateriaMR.Infrastructure.Data;

namespace ZapateriaMR.Infrastructure.Services;

public class AuditoriaService : IAuditoriaService
{
    private readonly ApplicationDbContext _context;

    public AuditoriaService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task RegistrarAsync(RegistrarAuditoriaDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.EntidadAfectada))
        {
            return;
        }

        var auditoria = new Auditoria
        {
            UsuarioId = dto.UsuarioId,
            NombreUsuario = dto.NombreUsuario,
            Accion = dto.Accion,
            EntidadAfectada = dto.EntidadAfectada.Trim(),
            RegistroId = dto.RegistroId,
            Detalle = dto.Detalle,
            DireccionIp = dto.DireccionIp,
            Fecha = DateTime.UtcNow
        };

        _context.Auditorias.Add(auditoria);

        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<AuditoriaListadoDto>> ObtenerAuditoriasAsync(string? busqueda = null)
    {
        var query = _context.Auditorias
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var termino = busqueda.Trim();

            query = query.Where(a =>
                a.EntidadAfectada.Contains(termino) ||
                (a.NombreUsuario != null && a.NombreUsuario.Contains(termino)) ||
                (a.RegistroId != null && a.RegistroId.Contains(termino)) ||
                (a.Detalle != null && a.Detalle.Contains(termino)));
        }

        return await query
            .OrderByDescending(a => a.Fecha)
            .Select(a => new AuditoriaListadoDto
            {
                Id = a.Id,
                UsuarioId = a.UsuarioId,
                NombreUsuario = a.NombreUsuario,
                Accion = a.Accion,
                EntidadAfectada = a.EntidadAfectada,
                RegistroId = a.RegistroId,
                Detalle = a.Detalle,
                DireccionIp = a.DireccionIp,
                Fecha = a.Fecha
            })
            .ToListAsync();
    }
}
using ConsultaACH.Blazor.Modelos;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ConsultaACH.Blazor.Datos;

public class ContextoBD : IdentityDbContext<Usuario>
{
    public ContextoBD(
        Microsoft.EntityFrameworkCore.DbContextOptions<ContextoBD> options)
        : base(options)
    {
    }

    public DbSet<TransaccionACH> TransaccionesACH { get; set; }
}
using Microsoft.AspNetCore.Identity;

namespace ConsultaACH.Blazor.Modelos;

public class Usuario : IdentityUser
{
    public string? NombreCompleto { get; set; }
}
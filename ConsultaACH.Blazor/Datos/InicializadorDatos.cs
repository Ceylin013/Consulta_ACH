using ConsultaACH.Blazor.Modelos;
using Microsoft.AspNetCore.Identity;

namespace ConsultaACH.Blazor.Datos;

public static class InicializadorDatos
{
    public static async Task InicializarAsync(
        IServiceProvider servicios,
        IConfiguration configuracion)
    {
        var administradorRoles =
            servicios.GetRequiredService<RoleManager<IdentityRole>>();

        var administradorUsuarios =
            servicios.GetRequiredService<UserManager<Usuario>>();

        // Crear roles iniciales
        string[] roles = { "Administrador", "Contador" };

        foreach (var rol in roles)
        {
            if (!await administradorRoles.RoleExistsAsync(rol))
            {
                var resultado = await administradorRoles.CreateAsync(
                    new IdentityRole(rol));

                if (!resultado.Succeeded)
                    throw new Exception("No se pudo crear el rol: " + rol);
            }
        }

        // Obtener credenciales del administrador
        var nombreUsuario = configuracion["Administrador:Usuario"];
        var correo = configuracion["Administrador:Correo"];
        var contrasena = configuracion["Administrador:Contrasena"];

        if (string.IsNullOrWhiteSpace(nombreUsuario) ||
            string.IsNullOrWhiteSpace(correo) ||
            string.IsNullOrWhiteSpace(contrasena))
        {
            throw new Exception(
                "Faltan las credenciales iniciales del administrador.");
        }

        // Comprobacion si el administrador ya existe
        var usuario = await administradorUsuarios
            .FindByNameAsync(nombreUsuario);

        if (usuario == null)
        {
            usuario = new Usuario
            {
                UserName = nombreUsuario,
                Email = correo,
                NombreCompleto = "Administrador del sistema",
                EmailConfirmed = true
            };

            var resultado = await administradorUsuarios
                .CreateAsync(usuario, contrasena);

            if (!resultado.Succeeded)
            {
                var errores = string.Join(", ",
                    resultado.Errors.Select(e => e.Description));

                throw new Exception(
                    "No se pudo crear el administrador: " + errores);
            }

            var resultadoRol = await administradorUsuarios.AddToRoleAsync(usuario, "Administrador");

            if (!resultadoRol.Succeeded)
            {
                var errores = string.Join(", ",
                    resultadoRol.Errors.Select(e => e.Description));

                throw new Exception(
                    "No se pudo asignar el rol de administrador: " + errores);
            }
        }
    }
}
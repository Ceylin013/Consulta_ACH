using ConsultaACH.Blazor.Modelos;
using Microsoft.AspNetCore.Identity;

namespace ConsultaACH.Blazor.Servicios;

public class ServicioUsuarios
{
    private readonly UserManager<Usuario> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ServicioUsuarios(
        UserManager<Usuario> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // Crear un usuario y asignarle un rol
    public async Task<IdentityResult> CrearUsuarioAsync(
        Usuario usuario,
        string contrasena,
        string nombreRol)
    {
        // Verificar que el rol exista
        if (!await _roleManager.RoleExistsAsync(nombreRol))
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Description = "El rol seleccionado no existe."
                });
        }

        // Crear el usuario
        var resultado = await _userManager.CreateAsync(
            usuario, contrasena);

        if (!resultado.Succeeded)
        {
            return resultado;
        }

        // Asignar el rol al usuario
        var resultadoRol = await _userManager.AddToRoleAsync(
            usuario, nombreRol);

        // Si falla la asignación, eliminar el usuario creado
        if (!resultadoRol.Succeeded)
        {
            await _userManager.DeleteAsync(usuario);
            return resultadoRol;
        }

        return IdentityResult.Success;
    }

    // Crear un rol
    public async Task<IdentityResult> CrearRolAsync(string nombreRol)
    {
        if (await _roleManager.RoleExistsAsync(nombreRol))
        {
            return IdentityResult.Success;
        }

        return await _roleManager.CreateAsync(
            new IdentityRole(nombreRol));
    }

    // Asignar un rol a un usuario existente
    public async Task<IdentityResult> AsignarRolAsync(
        Usuario usuario,
        string nombreRol)
    {
        if (!await _roleManager.RoleExistsAsync(nombreRol))
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Description = "El rol seleccionado no existe."
                });
        }

        return await _userManager.AddToRoleAsync(
            usuario, nombreRol);
    }

    // Buscar usuario por nombre
    public async Task<Usuario?> BuscarUsuarioAsync(
        string nombreUsuario)
    {
        return await _userManager.FindByNameAsync(nombreUsuario);
    }

    // Obtener todos los usuarios registrados
    public async Task<IList<Usuario>> ObtenerUsuariosAsync()
    {
        return await Task.FromResult(_userManager.Users.ToList());
    }
}
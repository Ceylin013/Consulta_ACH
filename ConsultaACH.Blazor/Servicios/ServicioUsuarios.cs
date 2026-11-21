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

    //creacion y asignacion del rol a usuario
    public async Task<IdentityResult> CrearUsuarioAsync(
        Usuario usuario,
        string contrasena,
        string nombreRol)
    {
        //verificacion del rol 
        if (!await _roleManager.RoleExistsAsync(nombreRol))
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Description = "El rol seleccionado no existe."
                });
        }

        //crear el usuario
        var resultado = await _userManager.CreateAsync(
            usuario, contrasena);

        if (!resultado.Succeeded)
        {
            return resultado;
        }

        //asignar el rol al usuario
        var resultadoRol = await _userManager.AddToRoleAsync(
            usuario, nombreRol);

        //si falla la asignación, eliminar el usuario creado
        if (!resultadoRol.Succeeded)
        {
            await _userManager.DeleteAsync(usuario);
            return resultadoRol;
        }

        return IdentityResult.Success;
    }

    //crear un rol
    public async Task<IdentityResult> CrearRolAsync(string nombreRol)
    {
        if (await _roleManager.RoleExistsAsync(nombreRol))
        {
            return IdentityResult.Success;
        }

        return await _roleManager.CreateAsync(
            new IdentityRole(nombreRol));
    }

    //asignar un rol a un usuario existente
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

    //buscar usuario por nombre
    public async Task<Usuario?> BuscarUsuarioAsync(
        string nombreUsuario)
    {
        return await _userManager.FindByNameAsync(nombreUsuario);
    }

    //obtener todos los usuarios registrados
    public async Task<IList<Usuario>> ObtenerUsuariosAsync()
    {
        //return await Task.FromResult(_userManager.Users.ToList());
        return _userManager.Users.ToList();
    }

    //obtener el rol de un usuario
    public async Task<string> ObtenerRolAsync(Usuario usuario)
    {
        var roles = await _userManager.GetRolesAsync(usuario);

        return roles.FirstOrDefault() ?? "Sin rol";
    }

    //activar o desactivar una cuenta
    public async Task<IdentityResult> CambiarEstadoAsync(
        Usuario usuario, bool activo)
    {
        usuario.Activo = activo;

        var resultado = await _userManager.UpdateAsync(usuario);

        if (resultado.Succeeded && !activo)
        {
            await _userManager.UpdateSecurityStampAsync(usuario);
        }

        return resultado;
    }

    //editar información del usuario
    public async Task<IdentityResult> ActualizarUsuarioAsync(
        Usuario usuario,
        string nombreCompleto,
        string nombreUsuario,
        string correo)
    {
        usuario.NombreCompleto = nombreCompleto;
        usuario.UserName = nombreUsuario;
        usuario.Email = correo;

        return await _userManager.UpdateAsync(usuario);
    }

    //cambiar el rol de un usuario
    public async Task<IdentityResult> CambiarRolAsync(
        Usuario usuario, string nuevoRol)
    {
        if (!await _roleManager.RoleExistsAsync(nuevoRol))
        {
            return IdentityResult.Failed(new IdentityError
            {
                Description = "El rol seleccionado no existe."
            });
        }

        var rolesActuales = await _userManager.GetRolesAsync(usuario);

        var quitarRoles = await _userManager.RemoveFromRolesAsync(
            usuario, rolesActuales);

        if (!quitarRoles.Succeeded)
            return quitarRoles;

        var agregarRol = await _userManager.AddToRoleAsync(
            usuario, nuevoRol);

        if (!agregarRol.Succeeded)
        {
            await _userManager.AddToRolesAsync(usuario, rolesActuales);
            return agregarRol;
        }

        return IdentityResult.Success;
    }

    //restablecer contraseña
    public async Task<IdentityResult> RestablecerContrasenaAsync(
        Usuario usuario, string nuevaContrasena)
    {
        var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);

        return await _userManager.ResetPasswordAsync(
            usuario, token, nuevaContrasena);
    }

}
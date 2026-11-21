using ConsultaACH.Blazor.Modelos;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;

namespace ConsultaACH.Blazor.Components.Seguridad;

public class ValidarSesion : RevalidatingServerAuthenticationStateProvider
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ValidarSesion(
        ILoggerFactory loggerFactory,
        IServiceScopeFactory scopeFactory)
        : base(loggerFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override TimeSpan RevalidationInterval =>
        TimeSpan.FromMinutes(1);

    protected override async Task<bool> ValidateAuthenticationStateAsync(
        AuthenticationState authenticationState,
        CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var administradorUsuarios =
            scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();

        var usuario = await administradorUsuarios.GetUserAsync(
            authenticationState.User);

        return usuario != null && usuario.Activo;
    }
}
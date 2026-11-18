using ConsultaACH.Components;
using ConsultaACH.Blazor.Datos;
using ConsultaACH.Blazor.Modelos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Authorization;
using ConsultaACH.Blazor.Servicios;

var builder = WebApplication.CreateBuilder(args);
//conexion db
var cadenaConexion = builder.Configuration
    .GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ContextoBD>(options =>
    options.UseMySQL(cadenaConexion!));
//config identity
builder.Services.AddIdentity<Usuario, IdentityRole>(options =>
    {
        // Configuración de contraseñas
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;

        // Bloqueo por intentos fallidos
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(10);

        // Configuración del usuario email
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ContextoBD>()
    .AddDefaultTokenProviders();
//estado de autenticacion
builder.Services.AddAuthorization();
builder.Services.AddScoped<ServicioUsuarios>();

builder.Services.AddCascadingAuthenticationState();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//construccion de applicacion
var app = builder.Build();

// Inicializar roles y administrador
using (var scope = app.Services.CreateScope())
{
    await InicializadorDatos.InicializarAsync(
        scope.ServiceProvider,
        app.Configuration);
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
//nuevos
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

//nuevo inicio sesion
app.MapPost("/login", async (
    HttpContext contexto,
    SignInManager<Usuario> administradorInicioSesion,
    Microsoft.AspNetCore.Antiforgery.IAntiforgery antiforgery) =>
{
    await antiforgery.ValidateRequestAsync(contexto);

    var formulario = await contexto.Request.ReadFormAsync();

    var usuario = formulario["usuario"].ToString();
    var contrasena = formulario["contrasena"].ToString();

    Console.WriteLine("------ PRUEBA DE LOGIN ------");
    Console.WriteLine($"Usuario recibido: {usuario}");
    Console.WriteLine($"Formulario recibido correctamente");


    var resultado = await administradorInicioSesion.PasswordSignInAsync(
        usuario,
        contrasena,
        false,
        true
    );

    if (resultado.Succeeded)
    {
        return Results.Redirect("/home");
    }

    if (resultado.IsLockedOut)
    {
        return Results.Redirect("/?error=bloqueado");
    }

    return Results.Redirect("/?error=invalido");
});

//cierre sesion
app.MapPost("/logout", async (
    HttpContext contexto,
    SignInManager<Usuario> administradorInicioSesion,
    Microsoft.AspNetCore.Antiforgery.IAntiforgery antiforgery) =>
{
    Console.WriteLine("------ PRUEBA DE CIERRE DE SESIÓN ------");
    Console.WriteLine($"Tipo de contenido: {contexto.Request.ContentType}");

    if (contexto.Request.HasFormContentType)
    {
        var formulario = await contexto.Request.ReadFormAsync();
        Console.WriteLine("Campos recibidos: " + string.Join(", ", formulario.Keys));
    }
    else
    {
        Console.WriteLine("No se recibió un formulario.");
    }

    try
    {
        await antiforgery.ValidateRequestAsync(contexto);
    }
    catch (Microsoft.AspNetCore.Antiforgery.AntiforgeryValidationException ex)
    {
        Console.WriteLine($"Error de validación: {ex.Message}");
        return Results.BadRequest("Token antifalsificación inválido o ausente.");
    }

    await administradorInicioSesion.SignOutAsync();
    return Results.Redirect("/");
});

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

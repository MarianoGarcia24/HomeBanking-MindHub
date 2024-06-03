using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Implementation;
using HomeBankingMindHub.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

//Contexto de la DB
builder.Services.AddDbContext<HomeBankingContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MyDBConnection")));

//Scoped's para los repositorios (cada uno genera una conexión y contexto separado)
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

//Autenticacion
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
            options.LoginPath = new PathString("/index.html");
        });

//Autorizacion
//Se utiliza el claim client que va a tener el mail del cliente.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ClientOnly", policy => policy.RequireClaim("Client"));
});

var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
    try
    {
        var service = scope.ServiceProvider;
        var context = service.GetRequiredService<HomeBankingContext>();
        DBInitializer.Initialize(context);
    }
    catch(Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ha ocurrido un error");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapRazorPages();

app.UseSwagger();

app.Run();

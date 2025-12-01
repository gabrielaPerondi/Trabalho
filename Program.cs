using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using TrabalhoElvis2.Context; // <-- certifique-se de usar seu namespace correto

var builder = WebApplication.CreateBuilder(args);

//  Adiciona suporte a MVC
builder.Services.AddControllersWithViews();

//  Adiciona o contexto do banco de dados (ajuste o nome da ConnectionString no appsettings.json)
builder.Services.AddDbContext<LoginContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoPadrao"))
);

//  Adiciona IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

//  Adiciona sessão
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//  Adiciona autenticação e autorização (caso use Identity futuramente)
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

//  Middlewares
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}
app.UseStaticFiles();

app.UseRouting();

//  Ordem certa: sessão vem ANTES da autorização
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Adiciona instrução de rota para garantir acesso por /Condomino
app.MapControllerRoute(
    name: "condomino",
    pattern: "Condomino/{action=Index}/{id?}",
    defaults: new { controller = "Condomino" }
);

//  Define a rota padrão
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Usuario}/{action=Login}/{id?}"
);

//  Executa o app
app.Run();

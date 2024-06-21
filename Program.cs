using System.Configuration;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using WebRegistry.Models;
using WebRegistry.Components;
using WebRegistry.Services.AuthorizationProvider;
using WebRegistry.Services.DwgConverter;
using WebRegistry.Services.EmailSender;
using WebRegistry.Services.ShifrRezerver;
using Serilog;
using WebRegistry.BackgroundServises;
using WebRegistry.Services.UserActionLog;
using WebRegistry.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMemoryCache();

builder.Services.AddSerilog((services, lc) => lc
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console().WriteTo.File(@"C:\web\WebregistryLog\log.txt", 
        rollingInterval: RollingInterval.Day,
        rollOnFileSizeLimit: true,
        fileSizeLimitBytes: 10 * 1024 * 1024,
        retainedFileCountLimit: 31,
        shared: true,
        flushToDiskInterval: TimeSpan.FromSeconds(60)));

string? connection = builder.Configuration.GetConnectionString("DevelopConnection");


builder.Services.AddDbContext<NomenclatureContext>(options => options.UseMySql(connection, ServerVersion.Parse("8.0.36-mysql"))
    .ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.NavigationBaseIncludeIgnored))
    ) ;
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
   .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
    
});

builder.Services.Configure<IISServerOptions>(options =>
{
    options.AutomaticAuthentication = true;
});

builder.Services.AddSimpleRoleAuthorization<SialSimpleRoleProvider>(); //подключение политики на основе собственных ролей, не AD
builder.Services.AddSingleton<IDwgConverterService, AsposeConverter>(); //генерция картинок чертежа
builder.Services.AddScoped<IShifrService, ShifrWorker>(); //генерация шифра изделий, резервирование шифров
builder.Services.AddControllersWithViews();
builder.Services.AddRazorComponents().AddInteractiveServerComponents(); //чтобы вставлять компоненты Blazor
builder.Services.AddSingleton<IEmailService, EmailSender>(); //отправка сообщений по почте
builder.Services.AddHostedService<IzdelieFilepathExistService>(); //служба проверки пустых ссылок на файлы чертежей
builder.Services.AddHostedService<UserFioUpdateService>(); //служба обновления ФИО юзеров из AD
builder.Services.AddScoped<IUserActionLogger, UserActionToDB>(); // запись в базу важных действий пользователей
builder.Services.AddHttpClient<_1CService>(); //для получение информации с 1С
builder.Services.AddHostedService<_1CSynchronizeService>(); //синхронизация снятых с производства изделий из 1С
builder.Services.AddSwaggerGen();


Log.CloseAndFlush();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    //app.UseHsts();
    //app.UseHttpsRedirection();
}
app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();
app.UseSerilogRequestLogging();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorComponents<RedBookGrid>().AddInteractiveServerRenderMode();
app.Run();

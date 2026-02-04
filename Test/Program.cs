using Microsoft.Extensions.Options;
using Serilog;
using System.Net.Http;
using System.Net.Http.Headers;
using Test.Models;
using TestData;
using TestServices.Utilities;

var builder = WebApplication.CreateBuilder(args);
var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
builder.Services.AddControllersWithViews();
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

builder.Services.AddHttpClient("MyApiClient", (serviceProvider, client) =>
{
    var apiSettings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
    client.BaseAddress = new Uri(apiSettings.BaseUrl);
    client.Timeout = TimeSpan.FromDays(24);  
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
builder.Services.AddHttpClient("AuthorizedClient")
    .ConfigureHttpClient((sp, client) =>
    {
        var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
        var token = httpContextAccessor.HttpContext.Request.Cookies["accessToken"];
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    });
builder.Services.AddSingleton<DapperContext>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Login}/{id?}")
    .WithStaticAssets();


app.Run();

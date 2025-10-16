using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using Testdata.Viewmodel;
using TestData.Models.Entities;
using TestServices.JwtToken;
using TestServices.Utilities;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
var key = builder.Configuration["JwtSettings:Key"];
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddScoped<IJwtAuth>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    return new JwtAuth(configuration);
});
builder.Services.AddCors();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                var response = new ResponseModel<object>
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Status = false,
                    Message = "Unauthorized access."
                };
                var json = JsonSerializer.Serialize(response);

                return context.Response.WriteAsync(json);
            }
        };
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key))
        };
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Test",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.OperationFilter<AuthorizeCheckOperationFilter>();
});
builder.Services.AddDbContext<Db23320Context>(o => o.UseSqlServer("DefaultConnection"));
builder.Services.AddScoped<ICustomerInterface, Customer>();
builder.Services.AddScoped<IJwtAuth, JwtAuth>();
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Test Api v1");
    c.RoutePrefix = string.Empty;
});

app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();




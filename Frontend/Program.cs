using Frontend.Components;
using MudBlazor.Services;
using Frontend.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Blazored.LocalStorage;
using Frontend.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient<ClientApiService>(client =>
{
    var baseAddress = builder.Configuration["ApiUrls:ClientApi"]
        ?? throw new InvalidOperationException("ClientApi base address not configured.");
    client.BaseAddress = new Uri(baseAddress);
});
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<IAuthClientService, AuthClientService>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddMudServices();
builder.Services.AddSingleton<CartService>();

// builder.Services.AddRazorPages();
// builder.Services.AddServerSideBlazor();
// builder.Services.AddScoped(sp => new HttpClient 
// { 
//     BaseAddress = new Uri("http://localhost:5058/")
// });

builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

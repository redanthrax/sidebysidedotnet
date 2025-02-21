
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpForwarder();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSystemWebAdapters()
    .AddRemoteAppClient(options => {
        options.RemoteAppUrl = new Uri(builder.Configuration["ProxyTo"]!);
        options.ApiKey = builder.Configuration["RemoteAppApiKey"]!;
    })
    .AddAuthenticationClient(true);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSystemWebAdapters();

app.MapDefaultControllerRoute();
app.MapForwarder("/{**catch-all}", app.Configuration["ProxyTo"]!).Add(static builder => ((RouteEndpointBuilder)builder).Order = int.MaxValue);

app.Run();

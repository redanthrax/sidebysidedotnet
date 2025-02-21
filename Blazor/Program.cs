using Blazor.Components;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add YARP
builder.Services.AddHttpForwarder();

// Add System Web Adapters and setup session
builder.Services.AddSystemWebAdapters()
    .AddRemoteAppClient(options => {
        options.RemoteAppUrl = new(builder.Configuration["ProxyTo"]!);
        options.ApiKey = builder.Configuration["RemoteAppApiKey"]!;
    })
    .AddAuthenticationClient(true);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAuthorization(options => {
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.UseSystemWebAdapters();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapForwarder("/{**catch-all}", app.Configuration["ProxyTo"]!)
   .Add(static builder => ((RouteEndpointBuilder)builder).Order = int.MaxValue);

app.Run();
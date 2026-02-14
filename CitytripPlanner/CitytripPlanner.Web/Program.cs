using CitytripPlanner.Features.Citytrips.Domain;
using CitytripPlanner.Features.UserProfiles.Domain;
using CitytripPlanner.Infrastructure.Citytrips;
using CitytripPlanner.Infrastructure.UserProfiles;
using CitytripPlanner.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<ICitytripRepository>();
    cfg.RegisterServicesFromAssemblyContaining<IUserProfileRepository>();
});
builder.Services.AddSingleton<ICitytripRepository, InMemoryCitytripRepository>();
builder.Services.AddSingleton<IUserProfileRepository, InMemoryUserProfileRepository>();
builder.Services.AddSingleton<ICurrentUserService, InMemoryCurrentUserService>();
builder.Services.AddScoped<IUserInteractionStore, InMemoryUserInteractionStore>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

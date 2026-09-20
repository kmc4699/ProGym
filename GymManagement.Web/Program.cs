using GymManagement;
using GymManagement.Web.Components;
using GymManagement.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Shared app services - singletons so every page sees the same data
// and calls the same booking / reporting logic.
builder.Services.AddSingleton<PersistenceService>(sp =>
{
    var path = Path.Combine(AppContext.BaseDirectory, "progym-data.json");
    var logger = sp.GetService<ILogger<PersistenceService>>();
    return new PersistenceService(path, logger);
});
builder.Services.AddSingleton<GymDataStore>(sp =>
{
    var store = new GymDataStore();
    sp.GetRequiredService<PersistenceService>().LoadInto(store);
    return store;
});
builder.Services.AddSingleton<BookingService>();
builder.Services.AddSingleton<ReportingService>();

var app = builder.Build();

// Save state to disk when the app shuts down gracefully.
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStopping.Register(() =>
{
    var persistence = app.Services.GetRequiredService<PersistenceService>();
    var store = app.Services.GetRequiredService<GymDataStore>();
    try { persistence.Save(store); }
    catch (Exception ex)
    {
        app.Services.GetService<ILogger<Program>>()?
            .LogError(ex, "Failed to save GymDataStore on shutdown");
    }
});

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

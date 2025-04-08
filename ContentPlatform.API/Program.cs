using Supabase;
using ContentPlatform.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Read config
builder.Services.Configure<SupabaseOptions>(
    builder.Configuration.GetSection("Supabase"));

// Add Supabase client
builder.Services.AddSingleton<Client>(provider =>
{
    var config = provider.GetRequiredService<IOptions<SupabaseOptions>>().Value;
    var supabase = new Client(config.Url, config.ApiKey);
    supabase.InitializeAsync().Wait(); // Consider moving this to background init
    return supabase;
});

// Add custom services
builder.Services.AddScoped<ISupabaseRepository, SupabaseRepository>();
builder.Services.AddScoped<IProjectService, ProjectService>();

// Swagger + Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

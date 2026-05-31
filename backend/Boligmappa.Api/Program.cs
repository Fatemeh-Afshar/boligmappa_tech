using System.Text.Json.Serialization;
using Boligmappa.Api.Services;
using Boligmappa.Api.Stores;

var builder = WebApplication.CreateBuilder(args);

const string FrontendCorsPolicy = "frontend";

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Serialize enums as their string names (e.g. "Warranty") rather than ints,
        // which keeps the API self-documenting and the frontend code readable.
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Singleton: the in-memory store must retain data across requests.
builder.Services.AddSingleton<IDocumentStore, InMemoryDocumentStore>();
builder.Services.AddScoped<IDocumentService, DocumentService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
        policy.WithOrigins("http://localhost:5173") // Vite dev server
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// Swagger is enabled in all environments so the API is browsable from the
// packaged Windows build too (at /swagger), not just during development.
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(FrontendCorsPolicy);

// Serve the built frontend (copied into wwwroot at publish time) from the same
// origin as the API. UseDefaultFiles maps "/" to index.html; the fallback sends
// any non-API, non-file route back to index.html so the SPA handles routing.
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();

using DotNetEnv;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi;
using photo_album.Api.Extensions;
using photo_album.Application.Extensions;
using photo_album.Application.Validators.Photo;
using photo_album.Infrastructure.Extensions;

Env.NoClobber().TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5066",
                "https://localhost:7013")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste JWT token"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = UploadPhotoRequestDtoValidator.MaxFileSizeBytes;
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = UploadPhotoRequestDtoValidator.MaxFileSizeBytes;
});

builder.Services
    .AddMediatrConfiguration()
    .AddJwtConfiguration()
    .AddValidationConfiguration()
    .AddAutoMapperConfiguration()
    .AddPhotoAlbumContextConfiguration();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var storageRoot = Path.Combine(Directory.GetCurrentDirectory(), "storage");
Directory.CreateDirectory(storageRoot);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(storageRoot),
    RequestPath = "/storage"
});

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.ApplyMigrationsAsync();

await app.RunAsync();

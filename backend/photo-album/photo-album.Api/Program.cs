using DotNetEnv;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.OpenApi;
using photo_album.Api.Extensions;
using photo_album.Application.Extensions;
using photo_album.Application.Validators.Photo;
using photo_album.Infrastructure.Extensions;

Env.NoClobber().TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
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

    // Применяем требование безопасности ко всем эндпоинтам
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.ApplyMigrationsAsync();

await app.RunAsync();

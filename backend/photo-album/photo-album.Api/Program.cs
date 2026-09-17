using DotNetEnv;
using photo_album.Api.Extensions;
using photo_album.Application.Extensions;
using photo_album.Infrastructure.Extensions;

Env.NoClobber().TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services
    .AddMediatrConfiguration()
    .AddJwtConfiguration()
    .AddValidationConfiguration()
    .AddAutoMapperConfiguration()
    .AddPhotoAlbumContextConfiguration(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

await app.ApplyMigrationsAsync();

await app.RunAsync();

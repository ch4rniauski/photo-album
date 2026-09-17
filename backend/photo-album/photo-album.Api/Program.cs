using photo_album.Application.Extensions;
using photo_album.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services
    .AddMediatrConfiguration()
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

await app.RunAsync();

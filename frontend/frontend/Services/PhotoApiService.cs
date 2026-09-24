using System.Net.Http.Json;
using frontend.Models;

namespace frontend.Services;

public sealed class PhotoApiService
{
    private readonly HttpClient _httpClient;

    public PhotoApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<PhotoDto>> GetPhotosAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var photos = await _httpClient.GetFromJsonAsync<List<PhotoDto>>(
            $"api/Photos?page={page}&pageSize={pageSize}",
            cancellationToken);

        return photos ?? [];
    }

    public async Task<IReadOnlyList<PhotoDto>> SearchPhotosAsync(
        string search,
        CancellationToken cancellationToken = default)
    {
        var encoded = Uri.EscapeDataString(search);

        var photos = await _httpClient.GetFromJsonAsync<List<PhotoDto>>(
            $"api/Photos/search?search={encoded}",
            cancellationToken);

        return photos ?? [];
    }

    public string ResolveThumbnailUrl(string thumbnailUrl)
    {
        if (string.IsNullOrWhiteSpace(thumbnailUrl))
        {
            return string.Empty;
        }

        if (Uri.TryCreate(thumbnailUrl, UriKind.Absolute, out _))
        {
            return thumbnailUrl;
        }

        var baseAddress = _httpClient.BaseAddress?.ToString().TrimEnd('/') ?? string.Empty;
        
        return $"{baseAddress}/{thumbnailUrl.TrimStart('/')}";
    }
}

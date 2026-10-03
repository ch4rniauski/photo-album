using System.Net.Http.Headers;
using System.Net.Http.Json;
using frontend.Models;

namespace frontend.Services;

public sealed class PhotoApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public PhotoApiService(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
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

    public async Task<OriginalPhotoContent?> GetOriginalPhotoAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(
            $"api/Photos/{id}/original",
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/jpeg";
        var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);

        return new OriginalPhotoContent(bytes, contentType);
    }

    public async Task<(bool Success, bool? IsLiked)> GetMyReactionAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        using var response = await _authService.SendAuthorizedAsync(
            () => new HttpRequestMessage(
                HttpMethod.Get,
                $"api/Photos/{id}/reaction"),
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return (false, null);
        }

        var payload = await response.Content.ReadFromJsonAsync<PhotoReactionResponseDto>(cancellationToken);

        if (payload is null)
        {
            return (false, null);
        }

        return (true, payload.IsLiked);
    }

    public Task<(PhotoMutationResponseDto? Photo, string? ErrorMessage)> LikePhotoAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return SendAuthorizedMutationAsync(
            HttpMethod.Put,
            $"api/Photos/{id}/likes",
            content: null,
            cancellationToken);
    }

    public Task<(PhotoMutationResponseDto? Photo, string? ErrorMessage)> DislikePhotoAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return SendAuthorizedMutationAsync(
            HttpMethod.Put,
            $"api/Photos/{id}/dislikes",
            content: null,
            cancellationToken);
    }

    public Task<(PhotoMutationResponseDto? Photo, string? ErrorMessage)> RenamePhotoAsync(
        Guid id,
        string name,
        CancellationToken cancellationToken = default)
    {
        return SendAuthorizedMutationAsync(
            HttpMethod.Put,
            $"api/Photos/{id}",
            new RenamePhotoRequestDto(name),
            cancellationToken);
    }

    public async Task<(PhotoMutationResponseDto? Photo, string? ErrorMessage)> UploadPhotoAsync(
        Stream content,
        string fileName,
        string contentType,
        string name,
        CancellationToken cancellationToken = default)
    {
        await using var buffer = new MemoryStream();
        await content.CopyToAsync(buffer, cancellationToken);
        var bytes = buffer.ToArray();

        using var response = await _authService.SendAuthorizedAsync(
            () =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "api/Photos");
                var form = new MultipartFormDataContent();
                var fileContent = new ByteArrayContent(bytes);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(
                    string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType);

                form.Add(fileContent, "photo", fileName);
                form.Add(new StringContent(name), "name");
                request.Content = form;

                return request;
            },
            cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var photo = await response.Content.ReadFromJsonAsync<PhotoMutationResponseDto>(cancellationToken);

            return (photo, null);
        }

        var errorMessage = await TryReadErrorDetailAsync(response, cancellationToken);

        return (null, errorMessage ?? "Upload failed.");
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

    private async Task<(PhotoMutationResponseDto? Photo, string? ErrorMessage)> SendAuthorizedMutationAsync(
        HttpMethod method,
        string url,
        object? content,
        CancellationToken cancellationToken)
    {
        using var response = await _authService.SendAuthorizedAsync(
            () =>
            {
                var request = new HttpRequestMessage(method, url);

                if (content is not null)
                {
                    request.Content = JsonContent.Create(content);
                }

                return request;
            },
            cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var photo = await response.Content.ReadFromJsonAsync<PhotoMutationResponseDto>(cancellationToken);

            return (photo, null);
        }

        var errorMessage = await TryReadErrorDetailAsync(response, cancellationToken);

        return (null, errorMessage ?? "Request failed.");
    }

    private static async Task<string?> TryReadErrorDetailAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        try
        {
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetailsDto>(cancellationToken);

            if (!string.IsNullOrWhiteSpace(problem?.Detail))
            {
                return problem.Detail;
            }
        }
        catch
        {
            // Ignore parse failures and fall back to a generic message.
        }

        return null;
    }
}

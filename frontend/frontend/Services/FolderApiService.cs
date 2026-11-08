using System.Net.Http.Json;
using frontend.Models;

namespace frontend.Services;

public sealed class FolderApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public FolderApiService(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    public async Task<IReadOnlyList<FolderDto>> GetFoldersAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var folders = await _httpClient.GetFromJsonAsync<List<FolderDto>>(
            $"api/Folders?page={page}&pageSize={pageSize}",
            cancellationToken);

        return folders ?? [];
    }

    public async Task<FolderDto?> GetFolderByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<FolderDto>(
                $"api/Folders/{id}",
                cancellationToken);
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<IReadOnlyList<PhotoDto>> GetFolderPhotosAsync(
        Guid id,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var photos = await _httpClient.GetFromJsonAsync<List<PhotoDto>>(
            $"api/Folders/{id}/photos?page={page}&pageSize={pageSize}",
            cancellationToken);

        return photos ?? [];
    }

    public async Task<(IReadOnlyList<FolderDto> Folders, string? ErrorMessage)> GetMyFoldersAsync(
        CancellationToken cancellationToken = default)
    {
        using var response = await _authService.SendAuthorizedAsync(
            () => new HttpRequestMessage(HttpMethod.Get, "api/Folders/mine"),
            cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var folders = await response.Content.ReadFromJsonAsync<List<FolderDto>>(cancellationToken);

            return (folders ?? [], null);
        }

        var errorMessage = await TryReadErrorDetailAsync(response, cancellationToken);

        return ([], errorMessage ?? "Could not load your folders.");
    }

    public async Task<(FolderDto? Folder, string? ErrorMessage)> CreateFolderAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        using var response = await _authService.SendAuthorizedAsync(
            () =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "api/Folders")
                {
                    Content = JsonContent.Create(new CreateFolderRequestDto(name))
                };

                return request;
            },
            cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var folder = await response.Content.ReadFromJsonAsync<FolderDto>(cancellationToken);

            return (folder, null);
        }

        var errorMessage = await TryReadErrorDetailAsync(response, cancellationToken);

        return (null, errorMessage ?? "Could not create folder.");
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

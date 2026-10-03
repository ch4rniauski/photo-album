using System.Net.Http.Json;
using frontend.Models;

namespace frontend.Services;

public sealed class AdminApiService
{
    private readonly AuthService _authService;

    public AdminApiService(AuthService authService)
    {
        _authService = authService;
    }

    public async Task<PagedResultDto<AdminUserDto>?> GetUsersAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        using var response = await _authService.SendAuthorizedAsync(
            () => new HttpRequestMessage(
                HttpMethod.Get,
                $"api/Admin/users?page={page}&pageSize={pageSize}"),
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<PagedResultDto<AdminUserDto>>(cancellationToken);
    }

    public async Task<PagedResultDto<UserActivityDto>?> GetActivitiesAsync(
        int page,
        int pageSize,
        Guid? userId = null,
        string? action = null,
        CancellationToken cancellationToken = default)
    {
        var query = $"api/Admin/activities?page={page}&pageSize={pageSize}";

        if (userId is not null)
        {
            query += $"&userId={userId}";
        }

        if (!string.IsNullOrWhiteSpace(action))
        {
            query += $"&action={Uri.EscapeDataString(action)}";
        }

        using var response = await _authService.SendAuthorizedAsync(
            () => new HttpRequestMessage(HttpMethod.Get, query),
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<PagedResultDto<UserActivityDto>>(cancellationToken);
    }
}

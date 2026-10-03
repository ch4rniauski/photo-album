using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using frontend.Models;
using Microsoft.JSInterop;

namespace frontend.Services;

public sealed class AuthService
{
    private const string AccessTokenKey = "photo_album_access_token";
    private const string RefreshTokenKey = "photo_album_refresh_token";
    private const string UserIdKey = "photo_album_user_id";
    private const string RoleKey = "photo_album_role";
    private const string AdminRole = "Admin";

    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    private bool _initialized;

    public AuthService(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }

    public string? AccessToken { get; private set; }

    public string? UserId { get; private set; }

    public string? Role { get; private set; }

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(AccessToken);

    public bool IsAdmin =>
        IsAuthenticated &&
        string.Equals(Role, AdminRole, StringComparison.Ordinal);

    public event Action? AuthenticationStateChanged;

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (_initialized)
        {
            return;
        }

        AccessToken = await _jsRuntime.InvokeAsync<string?>(
            "localStorage.getItem",
            cancellationToken,
            AccessTokenKey);

        UserId = await _jsRuntime.InvokeAsync<string?>(
            "localStorage.getItem",
            cancellationToken,
            UserIdKey);

        Role = await _jsRuntime.InvokeAsync<string?>(
            "localStorage.getItem",
            cancellationToken,
            RoleKey);

        _initialized = true;
    }

    public async Task<(bool Success, string? ErrorMessage)> RegisterAsync(
        string userName,
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var request = new RegisterRequestDto(userName, email, password);

        using var response = await _httpClient.PostAsJsonAsync(
            "api/Users/register",
            request,
            cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return (true, null);
        }

        var errorMessage = await TryReadErrorDetailAsync(response, cancellationToken);

        return (false, errorMessage ?? "Registration failed.");
    }

    public async Task<bool> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var request = new LoginRequestDto(email, password);

        using var response = await _httpClient.PostAsJsonAsync(
            "api/Users/login",
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var payload = await response.Content.ReadFromJsonAsync<LoginResponseDto>(cancellationToken);

        if (payload is null ||
            string.IsNullOrWhiteSpace(payload.AccessToken))
        {
            return false;
        }

        AccessToken = payload.AccessToken;
        UserId = payload.UserId;
        Role = payload.Role;

        await _jsRuntime.InvokeVoidAsync(
            "localStorage.setItem",
            cancellationToken,
            AccessTokenKey,
            payload.AccessToken);

        await _jsRuntime.InvokeVoidAsync(
            "localStorage.setItem",
            cancellationToken,
            RefreshTokenKey,
            payload.RefreshToken);

        await _jsRuntime.InvokeVoidAsync(
            "localStorage.setItem",
            cancellationToken,
            UserIdKey,
            payload.UserId);

        await _jsRuntime.InvokeVoidAsync(
            "localStorage.setItem",
            cancellationToken,
            RoleKey,
            payload.Role);

        AuthenticationStateChanged?.Invoke();

        return true;
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

    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        AccessToken = null;
        UserId = null;
        Role = null;

        await _jsRuntime.InvokeVoidAsync(
            "localStorage.removeItem",
            cancellationToken,
            AccessTokenKey);

        await _jsRuntime.InvokeVoidAsync(
            "localStorage.removeItem",
            cancellationToken,
            RefreshTokenKey);

        await _jsRuntime.InvokeVoidAsync(
            "localStorage.removeItem",
            cancellationToken,
            UserIdKey);

        await _jsRuntime.InvokeVoidAsync(
            "localStorage.removeItem",
            cancellationToken,
            RoleKey);

        AuthenticationStateChanged?.Invoke();
    }

    public void ApplyAuthorizationHeader(HttpRequestMessage request)
    {
        if (string.IsNullOrWhiteSpace(AccessToken))
        {
            return;
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
    }

    public async Task<HttpResponseMessage> SendAuthorizedAsync(
        Func<HttpRequestMessage> createRequest,
        CancellationToken cancellationToken = default)
    {
        var response = await SendOnceAsync(createRequest, cancellationToken);

        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            return response;
        }

        response.Dispose();

        var refreshed = await TryRefreshAccessTokenAsync(cancellationToken);

        if (!refreshed)
        {
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        return await SendOnceAsync(createRequest, cancellationToken);
    }

    public async Task<bool> TryRefreshAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        var accessTokenBeforeWait = AccessToken;

        await _refreshLock.WaitAsync(cancellationToken);

        try
        {
            if (!string.IsNullOrWhiteSpace(AccessToken) &&
                AccessToken != accessTokenBeforeWait)
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(UserId))
            {
                UserId = await _jsRuntime.InvokeAsync<string?>(
                    "localStorage.getItem",
                    cancellationToken,
                    UserIdKey);
            }

            var refreshToken = await _jsRuntime.InvokeAsync<string?>(
                "localStorage.getItem",
                cancellationToken,
                RefreshTokenKey);

            if (string.IsNullOrWhiteSpace(UserId) ||
                string.IsNullOrWhiteSpace(refreshToken))
            {
                return false;
            }

            var request = new UpdateAccessTokenRequestDto(UserId, refreshToken);

            using var response = await _httpClient.PostAsJsonAsync(
                "api/Users/access-token",
                request,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var payload = await response.Content.ReadFromJsonAsync<UpdateAccessTokenResponseDto>(
                cancellationToken);

            if (payload is null ||
                string.IsNullOrWhiteSpace(payload.AccessToken))
            {
                return false;
            }

            AccessToken = payload.AccessToken;

            await _jsRuntime.InvokeVoidAsync(
                "localStorage.setItem",
                cancellationToken,
                AccessTokenKey,
                payload.AccessToken);

            AuthenticationStateChanged?.Invoke();

            return true;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    private async Task<HttpResponseMessage> SendOnceAsync(
        Func<HttpRequestMessage> createRequest,
        CancellationToken cancellationToken)
    {
        using var request = createRequest();
        ApplyAuthorizationHeader(request);

        return await _httpClient.SendAsync(request, cancellationToken);
    }
}

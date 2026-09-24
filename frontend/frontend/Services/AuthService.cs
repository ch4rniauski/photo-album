using System.Net.Http.Json;
using frontend.Models;
using Microsoft.JSInterop;

namespace frontend.Services;

public sealed class AuthService
{
    private const string AccessTokenKey = "photo_album_access_token";
    private const string RefreshTokenKey = "photo_album_refresh_token";
    private const string UserIdKey = "photo_album_user_id";

    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;

    private bool _initialized;

    public AuthService(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }

    public string? AccessToken { get; private set; }

    public string? UserId { get; private set; }

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(AccessToken);

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
        
        _initialized = true;
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

        AuthenticationStateChanged?.Invoke();
        
        return true;
    }

    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        AccessToken = null;
        UserId = null;

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

        AuthenticationStateChanged?.Invoke();
    }
}

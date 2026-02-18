using Microsoft.JSInterop;
using System.Text.Json;

namespace Hyip_Payments.Web.Client.Services
{
    /// <summary>
    /// Service for storing and retrieving authentication tokens
    /// Uses browser localStorage for persistence
    /// </summary>
    public interface IAuthTokenService
    {
        Task<string?> GetTokenAsync();
        Task SetTokenAsync(string token);
        Task RemoveTokenAsync();
        Task<bool> IsAuthenticatedAsync();
    }

    public class AuthTokenService : IAuthTokenService
    {
        private readonly IJSRuntime _jsRuntime;
        private const string TokenKey = "authToken";

        public AuthTokenService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<string?> GetTokenAsync()
        {
            try
            {
                return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", TokenKey);
            }
            catch
            {
                return null;
            }
        }

        public async Task SetTokenAsync(string token)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
        }

        public async Task RemoveTokenAsync()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }
    }
}

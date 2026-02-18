using System.Net.Http.Headers;

namespace Hyip_Payments.Web.Client.Services
{
    /// <summary>
    /// HTTP message handler that automatically adds JWT Bearer token to all API requests
    /// </summary>
    public class AuthorizationMessageHandler : DelegatingHandler
    {
        private readonly IAuthTokenService _tokenService;

        public AuthorizationMessageHandler(IAuthTokenService tokenService)
        {
            _tokenService = tokenService;
            InnerHandler = new HttpClientHandler(); // Set default inner handler
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            // Get token from localStorage
            var token = await _tokenService.GetTokenAsync();

            // Add Authorization header if token exists
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}

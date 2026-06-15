using Microsoft.JSInterop;
using Moeen.Dashboard.Services.Abstractions;
using System.Net.Http.Headers;

namespace Moeen.Dashboard.Infrastructure.Http.Handlers
{
    public class AuthHandler : DelegatingHandler
    {
        private readonly ITokenService _token;
        private readonly ILogger<AuthHandler> _logger;

        public AuthHandler(ITokenService token, ILogger<AuthHandler> logger)
        {
            _token = token;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = await TryGetTokenAsync();
            var hasToken = !string.IsNullOrWhiteSpace(token);

            _logger.LogInformation(
                "Dashboard API request. Method={Method}, Url={Url}, HasToken={HasToken}, TokenLength={TokenLength}",
                request.Method,
                request.RequestUri,
                hasToken,
                token?.Length ?? 0);

            if (hasToken)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                _logger.LogInformation("Dashboard API request has bearer header. Url={Url}", request.RequestUri);
            }
            else
            {
                _logger.LogWarning("Dashboard API request has no token, so no bearer header was attached. Url={Url}", request.RequestUri);
            }

            var response = await base.SendAsync(request, cancellationToken);

            _logger.LogInformation(
                "Dashboard API response. Url={Url}, StatusCode={StatusCode}",
                request.RequestUri,
                (int)response.StatusCode);

            return response;
        }

        private async Task<string?> TryGetTokenAsync()
        {
            try
            {
                return await _token.Get();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Could not read auth token because browser storage is not available yet.");
                return null;
            }
            catch (JSException ex)
            {
                _logger.LogWarning(ex, "Could not read auth token from browser storage.");
                return null;
            }
        }
    }
}

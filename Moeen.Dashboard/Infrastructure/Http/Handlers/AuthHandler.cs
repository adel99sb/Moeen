using Microsoft.JSInterop;
using Moeen.Dashboard.Services.Abstractions;
using System.Net.Http.Headers;

namespace Moeen.Dashboard.Infrastructure.Http.Handlers
{
    public class AuthHandler : DelegatingHandler
    {
        private readonly ITokenService _token;

        public AuthHandler(ITokenService token)
        {
            _token = token;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = await TryGetTokenAsync();

            if (!string.IsNullOrWhiteSpace(token))
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<string?> TryGetTokenAsync()
        {
            try
            {
                return await _token.Get();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
            catch (JSException)
            {
                return null;
            }
        }
    }
}

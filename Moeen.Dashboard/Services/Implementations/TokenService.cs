using Moeen.Dashboard.Services.Abstractions;

namespace Moeen.Dashboard.Services.Implementations
{
    using Microsoft.JSInterop;

    public class TokenService : ITokenService
    {
        private const string Key = "auth_token";
        private readonly IJSRuntime _js;

        public TokenService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task Save(string token)
            => await _js.InvokeVoidAsync("storage.set", Key, token);

        public async Task<string> Get()
            => await _js.InvokeAsync<string>("storage.get", Key);

        public async Task Clear()
            => await _js.InvokeVoidAsync("storage.remove", Key);
    }
}

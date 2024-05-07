using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Chinook.Components
{
    public class ChinookComponentBase: ComponentBase
    {
        [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; } = default!;       
        public string InfoMessage = string.Empty;
        public string ErrorMessage = string.Empty;
        public string CurrentUserId = string.Empty;

        public async Task<string> GetUserId()
        {
            var user = (await AuthenticationState!).User;
            var userId = user.FindFirst(u => u.Type.Contains(ClaimTypes.NameIdentifier))?.Value;
            return CurrentUserId = userId!;
        }

        public void CloseInfoMessage()
        {
            InfoMessage = string.Empty;
        }

        public void CloseErrorMessage()
        {
            ErrorMessage = string.Empty;
        }
    }
}

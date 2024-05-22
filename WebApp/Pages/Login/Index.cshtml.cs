using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Dto;
using WebApp.Services;
using System.Threading.Tasks;

namespace WebApp.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AuthApiService _authApiService;
        
        [BindProperty]
        public string? Username { get; set; }

        [BindProperty]
        public string? Password { get; set; }

        public string? UsernameError { get; set; }
        public string? PasswordError { get; set; }
        
        public string? FormError { get; set; }

        public LoginModel(AuthApiService authApiService)
        {
            _authApiService = authApiService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            bool hasError = false;

            if (string.IsNullOrEmpty(Username))
            {
                UsernameError = "Username is required.";
                hasError = true;
            }

            if (string.IsNullOrEmpty(Password))
            {
                PasswordError = "Password is required.";
                hasError = true;
            }

            if (hasError)
            {
                return Page();
            }

            var playerDto = new PlayerDto
            {
                Username = Username,
                Password = Password
            };

            var response = await _authApiService.Login(playerDto);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var accessToken = response.Data?.accessToken;
                var refreshToken = response.Data?.refreshToken;

                if (accessToken != null && refreshToken != null)
                {
                    HttpContext.Response.Cookies.Append("accessToken", accessToken);
                    HttpContext.Response.Cookies.Append("refreshToken", refreshToken);
                }

                return RedirectToPage("Scoreboard");
            }
            else
            {
                FormError = "Invalid username or password";
                return Page();
            }
        }
    }
}

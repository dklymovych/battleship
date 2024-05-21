using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Dto;
using WebApp.Services;
using System.Threading.Tasks;

namespace WebApp.Pages
{
    public class RegistrationModel : PageModel
    {
        private readonly AuthApiService _authApiService;

        [BindProperty]
        public string? Username { get; set; }

        [BindProperty]
        public string? Password { get; set; }

        [BindProperty]
        public string? ConfirmPassword { get; set; }

        public string? UsernameError { get; set; }
        public string? PasswordError { get; set; }
        public string? ConfirmPasswordError { get; set; }
        public string? FormError { get; set; }

        public RegistrationModel(AuthApiService authApiService)
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

            if (Password != ConfirmPassword)
            {
                ConfirmPasswordError = "Passwords do not match.";
                hasError = true;
            }
            if (string.IsNullOrEmpty(ConfirmPassword))
            {
                ConfirmPasswordError = "Confirm password is required.";
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

            var result = await _authApiService.Register(playerDto);

            if (result.StatusCode == System.Net.HttpStatusCode.OK || result.StatusCode == System.Net.HttpStatusCode.Created)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                FormError = "This username is already used " + result.Data;
                return Page();
            }
        }
    }
}

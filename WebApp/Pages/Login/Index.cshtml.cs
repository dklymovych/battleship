using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Dto;
using WebApp.Services;

namespace WebApp.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AuthApiService _authApiService;

        public LoginModel(AuthApiService authApiService)
        {
            _authApiService = authApiService;
            Player = new PlayerDto();
            ErrorMessage = string.Empty;
        }

        [BindProperty]
        public PlayerDto Player { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var response = await _authApiService.Login(Player);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var accessToken = response.Data?.accessToken;
                var refreshToken = response.Data?.refreshToken;

                if (accessToken != null && refreshToken != null)
                {
                    HttpContext.Response.Cookies.Append("accessToken", accessToken);
                    HttpContext.Response.Cookies.Append("refreshToken", refreshToken);
                }

                return RedirectToPage("/Index");
            }
            else
            {
                ErrorMessage = "Invalid username or password";
                return Page();
            }
        }
    }
}
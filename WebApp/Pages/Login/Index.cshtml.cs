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
                    Console.WriteLine("Access Token: " + accessToken);
                    Console.WriteLine("Refresh Token: " + refreshToken);
                    var script = $"<script>localStorage.setItem('accessToken', '{accessToken}'); localStorage.setItem('refreshToken', '{refreshToken}');</script>";
                    HttpContext.Response.Headers.Append("X-LocalStorage-Script", script);
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
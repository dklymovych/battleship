using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Dto;
using WebApp.Services;

namespace WebApp.Pages
{
    public class ScoreboardModel : PageModel
    {
        private readonly GameApiService _gameApiService;

        public ScoreboardDto Scoreboard { get; set; }

        public ScoreboardModel(GameApiService gameApiService)
        {
            _gameApiService = gameApiService;
        }

        public async Task OnGetAsync()
        {
            var response = await _gameApiService.Scoreboard();

            if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Data != null)
            {
                Scoreboard = response.Data;
            }
            else
            {
                Scoreboard = new ScoreboardDto { scoreboard = Array.Empty<ScoreboardItemDto>() };
            }
        }
    }
}
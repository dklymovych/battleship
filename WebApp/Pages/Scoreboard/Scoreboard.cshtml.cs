using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Dto;
using WebApp.Services;

namespace WebApp.Pages
{
    public class ScoreboardModel : PageModel
    {
        private readonly GameApiService _gameApiService;

        public ScoreboardModel(GameApiService gameApiService)
        {
            _gameApiService = gameApiService;
        }

        public ScoreboardDto Scoreboard { get; set; } = new ScoreboardDto();

        public async Task OnGetAsync()
        {
            var response = await _gameApiService.Scoreboard();
            if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Data != null)
            {
                Scoreboard = response.Data;
            }
            else
            {
                // Handle error
                Scoreboard = new ScoreboardDto
                {
                    scoreboard = new[]
                    {
                        new ScoreboardItemDto { username = "User1", winRate = 75, numberOfGames = 20 },
                        new ScoreboardItemDto { username = "User2", winRate = 60, numberOfGames = 15 },
                        new ScoreboardItemDto { username = "User3", winRate = 85, numberOfGames = 25 },
                    }
                };
            }
        }
    }
}
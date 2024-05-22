using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using WebApp.Dto;
using WebApp.Services;
using Microsoft.Extensions.Logging;

namespace WebApp.Pages
{
    public class CreateGameModel : PageModel
    {
        private readonly GameApiService _gameApiService;
        private readonly ILogger<CreateGameModel> _logger;

        [BindProperty]
        public bool IsPublic { get; set; } = true;

        public CreateGameModel(GameApiService gameApiService, ILogger<CreateGameModel> logger)
        {
            _gameApiService = gameApiService;
            _logger = logger;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostCreateRoomAsync()
        {
            try
            {
                var shipsPosition = GetShipPositionsFromRequest();
                var createRoomDto = new CreateRoomDto
                {
                    IsPublic = IsPublic,
                    ShipsPosition = shipsPosition
                };

                _logger.LogInformation("Received CreateRoom request with data: {0}", JsonSerializer.Serialize(createRoomDto));

                var response = await _gameApiService.CreateRoom(createRoomDto);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation("Room created successfully with GameCode: {0}", response.Data.gameCode);
                    return new JsonResult(new { success = true, gameCode = response.Data.gameCode });
                }

                _logger.LogError("Failed to create room. StatusCode: {0}, Message: {1}", response.StatusCode, response.Data);
                return new JsonResult(new { success = false, message = "Failed to create room" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception in OnPostCreateRoomAsync: {0}", ex.ToString());
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        private Dictionary<string, List<List<CoordinateDto>>> GetShipPositionsFromRequest()
        {
            var requestBody = Request.Body;
            using var reader = new StreamReader(requestBody);
            var body = reader.ReadToEnd();
            var shipsPosition = JsonSerializer.Deserialize<Dictionary<string, List<List<CoordinateDto>>>>(body);
            return shipsPosition;
        }
    }
}

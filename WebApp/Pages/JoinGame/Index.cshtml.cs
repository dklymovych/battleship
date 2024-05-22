using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

public class JoinGameModel : PageModel
{
    private readonly HttpClient _httpClient;

    public JoinGameModel(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [BindProperty]
    public string RoomNumber { get; set; }
    public string Message { get; set; }

    [BindProperty]
    public Dictionary<string, string> ShipPositions { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostJoinRoomAsync()
    {
        if (string.IsNullOrEmpty(RoomNumber))
        {
            Message = "Room number cannot be empty.";
            return Page();
        }

        if (ShipPositions == null || ShipPositions.Count == 0)
        {
            Message = "Ship positions should be set.";
            return Page();
        }

        var jsonContent = new StringContent(JsonSerializer.Serialize(ShipPositions), Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync($"/api/Game/JoinRoom/{RoomNumber}", jsonContent);
        if (response.IsSuccessStatusCode)
        {
            Message = "Successfully joined room " + RoomNumber;
        }
        else
        {
            Message = "Failed to join room. Please try again.";
        }
        return Page();
    }

    public async Task<IActionResult> OnPostRandomRoomAsync()
    {
        if (ShipPositions == null || ShipPositions.Count == 0)
        {
            Message = "Ship positions should be set.";
            return Page();
        }

        var jsonContent = new StringContent(JsonSerializer.Serialize(ShipPositions), Encoding.UTF8, "application/json");

        var response = await _httpClient.GetAsync("/api/Game/RandomRoom");
        if (response.IsSuccessStatusCode)
        {
            var gameCode = await response.Content.ReadAsStringAsync();
            var joinResponse = await _httpClient.PutAsync($"/api/Game/JoinRoom/{gameCode}", jsonContent);
            if (joinResponse.IsSuccessStatusCode)
            {
                Message = "Successfully joined random room " + gameCode;
            }
            else
            {
                Message = "Failed to join random room. Please try again.";
            }
        }
        else
        {
            Message = "No available rooms. Please try again later.";
        }
        return Page();
    }
}

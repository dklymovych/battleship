using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;

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
    public string ShipPositions { get; set; }

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

        if (string.IsNullOrEmpty(ShipPositions))
        {
            Message = "Ship positions should be set.";
            return Page();
        }
        
        var shipPositions = JsonSerializer.Deserialize<Dictionary<string, string>>(ShipPositions);
        var jsonContent = new StringContent(JsonSerializer.Serialize(shipPositions), Encoding.UTF8, "application/json");

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
        if (string.IsNullOrEmpty(ShipPositions))
        {
            Message = "Ship positions should be set.";
            return Page();
        }

        var shipPositions = JsonSerializer.Deserialize<Dictionary<string, string>>(ShipPositions);
        var jsonContent = new StringContent(JsonSerializer.Serialize(shipPositions), Encoding.UTF8, "application/json");

        var response = await _httpClient.GetAsync("/api/Game/RandomRoom");
        if (response.IsSuccessStatusCode)
        {
            var gameCode = await response.Content.ReadAsStringAsync();
            var joinResponse = await _httpClient.PutAsync($"/api/Game/JoinRoom/{gameCode}", jsonContent);
            if (joinResponse.IsSuccessStatusCode)
            {
                Message = "Successfully joined random room " + response;
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


    // public async Task<IActionResult> OnPostAsync()
    // {
    //     if (string.IsNullOrEmpty(RoomNumber))
    //     {
    //         Message = "Room number cannot be empty.";
    //         return Page();
    //     }

    //     var response = await _httpClient.PutAsync($"/api/Game/JoinRoom/{RoomNumber}", null);

    //     if (response.IsSuccessStatusCode)
    //     {
    //         Message = "Successfully joined room " + RoomNumber;
    //     }
    //     else
    //     {
    //         Message = "Failed to join room. Please try again.";
    //     }

    //     return Page();
    // }

    // public async Task<IActionResult> OnPostRandomRoomAsync()
    // {
    //     var response = await _httpClient.GetFromJsonAsync<string>("/api/Game/RandomRoom");

    //     if (!string.IsNullOrEmpty(response))
    //     {
    //         var joinResponse = await _httpClient.PutAsync($"/api/Game/JoinRoom/{response}", null);

    //         if (joinResponse.IsSuccessStatusCode)
    //         {
    //             Message = "Successfully joined random room " + response;
    //         }
    //         else
    //         {
    //             Message = "Failed to join random room. Please try again.";
    //         }
    //     }
    //     else
    //     {
    //         Message = "No available rooms. Please try again later.";
    //     }

    //     return Page();
    // }
}

using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Server.Database;
using Server.Models;
using Server.Dto;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace Server.Controllers;

using ShipsPosition = Dictionary<string, List<List<Coordinate>>>;

[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ApiController]
public class GameController : ControllerBase
{
    private readonly DataContext _context;

    public GameController(DataContext context)
    {
        _context = context;
    }

    [HttpPost("CreateRoom")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult CreateRoom([FromBody] CreateRoomDto createRoomDtoDto)
    {
        Room room = new Room
        {
            GameCode = createRoomDtoDto.GameCode,
            IsPublic = createRoomDtoDto.IsPublic,
            Player1 = _currentPlayer,
            ShipsPosition1 = JsonSerializer.Deserialize<ShipsPosition>(createRoomDtoDto.ShipsPosition)!
        };

        _context.Add(room);
        _context.SaveChanges();

        return StatusCode(StatusCodes.Status201Created);
    }

    private Player _currentPlayer => (
        _context.Players.FirstOrDefault(p => p.Username == User.FindFirstValue(ClaimTypes.Name))!
    );
}

using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Server.Database;
using Server.Models;
using Server.Dto;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

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
    public ActionResult CreateRoom([FromBody] CreateRoomDto createRoomDto)
    {
        Room room = new Room
        {
            GameCode = createRoomDto.GameCode,
            IsPublic = createRoomDto.IsPublic,
            Player1 = _currentPlayer,
            ShipsPosition1 = JsonSerializer.Deserialize<ShipsPosition>(createRoomDto.ShipsPosition)!
        };

        _context.Add(room);
        _context.SaveChanges();

        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPut("JoinRoom/{gameCode}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult JoinRoom([FromRoute] string gameCode, [FromBody] JoinRoomDto joinRoomDto)
    {
        Room? room = FindRoom(gameCode);

        if (room == null)
            return NotFound();

        if (room.Player1 == _currentPlayer)
            return BadRequest();
        
        if (room.Player2 != null)
            return BadRequest();

        room.Player2 = _currentPlayer;
        room.GameStartedAt = DateTime.Now;
        room.ShipsPosition2 = JsonSerializer.Deserialize<ShipsPosition>(joinRoomDto.ShipsPosition)!;

        _context.SaveChanges();
        return Ok();
    }

    private Room? FindRoom(string gameCode) {
        return _context.Rooms
            .Include(r => r.Player1)
            .Include(r => r.Player2)
            .Include(r => r.Winner)
            .FirstOrDefault(r => r.GameCode == gameCode);
    }

    private Player _currentPlayer => (
        _context.Players.FirstOrDefault(p => p.Username == User.FindFirstValue(ClaimTypes.Name))!
    );
}

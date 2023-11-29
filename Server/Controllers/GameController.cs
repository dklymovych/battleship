using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Server.Database;
using Server.Models;
using Server.Dto;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Server.Controllers;

using ShipsPosition = Dictionary<string, List<List<Coordinate>>>;

[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ApiController]
public class GameController : ControllerBase
{
    private readonly DataContext _context;
    private const int FIELD_SIZE = 10; 
    private enum FieldCellType { Empty, Miss, Attacked, Ship }

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
        Room room = new()
        {
            GameCode = GenerateGameCode(),
            IsPublic = createRoomDto.IsPublic,
            Player1 = _currentPlayer,
            ShipsPosition1 = JsonSerializer.Deserialize<ShipsPosition>(createRoomDto.ShipsPosition)!
        };

        _context.Add(room);
        _context.SaveChanges();

        return StatusCode(StatusCodes.Status201Created, new { GameCode = room.GameCode });
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

        StartGameDto startGame = new()
        {
            Player1Name = room.Player1.Username,
            Player2Name = room.Player2.Username,
            MakeMove = false,
            Battlefield = BuildBattlefield(room.ShipsPosition2) 
        };

        return Ok(startGame);
    }

    [HttpGet("WaitForGame/{gameCode}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult WaitForGame([FromRoute] string gameCode)
    {
        Room? room = FindRoom(gameCode);

        if (room == null)
            return NotFound();

        if (room.Player1 != _currentPlayer)
            return BadRequest();

        if (room.Player2 == null)
            return NoContent();

        StartGameDto startGame = new()
        {
            Player1Name = room.Player1.Username,
            Player2Name = room.Player2.Username,
            MakeMove = true,
            Battlefield = BuildBattlefield(room.ShipsPosition1)
        };

        return Ok(startGame);
    }

    [HttpDelete("WaitForGame/{gameCode}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult CancelWaitForGame([FromRoute] string gameCode)
    {
        Room? room = FindRoom(gameCode);

        if (room == null)
            return NotFound();

        if (room.Player1 != _currentPlayer)
            return BadRequest();

        if (room.Player2 != null)
            return BadRequest();

        _context.Rooms.Remove(room);
        _context.SaveChanges();        

        return Ok();
    }

    private Room? FindRoom(string gameCode)
    {
        return _context.Rooms
            .Include(r => r.Player1)
            .Include(r => r.Player2)
            .Include(r => r.Winner)
            .FirstOrDefault(r => r.GameCode == gameCode);
    }

    private Player _currentPlayer => (
        _context.Players.FirstOrDefault(p => p.Username == User.FindFirstValue(ClaimTypes.Name))!
    );

    private static string GenerateGameCode()
    {
        const int GAME_CODE_LENGTH = 6;

        StringBuilder gameCode = new();
        Random random = new();

        for (int i = 0; i < GAME_CODE_LENGTH; ++i)
        {
            int ch = random.Next(0, 2) == 1 ? random.Next(48, 58) : random.Next(65, 91);
            gameCode.Append(Convert.ToChar(ch));
        }

        return gameCode.ToString();
    }

    private static int[] BuildBattlefield(ShipsPosition shipsPosition)
    {
        int[] battlefield = new int[FIELD_SIZE * FIELD_SIZE];

        foreach (var (key, ships) in shipsPosition)
        {
            foreach (var ship in ships)
            {
                foreach (var coordinate in ship)
                {
                    battlefield[coordinate.Y * FIELD_SIZE + coordinate.X] = (int)FieldCellType.Ship;
                }
            }
        }  

        return battlefield;
    }
}

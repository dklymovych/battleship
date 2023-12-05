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

        StartGameDto startGameDto = new()
        {
            Player1Name = room.Player1.Username,
            Player2Name = room.Player2.Username,
            MakeMove = false,
            Battlefield = Battlefield.Build(room.ShipsPosition2) 
        };

        return Ok(startGameDto);
    }

    [HttpGet("RandomRoom")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult RandomRoom()
    {
        var rooms = _context.Rooms.Where(r => r.IsPublic && r.Player2 == null).ToList();

        if (!rooms.Any())
            return NotFound();

        Random random = new();
        string gameCode = rooms[random.Next(0, rooms.Count)].GameCode;

        return Ok(new { GameCode = gameCode });
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

        StartGameDto startGameDto = new()
        {
            Player1Name = room.Player1.Username,
            Player2Name = room.Player2.Username,
            MakeMove = true,
            Battlefield = Battlefield.Build(room.ShipsPosition1)
        };

        return Ok(startGameDto);
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

    [HttpPost("MakeMove/{gameCode}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult MakeMove([FromRoute] string gameCode, [FromBody] CoordinateDto coordinateDto)
    {
        Room? room = FindRoom(gameCode);

        if (room == null)
            return NotFound();

        if (!IsPlayerInRoom(room))
            return BadRequest(); 
        
        if (!IsMoveAllowed(room))
            return BadRequest();

        ShipsPosition shipsPosition = room.Player1 == _currentPlayer
            ? room.ShipsPosition2! : room.ShipsPosition1;

        Battlefield battlefield = new(shipsPosition);

        var history = _context.History
            .Where(m => (m.GameCode == gameCode) && (m.Player == _currentPlayer))
            .AsEnumerable();

        foreach (var move in history)
        {
            battlefield.MakeMove(move.X, move.Y);
        }

        var (isHit, isDestroy) = battlefield.MakeMove(coordinateDto.X, coordinateDto.Y);
        battlefield.ClearShips();

        if (battlefield.AllShipsDestroyed())
        {
            room.Winner = _currentPlayer;
            room.GameEndedAt = DateTime.Now;
        }

        PlayerMove playerMove = new()
        {
            Room = room,
            Player = _currentPlayer,
            IsHit = isHit,
            IsDestroy = isDestroy,
            X = coordinateDto.X,
            Y = coordinateDto.Y
        };

        _context.Add(playerMove);
        _context.SaveChanges();

        BattlefieldDto battlefieldDto = new()
        {
            Battlefield = battlefield.GetField(),
            IsMoveAllowed = IsMoveAllowed(room),
            WinnerName = room.Winner?.Username
        };

        return StatusCode(StatusCodes.Status201Created, battlefieldDto);
    }

    [HttpGet("WaitForMove/{gameCode}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult WaitForMove([FromRoute] string gameCode)
    {
        Room? room = FindRoom(gameCode);

        if (room == null)
            return NotFound();

        if (!IsPlayerInRoom(room))
            return BadRequest();

        ShipsPosition shipsPosition = room.Player1 == _currentPlayer
            ? room.ShipsPosition1 : room.ShipsPosition2!;

        Battlefield battlefield = new(shipsPosition);

        var history = _context.History
            .Where(m => (m.GameCode == gameCode) && (m.Player != _currentPlayer))
            .AsEnumerable();

        foreach (var move in history)
        {
            battlefield.MakeMove(move.X, move.Y);
        }

        BattlefieldDto battlefieldDto = new()
        {
            Battlefield = battlefield.GetField(),
            IsMoveAllowed = IsMoveAllowed(room),
            WinnerName = room.Winner?.Username
        };

        return Ok(battlefieldDto);
    }

    [HttpDelete("WaitForMove/{gameCode}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult Surrender([FromRoute] string gameCode)
    {
        Room? room = FindRoom(gameCode);

        if (room == null)
            return NotFound();

        if (!IsPlayerInRoom(room))
            return BadRequest();

        room.Winner = room.Player1 == _currentPlayer
            ? room.Player2 : room.Player1;
        
        room.GameEndedAt = DateTime.Now;
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
    
    private bool IsPlayerInRoom(Room room) => (
        room.Player2 != null && (room.Player1 == _currentPlayer || room.Player2 == _currentPlayer)
    );

    private bool IsMoveAllowed(Room room)
    {
        if (room.Winner != null)
            return false;

        PlayerMove? lastMove = _context.History
            .OrderBy(m => m.Id)
            .LastOrDefault(m => m.GameCode == room.GameCode);

        if (lastMove == null)
            return room.Player1 == _currentPlayer;
        
        if (lastMove.Player == _currentPlayer)
            return lastMove.IsHit;

        return !lastMove.IsHit;
    }

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
}

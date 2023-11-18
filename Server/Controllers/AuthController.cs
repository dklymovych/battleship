using System.Security.Cryptography;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Net.Mime;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using Server.Database;
using Server.Models;
using Server.Dto;

namespace Server.Controllers;

[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(DataContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("Register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult Register([FromBody] PlayerDto playerDto)
    {
        Player? player = FindByUsername(playerDto.Username);
        if (player != null) return BadRequest();

        Player newPlayer = new Player
        {
            Username = playerDto.Username,
            Password = HashPassword(playerDto.Password),
            CreatedAt = DateTime.Now
        };

        _context.Add(newPlayer);
        _context.SaveChanges();

        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPost("Login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<JwtTokensDto> Login([FromBody] PlayerDto playerDto)
    {
        Player? player = FindByUsername(playerDto.Username);
        if (player == null) return Unauthorized();

        if (!CheckPassword(player, playerDto.Password)) 
        {
            return Unauthorized();
        }

        JwtTokensDto jwtTokens = new JwtTokensDto
        {
            AccessToken = CreateJwtToken(player, DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["Jwt:AccessTokenExpiration"]!)
            )),
            RefreshToken = CreateJwtToken(player, DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["Jwt:RefreshTokenExpiration"]!)
            ))
        };

        return Ok(jwtTokens);
    }

    [HttpPost("Refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<JwtTokensDto> Refresh([FromBody] RefreshTokenDto refreshTokenDto)
    {
        Player player = GetByJwtToken(refreshTokenDto.RefreshToken)!;

        JwtTokensDto jwtTokens = new JwtTokensDto
        {
            AccessToken = CreateJwtToken(player, DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["Jwt:AccessTokenExpiration"]!)
            )),
            RefreshToken = CreateJwtToken(player, DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["Jwt:RefreshTokenExpiration"]!)
            ))
        };

        return Ok(jwtTokens);
    }

    private Player? GetByJwtToken(string jwtToken)
    {
        string username = new JwtSecurityTokenHandler().ReadJwtToken(jwtToken).Claims
            .First(c => c.Type == ClaimTypes.Name).Value;
        
        return FindByUsername(username);
    }

    private Player? FindByUsername(string username)
    {
        return _context.Players.FirstOrDefault(p => p.Username == username);
    }

    private string CreateJwtToken(Player player, DateTime expiration)
    {
        Claim[] claims = new Claim[]
        {
            new Claim(ClaimTypes.Name, player.Username)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwtToken = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            signingCredentials: credentials,
            expires: expiration
        );

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }

    private bool CheckPassword(Player player, string password)
    {
        return player.Password == HashPassword(password);
    }

    private string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = sha256.ComputeHash(bytes);
            StringBuilder hashBuilder = new StringBuilder();

            foreach (byte b in hashBytes)
            {
                hashBuilder.Append(b.ToString("x2"));
            }

            return hashBuilder.ToString();
        }
    }
}

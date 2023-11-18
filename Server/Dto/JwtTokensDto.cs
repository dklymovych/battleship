namespace Server.Dto;

public class JwtTokensDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

public class RefreshTokenDto
{
    public string RefreshToken { get; set; } = string.Empty;
}

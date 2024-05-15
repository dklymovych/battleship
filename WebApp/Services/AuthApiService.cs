using WebApp.Dto;

namespace WebApp.Services;

public class AuthApiService : BaseApiService
{
    public AuthApiService(HttpClient httpClient, IConfiguration configuration) :
        base(httpClient, configuration) {}

    public async Task<ResponseDto<object>> Register(PlayerDto playerDto)
    {
        return await Post<PlayerDto, object>("Auth/Register", playerDto);
    }

    public async Task<ResponseDto<JwtTokensDto>> Login(PlayerDto playerDto)
    {
        return await Post<PlayerDto, JwtTokensDto>("Auth/Login", playerDto);
    }

    public async Task<ResponseDto<JwtTokensDto>> Refresh(RefreshTokenDto refreshTokenDto)
    {
        return await Post<RefreshTokenDto, JwtTokensDto>("Auth/Refresh", refreshTokenDto);
    }
}

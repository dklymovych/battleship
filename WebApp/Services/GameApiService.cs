using WebApp.Dto;

namespace WebApp.Services;

public class GameApiService : BaseApiService
{
    public GameApiService(HttpClient httpClient, IConfiguration configuration) :
        base(httpClient, configuration) {}

    public async Task<ResponseDto<GameCodeDto>> CreateRoom(CreateRoomDto createRoomDto)
    {
        return await Post<CreateRoomDto, GameCodeDto>("Game/CreateRoom", createRoomDto);
    }

    public async Task<ResponseDto<StartGameDto>> JoinRoom(string gameCode, JoinRoomDto joinRoomDto)
    {
        return await Put<JoinRoomDto, StartGameDto>($"Game/JoinRoom/{gameCode}", joinRoomDto);
    }

    public async Task<ResponseDto<GameCodeDto>> RandomRoom()
    {
        return await Get<GameCodeDto>("Game/RandomRoom");
    }

    public async Task<ResponseDto<StartGameDto>> WaitForGame(string gameCode)
    {
        return await Get<StartGameDto>($"Game/WaitForGame/{gameCode}");
    }

    public async Task<ResponseDto<object>> CancelWaitForGame(string gameCode)
    {
        return await Delete<object>($"Game/WaitForGame/{gameCode}");
    }

    public async Task<ResponseDto<BattlefieldDto>> MakeMove(string gameCode, CoordinateDto coordinateDto)
    {
        return await Post<CoordinateDto, BattlefieldDto>($"Game/MakeMove/{gameCode}", coordinateDto);
    }

    public async Task<ResponseDto<BattlefieldDto>> WaitForMove(string gameCode)
    {
        return await Get<BattlefieldDto>($"Game/WaitForMove/{gameCode}");
    }

    public async Task<ResponseDto<object>> Surrender(string gameCode)
    {
        return await Delete<object>($"Game/WaitForMove/{gameCode}");
    }

    public async Task<ResponseDto<ScoreboardDto>> Scoreboard()
    {
        return await Get<ScoreboardDto>("Game/Scoreboard");
    }
}

using System.Net.Http.Headers;
using System.Text.Json;
using WebApp.Dto;

namespace WebApp.Services;

public class BaseApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public BaseApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _configuration = configuration;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(_configuration["ApiBaseUrl"]!);
    }

    public void SetAuthHeader(string jwtToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
    }

    protected async Task<ResponseDto<T>> Get<T>(string url)
    {
        var res = await _httpClient.GetAsync(url);
        return await ParseResponse<T>(res);
    }

    protected async Task<ResponseDto<T>> Post<P, T>(string url, P data)
    {
        var res = await _httpClient.PostAsJsonAsync(url, data);
        return await ParseResponse<T>(res);
    }

    protected async Task<ResponseDto<T>> Put<P, T>(string url, P data)
    {
        var res = await _httpClient.PutAsJsonAsync(url, data);
        return await ParseResponse<T>(res);
    }

    protected async Task<ResponseDto<T>> Delete<T>(string url)
    {
        var res = await _httpClient.DeleteAsync(url);
        return await ParseResponse<T>(res);
    }

    private static async Task<ResponseDto<T>> ParseResponse<T>(HttpResponseMessage res)
    {
        T? data = default;
        var content = await res.Content.ReadAsStringAsync();

        if (res.IsSuccessStatusCode && content.Length != 0)
        {
            data = JsonSerializer.Deserialize<T>(content);
        }

        return new ResponseDto<T>
        {
            StatusCode = res.StatusCode,
            Data = data
        };
    }
}
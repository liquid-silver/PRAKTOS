using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Cats.Controllers;

[ApiController]
[Route("[controller]")]
public class CatResponseCodesController : Controller
{
    private readonly IMemoryCache _memoryCache;
    private static int _statusCode = 200;
    private readonly string _contentType = "image/jpeg";
    static readonly HttpClient Client = new HttpClient();

    public CatResponseCodesController(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    [HttpGet]
    [Route("Process_Url")]
    public async Task<FileContentResult> ProcessUrl(string url)
    {
        var result = Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                     && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

        if (!result)
        {
            return File(await CacheGet("https://sun9-west.userapi.com/sun9-50/s/v1/ig2/WTB8jwFgUXDS2PPvsUfwkvz62QAjaYmpCx9rRjRg4szJI5V_w78MPC1AI4Z9q7YOCO4IgdrevptNjJy8GaAUo-DT.jpg?size=951x736&quality=96&type=album"
                                                ), _contentType);
        }
        
        try
        {
            HttpResponseMessage response = await Client.GetAsync(url);
            _statusCode = Convert.ToInt32(response.StatusCode);
        }
        catch
        {
            return File(await CacheGet("https://sun9-west.userapi.com/sun9-50/s/v1/ig2/WTB8jwFgUXDS2PPvsUfwkvz62QAjaYmpCx9rRjRg4szJI5V_w78MPC1AI4Z9q7YOCO4IgdrevptNjJy8GaAUo-DT.jpg?size=951x736&quality=96&type=album"
                                                ), _contentType);
        }
        return File(await CacheGet($"https://http.cat/{_statusCode}.jpg"), _contentType);
    }

    private async Task<byte[]> CacheGet(string url)
    {
        if (_memoryCache.TryGetValue(url, out byte[] value))
        {
            return value;
        } 
        await CacheSet(url, await DownloadImage(url));
        return await CacheGet(url);
    }

    private async Task CacheSet(string url, byte[] image)
    {
        await Task.Run(() => _memoryCache.Set(url, image, TimeSpan.FromSeconds(5)));
    }
    
    private async Task<byte[]> DownloadImage(string url)
    {
        var data = await new HttpClient().GetAsync(url);
        var image = await data.Content.ReadAsByteArrayAsync();
        return image;
    }

    [HttpGet]
    [Route("Set_Code")]
    public void SetStatusCode(int code)
    {
        _statusCode = code;
    }
}


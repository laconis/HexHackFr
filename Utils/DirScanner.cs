using System.Net;

public class DirScanner
{
    private readonly HttpClient _client;
    private readonly string _baseUrl;
    private readonly HashSet<HttpStatusCode> _allowedCodes;

    public DirScanner(string baseUrl, IEnumerable<int>? allowedStatusCodes = null)
    {
        _baseUrl = baseUrl.TrimEnd('/');
        _client = new HttpClient();

        _allowedCodes = allowedStatusCodes != null
            ? new HashSet<HttpStatusCode>(allowedStatusCodes.Select(c => (HttpStatusCode)c))
            : new HashSet<HttpStatusCode>(); // vide = pas de filtre explicite
    }

    public async Task ScanAsync(IEnumerable<string> words, int concurrency = 50)
    {
        using var semaphore = new SemaphoreSlim(concurrency);

        var tasks = words.Select(async word =>
        {
            await semaphore.WaitAsync();
            try
            {
                await TestPathAsync(word);
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
    }


/*
* Retourne les liens sauf 404.
*/
private async Task TestPathAsyncWithout404(string word)
{
    var url = $"{_baseUrl}/{word}";

    try
    {
        var response = await _client.GetAsync(url);
        var code = (int)response.StatusCode;

        // On ignore uniquement 404
        if (code == 404)
            return;

        // Choix de la couleur
        ConsoleColor color = code switch
        {
            200 => ConsoleColor.Green,
            301 or 302 => ConsoleColor.Yellow,
            403 => ConsoleColor.Red,
            >= 500 => ConsoleColor.Magenta,
            _ => ConsoleColor.Cyan
        };

        Color.Write("[", ConsoleColor.DarkGray);
        Color.Write(code.ToString(), color);
        Color.Write("] ", ConsoleColor.DarkGray);
        Color.WriteLine(url, ConsoleColor.White);
    }
    catch
    {
        // On ignore les erreurs réseau
    }
}

}

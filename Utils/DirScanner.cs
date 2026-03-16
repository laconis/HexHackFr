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

    private async Task TestPathAsync(string word)
    {
        var url = $"{_baseUrl}/{word}";

        try
        {
            var response = await _client.GetAsync(url);
            var code = response.StatusCode;

            // Aucun filtre → logique par défaut (comme Gobuster)
            if (_allowedCodes.Count == 0)
            {
                if ((int)code >= 200 && (int)code < 400)
                    Console.WriteLine($"[{(int)code}] {url}");
                return;
            }

            // Filtre actif → n’afficher que les codes autorisés
            if (_allowedCodes.Contains(code))
            {
                Console.WriteLine($"[{(int)code}] {url}");
            }
        }
        catch
        {
            // On ignore les erreurs réseau
        }
    }
}

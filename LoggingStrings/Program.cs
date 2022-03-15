using LoggingStrings;
using System.Diagnostics;

var logger = new Logger();
logger.Log($"Now: {DateTime.Now:HH:mm}, Yesterday: {DateTime.Now.AddDays(-1)}");

var proc = Process.GetCurrentProcess();

logger.Log($"Process name {proc.ProcessName}, PID {proc.Id}");

await GetPageNewMethod(new Uri("https://www.bbc.co.uk"));

static async Task GetPageNewMethod(Uri url)
{
    using (new Logger().LogTimeTaken($"Getting page {url} at {DateTime.Now}"))
    {
        var client = new HttpClient();
        await client.GetStringAsync(url);
    }
}

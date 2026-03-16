using System.Net.NetworkInformation;

public Nmap{

/*
*  Retourne un boolean si le port est up  
*/
  public static bool IsHostUp(string ip)
{
    try
    {
        Ping ping = new Ping();
        var reply = ping.Send(ip, 200);
        return reply.Status == IPStatus.Success;
    }
    catch
    {
        return false;
    }
}

  /*
* Scanne une plage IP // 
*/
  public static IEnumerable<string> ScanNetwork(string baseIp)
{
    for (int i = 1; i < 255; i++)
    {
        string ip = $"{baseIp}.{i}";
        if (IsHostUp(ip))
            yield return ip;
    }
}


  /*
*  Retourne un boolean si le port est up  
*/
  public static IEnumerable<int> ScanPorts(string ip, IEnumerable<int> ports)
{
    foreach (var port in ports)
    {
        if (IsPortOpen(ip, port))
            yield return port;
    }
}

  /*
*  Retourne un boolean si le port est up  
*/
  public static void FullScan(string baseIp)
{
    Console.WriteLine("Découverte d'hôtes...");
    var hosts = ScanNetwork(baseIp).ToList();

    foreach (var host in hosts)
    {
        Console.WriteLine($"\nHost actif : {host}");
        var openPorts = ScanPorts(host, new[] { 21, 22, 23, 25, 80, 443, 445, 3389 });

        foreach (var port in openPorts)
        {
            Console.WriteLine($"  Port ouvert : {port}");

            var banner = GrabBanner(host, port);
            if (!string.IsNullOrEmpty(banner))
                Console.WriteLine($"    Service : {banner.Trim()}");
        }
    }
}

  
  
  

 }

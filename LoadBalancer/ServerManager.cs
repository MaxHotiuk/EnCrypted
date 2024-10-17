using System.Diagnostics;
using System.Collections.Generic;

public class ServerManager : IDisposable
{
    private const int MaxServers = 4;
    private List<Process> serverProcesses = [];

    public void StartNewServer(int port)
    {
        if (serverProcesses.Count < MaxServers)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --urls https://localhost:{port}",
                WorkingDirectory = "../API/EnCryptedAPI",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = Process.Start(startInfo);
            if (process != null)
            {
                serverProcesses.Add(process);
                Console.WriteLine($"Started new server on port {port}");
            }
            else
            {
                Console.WriteLine($"Failed to start server on port {port}");
            }
        }
        else
        {
            Console.WriteLine("Maximum number of servers running.");
        }
    }

    public void StopServer(int port)
    {
        var serverProcess = serverProcesses.FirstOrDefault(p => p.StartInfo.Arguments.Contains($"https://localhost:{port}"));
        if (serverProcess != null && !serverProcess.HasExited)
        {
            serverProcess.Kill();
            serverProcesses.Remove(serverProcess);
            Console.WriteLine($"Stopped server on port {port}");
        }
    }

    public void Dispose()
    {
        foreach (var process in serverProcesses)
        {
            if (!process.HasExited)
            {
                Console.WriteLine($"Stopping server on port {process.StartInfo.Arguments}");
                process.Kill();
                process.WaitForExit(); // Optional: Wait for the process to exit
            }
        }
        serverProcesses.Clear();
    }

    public int GetServerCount() => serverProcesses.Count;
}

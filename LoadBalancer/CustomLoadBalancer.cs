using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Yarp.ReverseProxy.LoadBalancing;
using Yarp.ReverseProxy.Model;

namespace LoadBalancer
{
    public class CustomLoadBalancer : ILoadBalancingPolicy
    {
        private readonly HttpClient _httpClient;
        private readonly ConcurrentDictionary<string, int> _serverTaskCounts = new();
        private int _roundRobinCounter = 0;

        public CustomLoadBalancer(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string Name => "CustomLoadBalancer";

        public DestinationState? PickDestination(HttpContext context, ClusterState cluster, IReadOnlyList<DestinationState> availableDestinations)
        {
            if (availableDestinations.Count == 0)
            {
                return null;
            }

            if (context.Request.Path.StartsWithSegments("/api/Logic/dotask"))
            {
                return PickLeastBusyDestination(availableDestinations);
            }
            else
            {
                return PickRoundRobinDestination(availableDestinations);
            }
        }

        private DestinationState PickLeastBusyDestination(IReadOnlyList<DestinationState> availableDestinations)
        {
            DestinationState leastBusyDestination = availableDestinations[0];
            int minTasks = int.MaxValue;

            foreach (var destination in availableDestinations)
            {
                var tasksCount = GetTasksInProcess(destination.Model.Config.Address).Result;
                _serverTaskCounts.AddOrUpdate(destination.Model.Config.Address, tasksCount, (_, _) => tasksCount);

                if (tasksCount < minTasks)
                {
                    minTasks = tasksCount;
                    leastBusyDestination = destination;
                }
            }

            Console.WriteLine($"The least busy destination id {leastBusyDestination.DestinationId} with {minTasks} tasks in process");

            return leastBusyDestination;
        }

        private DestinationState PickRoundRobinDestination(IReadOnlyList<DestinationState> availableDestinations)
        {
            var index = Interlocked.Increment(ref _roundRobinCounter) % availableDestinations.Count;
            return availableDestinations[index];
        }

        private async Task<int> GetTasksInProcess(string serverAddress)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{serverAddress}/api/Logic/tasksinprocess");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Tasks in process from {serverAddress}: {content}");

                    var dictionary = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(content);

                    if (dictionary != null && dictionary.TryGetValue("tasksInProcess", out var tasksInProcess))
                    {
                        if (tasksInProcess.ValueKind == JsonValueKind.Number)
                        {
                            return tasksInProcess.GetInt32();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting tasks in process from {serverAddress}: {ex.Message}");
            }
            return 0;
        }

        private class TasksInfo
        {
            public int tasksInProcess { get; set; }
        }
    }
}

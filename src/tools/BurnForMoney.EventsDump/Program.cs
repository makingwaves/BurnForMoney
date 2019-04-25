using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BurnForMoney.Domain;
using BurnForMoney.Infrastructure.Persistence;
using Newtonsoft.Json;

/*
 * This tool is far from being perfect. This is just a quick and dirty implementation that gets work done !
 */

namespace BurnForMoney.EventsDump
{
    class Program
    {
        static void Main(string[] args)
        {
            var eventStoreConnectionString =
                "DefaultEndpointsProtocol=https;AccountName=bfmfuncprod;AccountKey=CzEDLFzB8XBS9q5XODbcwtP3gxFnEjr5EhSa4XOYfOcbxatkbq2ygdrQbCef2AzGmMIJGPAkpWrvuIniQGwrZA==;EndpointSuffix=core.windows.net";
            var outputFile = "events_dump.json";
            var eventStore = (EventStore) EventStore.Create(eventStoreConnectionString, null);

            var eventDict = new Dictionary<Guid, List<DomainEvent>>();
            var aggregates = eventStore.ListAggregates().Result;
            for (var i = 0; i < aggregates.Count; i++)
            {
                eventDict.Add(aggregates[i], new List<DomainEvent>());
                Console.WriteLine($"Read events for aggregate: {i}/{aggregates.Count}");

                var events = eventStore.GetEventsForAggregateAsync(aggregates[i]).Result
                    .OrderBy(de => de.Version).ToList();
                eventDict[aggregates[i]].AddRange(events);
            }

            using (var fs = new FileStream(outputFile, FileMode.Create))
            {
                DumpEvents(eventDict, fs);
            }
        }

        static void DumpEvents(Dictionary<Guid, List<DomainEvent>> data, Stream outputStream)
        {
                var jsonString = JsonConvert.SerializeObject(data,
                    new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All});

               WriteString(jsonString, outputStream);
        }

        static void WriteString(string data, Stream stream)
        {
            var buffer = Encoding.ASCII.GetBytes(data);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}

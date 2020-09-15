using System;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Newtonsoft.Json;

namespace EventHubSender
{
    class Program
    {
        private const string connectionString = "<eventhubConnectionString>";
        private const string eventHubName = "iotdevice";

        static async Task Main(string[] args)
        {
            Console.WriteLine("Processing Events");
            int nbrEvents = 1000;
            int seq = 0;

            // Get the seq number
            using (var sr = new StreamReader("seq.txt"))
            {
                seq = int.Parse(await sr.ReadToEndAsync());
            }

            // Create a producer client that you can use to send events to an event hub
            await using (var producerClient = new EventHubProducerClient(connectionString, eventHubName))
            {
                var randomizer = new Random();

                // Create a batch of events 
                using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();
                {
                    for (int i = 0; i < nbrEvents; i++)
                    {
                        string sensorValue = randomizer.Next(1, 2) == 1 ? "temperature" : "humidity";
                        string value = string.Empty;

                        if (sensorValue == "temperature")
                        {
                            value = randomizer.Next(70, 90).ToString();
                        }
                        else
                        {
                            value = randomizer.Next(0, 100).ToString();
                        }

                        var sensor = new SensorDevice 
                        { 
                            Id = seq,
                            SendTime = DateTime.UtcNow,
                            Sensor = sensorValue,
                            Value = value
                        };

                        eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(sensor))));

                        seq++;
                    }

                    // Use the producer client to send the batch of events to the event hub
                    await producerClient.SendAsync(eventBatch);

                    // Write the next sequence
                    using (var sw = new StreamWriter("seq.txt"))
                    {
                        await sw.WriteAsync(seq.ToString());
                    }
                }

                Console.WriteLine($"A batch of {nbrEvents} events has been published.");
            }
        }
    }
}

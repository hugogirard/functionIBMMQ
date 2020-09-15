using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventHubSender
{
    public class SensorDevice
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("sensor")]
        public string Sensor { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("sendTime")]
        public DateTime SendTime { get; set; }
    }
}

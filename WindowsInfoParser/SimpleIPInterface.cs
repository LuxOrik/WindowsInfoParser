using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WindowsInfoGatherer
{
    public sealed class SimpleIpInterface
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string MacAddress { get; set; }
        public List<IPAddress> IpAddresses { get; set; } = new List<IPAddress>(2);
    }

    internal sealed class SimpleIpAddressConverter : JsonConverter<IPAddress>
    {
        public override void WriteJson(JsonWriter writer, IPAddress value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override IPAddress ReadJson(JsonReader reader, Type objectType, IPAddress existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var s = (string) reader.Value;
            return IPAddress.Parse(s);
        }
    }
}

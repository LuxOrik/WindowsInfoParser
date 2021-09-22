using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

using Newtonsoft.Json;

namespace WindowsInfoGatherer
{
    public sealed class SimpleIpInterface
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public PhysicalAddress MacAddress { get; set; }
        public List<IPAddress> IpAddresses { get; set; } = new List<IPAddress>(2);

        public string MacAddressColon => string.Join(":", MacAddress.GetAddressBytes().Select(b => b.ToString("X2")));
        public string MacAddressDash => string.Join("-", MacAddress.GetAddressBytes().Select(b => b.ToString("X2")));
        public string MacAddressCompact => string.Join(string.Empty, MacAddress.GetAddressBytes().Select(b => b.ToString("X2")));
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
            var s = (string)reader.Value;
            return IPAddress.Parse(s);
        }
    }

    internal sealed class SimpleMacAddressConverter : JsonConverter<PhysicalAddress>
    {
        public override void WriteJson(JsonWriter writer, PhysicalAddress value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override PhysicalAddress ReadJson(JsonReader reader, Type objectType, PhysicalAddress existingValue, bool hasExistingValue,
                                                 JsonSerializer serializer)
        {
            var s = (string)reader.Value;
            return PhysicalAddress.Parse(s);
        }
    }
}

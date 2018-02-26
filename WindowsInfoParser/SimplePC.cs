using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using Newtonsoft.Json;

namespace WindowsInfoGatherer
{
    public sealed class SimplePC
    {
        public static readonly JsonSerializer Serializer = new JsonSerializer {Formatting = Formatting.Indented};

        static SimplePC()
        {
            Serializer.Converters.Add(new SimpleIpAddressConverter());
        }

        public int DefinitionVersion { get; set; } = 2;
        public string Name { get; set; }
        public string Domain { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public ulong TotalPhysicalMemory { get; set; }
        public string OSName { get; set; }
        public string OSVersion { get; set; }
        public DateTime OSInstallDate { get; set; }
        public List<SimpleIpInterface> Interfaces { get; set; } = new List<SimpleIpInterface>(1);

        internal string AccessMacAddress
        {
            get => Interfaces.FirstOrDefault()?.MacAddress;
            set { }
        }

        internal string AccessIpAddresses
        {
            get => string.Join(", ", Interfaces.FirstOrDefault()?.IpAddresses ?? new List<IPAddress>(0));
            set { }
        }

        public void WriteToFile(string path)
        {
            using (var writer = new JsonTextWriter(File.CreateText(path)))
            {
                Serializer.Serialize(writer, this);
            }
        }

        public static SimplePC GetCurrentMachineDefinition()
        {
            var ordi = new SimplePC();
            var t = new ManagementObjectSearcher("Select * FROM Win32_ComputerSystem");
            foreach (var obj in t.Get())
            {
                ordi.Name = obj["name"] as string;
                ordi.Manufacturer = obj["manufacturer"] as string;
                ordi.Model = obj["model"] as string;
                ordi.TotalPhysicalMemory = obj["totalphysicalmemory"] as ulong? ?? 0;
                ordi.Domain = obj["domain"] as string;
            }

            t = new ManagementObjectSearcher("Select * FROM Win32_OperatingSystem");
            foreach (var obj in t.Get())
            {
                ordi.OSName = obj["caption"] as string;
                ordi.OSVersion = obj["version"] as string;
                ordi.OSInstallDate = ManagementDateTimeConverter.ToDateTime((string)obj["InstallDate"]);
            }

            foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces()
                                                         .Where(ipInt => !ipInt.Name.StartsWith("Loopback Pseudo-Interface")))
            {
                var newIp = new SimpleIpInterface
                {
                    Name = netInterface.Name,
                    Description = netInterface.Description,
                    MacAddress = netInterface.GetPhysicalAddress().ToString()
                };

                foreach (var addr in netInterface.GetIPProperties().UnicastAddresses)
                {
                    newIp.IpAddresses.Add(addr.Address);
                }

                ordi.Interfaces.Add(newIp);
            }

            return ordi;
        }
    }
}

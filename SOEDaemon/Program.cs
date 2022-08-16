using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommandLine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using SOE.Core;

namespace SOEDaemon
{
    class Program
    {
        private static DaemonOptions Options;
        private static Dictionary<string, SOEServer> Servers;

        static void Main(string[] args)
        {
            /* byte[] packet = { 0x00, 0x09, 0x01, 0x78, 0x9c, 0x63, 0x60, 0x62, 0x90, 0x54, 0x62, 0x65, 0x60, 0x94, 0x66, 0x60, 0x60, 0x98, 0xc6, 0xc0, 0x06, 0x24, 0x1d, 0x18, 0x19, 0x18, 0xa2, 0xbd, 0x18, 0xe0, 0xc0, 0x90, 0xe7, 0xb0, 0x2d, 0x23, 0xa6, 0x92, 0x18, 0xc2, 0x4a, 0x62, 0x09, 0x2b, 0x89, 0x27, 0xac, 0x24, 0x81, 0xb0, 0x92, 0x44, 0xc2, 0x4a, 0x92, 0x08, 0x2b, 0x49, 0x26, 0xac, 0x24, 0x85, 0xb0, 0x92, 0x54, 0xc2, 0x4a, 0xd2, 0xf0, 0x2b, 0x71, 0x06, 0x2a, 0xc9, 0x86, 0x2a, 0x61, 0x04, 0x93, 0x0f, 0xec, 0xb0, 0x28, 0xc9, 0x21, 0xac, 0x24, 0x17, 0x5d, 0x09, 0x00, 0x5c, 0x8c, 0x2a, 0x67, 0x04, 0xea };
            byte[] data = packet.Skip(3).ToArray();

            byte[] decompressedData = data;

            // Decompress the old packet..
            MemoryStream dataStream = new MemoryStream(data);
            MemoryStream decompressed = new MemoryStream();

            if (packet[2] == 0x01)
            {
                using (ZlibStream zlibStream = new ZlibStream(dataStream, CompressionMode.Decompress))
                {
                    zlibStream.CopyTo(decompressed);
                }

                // Reconstruct the packet..
                decompressedData = decompressed.ToArray();
            }

            // Reconstruct the packet..
            byte[] newPacket = new byte[decompressedData.Length + 2];

            // OpCode
            int place = 0;
            for (int i = 0; i < 2; i++)
            {
                newPacket[place] = packet[i];
                place++;
            }

            // Data
            for (int i = 0; i < decompressedData.Length; i++)
            {
                newPacket[place] = decompressedData[i];
                place++;
            }

            File.WriteAllBytes(Path.GetRandomFileName(), newPacket);

            return; */

            Options = new DaemonOptions();

            // Successful parse?
            if (Parser.Default.ParseArguments(args, Options))
            {
                // Are we verbose?
                if (Options.Verbose)
                {
                    // Console.WriteLine verbosely
                    Console.WriteLine("Using configuration: {0}", Options.ConfigFile);
                }

                // Configure!
                Configure();
            }

            Process.GetCurrentProcess().WaitForExit();
        }

        static void Configure()
        {
            Servers = new Dictionary<string, SOEServer>();

            bool createDefault = !File.Exists(Options.ConfigFile);
            FileStream file = new FileStream(Options.ConfigFile, FileMode.OpenOrCreate);

            if (createDefault)
            {
                StreamWriter writer = new StreamWriter(file);
                Console.WriteLine("No Config!");

                // TODO
            }
            else
            {
                Console.WriteLine("Found Config!");
                // Read the contents of the file
                StreamReader reader = new StreamReader(file);
                JArray rootArray = new JArray();

                try
                {
                    rootArray = JArray.Parse(reader.ReadToEnd());
                    reader.Close();
                }
                catch (JsonReaderException)
                {
                    Console.WriteLine("Invalid configuration!");
                    return;
                }

                foreach (var server in rootArray.Children<JObject>())
                {
                    // Check if this property is an Object
                    if (server.Type != JTokenType.Object)
                    {
                        Console.WriteLine("Invalid configuration! Servers must be JSON Objects!");
                        Environment.Exit(0);
                    }

                    // Make a new config
                    Dictionary<string, dynamic> serverConfig = new Dictionary<string, dynamic>();
                    
                    // Go through the server properties
                    foreach (var propertyKeyval in server)
                    {
                        string name = propertyKeyval.Key;
                        JToken property = propertyKeyval.Value;

                        if (property.Type == JTokenType.Object)
                        {
                            // We have a component configuration
                            Dictionary<string, dynamic> componentConfig = new Dictionary<string, dynamic>();

                            // Get the component settings
                            foreach (var componentKeyval in (JObject)property)
                            {
                                componentConfig.Add(componentKeyval.Key, componentKeyval.Value.Value<object>());
                            }

                            // Add it to the server configuration
                            serverConfig.Add(propertyKeyval.Key, componentConfig);
                        }
                        else if (property.Type == JTokenType.Array)
                        {
                            serverConfig.Add(name, property.ToObject<string[]>());
                        }
                        else
                        {
                            // Server value
                            serverConfig.Add(name, property.Value<object>());
                        }
                    }

                    // Setup a new SOEServer instance
                    SOEServer newServer = new SOEServer(serverConfig);
                    newServer.Run();

                    // Add the new server to our servers list
                    try
                    {
                        string serverName = newServer.Configuration["Name"];
                        Servers.Add(serverName, newServer);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Invalid configuration! Two servers cannot have the same name!");
                        Environment.Exit(0);
                    }
                }
            }
        }
    }
}

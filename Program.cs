using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Fix_FontLink
{
    class RegistryValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }


    internal class Program
    {
        static void Main()
        {
            // Registry key path
            string registryKeyPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\FontLink\SystemLink";

            // Define the custom sorting order
            List<string> sortingOrder = (new string[] { "MSYH.TTC", "MSJH.TTC", "MEIRYO.TTC", "MALGUN.TTF", "TAHOMA.TTF" }).ToList();

            using (FileStream fs = File.OpenWrite(String.Format("{0}-regbackup.reg", DateTime.Now.ToString("yyyyMMddHHmmss"))))
            {
                using (TextWriter writer = new StreamWriter(fs))
                {
                    writer.WriteLine("Windows Registry Editor Version 5.00");
                    writer.WriteLine();
                    writer.WriteLine("[{0}]", registryKeyPath);

                    // Registry key to inspect
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKeyPath, true))
                    {
                        if (key == null)
                        {
                            Console.WriteLine("Registry key not found {0}", registryKeyPath);
                            return;
                        }

                        // Get all the value names under the registry key
                        string[] valueNames = key.GetValueNames().Where(p => p.StartsWith("Segoe UI")).ToArray();

                        // Iterate through the value names and add them to the list
                        List<RegistryValue> registryValues = new List<RegistryValue>();
                        foreach (string valueName in valueNames)
                        {
                            // Create a list of RegistryValue objects to store key and value pairs
                            List<string> values = (key.GetValue(valueName) as string[]).ToList();

                            Console.WriteLine("Backing up {0}", valueName);

                            // Backup registry value
                            writer.WriteLine("\"{0}\"=hex(7):{1}", valueName, ToHex7(values));
                            writer.Flush();

                            // Custom sorting
                            values.Sort((v1, v2) =>
                            {
                                int i1 = sortingOrder.FindIndex(p => v1.StartsWith(p));
                                int i2 = sortingOrder.FindIndex(p => v2.StartsWith(p));
                                if (i1 != -1 && i2 != -1)
                                {
                                    return i1.CompareTo(i2);
                                }
                                if (i2 != -1)
                                {
                                    return 1;
                                }
                                if (i1 != -1)
                                {
                                    return -1;
                                }
                                return 0;
                            });

                            Console.WriteLine("Applying changes to {0}", valueName);
                            key.SetValue(valueName, values.ToArray());
                        }
                    }
                }
                
            }

            Console.WriteLine("Press any key to close.");
            Console.ReadKey(true);
        }

        static string ToHex7(List<string> values)
        {
            UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
            values = new List<string>(values);
            values.Add("");
            return String.Join(",", values.Select(p =>
            {
                List<byte> arr = unicodeEncoding.GetBytes(p).ToList();
                arr.Add(0);
                arr.Add(0);

                return String.Join(",", BitConverter.ToString(arr.ToArray()).Replace('-', ',').ToLower());
            }));
        }
    }
}

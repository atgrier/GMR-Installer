using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Collections.Generic;

namespace loco_prog
{
    public partial class ArduinoSketch
    {
        private static readonly string LIBRARY_DIRECTORY = Path.GetFullPath(Path.Join(".", "libraries"));
        private static readonly string AVRDUDE = "avrdude";

        private static readonly string FLAGS_AVRDUDE = "-v -patmega32u4 -cavr109 -PCOM1 -b57600 -D";
        private static readonly string AVR_CONF = Path.Join(LIBRARY_DIRECTORY, "avrdude", "avrdude.conf");


        private readonly string sketch_name;
        private string sketch_contents;
        private string header_reference;
        private string parameters_contents;

        private string header_modified;
        private readonly Dictionary<string, string> parameters = new Dictionary<string, string>();

        public ArduinoSketch(string name)
        {
            sketch_name = name;

            ReadSketch();
            GetChanges();
            MakeChanges();
            SaveSketch();
            CompileSketch();
            UploadSketch();
        }

        private string ReadFile(string extension)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using Stream stream = assembly.GetManifestResourceStream($"loco_prog.main_scripts.{sketch_name}.{sketch_name}.{extension}");
            using StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private void ReadSketch()
        {
            sketch_contents = ReadFile("ino");
            header_reference = ReadFile("h");
            parameters_contents = ReadFile("xml");
        }

        private string Filename(string name, string extension)
        {
            return string.Concat(name, ".", extension);
        }

        private void CheckCreateDirectory(string directory)
        {
            if (Directory.Exists(directory))
                Directory.Delete(directory, true);
            Directory.CreateDirectory(directory);
        }

        private string GetChange(XmlNode xmlNode)
        {
            string value = xmlNode.SelectSingleNode("value").InnerText;
            string type_string = xmlNode.SelectSingleNode("type").InnerText;
            string min = xmlNode.SelectSingleNode("min").InnerText;
            string max = xmlNode.SelectSingleNode("max").InnerText;
            Console.WriteLine($"Input a <{type_string}> value for the variable: <" +
                $"{xmlNode.SelectSingleNode("name").InnerText}>. If left blank, the default <" +
                $"{value}> will be used. The valid range is from <{min}> to <{max}>.");
            Console.Write(" > ");
            string read_line = Console.ReadLine();
            bool try_parse = float.TryParse(read_line, out _);
            float float_value;
            if (try_parse)
            {
                try
                {
                    switch (type_string)
                    {
                        case "int":
                            float_value = float.Parse(read_line);
                            read_line = int.Parse(read_line).ToString();
                            break;
                        case "float":
                            float_value = float.Parse(read_line);
                            read_line = float_value.ToString();
                            break;
                        default:
                            goto case "int";
                    }
                    if (float_value < float.Parse(min) || float_value > float.Parse(max))
                    {
                        Console.WriteLine("Entry outside of valid range; using default value.");
                        read_line = "";
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Unable to parse entry; using default value.");
                    read_line = "";
                }
            }
            else
                read_line = "";
            read_line = read_line == "" ? value : read_line;
            return read_line;
        }

        private void GetChanges()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(parameters_contents);
            XmlNodeList parentNode = xmlDoc.SelectNodes("parameters/item");
            foreach (XmlNode childNode in parentNode)
                parameters[childNode.SelectSingleNode("name").InnerText] = GetChange(childNode);
        }

        private void MakeChanges()
        {
            header_modified = header_reference;
            foreach (KeyValuePair<string, string> parameter in parameters)
                header_modified = header_modified.Replace($"<{parameter.Key}>", parameter.Value);
        }

        private void SaveSketch()
        {
            string sketch_path = Path.Join(LIBRARY_DIRECTORY, "sketch");
            CheckCreateDirectory(sketch_path);
            File.WriteAllText(Path.Join(sketch_path, Filename(sketch_name, "ino.cpp")), sketch_contents);
            File.WriteAllText(Path.Join(sketch_path, Filename(sketch_name, "h")), header_modified);
        }

        private void UploadSketch()
        {
            RunProcess(AVRDUDE, $"\"-C{AVR_CONF}\" {FLAGS_AVRDUDE} \"-Uflash:w:{hex_out}:i\"");
        }
    }
}

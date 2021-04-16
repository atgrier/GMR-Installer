using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Collections.Generic;
//using ArduinoUploader;
//using ArduinoUploader.Hardware;

namespace loco_prog
{
    class Program
    {
        static void Main()
        {
            //Console.WriteLine("Hello World!");
            //var filename = @"loco_code.ino.hex";
            //var portname = "COM7";
            //var uploader = new ArduinoSketchUploader(
            //    new ArduinoSketchUploaderOptions()
            //    {
            //        FileName = filename,
            //        PortName = portname,
            //        ArduinoModel = ArduinoModel.UnoR3
            //    });
            //uploader.UploadSketch();

            string[] sketches = { "controller", "receiver" };
            foreach (string sketch in sketches)
            {
                new ArudinoSketch(sketch);
            }
        }
    }

    public class ArudinoSketch
    {
        private readonly string sketch_name;

        private string sketch;
        private string header_ref;
        private string parameters_ref;

        private string header_mod;
        private Dictionary<string, string> parameters_mod = new Dictionary<string, string>();

        public ArudinoSketch(string name)
        {
            sketch_name = name;

            ReadSketch();
            GetChanges();
            MakeChanges();
            SaveSketch();
        }

        private string ReadFile(string extension)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using Stream stream = assembly.GetManifestResourceStream(
                $"loco_prog.main_scripts.{ sketch_name}.{sketch_name}.{extension}");
            using StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private void ReadSketch()
        {
            sketch = ReadFile("ino");
            header_ref = ReadFile("h");
            parameters_ref = ReadFile("xml");
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
            xmlDoc.LoadXml(parameters_ref);
            XmlNodeList parentNode = xmlDoc.SelectNodes("parameters/item");
            foreach (XmlNode childNode in parentNode)
                parameters_mod[childNode.SelectSingleNode("name").InnerText] = GetChange(childNode);
        }

        private void MakeChanges()
        {
            header_mod = header_ref;
            foreach (KeyValuePair<string, string> parameter in parameters_mod)
                header_mod = header_mod.Replace($"<{parameter.Key}>", parameter.Value);
            Console.WriteLine(header_mod);
            Console.WriteLine("making changes");
        }

        private void SaveSketch()
        {
            Console.WriteLine("saving file");
        }
    }
}

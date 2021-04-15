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

        private string sketch_ref;
        private string header_ref;
        private string parameters_ref;

        private string sketch_mod;
        private string header_mod;
        private Dictionary<string, Tuple<Type, string>> parameters_mod = new Dictionary<string, Tuple<Type, string>>();

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
            sketch_ref = ReadFile("ino");
            header_ref = ReadFile("h");
            parameters_ref = ReadFile("xml");
        }

        private string GetChange(XmlNode xmlNode)
        {
            string value = xmlNode.SelectSingleNode("value").InnerText;
            string type = xmlNode.SelectSingleNode("type").InnerText;
            string min = xmlNode.SelectSingleNode("min").InnerText;
            string max = xmlNode.SelectSingleNode("max").InnerText;
            Console.WriteLine($"Input a <{type}> value for the variable: <" +
                $"{xmlNode.SelectSingleNode("name").InnerText}>. If left blank, the default <" +
                $"{value}> will be used. The valid range is from <{min}> to <{max}>.");
            Console.Write(" > ");
            string read_line = Console.ReadLine();
            if (!float.TryParse(read_line, out _) || float.Parse(read_line) < float.Parse(min) ||
                float.Parse(read_line) > float.Parse(max))
                read_line = "";
            value = read_line == "" ? value : read_line;
            return value;
        }

        private void GetChanges()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(parameters_ref);
            XmlNodeList parentNode = xmlDoc.SelectNodes("parameters/item");
            foreach (XmlNode childNode in parentNode)
            {
                Type type;
                switch (childNode.SelectSingleNode("type").InnerText)
                {
                    case "int":
                        type = typeof(int);
                        break;
                    case "float":
                        type = typeof(float);
                        break;
                    default:
                        Console.WriteLine("Invalid type encountered, defaulting to int.");
                        type = typeof(int);
                        break;
                }
                parameters_mod[childNode.SelectSingleNode("name").InnerText] =
                    new Tuple<Type, string>(type, GetChange(childNode));
            }
        }

        private void MakeChanges()
        {
            Console.WriteLine("making changes");
        }

        private void SaveSketch()
        {
            Console.WriteLine("saving file");
        }
    }
}

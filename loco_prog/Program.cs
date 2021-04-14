using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Collections.Generic;
using loco_prog.Properties;
//using ArduinoUploader;
//using ArduinoUploader.Hardware;

namespace loco_prog
{
    class Program
    {
        static void Main(string[] args)
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

            var assembly = Assembly.GetExecutingAssembly();
            using Stream stream = assembly.GetManifestResourceStream("loco_prog.main_scripts.controller.controller.h");
            using StreamReader reader = new StreamReader(stream);
            {
                string result = reader.ReadToEnd();
                Console.WriteLine(result);
            }

            //string[] sketches = { "controller", "receiver" };
            //foreach (string sketch in sketches)
            //{
            //    new ArudinoSketch("main_scripts", sketch);
            //}
        }
    }

    //public class ArudinoSketch
    //{
    //    private readonly string sketch_path;
    //    private readonly string header_path;
    //    private readonly string parameter_path;

    //    private readonly string sketch_ref;
    //    private readonly string header_ref;
    //    private readonly string parameters_ref;

    //    private string sketch_mod;
    //    private string header_mod;
    //    private Dictionary<string, Tuple<Type, string>> parameters_mod;

    //    public ArudinoSketch(string sketch_folder, string sketch_name)
    //    {
    //        sketch_path = Path.Combine(sketch_folder, string.Concat(sketch_name, ".ino"));
    //        header_path = Path.Combine(sketch_folder, string.Concat(sketch_name, ".h"));
    //        parameter_path = Path.Combine(sketch_folder, string.Concat(sketch_name, ".xml"));

    //        ReadSketch();
    //        GetChanges();
    //        MakeChanges();
    //        SaveSketch();
    //    }

    //    public ArudinoSketch(string sketch, string header, string parameters)
    //    {
    //        sketch_ref = sketch;
    //        header_ref = header;
    //        parameters_ref = parameters;

    //        GetChanges();
    //        MakeChanges();
    //        SaveSketch();
    //    }

    //    private void ReadSketch()
    //    {
    //        Console.WriteLine(Resources.ResourceManager.);
    //        Console.WriteLine(sketch_path);
    //    }

    //    private void GetChanges()
    //    {
    //        Console.WriteLine(header_path);
    //    }

    //    private void MakeChanges()
    //    {
    //        Console.WriteLine(parameter_path);
    //    }

    //    private void SaveSketch()
    //    {
    //        Console.WriteLine("saving file");
    //    }
    //}
}

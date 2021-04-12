using System;
using System.IO
using ArduinoUploader;
using ArduinoUploader.Hardware;

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
        }
    }

    public class ArudinoSketch
    {
        private readonly string sketch_path;
        private readonly string header_path;
        private readonly string parameter_path;

        public ArudinoSketch(string sketch_folder, string sketch_name)
        {
            sketch_path = Path.Combine(sketch_folder, string.Concat(sketch_name, ".ino"));
            header_path = Path.Combine(sketch_folder, string.Concat(sketch_name, ".h"));
            parameter_path = Path.Combine(sketch_folder, string.Concat(sketch_name, ".xml"));

            ReadSketch();
            GetChanges();
            MakeChanges();
            SaveSketch();
        }

        private void ReadSketch()
        {

        }

        private void GetChanges()
        {

        }

        private void MakeChanges()
        {

        }

        private void SaveSketch()
        {

        }
    }
}

using System;
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
            Console.WriteLine($"Enter Digit to select sketch, [0] {sketches[0]}, [1] {sketches[1]}:");
            Console.Write(" > ");
            string index_string = Console.ReadLine();
            if (!int.TryParse(index_string, out int index))
            {
                Console.WriteLine($"Unable to parse digit; defaulting to {sketches[0]}.");
                index = 0;
            }
            if (index < 0 || index > 1)
            {
                Console.WriteLine($"Digit out of valid range; defaulting to {sketches[0]}.");
                index = 0;
            }
            new ArduinoSketch(sketches[index]);
        }
    }
}

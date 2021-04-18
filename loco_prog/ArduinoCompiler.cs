using System;

namespace loco_prog
{
    public partial class ArduinoSketch
    {
        private static readonly string build_directory = ".\\build\\";

        private void CompileSketch()
        {
            CheckCreateDirectory(build_directory);
            Console.WriteLine("compiling file");
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

namespace loco_prog
{
    public partial class ArduinoSketch
    {
        private static readonly string build_directory = Path.Join(".", "build", "");

        private static string[] ARDUINO15_PATHS =
        {
            Path.Join("C:", Path.DirectorySeparatorChar.ToString(), "Users", Environment.UserName, "AppData", "Local", "Arduino15"),
            Path.Join("C:", Path.DirectorySeparatorChar.ToString(), "Users", Environment.UserName, "Documents", "ArduinoData", "packages"),
            Path.Join(Path.DirectorySeparatorChar.ToString(), "Users", Environment.UserName, "Library", "Arduino15"),
            Path.Join("~", ".arduino15")
        };
        private static string GCC_PATH = Path.Join("packages", "arduino", "tools", "avr-gcc", "7.3.0-atmel3.6.1-arduino7", "bin");

        private Dictionary<string, string> AVR_GCC = new Dictionary<string, string>() {
            {"avr-gcc" , "avr-gcc" },
            {"avr-g++" , "avr-g++" },
            {"avr-gcc-ar" , "avr-gcc-ar" },
            {"avr-objcopy" , "avr-objcopy" },
            {"avr-size" , "avr-size" }
        };

        private string GCC = "avr-gcc";
        private string GPP = "avr-g++";
        private string GCC_AR = "avr-gcc-ar";
        private string OBJCOPY = "avr-objcopy";
        private string GCC_SIZE = "avr-size";

        private static readonly string FLAGS_1 = "-c -g -Os -w -std=gnu++11 -fpermissive -fno-exceptions -ffunction-sections -fdata-sections -fno-threadsafe-statics -Wno-error=narrowing";
        private static readonly string FLAGS_2 = "-flto -w -x c++ -E -CC";
        private static readonly string FLAGS_3 = "-MMD -flto";
        private static readonly string FLAGS_4 = "-mmcu=atmega32u4 -DF_CPU=8000000L -DARDUINO=10813 -DARDUINO_AVR_FEATHER32U4 -DARDUINO_ARCH_AVR -DUSB_VID=0x239A -DUSB_PID=0x800C \"-DUSB_MANUFACTURER=\\\"Adafruit\\\"\" \"-DUSB_PRODUCT=\\\"Feather 32u4\\\"\"";
        private static readonly string FLAGS_5 = "-c -g -Os -w -std=gnu11 -ffunction-sections -fdata-sections -MMD -flto -fno-fat-lto-objects";
        private static readonly string FLAGS_6 = "-c - g - x assembler-with-cpp";
        private static readonly string FLAGS_7 = "-o nul -DARDUINO_LIB_DISCOVERY_PHASE";

        private string PATHS = "";

        private string file_sketch;

        private static readonly string[] FILES_GMR = { Path.Join("GMR", "Locomotive.cpp"),
            Path.Join("GMR", "Radio.cpp"), Path.Join("GMR", "TrainMotor.cpp") };
        private static readonly string[] FILES_RADIOHEAD = { Path.Join("RadioHead", "RHGenericDriver.cpp"),
            Path.Join("RadioHead", "RHGenericSPI.cpp"), Path.Join("RadioHead", "RHHardwareSPI.cpp"),
            Path.Join("RadioHead", "RHSPIDriver.cpp"), Path.Join("RadioHead", "RH_RF69.cpp") };
        private static readonly string[] FILES_SPI = { Path.Join("SPI", "SPI.cpp") };
        private static readonly string[][] FILES_LIBRARY = { FILES_GMR, FILES_RADIOHEAD, FILES_SPI };


        private static readonly string[] FILES_ARDUINO_1 = { "WInterrupts.c" };
        private static readonly string[] FILES_ARDUINO_2 = { "HardwareSerial.cpp", "PluggableUSB.cpp", "Print.cpp",
            "Stream.cpp", "USBCore.cpp", "WString.cpp" };

        private void CompileSketch()
        {
            file_sketch = Path.Join("sketch", $"{sketch_name}.ino.cpp");
            CheckCreateDirectory(build_directory);

            SetPaths();
            AddGCCToPath();

            DetectLibraries();
            //GenerateFunctionPrototypes();
            //CompileArduinoSketch();
            //CompileLibraries();
            //CompileCore();
            //LinkComponents();
        }

        private void SetPaths()
        {
            foreach (string path in new[] { "arduino", "Adafruit", "GMR", "RadioHead", "SPI" })
                PATHS = string.Concat(PATHS, $"\"-IC:{Path.GetFullPath(Path.Join(".", "libraries", path))}\" ");
            PATHS = PATHS.Remove(PATHS.Length - 1, 1);
            Console.WriteLine(PATHS);
        }

        // https://stackoverflow.com/a/3856090
        public static bool ExistsOnPath(string fileName)
        {
            return GetFullPath(fileName) != null;
        }

        // https://stackoverflow.com/a/3856090
        public static string GetFullPath(string fileName)
        {
            foreach (string extension in new[] { "", ".exe", ".sh" })
                if (File.Exists(string.Concat(fileName, extension)))
                    return Path.GetFullPath(fileName);

            var values = Environment.GetEnvironmentVariable("PATH");
            foreach (var path in values.Split(Path.PathSeparator))
            {
                foreach (string extension in new[] { "", ".exe", ".sh" })
                {
                    var fullPath = Path.Join(path, string.Concat(fileName, extension));
                    if (File.Exists(fullPath))
                        return fullPath;
                }
            }
            return null;
        }

        private void AddGCCToPath()
        {
            if (ExistsOnPath("avr-gcc"))
                return;


            string gcc_path = "";
            foreach (string arduino_path in ARDUINO15_PATHS)
            {
                string path = Path.Join(arduino_path, GCC_PATH);
                if (!ExistsOnPath(Path.Join(path, "avr-gcc")))
                    continue;

                gcc_path = path;
                break;
            }

            int i = 0;
            if (gcc_path == "")
                while (i < 3)
                {
                    i++;
                    Console.WriteLine("Unable to detect path to avr-gcc.");
                    Console.WriteLine("Please input full absolute path to the directory containing avr-gcc, e.g. ~/path/to/gcc/");
                    Console.Write(" > ");
                    gcc_path = Console.ReadLine();
                    if (ExistsOnPath(Path.Join(gcc_path, "avr-gcc")))
                        break;
                    else if (i == 3)
                        throw new FileNotFoundException("Too many unsuccessful tries. Quitting Program.");
                }

            string old_path = Environment.GetEnvironmentVariable("PATH");
            Environment.SetEnvironmentVariable("PATH", Path.Join(gcc_path, Path.PathSeparator.ToString(), old_path));
        }

        private void DetectLibraries()
        {
            Process.Start(GPP, $"{FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \"{Path.GetFullPath(Path.Join(library_directory, file_sketch))}\" {FLAGS_7}");

            // now I'm getting arduino import issues
            foreach (string[] files in FILES_LIBRARY)
                foreach (string file in files)
                    Process.Start(GPP, $"{FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \"{Path.Join(library_directory, file)}\" {FLAGS_7}");
        }

        private void GenerateFunctionPrototypes()
        {

        }

        private void CompileArduinoSketch()
        {
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_3} {FLAGS_4} {PATHS} \"{library_directory}{file_sketch}\" - o \"{build_directory}{file_sketch}.o\"");
        }

        private void CompileLibraries()
        {
            foreach (string[] files in FILES_LIBRARY)
                foreach (string file in files)
                    Process.Start($"{GPP} {FLAGS_1} {FLAGS_3} {FLAGS_4} {PATHS} \"{library_directory}{file}\" -o \"{build_directory}{file}.o\"");
        }

        private void CompileCore()
        {
            foreach (string file in FILES_ARDUINO_1)
                Process.Start($"{GCC} {FLAGS_5} {FLAGS_4} {PATHS} \"{library_directory}Arduino\\{file}\" -o \"{build_directory}core{file}.o\"");

            foreach (string file in FILES_ARDUINO_2)
                Process.Start($"{GPP} {FLAGS_1} {FLAGS_3} {FLAGS_4} {PATHS} \"{library_directory}Arduino\\{file}\" -o \"{build_directory}core{file}.o\"");

            foreach (string[] files in new[] { FILES_ARDUINO_1, FILES_ARDUINO_2 })
                foreach (string file in files)
                    Process.Start($"{GCC_AR} rcs \"{build_directory}core\\core.a\" \"{build_directory}core\\{file}.o\"");
        }

        private void LinkComponents()
        {

        }
    }
}

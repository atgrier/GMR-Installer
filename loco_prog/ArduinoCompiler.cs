using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

namespace loco_prog
{
    public partial class ArduinoSketch
    {
        private static readonly string BUILD_DIRECTORY = Path.GetFullPath(Path.Join(".", "build", ""));

        private static string[] ARDUINO15_PATHS =
        {
            Path.Join("C:", "", "Users", Environment.UserName, "AppData", "Local", "Arduino15"),
            Path.Join("C:", "", "Users", Environment.UserName, "Documents", "ArduinoData", "packages"),
            Path.Join("", "Users", Environment.UserName, "Library", "Arduino15"),
            Path.Join("~", ".arduino15")
        };
        private static string GCC_PATH_RELATIVE = Path.Join("packages", "arduino", "tools", "avr-gcc", "7.3.0-atmel3.6.1-arduino7", "bin");

        private string GCC = "avr-gcc";
        private string GPP = "avr-g++";
        private string GCC_AR = "avr-gcc-ar";
        private string OBJCOPY = "avr-objcopy";
        private string GCC_SIZE = "avr-size";
        private string CTAGS = "CTAGS";

        private static readonly string FLAGS_1 = "-c -g -Os -w -std=gnu++11 -fpermissive -fno-exceptions -ffunction-sections -fdata-sections -fno-threadsafe-statics -Wno-error=narrowing";
        private static readonly string FLAGS_2 = "-flto -w -x c++ -E -CC";
        private static readonly string FLAGS_3 = "-MMD -flto";
        private static readonly string FLAGS_4 = "-mmcu=atmega32u4 -DF_CPU=8000000L -DARDUINO=10813 -DARDUINO_AVR_FEATHER32U4 -DARDUINO_ARCH_AVR -DUSB_VID=0x239A -DUSB_PID=0x800C \"-DUSB_MANUFACTURER=\\\"Adafruit\\\"\" \"-DUSB_PRODUCT=\\\"Feather 32u4\\\"\"";
        private static readonly string FLAGS_5 = "-c -g -Os -w -std=gnu11 -ffunction-sections -fdata-sections -MMD -flto -fno-fat-lto-objects";
        private static readonly string FLAGS_6 = "-c -g -x assembler-with-cpp";
        private static readonly string FLAGS_7 = "-o nul -DARDUINO_LIB_DISCOVERY_PHASE";
        private static readonly string FLAGS_8 = "-w -Os -g -flto -fuse-linker-plugin -Wl,--gc-sections -mmcu=atmega32u4 -include Arduino.h";
        private static readonly string FLAGS_9 = "-u --language-force=c++ -f - --c++-kinds=svpf --fields=KSTtzns --line-directives";

        private string SEARCH_PATHS = "";

        private static readonly string[] FILES_GMR = { Path.Join("GMR", "Locomotive.cpp"),
            Path.Join("GMR", "Radio.cpp"), Path.Join("GMR", "TrainMotor.cpp") };
        private static readonly string[] FILES_RADIOHEAD = { Path.Join("RadioHead", "RHGenericDriver.cpp"),
            Path.Join("RadioHead", "RHGenericSPI.cpp"), Path.Join("RadioHead", "RHHardwareSPI.cpp"),
            Path.Join("RadioHead", "RHSPIDriver.cpp"), Path.Join("RadioHead", "RH_RF69.cpp") };
        private static readonly string[] FILES_SPI = { Path.Join("SPI", "SPI.cpp") };

        private static readonly string[][] FILES_LIBRARY = { FILES_GMR, FILES_RADIOHEAD, FILES_SPI };
        private static readonly string[] LIBRARY_NAMES = { "Arduino", "Adafruit", "GMR", "RadioHead", "SPI" };


        private static readonly string[] FILES_ARDUINO_0 = { "wiring_pulse.S" };
        private static readonly string[] FILES_ARDUINO_1 = { "hooks.c", "wiring.c", "wiring_pulse.c", "WInterrupts.c",
            "wiring_shift.c", "wiring_analog.c", "wiring_digital.c" };
        private static readonly string[] FILES_ARDUINO_2 = { "HardwareSerial.cpp", "CDC.cpp", "Tone.cpp", "HardwareSerial0.cpp",
            "HardwareSerial2.cpp", "main.cpp", "IPAddress.cpp", "USBCore.cpp", "abi.cpp", "HardwareSerial1.cpp",
            "HardwareSerial3.cpp", "WMath.cpp", "WString.cpp", "Print.cpp", "PluggableUSB.cpp", "Stream.cpp", "new.cpp" };

        private string file_sketch;
        private string sketch_path;
        private string output_path;

        private void CompileSketch()
        {
            file_sketch = Path.Join("sketch", $"{sketch_name}.ino.cpp");
            sketch_path = Path.Join(LIBRARY_DIRECTORY, file_sketch);
            output_path = Path.Join(BUILD_DIRECTORY, file_sketch);
            CheckCreateDirectory(Path.Join(BUILD_DIRECTORY, "sketch"));

            SetSearchPaths();
            AddGCCToPath();

            Console.WriteLine("Compiling...");
            DetectLibraries();
            //GenerateFunctionPrototypes();
            CompileArduinoSketch();
            CompileLibraries();
            CompileCore();
            LinkComponents();
        }

        private void RunProcess(string exec, string args)
        {
            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = exec,
                    Arguments = args,
                    //CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit();
        }

        private void SetSearchPaths()
        {
            string[] path_list = new string[LIBRARY_NAMES.Length];
            for (int i = 0; i < LIBRARY_NAMES.Length; i++)
                path_list[i] = $"\"-I{Path.GetFullPath(Path.Join(".", "libraries", LIBRARY_NAMES[i]))}\"";
            SEARCH_PATHS = string.Join(' ', path_list);
        }

        // https://stackoverflow.com/a/3856090
        public static bool ExistsOnPath(string fileName)
        {
            foreach (string extension in new[] { "", ".exe", ".sh" })
                if (File.Exists(string.Concat(fileName, extension)))
                    return true;

            string values = Environment.GetEnvironmentVariable("PATH");
            foreach (var path in values.Split(Path.PathSeparator))
                foreach (string extension in new[] { "", ".exe", ".sh" })
                {
                    string fullPath = Path.Join(path, string.Concat(fileName, extension));
                    if (File.Exists(fullPath))
                        return true;
                }
            return false;
        }

        private void AddGCCToPath()
        {
            if (ExistsOnPath("avr-gcc"))
                return;

            string gcc_path = "";
            for (int j = 0; j < ARDUINO15_PATHS.Length; j++)
            {
                string path = Path.Join(ARDUINO15_PATHS[j], GCC_PATH_RELATIVE);
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
            string ctag_path = Path.Join("C:", "", "Program Files (x86)", "Arduino", "tools-builder", "ctags", "5.8-arduino11");
            Environment.SetEnvironmentVariable("PATH", string.Join(Path.PathSeparator, gcc_path, ctag_path, old_path));
        }

        private void DetectLibraries()
        {
            RunProcess(GPP, $"{FLAGS_1} {FLAGS_2} {FLAGS_4} {SEARCH_PATHS} \"{sketch_path}\" {FLAGS_7}");

            foreach (string[] files in FILES_LIBRARY)
                for (int i = 0; i < files.Length; i++)
                    RunProcess(GPP, $"{FLAGS_1} {FLAGS_2} {FLAGS_4} {SEARCH_PATHS} \"{Path.Join(LIBRARY_DIRECTORY, files[i])}\" {FLAGS_7}");
        }

        private void GenerateFunctionPrototypes()
        {
            CheckCreateDirectory(Path.Join(BUILD_DIRECTORY, "preproc"));
            string fproto_output = Path.Join(BUILD_DIRECTORY, "preproc", "ctags_target_for_gcc_minus_e.cpp");
            RunProcess(GPP, $"{FLAGS_1} {FLAGS_2} {FLAGS_4} {SEARCH_PATHS} \"{sketch_path}\" -o \"{fproto_output}\" -DARDUINO_LIB_DISCOVERY_PHASE");
            RunProcess(CTAGS, $"{FLAGS_9} \"{fproto_output}\"");
        }

        private void CompileArduinoSketch()
        {
            RunProcess(GPP, $"{FLAGS_1} {FLAGS_3} {FLAGS_4} {SEARCH_PATHS} \"{sketch_path}\" -o \"{output_path}.o\"");
        }

        private void CompileLibraries()
        {
            foreach (string path in new string[] { "GMR", "RadioHead", "SPI" })
                CheckCreateDirectory(Path.Join(BUILD_DIRECTORY, path));

            for (int j = 0; j < FILES_LIBRARY.Length; j++)
                for (int i = 0; i < FILES_LIBRARY[j].Length; i++)
                {
                    string library_path = Path.Join(LIBRARY_DIRECTORY, FILES_LIBRARY[j][i]);
                    string library_out = Path.Join(BUILD_DIRECTORY, Filename(FILES_LIBRARY[j][i], "o"));
                    RunProcess(GPP, $"{FLAGS_1} {FLAGS_3} {FLAGS_4} {SEARCH_PATHS} \"{library_path}\" -o \"{library_out}\"");
                }
        }

        private void CompileCore()
        {
            CheckCreateDirectory(Path.Join(BUILD_DIRECTORY, "core"));

            string core_path;
            string core_out;
            for (int i = 0; i < FILES_ARDUINO_0.Length; i++)
            {
                core_path = Path.Join(LIBRARY_DIRECTORY, "Arduino", FILES_ARDUINO_0[i]);
                core_out = Path.Join(BUILD_DIRECTORY, "core", Filename(FILES_ARDUINO_0[i], "o"));
                RunProcess(GCC, $"{FLAGS_6} {FLAGS_4} {SEARCH_PATHS} \"{core_path}\" -o \"{core_out}\"");
            }

            for (int i = 0; i < FILES_ARDUINO_1.Length; i++)
            {
                core_path = Path.Join(LIBRARY_DIRECTORY, "Arduino", FILES_ARDUINO_1[i]);
                core_out = Path.Join(BUILD_DIRECTORY, "core", Filename(FILES_ARDUINO_1[i], "o"));
                RunProcess(GCC, $"{FLAGS_5} {FLAGS_4} {SEARCH_PATHS} \"{core_path}\" -o \"{core_out}\"");
            }

            for (int i = 0; i < FILES_ARDUINO_2.Length; i++)
            {
                core_path = Path.Join(LIBRARY_DIRECTORY, "Arduino", FILES_ARDUINO_2[i]);
                core_out = Path.Join(BUILD_DIRECTORY, "core", Filename(FILES_ARDUINO_2[i], "o"));
                RunProcess(GPP, $"{FLAGS_1} {FLAGS_3} {FLAGS_4} {SEARCH_PATHS} \"{core_path}\" -o \"{core_out}\"");
            }

            core_path = Path.Join(BUILD_DIRECTORY, "core", "core.a");
            foreach (string[] files in new[] { FILES_ARDUINO_1, FILES_ARDUINO_2 })
                for (int i = 0; i < files.Length; i++)
                {
                    core_out = Path.Join(BUILD_DIRECTORY, "core", Filename(files[i], "o"));
                    RunProcess(GCC_AR, $"rcs \"{core_path}\" \"{core_out}\"");
                }
        }

        private void LinkComponents()
        {
            string elf_out = Path.Join(BUILD_DIRECTORY, Filename(sketch_name, "ino.elf"));
            string paths = $"\"{Path.Join(BUILD_DIRECTORY, Filename(file_sketch, "o"))}\"";
            for (int j = 0; j < FILES_LIBRARY.Length; j++)
                for (int i = 0; i < FILES_LIBRARY[j].Length; i++)
                    paths = string.Join(' ', paths, $"\"{Path.Join(BUILD_DIRECTORY, Filename(FILES_LIBRARY[j][i], "o"))}\"");
            paths = string.Join(' ', paths, $"\"{Path.Join(BUILD_DIRECTORY, "core", "core.a")}\"");
            RunProcess(GCC, $"{FLAGS_8} -o \"{elf_out}\" {paths} \"-L{BUILD_DIRECTORY}\" -lm");
        }
    }
}

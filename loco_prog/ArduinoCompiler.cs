using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

namespace loco_prog
{
    public partial class ArduinoSketch
    {
        private static readonly string build_directory = ".\\build\\";

        private static Dictionary<string, string> AVR_GCC = new Dictionary<string, string>() {
            {"avr-gcc" , "avr-gcc" },
            {"avr-g++" , "avr-g++" },
            {"avr-gcc-ar" , "avr-gcc-ar" },
            {"avr-objcopy" , "avr-objcopy" },
            {"avr-size" , "avr-size" }
        };

        private static readonly string FLAGS_1 = "-c -g -Os -w -std=gnu++11 -fpermissive -fno-exceptions -ffunction-sections -fdata-sections -fno-threadsafe-statics -Wno-error=narrowing";
        private static readonly string FLAGS_2 = "-flto - w - x c++ -E -GCC";
        private static readonly string FLAGS_3 = "-MMD - flto";
        private static readonly string FLAGS_4 = "-mmcu=atmega32u4 -DF_CPU=8000000L -DARDUINO=10813 -DARDUINO_AVR_FEATHER32U4 -DARDUINO_ARCH_AVR -DUSB_VID=0x239A -DUSB_PID=0x800C \"-DUSB_MANUFACTURER=\\\"Adafruit\\\"\" \"-DUSB_PRODUCT=\\\"Feather 32u4\\\"\"";
        private static readonly string FLAGS_5 = "-c -g -Os -w -std=gnu11 -ffunction-sections -fdata-sections -MMD -flto -fno-fat-lto-objects";
        private static readonly string FLAGS_6 = "-c - g - x assembler-with-cpp";
        private static readonly string FLAGS_7 = "-o nul -DARDUINO_LIB_DISCOVERY_PHASE";

        private static readonly string PATHS = $"\"-IC:{Path.GetFullPath(Path.Combine(".", "libraries", "Arduino"))}\" " +
            $"\"-IC:{Path.GetFullPath(Path.Combine(".", "libraries", "Adafruit"))}\" " +
            $"\"-IC:{Path.GetFullPath(Path.Combine(".", "libraries", "GMR"))}\" " +
            $"\"-IC:{Path.GetFullPath(Path.Combine(".", "libraries", "RadioHead"))}\" " +
            $"\"-IC:{Path.GetFullPath(Path.Combine(".", "libraries", "Arduino"))}\"";

        private string file_sketch;
        private static readonly string[] FILES_GMR = { Path.Combine("GMR", "Locomotive.cpp"),
            Path.Combine("GMR", "Radio.cpp"), Path.Combine("GMR", "TrainMotor.cpp") };
        private static readonly string[] FILES_RADIOHEAD = { Path.Combine("RadioHead", "RHGenericDriver.cpp"),
            Path.Combine("RadioHead", "RHGenericSPI.cpp"), Path.Combine("RadioHead", "RHHardwareSPI.cpp"),
            Path.Combine("RadioHead", "RHSPIDriver.cpp"), Path.Combine("RadioHead", "RH_RF69.cpp") };
        private static readonly string[] FILES_SPI = { Path.Combine("SPI", "SPI.cpp") };
        private static readonly string[] FILES_ARDUINO_1 = { "WInterrupts.c" };
        private static readonly string[] FILES_ARDUINO_2 = { "HardwareSerial.cpp", "PluggableUSB.cpp", "Print.cpp",
            "Stream.cpp", "USBCore.cpp", "WString.cpp" };

        private void CompileSketch()
        {
            file_sketch = Path.Combine("sketch", $"{sketch_name}.ino.cpp");
            CheckCreateDirectory(build_directory);

            DetermineGCC();
            DetectLibraries();
            //GenerateFunctionPrototypes();
            //CompileArduinoSketch();
            //CompileLibraries();
            //CompileCore();
            //LinkComponents();
        }

        private void DetermineGCC()
        {
            if (Path.GetFullPath("avr-gcc") != null)
                return;

            string[] prefix;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                prefix = new[] { "windows", "exe" };
            else if (Environment.OSVersion.Platform == PlatformID.Unix)
                prefix = new[] { "linux", "sh" };
            else if (Environment.OSVersion.Platform == PlatformID.MacOSX)
                prefix = new[] { "linux", "sh" };
            //prefix = ["osx", "sh"];
            else
                throw new PlatformNotSupportedException();

            foreach (KeyValuePair<string, string> gcc_exec in AVR_GCC)
                AVR_GCC[gcc_exec.Key] = Path.GetFullPath(Path.Combine(library_directory, "avr-gcc", prefix[0], "bin", $"{gcc_exec}.{prefix[1]}"));

    }

        private void DetectLibraries()
    {
        Console.WriteLine(GPP);
        Console.WriteLine(File.Exists(GPP));
        Console.WriteLine(Path.GetFullPath(Path.Combine(library_directory, file_sketch)));
        Console.WriteLine(File.Exists(Path.GetFullPath(Path.Combine(library_directory, file_sketch))));
        Console.WriteLine($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \"{Path.GetFullPath(Path.Combine(library_directory, file_sketch))}\" {FLAGS_7}");

        Process.Start(GPP, $"{FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \"{Path.GetFullPath(Path.Combine(library_directory, file_sketch))}\" {FLAGS_7}");

        foreach (string file in FILES_GMR)
            Process.Start(GPP, $"{FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \"{library_directory}{file}\" {FLAGS_7}");

        foreach (string file in FILES_RADIOHEAD)
            Process.Start(GPP, $"{FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \"{library_directory}{file}\" {FLAGS_7}");

        foreach (string file in FILES_SPI)
            Process.Start(GPP, $"{FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \"{library_directory}{file}\" {FLAGS_7}");
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
        foreach (string file in FILES_GMR)
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_3} {FLAGS_4} {PATHS} \"{library_directory}{file}\" -o \"{build_directory}{file}.o\"");

        foreach (string file in FILES_RADIOHEAD)
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_3} {FLAGS_4} {PATHS} \"{library_directory}{file}\" -o \"{build_directory}{file}.o\"");

        foreach (string file in FILES_SPI)
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_3} {FLAGS_4} {PATHS} \"{library_directory}{file}\" -o \"{build_directory}{file}.o\"");
    }

    private void CompileCore()
    {
        foreach (string file in FILES_ARDUINO_1)
            Process.Start($"{GCC} {FLAGS_5} {FLAGS_4} {PATHS} \"{library_directory}Arduino\\{file}\" -o \"{build_directory}core{file}.o\"");

        foreach (string file in FILES_ARDUINO_2)
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_3} {FLAGS_4} {PATHS} \"{library_directory}Arduino\\{file}\" -o \"{build_directory}core{file}.o\"");

        foreach (string file in FILES_ARDUINO_1)
            Process.Start($"{GCC_AR} rcs \"{build_directory}core\\core.a\" \"{build_directory}core\\{file}.o\"");

        foreach (string file in FILES_ARDUINO_2)
            Process.Start($"{GCC_AR} rcs \"{build_directory}core\\core.a\" \"{build_directory}core\\{file}.o\"");
    }

    private void LinkComponents()
    {

    }
}
}

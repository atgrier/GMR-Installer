using System;
using System.Diagnostics;

namespace loco_prog
{
    public partial class ArduinoSketch
    {
        private static readonly string build_directory = ".\\build\\";

        private static readonly string GCC = $"{library_directory}gcc\\avr-gcc";
        private static readonly string GPP = $"{library_directory}gcc\\avr-g++";
        private static readonly string GCC_AR = $"{library_directory}gcc\\avr-gcc-ar";
        private static readonly string OBJ_COPY = $"{library_directory}gcc\\avr-objcopy";
        private static readonly string SIZE = $"{library_directory}gcc\\avr-size";

        private static readonly string FLAGS_1 = "-c -g -Os -w -std=gnu++11 -fpermissive -fno-exceptions -ffunction-sections -fdata-sections -fno-threadsafe-statics -Wno-error=narrowing";
        private static readonly string FLAGS_2 = "-flto - w - x c++ -E -GCC";
        private static readonly string FLAGS_3 = "-MMD - flto";
        private static readonly string FLAGS_4 = "-mmcu=atmega32u4 -DF_CPU=8000000L -DARDUINO=10813 -DARDUINO_AVR_FEATHER32U4 -DARDUINO_ARCH_AVR -DUSB_VID=0x239A -DUSB_PID=0x800C \"-DUSB_MANUFACTURER=\\\"Adafruit\\\"\" \"-DUSB_PRODUCT=\\\"Feather 32u4\\\"\"";
        private static readonly string FLAGS_5 = "-c -g -Os -w -std=gnu11 -ffunction-sections -fdata-sections -MMD -flto -fno-fat-lto-objects";
        private static readonly string FLAGS_6 = "-c - g - x assembler-with-cpp";
        private static readonly string FLAGS_7 = "-o nul -DARDUINO_LIB_DISCOVERY_PHASE";

        private static readonly string PATHS = "\" - IC:.\\\\Arduino\" \"-IC:.\\\\Adafruit\" \"-IC:.\\\\GMR\" \"-IC:.\\\\RadioHead\" \"-IC:.\\\\SPI\"";

        private void CompileSketch()
        {
            CheckCreateDirectory(build_directory);
            Console.WriteLine("compiling file");
        }

        private void DetectLibraries()
        {
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \".\\\\sketch\\\\receiver.ino.cpp\" {FLAGS_7}");
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \".\\\\GMR\\\\Locomotive.cpp\" {FLAGS_7}");
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \".\\\\GMR\\\\Radio.cpp\" {FLAGS_7}");
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \".\\\\GMR\\\\TrainMotor.cpp\" {FLAGS_7}");
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \".\\\\RadioHead\\\\RHCRC.cpp\" {FLAGS_7}");
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \".\\\\RadioHead\\\\RHDatagram.cpp\" {FLAGS_7}");
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \".\\\\RadioHead\\\\RHEncryptedDriver.cpp\" {FLAGS_7}");
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \".\\\\RadioHead\\\\RHGenericDriver.cpp\" {FLAGS_7}");
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \".\\\\RadioHead\\\\RHGenericSPI.cpp\" {FLAGS_7}");
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \".\\\\RadioHead\\\\RHHardwareSPI.cpp\" {FLAGS_7}");
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \".\\\\RadioHead\\\\RHMesh.cpp\" {FLAGS_7}");
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \".\\\\RadioHead\\\\RHNRFSPIDriver.cpp\" {FLAGS_7}");
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \".\\\\RadioHead\\\\RHReliableDatagram.cpp\" {FLAGS_7}");
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \".\\\\RadioHead\\\\RHRouter.cpp\" {FLAGS_7}");
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \".\\\\RadioHead\\\\RHSPIDriver.cpp\" {FLAGS_7}");
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \".\\\\RadioHead\\\\RHSoftwareSPI.cpp\" {FLAGS_7}");
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \".\\\\RadioHead\\\\RH_ASK.cpp\" {FLAGS_7}");
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \".\\\\RadioHead\\\\RH_GCC110.cpp\" {FLAGS_7}");
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \".\\\\RadioHead\\\\RH_E32.cpp\" {FLAGS_7}");
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \".\\\\RadioHead\\\\RH_MRF89.cpp\" {FLAGS_7}");
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \".\\\\RadioHead\\\\RH_NRF24.cpp\" {FLAGS_7}");
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \".\\\\RadioHead\\\\RH_NRF51.cpp\" {FLAGS_7}");
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \".\\\\RadioHead\\\\RH_NRF905.cpp\" {FLAGS_7}");
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \".\\\\RadioHead\\\\RH_RF22.cpp\" {FLAGS_7}");
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \".\\\\RadioHead\\\\RH_RF24.cpp\" {FLAGS_7}");
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \".\\\\RadioHead\\\\RH_RF69.cpp\" {FLAGS_7}");
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \".\\\\RadioHead\\\\RH_RF95.cpp\" {FLAGS_7}");
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \".\\\\RadioHead\\\\RH_Serial.cpp\" {FLAGS_7}");
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \".\\\\RadioHead\\\\RH_TCP.cpp\" {FLAGS_7}");
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_2} {FLAGS_4} {PATHS} \".\\\\SPI\\\\SPI.cpp\" {FLAGS_7}");
        }

        private void GenerateFunctionPrototypes()
        {

        }

        private void CompileArduinoSketch()
        {
            Process.Start($"{GPP} {FLAGS_1} {FLAGS_3} {FLAGS_4} {PATHS} \".\\\\sketch\\\\receiver.ino.cpp\" - o \".\\\\..\\\\build\\\\sketch\\\\receiver.ino.cpp.o\"");
        }

        private void CompileLibraries()
        {

        }

        private void CompileCore()
        {

        }

        private void LinkComponents()
        {

        }
    }
}

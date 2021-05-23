using Avalonia;
using System;


namespace loco_prog
{
    class Program
    {
        public static void Main(string[] args)
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            //new ArduinoSketch(GetSketch());
        }

        public string GetSketch()
        {
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
            return sketches[index];
        }

        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>().UsePlatformDetect().LogToTrace();
        }
    }
}

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Data;
using System;

namespace loco_prog
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private int controller_address;
        public int ControllerAddress
        {
            get
            {
                return controller_address;
            }
            set
            {
                controller_address = ValidateAddress(value);
            }
        }

        private int locomotive_address_1;
        public int LocomotiveAddress1
        {
            get
            {
                return locomotive_address_1;
            }
            set
            {
                locomotive_address_1 = ValidateAddress(value);
            }
        }

        private int locomotive_address_2;
        public int LocomotiveAddress2
        {
            get
            {
                return locomotive_address_2;
            }
            set
            {
                locomotive_address_2 = ValidateAddress(value);
            }
        }

        private int locomotive_address_3;
        public int LocomotiveAddress3
        {
            get
            {
                return locomotive_address_3;
            }
            set
            {
                locomotive_address_3 = ValidateAddress(value);
            }
        }

        private int locomotive_address_4;
        public int LocomotiveAddress4
        {
            get
            {
                return locomotive_address_4;
            }
            set
            {
                locomotive_address_4 = ValidateAddress(value);
            }
        }

        public int ValidateAddress(int value)
        {
            if (value < 0)
                throw new DataValidationException("Invalid address.");
            return value;
        }

        public void ProgramController(object sender, RoutedEventArgs e)
        {
            int controller = controller_address;
            int[] locomotive = new[] { locomotive_address_1, locomotive_address_2, locomotive_address_3, locomotive_address_4 };
            Console.WriteLine("program controller");
        }

        public void ProgramLocomotive(object sender, RoutedEventArgs e)
        {
            int controller = controller_address;
            int locomotive = locomotive_address_1;
            Console.WriteLine("program locomotive");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

    }
}

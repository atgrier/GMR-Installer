using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace loco_prog
{
    public partial class Controller : Window
    {
        public Controller()
        {
            InitializeComponent();
        }

        public void ControllerAddress()
        {
            var label = new TextBlock();
            label.DataContext = new StyledElement();
            var controller_address_block = new TextBox();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}

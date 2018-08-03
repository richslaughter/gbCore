using Avalonia;
using Avalonia.Markup.Xaml;

namespace GbCore.Ui
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
   }
}
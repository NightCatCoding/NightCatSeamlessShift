using Avalonia.Controls;
using NightCatSeamlessShift.ViewModels;

namespace NightCatSeamlessShift;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();

        Closed += (s, e) =>
        {
            if (DataContext is MainViewModel vm)
            {
                vm.Dispose();
            }
        };
    }
}
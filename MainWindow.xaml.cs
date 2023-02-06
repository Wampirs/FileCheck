using FileCheck.ViewModels.Base;
using System.Windows;

namespace FileCheck
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = VMLocator.Main;
        }
    }
}

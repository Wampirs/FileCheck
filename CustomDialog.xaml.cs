using FileCheck.ViewModels.Base;
using System;
using System.Linq;
using System.Windows;

namespace FileCheck
{
    /// <summary>
    /// Логика взаимодействия для CustomDialog.xaml
    /// </summary>
    public partial class CustomDialog : Window
    {
        public CustomDialog()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            var actWin = App.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            Owner = actWin;
        }

        public bool? ShowDialog(BaseVM vm)
        {
            this.DataContext = vm;
            return base.ShowDialog();
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

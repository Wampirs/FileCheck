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

        public void Close(bool res)
        {
            this.DialogResult = res;
            base.Close();
        }

        public static bool? ShowDialog(BaseVM vm)
        {
            var dial = new CustomDialog();
            dial.DataContext = vm;
            return dial.ShowDialog();
        }
    }
}

using System.Windows;

namespace FileCheck
{
    /// <summary>
    /// Логика взаимодействия для WaitWindow.xaml
    /// </summary>
    public partial class WaitWindow : Window
    {
        public WaitWindow()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
        }


        static WaitWindow inst = null;

        public static void ShowWaiter()
        {
            if (inst != null) return;
            inst = new WaitWindow();
            inst.Owner.IsEnabled = false;
            inst.Show();
        }

        public static void HideWaiter()
        {
            if (inst == null) return;
            inst.Close();
            inst.Owner.IsEnabled = true;
            inst = null;
        }
    }
}

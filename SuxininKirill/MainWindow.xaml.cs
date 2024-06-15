using AdonisUI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SuxininKirill
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : AdonisUI.Controls.AdonisWindow
    {
        public MainWindow()
        {
            InitializeComponent();


            this.DataContext = new ViewModels.MainWindow.MainWindow_ViewModel();
            this.Closing += (sender, e) =>
            {
                if (!this.IsVisible)
                    return;
                if (AdonisUI.Controls.MessageBox.Show(this, "Вы уверены что хотите выйти из приложения?", "Выйти из приложения?", AdonisUI.Controls.MessageBoxButton.OKCancel, AdonisUI.Controls.MessageBoxImage.Exclamation, AdonisUI.Controls.MessageBoxResult.Cancel) == AdonisUI.Controls.MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
                else
                {
                    //Принудительно выходим из приложения
                    Application.Current.Shutdown(0);
                }
            };

            if(Properties.Settings.Default.Theme == "Dark")
            {
                //ComboBoxItem_Selected(null,null);
                ComboBoxTheme.SelectedIndex = 0;
            }
            else
            {
                //ComboBoxItem_Selected_1(null, null);
                ComboBoxTheme.SelectedIndex = 1;
            }
        }

        private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            AdonisUI.ResourceLocator.SetColorScheme(System.Windows.Application.Current.Resources, ResourceLocator.DarkColorScheme);
            Properties.Settings.Default.Theme = "Dark";
            Properties.Settings.Default.Save();
        }

        private void ComboBoxItem_Selected_1(object sender, RoutedEventArgs e)
        {
            AdonisUI.ResourceLocator.SetColorScheme(System.Windows.Application.Current.Resources, ResourceLocator.LightColorScheme);
            Properties.Settings.Default.Theme = "Light";
            Properties.Settings.Default.Save();
        }
    }
}

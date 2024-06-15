using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace SuxininKirill
{
    /// <summary>
    /// Логика взаимодействия для User_MenuWindow.xaml
    /// </summary>
    public partial class User_MenuWindow : AdonisUI.Controls.AdonisWindow
    {
        public User_MenuWindow()
        {
            InitializeComponent();
            this.DataContext = new ViewModels.User_MenuWindow_ViewModel();
        }
    }
}

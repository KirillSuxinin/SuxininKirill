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

namespace SuxininKirill.AdditionalWindows
{
    /// <summary>
    /// Логика взаимодействия для MeExpedition.xaml
    /// </summary>
    public partial class MeExpedition : AdonisUI.Controls.AdonisWindow
    {
        public MeExpedition()
        {
            InitializeComponent();
            this.DataContext = new ViewModels.Additional.MeExpedition_ViewModel();
        }
    }
}

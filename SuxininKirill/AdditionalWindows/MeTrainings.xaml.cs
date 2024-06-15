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
    /// Логика взаимодействия для MeTrainings.xaml
    /// </summary>
    public partial class MeTrainings : AdonisUI.Controls.AdonisWindow
    {
        public MeTrainings()
        {
            InitializeComponent();
            this.DataContext = new ViewModels.Additional.MeTrainings_ViewModel(this);
        }
    }
}

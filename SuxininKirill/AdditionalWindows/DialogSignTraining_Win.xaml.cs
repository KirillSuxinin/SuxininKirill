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

namespace SuxininKirill.ViewModels.Additional
{
    /// <summary>
    /// Логика взаимодействия для DialogSignTraining_Win.xaml
    /// </summary>
    public partial class DialogSignTraining_Win : AdonisUI.Controls.AdonisWindow
    {
        public DialogSignTraining_Win()
        {
            InitializeComponent();
            this.DataContext = new ViewModels.Additional.DialogSignTraining_Win_ViewModel(this);
        }

        public string ID_Training;
    }
}

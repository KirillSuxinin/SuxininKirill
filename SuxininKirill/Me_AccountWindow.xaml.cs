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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SuxininKirill
{
    /// <summary>
    /// Логика взаимодействия для MeAccountWindow.xaml
    /// </summary>
    public partial class MeAccountWindow : AdonisUI.Controls.AdonisWindow
    {
        public MeAccountWindow()
        {
            InitializeComponent();
            string nameTable = "Members";
            string NamePoly = "Email";

            if (Properties.Settings.Default.Roly_User == "Instructor")
            {
                nameTable = "Instructor";
                NamePoly = "Name_Instrucor";//TODO: Сделать для инструкторов
            }

            this.DataContext = new ViewModels.Me_AccountWindow_ViewModel(nameTable,NamePoly, Properties.Settings.Default.Login_User, Properties.Settings.Default.ID_User.ToString());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SuxininKirill
{
    /// <summary>
    /// Логика взаимодействия для TableCommonSortedWindow.xaml
    /// </summary>
    public partial class TableCommonSortedWindow : AdonisUI.Controls.AdonisWindow
    {
        public TableCommonSortedWindow(string NameTable)
        {
            InitializeComponent();
            this.DataContext = new ViewModels.TableCommonSortedWindow_ViewModel(NameTable,MeGrid,this);
        }

    }




}

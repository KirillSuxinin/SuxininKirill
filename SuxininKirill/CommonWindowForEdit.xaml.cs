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
using System.Xml;

namespace SuxininKirill
{
    /// <summary>
    /// Логика взаимодействия для CommonWindowForEdit.xaml
    /// </summary>
    public partial class CommonWindowForEdit : AdonisUI.Controls.AdonisWindow
    {
        public CommonWindowForEdit(string nameTable)
        {
            InitializeComponent();
            this.Loaded += async (sender, e) =>
            {
                ViewModels.CommonWindowsForEdit_ViewModel ViewModel;
                await Task.Run(() =>
                {
                    ViewModel = new ViewModels.CommonWindowsForEdit_ViewModel(nameTable);
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        this.DataContext = ViewModel;
                    }));
                });

            };
        }
    }
}

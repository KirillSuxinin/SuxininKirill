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
    /// Логика взаимодействия для CommonWindowForUser.xaml
    /// </summary>
    public partial class CommonWindowForUser : AdonisUI.Controls.AdonisWindow
    {
        public CommonWindowForUser(string NameTable)
        {
            InitializeComponent();
            //Делаем привязку в событии Load чтобы добавить ассинхроности, имя таблицы передаём через замыкания
            this.Loaded += async (sender, e) =>
            {
                ViewModels.CommonWindowForUser_ViewModel ViewModel;//Пустая ссылка на объект нашей View Модели.
                await Task.Run(() =>
                {
                    ViewModel = new ViewModels.CommonWindowForUser_ViewModel(NameTable);//Создаём экземпляр модели.
                    //Нужно для того чтобы работа конструктора была ассинхронная и при запросах окно не висло
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        //Используем диспетчер чтобы обратиться к свойству окна (STAThread нам не позволяет из другого потока/задачи работать с окнами)
                        this.DataContext = ViewModel;
                    }));
                });
                
            };
        }
    }
}

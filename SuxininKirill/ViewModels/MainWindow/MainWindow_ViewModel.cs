using AdonisUI;
using AdonisUI.Controls;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuxininKirill.ViewModels.MainWindow
{
    class MainWindow_ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        public MainWindow_ViewModel()
        {
            ApplicationDB.Connection.StateChange += (sender, e) =>
            {
                StateConnect = e.CurrentState;
                if(StateConnect == System.Data.ConnectionState.Open)
                    IsLoading = System.Windows.Visibility.Hidden;
                if (StateConnect == System.Data.ConnectionState.Closed)
                    ApplicationDB.Connection.OpenAsync();//Переподключение к серверу (возникает при: неполадках с интернетом (Если уд) или при отключение клиента сервера (timeout)

                
            };
            IsLoading = System.Windows.Visibility.Visible;
            ApplicationDB.Connection.OpenAsync();
            //  AdonisUI.ResourceLocator.SetColorScheme(System.Windows.Application.Current.Resources, ResourceLocator.DarkColorScheme);
        }

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private System.Windows.Visibility _IsLoading = System.Windows.Visibility.Hidden;
        public System.Windows.Visibility IsLoading
        {
            get
            {
                return _IsLoading;
            }
            set
            {
                _IsLoading = value;
                OnPropertyChanged("IsLoading");
            }
        }

        public RelayCommand SetDark
        {
            get
            {
                return (new RelayCommand(obj =>
                {
                    Environment.Exit(0);
                }));
            }
        }

        private RelayCommand _GoGithub;
        public RelayCommand GoGithub
        {
            get
            {
                return _GoGithub ?? (_GoGithub = new RelayCommand(obj =>
                {
  
                    if(obj is AdonisUI.Controls.AdonisWindow)
                    {
                        //work with window
                        if(AdonisUI.Controls.MessageBox.Show(((AdonisUI.Controls.AdonisWindow)(obj)), "Открыть \"https://github.com/KirillSuxinin\" в браузере?","Открыть github?", AdonisUI.Controls.MessageBoxButton.OKCancel, AdonisUI.Controls.MessageBoxImage.Question, AdonisUI.Controls.MessageBoxResult.No) == AdonisUI.Controls.MessageBoxResult.OK)
                        {
                            Process.Start("https://github.com/KirillSuxinin");
                        }
                    }
                },"Кнопка перейти на Github"));
            }
        }

        private RelayCommand _OpenAuth;
        public RelayCommand OpenAuth
        {
            //Описание логики закрытия всех форм (если родительская форма не уходит в VISIBLE HIDE)
            //На каждом окне есть событие Closing - которое спрашивает пользователя точно ли закрыть приложение?
            //Если окно с событием открыто то вопрос существует.
            //Если окно скрыто с дальнейшем закрытием (Переход между формами) то проверяем статус VISIBLE
            get
            {
                return _OpenAuth ?? (_OpenAuth = new RelayCommand(async obj =>
                {
                    if(obj is AdonisUI.Controls.AdonisWindow)
                    {
                        await Task.Run(() =>
                        {
                            Task.Delay(150).Wait();//Искуственная ассинхронная задержка (чтобы анимация на кнопке была видна)
                            var @this = (obj as AdonisUI.Controls.AdonisWindow);
                            
                            @this.Dispatcher.Invoke(new Action(() =>
                            {
                                @this.Hide();
                                  Authorization auth = new Authorization();
                               // (auth.Background as System.Windows.Media.ImageBrush).Opacity = 0.2;
                                auth.Owner = @this;
                                if (!(bool)auth.ShowDialog())
                                    @this.Close();
                                else
                                    @this.Show();
                            }));

                        });


                    }
                    else
                    {
                        AdonisUI.Controls.MessageBox.Show("ERROR BINDING WINDOW", "ERROR", AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Error);
                    }
                },"Открыть окно авторизации."));
            }
        }

        private System.Data.ConnectionState _StateConnect = System.Data.ConnectionState.Closed;
        public System.Data.ConnectionState StateConnect {
            get
            {
                return _StateConnect;
            }
            set
            {
                _StateConnect = value;
                OnPropertyChanged("StateConnect");
                OnPropertyChanged("StatusConnect");
            }
        }

        public string StatusConnect
        {
            get
            {
                return $"Статус: {StateConnect}";
            }
            //Пустая привязка для установки параметров т.к. Adonius.ViewModel начинает ругаться если парамеры MVVM являются ReadOnly
            set { }
        }


        public string Server
        {
            get
            {
                if(ApplicationDB.Connection is MySqlConnection)
                {
                    return $"Server Ipv4: {new MySqlConnectionStringBuilder((ApplicationDB.Connection as MySqlConnection).ConnectionString).Server} ({new MySqlConnectionStringBuilder((ApplicationDB.Connection as MySqlConnection).ConnectionString).Database})";
                }
                else if(ApplicationDB.Connection is System.Data.SqlClient.SqlConnection)
                {
                    return $"Data Source: {(ApplicationDB.Connection as System.Data.SqlClient.SqlConnection).DataSource} ({(ApplicationDB.Connection as System.Data.SqlClient.SqlConnection).Database})";
                }
                return null;
            }
            set { }
        }
    }
}

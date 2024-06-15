using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SuxininKirill.ViewModels
{
    public class Authorization_ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }



        private RelayCommand _Back;
        public RelayCommand Back
        {
            get
            {
                return _Back ?? (_Back = new RelayCommand(async obj =>
                {
                    await Task.Run(() =>
                    {
                        Task.Delay(150).Wait();
                        if(obj is AdonisUI.Controls.AdonisWindow)
                        {
                            var @this = (obj as AdonisUI.Controls.AdonisWindow);
                            @this.Dispatcher.Invoke(new Action(() =>
                            {
                                @this.DialogResult = true;
                                @this.Close();
                            }));
                        }
                    });
                }));
            }
        }


        private string _Login = Properties.Settings.Default.SaveLogin ? Properties.Settings.Default.Login_User : "";
        private string _Password;

        public string Login
        {
            get
            {
                return _Login;
            }
            set
            {
                _Login = value;
                if (Properties.Settings.Default.SaveLogin)
                {
                    Properties.Settings.Default.Login_User = value;
                    Properties.Settings.Default.Save();
                }
                OnPropertyChanged("Login");
            }
        }
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                _Password = value;
                OnPropertyChanged("Password");
            }
        }


        private float OpacityBadLogin = 0f;
        public float BadLogin
        {
            get
            {
                return OpacityBadLogin;
            }
            set
            {
                OpacityBadLogin = value;
                OnPropertyChanged("BadLogin");
            }
        }

        private bool _saveLogin = Properties.Settings.Default.SaveLogin;

        public bool SaveLogin
        {
            get
            {
                return _saveLogin;
            }
            set
            {
                _saveLogin = value;
                Properties.Settings.Default.SaveLogin = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged("SaveLogin");
            }
        }

        private RelayCommand _AuthIn;
        public RelayCommand AuthIn
        {
            get
            {
                return _AuthIn ?? (_AuthIn = new RelayCommand(async obj =>
                {
                    await Task.Run(async () =>
                    {

                        //HERE WE NEED SQL
                        string SQL = $"Select * From [USER] Where Login = \'{Login}\' and Password = \'{Password}\';";
                        if (ApplicationDB.Connection is MySqlConnection)
                            SQL = SQL.Replace("[", "").Replace("]", "");

                        var a = ApplicationDB.Connection.CreateCommand();

                        a.CommandText = SQL;

                        var result = a.ExecuteReader();
                        DataTable table = new DataTable();
                        //Используем интерфейс IDataReader
                        //MySqlConnector.MySqlDataReader - для Linux он реализует интерфейс IDataReader
                        //System.Data.SqlClient.SqlDataReader - для Windows он реализует интерфейс IDataReader
                        //И DataTable имеет аргумент IDataReader что позволяет нам делать запросы на чтение без разницы какой экземпляр MySqlConnector или SqlConnection
                        table.Load(result);
                        result.Close();
                        
                        if (table.Rows.Count > 0)
                        {
                            int idUser = (int)table.Rows[0].ItemArray[0];

                            string roly = (string)table.Rows[0].ItemArray[3];

                            Properties.Settings.Default.ID_User = idUser;//Нужно для того чтобы быстро получить информацию для личного кабинета
                            Properties.Settings.Default.Roly_User = roly;//Роль нужна для передачи на все формы
                            Properties.Settings.Default.Save();

                            
                            
                            var @this = (obj as AdonisUI.Controls.AdonisWindow);
                            @this.Dispatcher.Invoke(new Action(() =>
                            {

                                AdonisUI.Controls.AdonisWindow menu = new User_MenuWindow();
                                if (roly == "Admin")
                                    menu = new Admin_MenuWindow();
                                else if (roly == "Instructor")
                                    menu = new Instructor_MenuWindow();


                                menu.WindowState = @this.WindowState;
                                menu.Owner = @this;
                                @this.Hide();
                                if ((bool)menu.ShowDialog())
                                {
                                    @this.ShowDialog();
                                }
                                else if (!(bool)menu.DialogResult)
                                {
                                    if (menu.Tag != null && menu.Tag.ToString() == "EXITACC")
                                    {
                                        @this.ShowDialog();
                                    }
                                    else
                                        System.Windows.Application.Current.Shutdown(0);
                                }

                            }));

                        }
                        else
                        {
                            result.Close();
                            if (BadLogin <= 0)
                            {
                                BadLogin = 1;
                                await Task.Run(() =>
                                {
                                    while (BadLogin >= 0)
                                    {
                                        BadLogin -= 0.05f;
                                        Task.Delay(100).Wait();
                                    }
                                });
                            }
                        }
                        result.Close();

                    });
                }));
            }
        }
    }
}

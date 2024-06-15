using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuxininKirill.ViewModels
{

    public class CommonButton : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private string _Text;
        private string _Table;
        
        public string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                _Text = value;
                OnPropertyChanged("Text");
            }
        }
        public string Table
        {
            get
            {
                return _Table;
            }
            set
            {
                _Table = value;
                OnPropertyChanged("Table");
            }
        }

        public RelayCommand _OpenCommon;

        public RelayCommand OpenCommon
        {
            get
            {
                return _OpenCommon ?? (_OpenCommon = new RelayCommand(obj =>
                {
                    if(obj != null)
                    {
                        if(obj is AdonisUI.Controls.AdonisWindow)
                        {
                            if(Table != null)
                            {
                                var @this = (obj as AdonisUI.Controls.AdonisWindow);
                                @this.Dispatcher.Invoke(new Action(() =>
                                {

                                    CommonWindowForUser menu = new CommonWindowForUser(Table);
                                    menu.WindowState = @this.WindowState;
                                    menu.Owner = @this;
                                    @this.Hide();
                                    if ((bool)menu.ShowDialog())
                                    {
                                        @this.ShowDialog();
                                    }
                                    else if (!(bool)menu.DialogResult)
                                        System.Windows.Application.Current.Shutdown(0);
                                }));
                            }
                        }
                    }
                }));
            }
        }

      
    }

    public class User_MenuWindow_ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public User_MenuWindow_ViewModel()
        {
            //Убраны таблицы которые обычный пользователь не дожен видить и убраны связующие таблицы
            CommonButton = new ObservableCollection<CommonButton>();
          //  CommonButton.Add(new ViewModels.CommonButton() { Text = "Восхождение", Table = "Climbing" });
            CommonButton.Add(new ViewModels.CommonButton() { Text = "Инвентарь", Table = "Equipment" });
            CommonButton.Add(new ViewModels.CommonButton() { Text = "Мероприятия клуба", Table = "EventsClub" });
            CommonButton.Add(new ViewModels.CommonButton() { Text = "Экспедиции", Table = "Expedition" });
           // CommonButton.Add(new ViewModels.CommonButton() { Text = "Проблемы со здоровьем", Table = "HealthProblems" });
            CommonButton.Add(new ViewModels.CommonButton() { Text = "Инструкторы", Table = "Instructor" });
          //  CommonButton.Add(new ViewModels.CommonButton() { Text = "Участники", Table = "Members" });
            CommonButton.Add(new ViewModels.CommonButton() { Text = "Горы", Table = "Mount" });
            CommonButton.Add(new ViewModels.CommonButton() { Text = "Маршруты", Table = "Route" });
            CommonButton.Add(new ViewModels.CommonButton() { Text = "Тренинги", Table = "Trainings" });
        }

        public ObservableCollection<CommonButton> CommonButton { get; set; }


        private RelayCommand _Back;
        public RelayCommand Back
        {
            get
            {
                return _Back ?? (_Back = new RelayCommand(obj =>
                {
                    if (obj != null)
                    {
                        if (obj is AdonisUI.Controls.AdonisWindow)
                        {
                            var @this = (obj as AdonisUI.Controls.AdonisWindow);
                            @this.Dispatcher.Invoke(new Action(() =>
                            {
                                @this.DialogResult = true;
                                @this.Close();
                            }));
                        }
                    }


                }, "Вернуться назад"));
            }
        }

        public RelayCommand MeAccount
        {
            get
            {
                return (new RelayCommand((obj) =>
                {
                    if (obj != null)
                    {
                        AdonisUI.Controls.AdonisWindow @this = (obj as AdonisUI.Controls.AdonisWindow);
                        AdonisUI.Controls.AdonisWindow menu = new MeAccountWindow();

                        menu.WindowState = @this.WindowState;
                        menu.Owner = @this;
                        @this.Hide();
                        if ((bool)menu.ShowDialog())
                        {
                            if (menu.Tag != null && menu.Tag.ToString() == "EXITACC")
                            {
                                @this.Tag = "EXITACC";
                                @this.Close();
                            }
                            else
                                @this.ShowDialog();
                        }
                        else if (!(bool)menu.DialogResult)
                            System.Windows.Application.Current.Shutdown(0);
                    }
                }));
            }
        }

        public RelayCommand MeExpedition
        {
            get
            {
                return (new RelayCommand(obj =>
                {
                    if(obj != null)
                    {
                        var @this = (obj as AdonisUI.Controls.AdonisWindow);
                        AdonisUI.Controls.AdonisWindow win = new AdditionalWindows.MeExpedition();

                        win.WindowState = @this.WindowState;
                        win.Owner = @this;
                        @this.Hide();
                        if ((bool)win.ShowDialog())
                        {
                            if (win.Tag != null && win.Tag.ToString() == "EXITACC")
                            {
                                @this.Tag = "EXITACC";
                                @this.Close();
                            }
                            else
                                @this.ShowDialog();
                        }
                        else if (!(bool)win.DialogResult)
                            System.Windows.Application.Current.Shutdown(0);
                    }
                }));
            }
        }

        public RelayCommand MeTrainings
        {
            get
            {
                return (new RelayCommand(obj =>
                {
                    if (obj != null)
                    {
                        var @this = (obj as AdonisUI.Controls.AdonisWindow);
                        AdonisUI.Controls.AdonisWindow win = new AdditionalWindows.MeTrainings();

                        win.WindowState = @this.WindowState;
                        win.Owner = @this;
                        @this.Hide();
                        if ((bool)win.ShowDialog())
                        {
                            if (win.Tag != null && win.Tag.ToString() == "EXITACC")
                            {
                                @this.Tag = "EXITACC";
                                @this.Close();
                            }
                            else
                                @this.ShowDialog();
                        }
                        else if (!(bool)win.DialogResult)
                            System.Windows.Application.Current.Shutdown(0);
                    }
                }));
            }
        }
    }
}

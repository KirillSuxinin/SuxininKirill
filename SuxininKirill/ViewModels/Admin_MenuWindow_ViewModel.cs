using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuxininKirill.ViewModels
{

    public class CommonButtonAdmin : INotifyPropertyChanged
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

        private RelayCommand _OpenCommon;

        public RelayCommand OpenCommon
        {
            get
            {
                return _OpenCommon ?? (_OpenCommon = new RelayCommand(obj =>
                {
                    if (obj != null)
                    {
                        if (obj is AdonisUI.Controls.AdonisWindow)
                        {
                            if (Table != null)
                            {
                                var @this = (obj as AdonisUI.Controls.AdonisWindow);
                                @this.Dispatcher.Invoke(new Action(() =>
                                {

                                    CommonWindowForEdit menu = new CommonWindowForEdit(Table);
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
                                }));
                            }
                        }
                    }
                }));
            }
        }
    }

    public class Admin_MenuWindow_ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public Admin_MenuWindow_ViewModel()
        {
            CommonButton = new ObservableCollection<CommonButtonAdmin>();
            CommonButton.Add(new ViewModels.CommonButtonAdmin() { Text = "Восхождение", Table = "Climbing" }); //
            CommonButton.Add(new ViewModels.CommonButtonAdmin() { Text = "Инвентарь", Table = "Equipment" }); //
            CommonButton.Add(new ViewModels.CommonButtonAdmin() { Text = "Мероприятия клуба", Table = "EventsClub" }); //
            CommonButton.Add(new ViewModels.CommonButtonAdmin() { Text = "Экспедиции", Table = "Expedition" }); //
            CommonButton.Add(new ViewModels.CommonButtonAdmin() { Text = "Проблемы со здоровьем", Table = "HealthProblems" }); //
            CommonButton.Add(new ViewModels.CommonButtonAdmin() { Text = "Инструкторы", Table = "Instructor" }); //
            CommonButton.Add(new ViewModels.CommonButtonAdmin() { Text = "Участники", Table = "Members" }); //
            CommonButton.Add(new ViewModels.CommonButtonAdmin() { Text = "Участникик мероприятия", Table = "MembersEvent" }); //
            CommonButton.Add(new ViewModels.CommonButtonAdmin() { Text = "Участники экспедиций", Table = "MembersExpedition" }); //
            CommonButton.Add(new ViewModels.CommonButtonAdmin() { Text = "Участники тренингов", Table = "MembersTrainings" }); //
            CommonButton.Add(new ViewModels.CommonButtonAdmin() { Text = "Горы", Table = "Mount" }); //
            CommonButton.Add(new ViewModels.CommonButtonAdmin() { Text = "Аренда снаряжения", Table = "RentEquipment" }); //
            CommonButton.Add(new ViewModels.CommonButtonAdmin() { Text = "Маршруты", Table = "Route" }); //
            CommonButton.Add(new ViewModels.CommonButtonAdmin() { Text = "Тренинги", Table = "Trainings" }); //
            CommonButton.Add(new ViewModels.CommonButtonAdmin() { Text = "Склад снаряжения", Table = "Warehouse" }); //
            CommonButton.Add(new ViewModels.CommonButtonAdmin() { Text = "Пользователи", Table = "USER" }); //
        }

        public ObservableCollection<CommonButtonAdmin> CommonButton { get; set; }

        public string MTitle
        {
            get
            {
                return "Меню администратора";
            }
        }

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


    }
}

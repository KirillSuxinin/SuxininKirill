using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuxininKirill.ViewModels
{
    public class CommonButtonI : INotifyPropertyChanged
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

        public bool CanEdit = false;

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

                                    AdonisUI.Controls.AdonisWindow menu = new CommonWindowForUser(Table);
                                    if (this.CanEdit)
                                        menu = new CommonWindowForEdit(Table);

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
    internal class Instructor_MenuWindow_ViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public ObservableCollection<CommonButtonI> CommonButton { get; set; }


        public Instructor_MenuWindow_ViewModel()
        {
            CommonButton = new ObservableCollection<CommonButtonI>();
            CommonButton.Add(new CommonButtonI() { Text = "Восхождение", Table = "Climbing", CanEdit = true }); //
            CommonButton.Add(new CommonButtonI() { Text = "Инвентарь", Table = "Equipment",CanEdit = true }); //
            CommonButton.Add(new CommonButtonI() { Text = "Мероприятия клуба", Table = "EventsClub",CanEdit = true }); //
            CommonButton.Add(new CommonButtonI() { Text = "Экспедиции", Table = "Expedition",CanEdit = true }); //
            CommonButton.Add(new CommonButtonI() { Text = "Проблемы со здоровьем", Table = "HealthProblems" }); //
            CommonButton.Add(new CommonButtonI() { Text = "Инструкторы", Table = "Instructor" }); //
            CommonButton.Add(new CommonButtonI() { Text = "Участники", Table = "Members",CanEdit = true }); //
            CommonButton.Add(new CommonButtonI() { Text = "Участникик мероприятия", Table = "MembersEvent" , CanEdit = true }); //
            CommonButton.Add(new CommonButtonI() { Text = "Участники экспедиций", Table = "MembersExpedition", CanEdit = true }); //
            CommonButton.Add(new CommonButtonI() { Text = "Участники тренингов", Table = "MembersTrainings" }); //
            CommonButton.Add(new CommonButtonI() { Text = "Горы", Table = "Mount" }); //
            CommonButton.Add(new CommonButtonI() { Text = "Аренда снаряжения", Table = "RentEquipment" }); //
            CommonButton.Add(new CommonButtonI() { Text = "Маршруты", Table = "Route",CanEdit = true }); //
            CommonButton.Add(new CommonButtonI() { Text = "Тренинги", Table = "Trainings",CanEdit = true }); //
            CommonButton.Add(new CommonButtonI() { Text = "Склад снаряжения", Table = "Warehouse" }); //
            CommonButton.Add(new CommonButtonI() { Text = "Пользователи", Table = "USER",CanEdit = false }); //
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
    }
}

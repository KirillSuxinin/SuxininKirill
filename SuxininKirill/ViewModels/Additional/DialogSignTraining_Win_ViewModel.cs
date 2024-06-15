using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuxininKirill.ViewModels.Additional
{

    class DialogSignTraining_Win_ViewModel : INotifyPropertyChanged
    {
       public class T_Item : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

            private string _Name_Trainings;
            public string Name_Trainings
            {
                get
                {
                    return _Name_Trainings;
                }
                set
                {
                    _Name_Trainings = value;
                    OnPropertyChanged("Name_Trainings");
                }
            }

            private string _Date_Trainings;
            public string Date_Trainings
            {
                get
                {
                    return _Date_Trainings.Split(' ')[0];
                }
                set
                {
                    _Date_Trainings = value;
                    OnPropertyChanged(nameof(Date_Trainings));
                }
            }

            private string _ID_Trainings;
            public string ID_Trainings
            {
                get
                {
                    return _ID_Trainings;
                }
                set
                {
                    _ID_Trainings = value;
                    OnPropertyChanged(nameof(ID_Trainings));
                }
            }

            public static T_Item Load(DataRow row)
            {
                T_Item t = new T_Item()
                {
                    Date_Trainings = row["Date_Trainings"].ToString(),
                    ID_Trainings = row["ID_Trainings"].ToString(),
                    Name_Trainings = row["Name_Trainings"].ToString()
                };

                return t;
            }

        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(prop));

        public DialogSignTraining_Win_ViewModel(AdonisUI.Controls.AdonisWindow win)
        {
            string sql = "Select * From Trainings;";
            var command = ApplicationDB.Connection.CreateCommand();
            command.CommandText = sql;
            var table = new DataTable();
            var IRead = command.ExecuteReader();
            table.Load(IRead);
            Trainings = new ObservableCollection<T_Item>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                T_Item t = T_Item.Load(table.Rows[i]);
                Trainings.Add(t);
            }
            this.win = win;
            IRead.Close();
        }
        private AdonisUI.Controls.AdonisWindow win;
        public ObservableCollection<T_Item> Trainings { get; set; }

        public RelayCommand Cancel
        {
            get
            {
                return (new RelayCommand(obj =>
                {
                    if(obj != null)
                    {
                        if(obj is AdonisUI.Controls.AdonisWindow)
                        {
                            (obj as AdonisUI.Controls.AdonisWindow).DialogResult = false;
                            (obj as AdonisUI.Controls.AdonisWindow).Close();
                        }
                    }
                }));
            }
        }

        public RelayCommand OK
        {
            get
            {
                return (new RelayCommand(obj =>
                {
                    if(obj != null)
                    {
                        string id = obj.ToString();
                        (win as DialogSignTraining_Win).ID_Training = id;
                        win.DialogResult = true;
                        win.Close();
                    }
                }));
            }
        }

    }
}

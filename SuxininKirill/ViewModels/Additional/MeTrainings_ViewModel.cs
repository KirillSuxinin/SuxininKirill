using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SuxininKirill.ViewModels.Additional
{

    class MeTraining_Item : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(prop, new PropertyChangedEventArgs(prop));

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
                OnPropertyChanged(nameof(Name_Trainings));
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

        private string _Name_Instructor;

        public string Name_Instructor
        {
            get
            {
                return _Name_Instructor;
            }
            set
            {
                _Name_Instructor = value;
                OnPropertyChanged(nameof(Name_Instructor));
            }
        }

        private string _Qualification;
        public string Qualification
        {
            get
            {
                return _Qualification;
            }
            set
            {
                _Qualification = value;
                OnPropertyChanged(nameof(Qualification));
            }
        }

        private string _Id_Trainings;
        public string ID_Trainings
        {
            get
            {
                return _Id_Trainings;
            }
            set
            {
                _Id_Trainings = value;
                OnPropertyChanged("ID_Trainings");
            }
        }

        public static MeTraining_Item Load(DataRow loadrow)
        {
            MeTraining_Item it = new MeTraining_Item()
            {
                Name_Trainings = loadrow["Name_Trainings"].ToString(),
                Date_Trainings = loadrow["Date_Trainings"].ToString(),
                Name_Instructor = loadrow["Name_Instrucor"].ToString(),
                Qualification = loadrow["Qualification"].ToString(),
                ID_Trainings = loadrow["ID_Trainings"].ToString()
            };


            return it;
        }

    }

    class MeTrainings_ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(prop,new PropertyChangedEventArgs(prop));

        private RelayCommand _Back;
        public virtual RelayCommand Back
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

        public MeTrainings_ViewModel(AdonisUI.Controls.AdonisWindow win)
        {
            string memberKey = string.Empty;
            this.WIN = win;
            string sql_get_member_key = $"SELECT ID_Member FROM [Members] Where [Email] = \'{Properties.Settings.Default.Login_User}\'";

            {
                var com = ApplicationDB.Connection.CreateCommand();
                com.CommandText = sql_get_member_key;
                var iReader = com.ExecuteReader();
                DataTable table = new DataTable();
                table.Load(iReader);
                memberKey = table.Rows[0].ItemArray[0].ToString();
                iReader.Close();
            }
            this.member_key = memberKey;
            string sql = $"Select * From Trainings JOIN MembersTrainings ON (Trainings.ID_Trainings = MembersTrainings.ID_Trainings) JOIN Instructor ON(Instructor.ID_Instructor = Trainings.ID_Instructor) and MembersTrainings.ID_Member = {memberKey}";
            var command = ApplicationDB.Connection.CreateCommand();
            command.CommandText = sql;
            var IRead = command.ExecuteReader();
            var ITable = new DataTable();
            ITable.Load(IRead);
            MeTrainings = new ObservableCollection<MeTraining_Item>();
            for(int i =0; i < ITable.Rows.Count; i++)
            {
                MeTraining_Item it = MeTraining_Item.Load(ITable.Rows[i]);
                MeTrainings.Add(it);
            }
        }
        private string member_key;
        private AdonisUI.Controls.AdonisWindow WIN;
        public ObservableCollection<MeTraining_Item> MeTrainings { get; set; }

        public RelayCommand SignUp
        {
            get
            {
                return (new RelayCommand(obj =>
                {
                    if(obj != null)
                    {
                        if(obj is AdonisUI.Controls.AdonisWindow)
                        {
                            var @this = (obj as AdonisUI.Controls.AdonisWindow);


                            var dialog = new DialogSignTraining_Win();
                            dialog.Owner = @this;
                            if ((bool)dialog.ShowDialog())
                            {
                                string id_training = dialog.ID_Training;
                                string sql_insert = $"INSERT INTO MembersTrainings VALUES({id_training},{member_key});";
                                var command = ApplicationDB.Connection.CreateCommand();
                                command.CommandText = sql_insert;
                                var IREAD = command.ExecuteNonQuery();
                                if(IREAD > 0)
                                {
                                    @this.DataContext = new MeTrainings_ViewModel(WIN);
                                }
                            }


                            
                        }
                    }

                }));
            }
        }

        public RelayCommand UnSignUp
        {
            get
            {
                return (new RelayCommand(obj =>
                {
                    if(obj != null)
                    {
                        string ID_Trainings = obj as string;
                        string sql = $"DELETE FROM MembersTrainings Where ID_Trainings = \'{ID_Trainings}\' and ID_Member = \'{member_key}\';";
                        var command = ApplicationDB.Connection.CreateCommand();
                        command.CommandText = sql;
                        var IRES = command.ExecuteNonQuery();
                        if(IRES > 0)
                        {
                            WIN.DataContext = new MeTrainings_ViewModel(WIN);
                        }
                    }
                }));
            }
        }


    }
}

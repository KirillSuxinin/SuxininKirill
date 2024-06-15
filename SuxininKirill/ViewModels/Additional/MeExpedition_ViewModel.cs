using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SuxininKirill.ViewModels.Additional
{

    public class MeExpedition_Item : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private string _NameExpedition;
        public string NameExpedition
        {
            get
            {
                return _NameExpedition;
            }
            set
            {
                _NameExpedition = value;
                OnPropertyChanged(nameof(NameExpedition));
            }
        }


        private string _Date_Start;
        public string Date_Start
        {
            get
            {
                return _Date_Start;
            }
            set
            {
                _Date_Start = value.Split(' ')[0];
                OnPropertyChanged(nameof(Date_Start));
            }
        }

        private string _Date_Finish;
        public string Date_Finish
        {
            get
            {
                return _Date_Finish;
            }
            set
            {
                _Date_Finish = value.Split(' ')[0];
                OnPropertyChanged(nameof(Date_Finish));
            }
        }

        private string _Role;
        public string Role
        {
            get
            {
                return _Role;
            }
            set
            {
                _Role = value;
                OnPropertyChanged(nameof(Role));
            }
        }

        private string _Name_Route;
        public string Name_Route
        {
            get
            {
                return _Name_Route;
            }
            set
            {
                _Name_Route = value;
                OnPropertyChanged(nameof(Name_Route));
            }
        }

        private string _Description_Route;

        public string Description_Route
        {
            get
            {
                return _Description_Route;
            }
            set
            {
                _Description_Route = value;
                OnPropertyChanged(nameof(Description_Route));
            }
        }

        private string _Level_Dange;

        public string Level_Dange
        {
            get
            {
                return _Level_Dange;
            }
            set
            {
                _Level_Dange = value;
                OnPropertyChanged(nameof(Level_Dange));
            }
        }

        public SolidColorBrush Color_Level_Dange
        {
            get
            {
                if(Level_Dange == "1")
                {
                    return new SolidColorBrush(Colors.Green);
                }
                if(Level_Dange == "2")
                {
                    return new SolidColorBrush(Colors.Yellow);
                }
                if(Level_Dange == "3")
                {
                    return new SolidColorBrush(Colors.Red);
                }

                return new SolidColorBrush(Colors.Transparent);
            }
        }

        private string _Location_End;
        public string Location_End
        {
            get
            {
                return _Location_End;
            }
            set
            {
                _Location_End = value;
                OnPropertyChanged(nameof(Location_End));
            }
        }

        public static MeExpedition_Item Load(DataRow loadRow)
        {
            MeExpedition_Item item = new MeExpedition_Item()
            {
                NameExpedition = loadRow["Name_Expedition"].ToString(),
                Name_Route = loadRow["Name_Route"].ToString(),
                Date_Start = loadRow["Date_Start"].ToString(),
                Date_Finish = loadRow["Date_Finish"].ToString(),
                Role = loadRow["Role"].ToString(),
                Description_Route = loadRow["Description_Route"].ToString(),
                Level_Dange = loadRow["Level_Dange"].ToString(),
                Location_End = loadRow["Location_End"].ToString()
            };


            return item;
        }

    }

    internal class MeExpedition_ViewModel : INotifyPropertyChanged
    {
        //SQL: Select * From Expedition JOIN MembersExpedition ON (Expedition.ID_Expedition = MembersExpedition.ID_Expedition) JOIN [Route] ON (Expedition.ID_Route = Route.ID_Route)  and MembersExpedition.ID_Member = 10

        

        public MeExpedition_ViewModel()
        {
            string memberKey = string.Empty;
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


            string sql = $"Select * From Expedition JOIN MembersExpedition ON (Expedition.ID_Expedition = MembersExpedition.ID_Expedition) JOIN [Route] ON (Expedition.ID_Route = Route.ID_Route)  and MembersExpedition.ID_Member = {memberKey}";
            var command = ApplicationDB.Connection.CreateCommand();
            command.CommandText = sql;
            var IRead = command.ExecuteReader();
            DataTable Table = new DataTable();
            Table.Load(IRead);
            MeExpeditions = new ObservableCollection<MeExpedition_Item>();
            for(int i = 0; i < Table.Rows.Count; i++)
            {
                MeExpedition_Item nItem = MeExpedition_Item.Load(Table.Rows[i]);
                MeExpeditions.Add(nItem);

               
            }


            IRead.Close();

        }

        public ObservableCollection<MeExpedition_Item> MeExpeditions { get; set; }

        private MeExpedition_Item _SelectItem;


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

        public MeExpedition_Item SelectItem
        {
            get
            {
                return _SelectItem;
            }
            set
            {
                _SelectItem = value;
                OnPropertyChanged(nameof(SelectItem));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(propertyName, new PropertyChangedEventArgs(propertyName));

    }
}

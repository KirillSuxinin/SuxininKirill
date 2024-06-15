using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Xml;

namespace SuxininKirill.ViewModels
{
    public class MeItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public MeItem(string nameRow, string originalName, string valueRow)
        {
            NameRow = nameRow;
            ValueRow = valueRow;
            this.OriginalName = originalName;
        }

        private string _NameRow;
        private object _ValueRow;

        public string OriginalName;

        public string NameRow
        {
            get
            {
                return _NameRow;
            }
            set
            {
                _NameRow = value;
                OnPropertyChanged("NameRow");
            }
        }

        public object ValueRow
        {
            get
            {
                if (_ValueRow is DateTime)
                {
                    Debugger.Log(0, "Debug", "Get DATA!\n");
                    return ((DateTime)(_ValueRow)).ToString("dd.MM.yyyy H:mm:ss");
                }


                return _ValueRow;
            }
            set
            {
                _ValueRow = value;
                OnPropertyChanged("ValueRow");
            }
        }

        public string GetDataNoFormatMask() => _ValueRow.ToString();


        private bool _ReadOnly;
        public bool ReadOnly
        {
            get
            {
                return _ReadOnly;
            }
            set
            {
                _ReadOnly = value;
                OnPropertyChanged("ReadOnly");
            }
        }

    }

    public class Me_AccountWindow_ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public Me_AccountWindow_ViewModel(string NameTable,string NamePolyForLogin,string Login,string id)
        {
            if (ApplicationDB.Connection.State != System.Data.ConnectionState.Open)
            {
                AdonisUI.Controls.MessageBox.Show($"Ошибка подключение к Базе Данных. ({ApplicationDB.Connection.State.ToString()})", "Ошибка", AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Error);
            }

            CommonSource = new ObservableCollection<MeItem>();
            string sql_get_structure = $"Select COLUMN_NAME From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME = \'{NameTable}\'";
            if (ApplicationDB.Connection is MySqlConnector.MySqlConnection)
                sql_get_structure = $"SHOW COLUMNS FROM {NameTable}";
            var command = ApplicationDB.Connection.CreateCommand();
            command.CommandText = sql_get_structure;

            StructureTable = new DataTable();

            IDataReader reader = command.ExecuteReader();
            StructureTable.Load(reader);
            CommonSource = new ObservableCollection<MeItem>();
            this.NamePolyForLogin = NamePolyForLogin;
            string sql_get_data = $"Select * From [{NameTable}] Where [{NamePolyForLogin}] = \'{Login}\'";
            this.NameTable = NameTable;
            if (ApplicationDB.Connection is MySqlConnector.MySqlConnection)
                sql_get_data = sql_get_data.Replace("[", "").Replace("]", "");
            var req_com = ApplicationDB.Connection.CreateCommand();
            Table = new DataTable();
            req_com.CommandText = sql_get_data;
            IDataReader request = req_com.ExecuteReader();
            Table.Load(request);
           // table = dat_table;
           if(Table.Rows.Count <= 0)
            {
                MessageBox.Show("Данные логина были изменены!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning); return;
            }
            for(int i = 0; i < StructureTable.Rows.Count; i++)
            {
                var ms = new MeItem(StructureTable.Rows[i].ItemArray[0].ToString(), StructureTable.Rows[i].ItemArray[0].ToString(), Table.Rows[0].ItemArray[i].ToString());
                string name = ms.NameRow.ToString();
                if (ApplicationDB.LocalizationHelper.GetNameRusRow(name.Trim()) != null)
                    name = ApplicationDB.LocalizationHelper.GetNameRusRow(name.Trim());
                ms.NameRow = name;
                if (ms.OriginalName.ToUpper().Contains("id".ToUpper()))
                    ms.ReadOnly = true;
                CommonSource.Add(ms);
            }


            request.Close();
            reader.Close();



            //Запоминаем структура таблицы
          //  StructureTable = table;
        }
        private DataTable StructureTable;
        private DataTable Table;
        private string NameTable;
        private string NamePolyForLogin;
        private int Position;
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
        public ObservableCollection<MeItem> CommonSource { get; set; }

        private RelayCommand _Save;
        public RelayCommand Save
        {
            get
            {
                return _Save ?? (new RelayCommand(obj =>
                {


                    //формирование запроса
                    //Обновляем сначала главную таблицу
                    string sqlUpdate = $"Update [{NameTable}] Set ";


                    for (int i = 0; i < StructureTable.Rows.Count; i++)
                    {

                        sqlUpdate += StructureTable.Rows[i].ItemArray[0].ToString() + " = \'" + CommonSource.Where(z => z.OriginalName == StructureTable.Rows[i].ItemArray[0].ToString()).FirstOrDefault().ValueRow + "\', ";
                    }
                    sqlUpdate = sqlUpdate.TrimEnd(',', ' ');
                    sqlUpdate += " Where ";
                    for (int i = 0; i < StructureTable.Rows.Count; i++)
                    {
                        try
                        {
                            sqlUpdate += StructureTable.Rows[i].ItemArray[0].ToString() + " = \'" + Table.Rows[Position].ItemArray[i].ToString() + "\' and ";
                        }
                        catch
                        {

                        }
                    }
                    sqlUpdate = sqlUpdate.TrimEnd(',', ' ');
                    int index = sqlUpdate.LastIndexOf("and");
                    if (index > 0)
                        sqlUpdate = sqlUpdate.Remove(index, 3);
                    else
                        MessageBox.Show("Пожалуйста повторите запрос позже.", "Ошибка запроса. (Одновременный запрос)", MessageBoxButton.OK, MessageBoxImage.Error);
                    if (ApplicationDB.Connection is MySqlConnector.MySqlConnection)
                        sqlUpdate = sqlUpdate.Replace("[", "").Replace("]", "");
                    var command = ApplicationDB.Connection.CreateCommand();
                    command.CommandText = sqlUpdate;
                    try
                    {
                       int a = command.ExecuteNonQuery();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(""+ex,"ERROR",MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    //Обновляем таблицу USER чтобы данные совпадали т.к. привязка Login - Email|Name_Instructro
                    string updateUSER = $"UPDATE [USER] Set [Login] = ";
                    for(int i = 0; i < StructureTable.Rows.Count; i++)
                    {
                        if (StructureTable.Rows[i].ItemArray[0].ToString() == NamePolyForLogin)
                            updateUSER += $"\'{CommonSource.Where(z => z.OriginalName == StructureTable.Rows[i].ItemArray[0].ToString()).FirstOrDefault().ValueRow}\' Where [Login] = \'{Table.Rows[0].ItemArray[i].ToString()}\'";
                    }
                    
                    try
                    {
                        var userCommand = ApplicationDB.Connection.CreateCommand();
                        if (ApplicationDB.Connection is MySqlConnector.MySqlConnection)
                            updateUSER = updateUSER.Replace("[", "").Replace("]", "");
                        userCommand.CommandText = updateUSER;
                        userCommand.ExecuteNonQuery();

                    }
                    catch(Exception g)
                    {
                        MessageBox.Show("" + g, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);

                    }

                }));
            }
        }
    }
}

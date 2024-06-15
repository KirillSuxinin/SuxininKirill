using AdonisUI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using System.Windows.Documents;

namespace SuxininKirill.ViewModels
{
    public class CommonItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        public CommonItem() { }

        public CommonItem(string nameRow, string originalName, string valueRow)
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

        private RelayCommand _SetMultiLine;

        public virtual RelayCommand SetMultiLine
        {
            get
            {
                return _SetMultiLine ?? (_SetMultiLine = new RelayCommand(obj =>
                {
                    if (obj != null)
                    {
                        if (obj is TextBox)
                        {
                            TextBox textBox = (TextBox)obj;
                            textBox.AcceptsReturn = true;
                            textBox.TextWrapping = System.Windows.TextWrapping.Wrap;
                        }
                    }
                }));
            }
        }

        private System.Windows.Visibility _IsCalendar = System.Windows.Visibility.Hidden;

        public System.Windows.Visibility IsCalendar
        {
            get
            {
                return _IsCalendar;
            }
            set
            {
                _IsCalendar = value;
                OnPropertyChanged("IsCalendar");
            }
        }

        private RelayCommand _ClickCalendar;
        public RelayCommand ClickCalendar
        {
            get
            {
                return _ClickCalendar ?? (new RelayCommand(obj =>
                {
                    Debugger.Log(0, "Debug", $"{obj == null}\n");
                    if (obj != null)
                    {
                        if (obj is ContextMenu)
                        {
                            (obj as ContextMenu).Visibility = System.Windows.Visibility.Hidden;
                        }
                    }
                }));
            }
        }

    }

    internal class CommonWindowsForEdit_ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public CommonWindowsForEdit_ViewModel(string NameTable)
        {
            if (ApplicationDB.Connection.State != System.Data.ConnectionState.Open)
            {
                AdonisUI.Controls.MessageBox.Show($"Ошибка подключение к Базе Данных. ({ApplicationDB.Connection.State.ToString()})", "Ошибка", AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Error);
            }
            _NameTable = NameTable;
            CommonSource = new ObservableCollection<CommonItem>();


            string sql_get_structure = $"Select COLUMN_NAME From INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = \'{NameTable}\'";
            if (ApplicationDB.Connection is MySqlConnector.MySqlConnection)
                sql_get_structure = $"SHOW COLUMNS FROM {NameTable}";
            var command = ApplicationDB.Connection.CreateCommand();
            command.CommandText = sql_get_structure;

            DataTable table = new DataTable();

            IDataReader reader = command.ExecuteReader();
            table.Load(reader);
            //Запоминаем структура таблицы
            StructureTable = table;

            string sql_get_data = $"Select * From [{NameTable}]";
            if (ApplicationDB.Connection is MySqlConnector.MySqlConnection)
                sql_get_data = sql_get_data.Replace("[", "").Replace("]", "");
            var commandData = ApplicationDB.Connection.CreateCommand();
            commandData.CommandText = sql_get_data;
            IDataReader dataReader = commandData.ExecuteReader();
            DataTable tableData = new DataTable();
            tableData.Load(dataReader);
            Table = tableData;
            //Обработчик если в таблице нету информации
            if(Table.Rows.Count <= 0)
            {
                reader.Close();
                dataReader.Close();
                return;
            }
            OnPropertyChanged("Counter");
            //Иницилизация элементов на окно связка (TextBlock - TextBox)
            for (int i = 0; i < table.Rows.Count; i++)
            {
                string rowName = table.Rows[i].ItemArray[0].ToString().Trim();
                if (ApplicationDB.LocalizationHelper.GetNameRusRow(table.Rows[i].ItemArray[0].ToString().Trim()) != null)
                    rowName = ApplicationDB.LocalizationHelper.GetNameRusRow(table.Rows[i].ItemArray[0].ToString().Trim());

                var element = new CommonItem(rowName, table.Rows[i].ItemArray[0].ToString(), tableData.Rows[Position].ItemArray[i].ToString());
                element.ReadOnly = false;

                if (element.OriginalName.ToUpper().Contains("DATE"))
                    element.IsCalendar = System.Windows.Visibility.Visible;
                

                CommonSource.Add(element);
            }
            //Закрываем все потоки данных. В ином случае запросы будут не возможны
            reader.Close();
            dataReader.Close();
        }


        public string EditTitle
        {
            get
            {
                return "Редактирование";
            }
        }


        private string _NameTable;
        public string NameTable
        {
            get
            {
                return _NameTable;
            }
            set { }
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

        public string Counter
        {
            get
            {
                return $"{Position} - {Table.Rows.Count - 1}";
            }
            set { }
        }

        private float _OpacityMessage = 0.0f;
        public float OpacityMessage
        {
            get
            {
                return _OpacityMessage;
            }
            set
            {
                _OpacityMessage = value;
                OnPropertyChanged("OpacityMessage");
            }
        }

        private string _Message;
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                _Message = value;
                OnPropertyChanged("Message");
            }
        }

        private void SetData(int position)
        {
            if (position < 0 || position > Table.Rows.Count - 1)
            {
                AdonisUI.Controls.MessageBox.Show("POS: " + position + "\nТаблица пуста!","Warning!",MessageBoxButton.OK,MessageBoxImage.Warning);
                CommonSource.Clear();
                return;
            }

            CommonSource.Clear();
            for (int i = 0; i < StructureTable.Rows.Count; i++)
            {
                string rowName = StructureTable.Rows[i].ItemArray[0].ToString().Trim();
                if (ApplicationDB.LocalizationHelper.GetNameRusRow(StructureTable.Rows[i].ItemArray[0].ToString().Trim()) != null)
                    rowName = ApplicationDB.LocalizationHelper.GetNameRusRow(StructureTable.Rows[i].ItemArray[0].ToString().Trim());

                var element = new CommonItem(rowName, StructureTable.Rows[i].ItemArray[0].ToString(), Table.Rows[Position].ItemArray[i].ToString());
                element.ReadOnly = false;
                if (element.OriginalName.ToUpper().Contains("DATE"))
                    element.IsCalendar = System.Windows.Visibility.Visible;
                CommonSource.Add(element);
            }


        }

        private DataTable StructureTable;

        /// <summary>
        /// Представляет все связки (TextBlock - TextBox)
        /// </summary>
        public ObservableCollection<CommonItem> CommonSource { get; set; }
        /// <summary>
        /// Все данные из таблицы
        /// </summary>
        public DataTable Table;
        private int Position = 0;

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

        private RelayCommand _MoveLast;

        public RelayCommand MoveLast
        {
            get
            {
                return _MoveLast ?? (_MoveLast = new RelayCommand(obj =>
                {
                    Position = Table.Rows.Count - 1;
                    OnPropertyChanged("Counter");
                    SetData(Position);
                }));
            }
        }

        private RelayCommand _MoveFirst;
        public RelayCommand MoveFirst
        {
            get
            {
                return _MoveFirst ?? (new RelayCommand(obj =>
                {
                    Position = 0;
                    OnPropertyChanged("Counter");
                    SetData(Position);
                }));
            }
        }

        private RelayCommand _MovePreview;
        public RelayCommand MovePreview
        {
            get
            {
                return _MovePreview ?? (_MovePreview = new RelayCommand(obj =>
                {
                    if (Position > 0)
                    {
                        Position--;
                        OnPropertyChanged("Counter");
                        SetData(Position);
                    }
                }));
            }
        }

        private RelayCommand _MoveNext;
        public RelayCommand MoveNext
        {
            get
            {
                return _MoveNext ?? (_MoveNext = new RelayCommand(obj =>
                {
                    if (Position < Table.Rows.Count - 1)
                    {
                        Position++;
                        OnPropertyChanged("Counter");
                        SetData(Position);
                    }
                }));
            }
        }

        private RelayCommand _Save;
        public RelayCommand Save
        {
            get
            {
                return _Save ?? (new RelayCommand(async obj =>
                {
                    IsLoading = System.Windows.Visibility.Visible;
                    //Проверка данных
                    foreach (var v in CommonSource)
                    {
                        if(string.IsNullOrWhiteSpace(v.ValueRow.ToString()))
                        {
                            if (obj != null) { 
                                if(obj is AdonisUI.Controls.AdonisWindow)
                                {
                                    AdonisUI.Controls.MessageBox.Show((obj as AdonisUI.Controls.AdonisWindow), $"Поле \"{v.NameRow}\" пустое!", "Ошибка", AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Error);
                                }
                            }
                            return;
                        }
                    }

                    if(Table.Rows.Count <= 0)
                    {
                        string sql_to_insert = $"INSERT INTO [{NameTable}] VALUES( ";
                        if (ApplicationDB.Connection is MySqlConnector.MySqlConnection)
                            sql_to_insert = sql_to_insert.Replace("[", "").Replace("]", "");
                        for (int i = 0; i < CommonSource.Count; i++)
                        {
                            if (CommonSource[i].ValueRow is DateTime)
                                sql_to_insert += "\'" + CommonSource[i].GetDataNoFormatMask() + "\',";
                            else
                                sql_to_insert += "\'" + CommonSource[i].ValueRow + "\',";
                        }
                        sql_to_insert = sql_to_insert.TrimEnd(',', ' ');
                        sql_to_insert += " );";
                        var com = ApplicationDB.Connection.CreateCommand();
                        com.CommandText = sql_to_insert;
                        IsLoading = System.Windows.Visibility.Visible;
                        int res = com.ExecuteNonQuery();
                        if (res > 0)
                        {
                            Message = "Добавлено: " + res.ToString();

                            if (OpacityMessage <= 0)
                            {
                                await Task.Run(() =>
                                {
                                    OpacityMessage = 0f;
                                    while (OpacityMessage < 1f)
                                    {
                                        OpacityMessage += 0.1f;
                                        Task.Delay(20).Wait();
                                    }
                                    Task.Delay(1000).Wait();
                                    while (OpacityMessage > 0f)
                                    {
                                        OpacityMessage -= 0.1f;

                                        Task.Delay(300).Wait();
                                    }
                                });
                            }

                            string sql_get_data = $"Select * From [{NameTable}]";
                            if (ApplicationDB.Connection is MySqlConnector.MySqlConnection)
                                sql_get_data = sql_get_data.Replace("[", "").Replace("]", "");
                            var commandData = ApplicationDB.Connection.CreateCommand();
                            commandData.CommandText = sql_get_data;
                            IDataReader dataReader = commandData.ExecuteReader();
                            DataTable tableData = new DataTable();
                            tableData.Load(dataReader);
                            Table = tableData;
                            OnPropertyChanged("CommonSource");
                            OnPropertyChanged("Counter");
                            Position = 0;
                            dataReader.Close();
                        }
                        else
                        {
                            MessageBox.Show("Неизвестная ошибка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        IsLoading = System.Windows.Visibility.Hidden;
                        return;
                    }

                    //формирование запроса
                    string sqlUpdate = $"Update [{NameTable}] Set ";
                    if (ApplicationDB.Connection is MySqlConnector.MySqlConnection)
                        sqlUpdate = sqlUpdate.Replace("[", "").Replace("]", "");

                    for(int i = 0; i < StructureTable.Rows.Count; i++)
                    {
                        if (CommonSource.Where(z => z.OriginalName == StructureTable.Rows[i].ItemArray[0].ToString()).FirstOrDefault().IsCalendar == System.Windows.Visibility.Visible)
                        {
                            sqlUpdate += StructureTable.Rows[i].ItemArray[0].ToString() + " = \'" + CommonSource.Where(z => z.OriginalName == StructureTable.Rows[i].ItemArray[0].ToString()).FirstOrDefault().GetDataNoFormatMask() + "\', ";
                        }
                        else
                            sqlUpdate += StructureTable.Rows[i].ItemArray[0].ToString() + " = \'" + CommonSource.Where(z => z.OriginalName == StructureTable.Rows[i].ItemArray[0].ToString()).FirstOrDefault().ValueRow + "\', ";
                    }
                    sqlUpdate = sqlUpdate.TrimEnd(',', ' ');
                    sqlUpdate += " Where ";
                    for(int i = 0; i < StructureTable.Rows.Count; i++)
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
                    var command = ApplicationDB.Connection.CreateCommand();
                    command.CommandText = sqlUpdate;
                    try
                    {
                        int a = command.ExecuteNonQuery();
                        if(a > 0)
                        {
                            //MessageBox.Show(a.ToString());
                            Message = "Изменения: " + a.ToString();

                            if (OpacityMessage <= 0)
                            {
                                await Task.Run(() =>
                                {
                                    OpacityMessage = 0f;
                                    while (OpacityMessage < 1f)
                                    {
                                        OpacityMessage += 0.1f;
                                        Task.Delay(20).Wait();
                                    }
                                    Task.Delay(1000).Wait();
                                    while(OpacityMessage > 0f)
                                    {
                                        OpacityMessage -= 0.1f;
                                        
                                        Task.Delay(300).Wait();
                                    }
                                });
                            }

                            string sql_get_data = $"Select * From [{NameTable}]";
                            if (ApplicationDB.Connection is MySqlConnector.MySqlConnection)
                                sql_get_data = sql_get_data.Replace("[", "").Replace("]", "");
                            var commandData = ApplicationDB.Connection.CreateCommand();
                            commandData.CommandText = sql_get_data;
                            IDataReader dataReader = commandData.ExecuteReader();
                            DataTable tableData = new DataTable();
                            tableData.Load(dataReader);
                            Table = tableData;
                            OnPropertyChanged("Counter");
                            OnPropertyChanged("CommonSource");
                            dataReader.Close();
                            
                        }
                        else
                        {
                            string sql_to_insert = $"INSERT INTO [{NameTable}] VALUES( ";
                            if (ApplicationDB.Connection is MySqlConnector.MySqlConnection)
                                sql_to_insert = sql_to_insert.Replace("[", "").Replace("]", "");
                            for(int i = 0; i < CommonSource.Count; i++)
                            {
                                sql_to_insert += "\'" + CommonSource[i].ValueRow + "\',";
                            }
                            sql_to_insert = sql_to_insert.TrimEnd(',', ' ');
                            sql_to_insert += " );";
                            var com = ApplicationDB.Connection.CreateCommand();
                            com.CommandText = sql_to_insert;
                            int res = com.ExecuteNonQuery();
                            if(res > 0)
                            {
                                Message = "Добавлено: " + res.ToString();

                                if (OpacityMessage <= 0)
                                {
                                    await Task.Run(() =>
                                    {
                                        OpacityMessage = 0f;
                                        while (OpacityMessage < 1f)
                                        {
                                            OpacityMessage += 0.1f;
                                            Task.Delay(20).Wait();
                                        }
                                        Task.Delay(1000).Wait();
                                        while (OpacityMessage > 0f)
                                        {
                                            OpacityMessage -= 0.1f;

                                            Task.Delay(300).Wait();
                                        }
                                    });
                                }

                                string sql_get_data = $"Select * From [{NameTable}]";
                                if (ApplicationDB.Connection is MySqlConnector.MySqlConnection)
                                    sql_get_data = sql_get_data.Replace("[", "").Replace("]", "");
                                var commandData = ApplicationDB.Connection.CreateCommand();
                                commandData.CommandText = sql_get_data;
                                IDataReader dataReader = commandData.ExecuteReader();
                                DataTable tableData = new DataTable();
                                tableData.Load(dataReader);
                                Table = tableData;
                                OnPropertyChanged("Counter");
                                OnPropertyChanged("CommonSource");
                                dataReader.Close();
                            }
                            else
                            {
                                MessageBox.Show("Неизвестная ошибка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        IsLoading = System.Windows.Visibility.Hidden;
                        if (ex.Message.ToUpper().Contains("REFERENCE"))
                        {
                            //Обработка update для ограничения REFERENCE KEY
                            if(MessageBox.Show($"Ограничение ключа в таблице \'{GetNameTableByExceptionREFERENCE(ex.Message)}\'!","Ошибка",MessageBoxButton.OK,MessageBoxImage.Error) == MessageBoxResult.OK)
                            {

                            }
                        }
                        else
                        {
                            MessageBox.Show($"{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    IsLoading = System.Windows.Visibility.Hidden;
                }));
            }
        }

        private RelayCommand _Add;
        public RelayCommand Add
        {
            get
            {
                return _Add ?? (new RelayCommand(obj =>
                {
                    var @new = new object[] { };
                    if (Table.Rows.Count <= 0)
                    {
                        
                        for(int i = 0; i < StructureTable.Rows.Count; i++)
                        {
                            string rowName = StructureTable.Rows[i].ItemArray[0].ToString().Trim();
                            if (ApplicationDB.LocalizationHelper.GetNameRusRow(StructureTable.Rows[i].ItemArray[0].ToString().Trim()) != null)
                                rowName = ApplicationDB.LocalizationHelper.GetNameRusRow(StructureTable.Rows[i].ItemArray[0].ToString().Trim());
                            CommonSource.Add(new CommonItem(rowName, StructureTable.Rows[i].ItemArray[0].ToString().Trim(), ""));
                        }
                    }
                    else
                    {
                        @new = Table.Rows[Table.Rows.Count - 1].ItemArray;
                        @new[0] = ((int)Table.Rows[Table.Rows.Count - 1].ItemArray[0]) + 1;
                        for (int i = 1; i < @new.Length; i++)
                            if (@new[i] is string)
                                @new[i] = "";
                        Table.Rows.Add(@new);
                        Position = Table.Rows.Count - 1;
                        OnPropertyChanged("Counter");
                        CommonSource.Clear();
                        for (int i = 0; i < StructureTable.Rows.Count; i++)
                        {
                            string rowName = StructureTable.Rows[i].ItemArray[0].ToString().Trim();
                            if (ApplicationDB.LocalizationHelper.GetNameRusRow(StructureTable.Rows[i].ItemArray[0].ToString().Trim()) != null)
                                rowName = ApplicationDB.LocalizationHelper.GetNameRusRow(StructureTable.Rows[i].ItemArray[0].ToString().Trim());

                            var element = new CommonItem(rowName, StructureTable.Rows[i].ItemArray[0].ToString(), Table.Rows[Position].ItemArray[i].ToString());
                            element.ReadOnly = false;
                            CommonSource.Add(element);
                        }
                    }
                }));
            }
        }

        private RelayCommand _Remove;
        public RelayCommand Remove
        {
            get
            {
                return _Remove ?? (new RelayCommand(async obj =>
                {

                    if(MessageBox.Show("Вы уверены что хотите удалить эти данные?","Подтверждение",MessageBoxButton.YesNo,MessageBoxImage.Question) == MessageBoxResult.No)
                    {
                        return;
                    }

                    string firstColumn = CommonSource[0].OriginalName;
                    string sql_to_delete = $"DELETE FROM [{NameTable}] Where [{firstColumn}] = \'{Table.Rows[Position].ItemArray[0]}\'";
                    if (ApplicationDB.Connection is MySqlConnector.MySqlConnection)
                        sql_to_delete = sql_to_delete.Replace("[", "").Replace("]", "");
                    var command = ApplicationDB.Connection.CreateCommand();
                    command.CommandText = sql_to_delete;

                    IsLoading = System.Windows.Visibility.Visible;

                    int res = command.ExecuteNonQuery();

                    if (res > 0)
                    {
                        if (NameTable == "USER" && Table.Rows[Position].ItemArray[0].ToString() == Properties.Settings.Default.ID_User.ToString())
                        {
                            MessageBox.Show("Данный аккаунт больше не существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            if (obj != null)
                            {
                                (obj as AdonisUI.Controls.AdonisWindow).DialogResult = true;
                                (obj as AdonisUI.Controls.AdonisWindow).Tag = "EXITACC";
                                (obj as AdonisUI.Controls.AdonisWindow).Close();
                            }
                        }


                        Message = "Удалено: " + res.ToString();

                        if (OpacityMessage <= 0)
                        {
                            await Task.Run(() =>
                            {
                                OpacityMessage = 0f;
                                while (OpacityMessage < 1f)
                                {
                                    OpacityMessage += 0.1f;
                                    Task.Delay(20).Wait();
                                }
                                Task.Delay(1000).Wait();
                                while (OpacityMessage > 0f)
                                {
                                    OpacityMessage -= 0.1f;

                                    Task.Delay(300).Wait();
                                }
                            });
                        }
                        string sql_get_data = $"Select * From [{NameTable}]";
                        if (ApplicationDB.Connection is MySqlConnector.MySqlConnection)
                            sql_get_data = sql_get_data.Replace("[", "").Replace("]", "");
                        var commandData = ApplicationDB.Connection.CreateCommand();
                        commandData.CommandText = sql_get_data;
                        IsLoading = System.Windows.Visibility.Visible;
                        IDataReader dataReader = commandData.ExecuteReader();
                        DataTable tableData = new DataTable();
                        tableData.Load(dataReader);
                        
                        Table = tableData;
                        if (Position > 0)
                            Position--;
                        else
                            Position = 0;
                        OnPropertyChanged("Counter");
                        dataReader.Close();
                        CommonSource.Clear();

                        SetData(Position);
                    }
                    IsLoading = System.Windows.Visibility.Hidden;
                }));
            }
        }

        public RelayCommand SortViewer
        {
            get
            {
                return (new RelayCommand(obj =>
                {
                    //TODO: SORTED WINDOW
                    if(obj != null)
                    {
                        if(obj is AdonisUI.Controls.AdonisWindow)
                        {
                            var @this = (obj as AdonisUI.Controls.AdonisWindow);
                            TableCommonSortedWindow win = new TableCommonSortedWindow(NameTable);

                            win.WindowState = @this.WindowState;
                            win.Owner = @this;
                            @this.Hide();
                            if ((bool)win.ShowDialog())
                            {
                                @this.ShowDialog();
                            }
                            else if (!(bool)win.DialogResult)
                            {
                                if (win.Tag != null && win.Tag.ToString() == "EXITACC")
                                {
                                    @this.ShowDialog();
                                }
                                else
                                    System.Windows.Application.Current.Shutdown(0);
                            }
                        }
                    }
                }));
            }
        }

        private string GetNameTableByExceptionREFERENCE(string MessageException)
        {
            return MessageException.Split(' ')[14].Trim(',', '\"').Replace("dbo.", "");
        }
    }
}

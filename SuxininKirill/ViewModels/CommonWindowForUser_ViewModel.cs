using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace SuxininKirill.ViewModels
{



    public class CommonWindowForUser_ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="NameTable">Название таблицы</param>
        public CommonWindowForUser_ViewModel(string NameTable)
        {
            //Проверка подключение к бд
            if (ApplicationDB.Connection.State != System.Data.ConnectionState.Open)
            {
                AdonisUI.Controls.MessageBox.Show($"Ошибка подключение к Базе Данных. ({ApplicationDB.Connection.State.ToString()})", "Ошибка", AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Error);
            }
            //Установка свойств
            _NameTable = NameTable;
            CommonSource = new ObservableCollection<CommonItem>();

            //Запрос на получения структуры таблицы
            string sql_get_structure = $"Select COLUMN_NAME From INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = \'{NameTable}\'";
            //Запрос на получения структуры таблицы (Для Linux)
            if (ApplicationDB.Connection is MySqlConnector.MySqlConnection)
                sql_get_structure = $"SHOW COLUMNS FROM {NameTable}";
            var command = ApplicationDB.Connection.CreateCommand();
            command.CommandText = sql_get_structure;
            //
            DataTable table = new DataTable();

            IDataReader reader = command.ExecuteReader();
            table.Load(reader);
            //Запоминаем структура таблицы
            StructureTable = table;

            //Запрос на получения данных
            string sql_get_data = $"Select * From [{NameTable}]";
            //Удаляем [] для Linux
            if (ApplicationDB.Connection is MySqlConnector.MySqlConnection)
                sql_get_data = sql_get_data.Replace("[", "").Replace("]", "");
            var commandData = ApplicationDB.Connection.CreateCommand();
            commandData.CommandText = sql_get_data;
            IDataReader dataReader = commandData.ExecuteReader();
            DataTable tableData = new DataTable();
            tableData.Load(dataReader);
            Table = tableData;
            //Обновляем счётчик на фронте
            OnPropertyChanged("Counter");
            //Иницилизация элементов на окно связка (TextBlock - TextBox)
            if (tableData.Rows.Count <= 0)
            {
                reader.Close();
                dataReader.Close();

                System.Windows.MessageBox.Show("Данные отсутствуют.", "Ошибка данных", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);


                return;

            }
            //Работа со строками данных
            for (int i = 0; i < table.Rows.Count; i++)
            {
                //Получаем название столбца
                string rowName = table.Rows[i].ItemArray[0].ToString().Trim();
                //Перевод названия стоблца
                if (ApplicationDB.LocalizationHelper.GetNameRusRow(table.Rows[i].ItemArray[0].ToString().Trim()) != null)
                    rowName = ApplicationDB.LocalizationHelper.GetNameRusRow(table.Rows[i].ItemArray[0].ToString().Trim());
                //Добавление элемента к нашему свойству которое привязано к главной форме
                var element = new CommonItem(rowName, table.Rows[i].ItemArray[0].ToString(), tableData.Rows[Position].ItemArray[i].ToString());

                element.ReadOnly = true;
                CommonSource.Add(element);
            }
            //Закрываем все потоки данных. В ином случае запросы будут не возможны
            reader.Close();
            dataReader.Close();
        }

        private string _NameTable;
        /// <summary>
        /// Название таблицы
        /// </summary>
        public string NameTable
        {
            get
            {
                return _NameTable;
            }
            set { }
        }

        /// <summary>
        /// Счётчик 
        /// </summary>
        public string Counter
        {
            get
            {
                return $"{Position} - {Table.Rows.Count - 1}";
            }
            set { }
        }
        /// <summary>
        /// Установка данных в зависимости от позиции
        /// </summary>
        /// <param name="position">Позиция строки</param>
        private void SetData(int position)
        {
            //Проверка данных
            if (position < 0 || position > Table.Rows.Count - 1)
            {
                AdonisUI.Controls.MessageBox.Show("Данные отсутствуют", "Ошибка данных", AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Error);
                return;
            }
            //Удаляем старые данные
            CommonSource.Clear();
            
            for (int i = 0; i < StructureTable.Rows.Count; i++)
            {
                string rowName = StructureTable.Rows[i].ItemArray[0].ToString().Trim();
                if (ApplicationDB.LocalizationHelper.GetNameRusRow(StructureTable.Rows[i].ItemArray[0].ToString().Trim()) != null)
                    rowName = ApplicationDB.LocalizationHelper.GetNameRusRow(StructureTable.Rows[i].ItemArray[0].ToString().Trim());

                var element = new CommonItem(rowName, StructureTable.Rows[i].ItemArray[0].ToString(), Table.Rows[Position].ItemArray[i].ToString());
                element.ReadOnly = true;
                CommonSource.Add(element);
            }


        }
        /// <summary>
        /// Структура таблицы
        /// </summary>
        private DataTable StructureTable;

        /// <summary>
        /// Представляет все связки (TextBlock - TextBox)
        /// </summary>
        public ObservableCollection<CommonItem> CommonSource { get; set; }
        /// <summary>
        /// Все данные из таблицы
        /// </summary>
        public DataTable Table;
        /// <summary>
        /// Позиция строки
        /// </summary>
        private int Position = 0;

        private RelayCommand _Back;
        /// <summary>
        /// Команда "Назад"
        /// </summary>
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
        /// <summary>
        /// Команда "След. запис"
        /// </summary>
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
    }
}

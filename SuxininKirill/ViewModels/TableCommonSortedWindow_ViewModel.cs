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
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml;

namespace SuxininKirill.ViewModels
{


    internal class TableCommonSortedWindow_ViewModel : INotifyPropertyChanged
    {
        public TableCommonSortedWindow_ViewModel(string TableName,DataGrid meGrid,Window win)
        {
          
            string sql_get_data = $"Select * From [{TableName}]";
            if (ApplicationDB.Connection is MySqlConnector.MySqlConnection)
                sql_get_data = sql_get_data.Replace("[", "").Replace("]", "");
            var commandData = ApplicationDB.Connection.CreateCommand();
            commandData.CommandText = sql_get_data;
            IDataReader dataReader = commandData.ExecuteReader();
            DataTable tableData = new DataTable();
            tableData.Load(dataReader);
            if (tableData.Rows.Count <= 0)
            {
                dataReader.Close();

                System.Windows.MessageBox.Show("Данные отсутствуют.", "Ошибка данных", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

                return;
            }
            //meGrid.ItemsSource = tableData.AsDataView();
            MeTable = tableData;


            SortElement = new ObservableCollection<DataColumn>();
            CategoryFind = new ObservableCollection<string>();
            CategoryFind.Add("*");
            foreach (DataColumn v in tableData.Columns)
            {
                CategoryFind.Add(v.ColumnName);
                SortElement.Add(v);
            }
            //Закрываем все потоки данных. В ином случае запросы будут не возможны

            dataReader.Close();
            Table = TableName;
            this.Grid = meGrid;
            this.Window = win;
        }

        private Window Window;
        private DataGrid Grid;
        private string Table;
        private DataTable MeTable;
        public DataView MeTableGrid
        {
            get
            {
                return MeTable.AsDataView();
            }
            set { }
        }
        public string MeTitle
        {
            get
            {
                return $"Сортировка {Table}";
            }
        }


        public ObservableCollection<string> CategoryFind { get; set; }
        public ObservableCollection<DataColumn> SortElement { get; set; }

        private int _SelectIndex = -1;
        public int SelectIndex
        {
            get
            {
                return _SelectIndex;
            }
            set
            {
                _SelectIndex = value;
                OnPropertyChanged(nameof(SelectIndex));
            }
        }


        private System.Windows.Visibility _IsLoading = Visibility.Hidden;
        public System.Windows.Visibility IsLoading
        {
            get
            {
                return _IsLoading;
            }
            set
            {
                _IsLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        private bool _ByASC;
        public bool ByASC
        {
            get
            {
                return _ByASC;
            }
            set
            {
                _ByASC = value;
                OnPropertyChanged(nameof(ByASC));
            }
        }

        private bool _ByDES;
        public bool ByDES
        {
            get
            {
                return _ByDES;
            }
            set
            {
                _ByDES = value;
                OnPropertyChanged(nameof(ByDES));
            }
        }

        private string _CategorySelect;
        public string CategorySelect
        {
            get
            {
                return _CategorySelect;
            }
            set
            {
                _CategorySelect = value;
                OnPropertyChanged(nameof(CategorySelect));
            }
        }


        public RelayCommand InvokeSORT
        {
            get
            {
                return (new RelayCommand(obj =>
                {
                    if(obj != null)
                    {
                        if(obj is DataGrid)
                        {
                            var dataGrid = (obj as DataGrid);
                            SortDataGrid(dataGrid, SelectIndex, ByASC ? ListSortDirection.Ascending : ListSortDirection.Descending);
                        }
                    }

                }));
            }
        }

        private void SortDataGrid(DataGrid dataGrid, int columnIndex = 0, ListSortDirection sortDirection = ListSortDirection.Ascending)
        {
            if (columnIndex == -1)
                columnIndex = 0;
            var column = dataGrid.Columns[columnIndex];
            // Clear current sort descriptions
            dataGrid.Items.SortDescriptions.Clear();
            // Add the new sort description
            dataGrid.Items.SortDescriptions.Add(new SortDescription(column.SortMemberPath, sortDirection));
            // Apply sort
            foreach (var col in dataGrid.Columns)
            {
                col.SortDirection = null;
            }
            column.SortDirection = sortDirection;
            // Refresh items to display sort
            dataGrid.Items.Refresh();
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

        public RelayCommand ChangeTable
        {
            get
            {
                return (new RelayCommand(obj =>
                {
                    Button sender = (obj as Button);
                    sender.ContextMenu = new ContextMenu();

                    List<string> tables = new List<string>()
                    {
                        "Trainings",
                        "Expedition",
                        "USER",
                        "Members",
                        "Equipment",
                        "EventsClub",
                        "HealthProblems",
                        "Instructor",
                        "MembersExpedition",
                        "Mount",
                        "Route",
                        "RentEquipment",
                        "Warehouse",
                        "MembersTrainings",
                        "MembersEvent",
                        "Climbing"
                    };
                    sender.ContextMenu.Items.Clear();
                    for(int i = 0; i < tables.Count; i++)
                    {
                        MenuItem it = new MenuItem();
                        it.Header = tables[i];
                        it.Click += (_sender, e) =>
                        {
                            this.Window.DataContext = new TableCommonSortedWindow_ViewModel((_sender as MenuItem).Header.ToString(), this.Grid, this.Window);
                            OnPropertyChanged("MeTableGrid");
                        };
                        sender.ContextMenu.Items.Add(it);
                    }

                    sender.ContextMenu.IsOpen = true;
                }));
            }
        }

        public RelayCommand InvokeSqlFind
        {
            get
            {
                return (new RelayCommand(obj =>
                {
                    if(obj != null)
                    {
                        string category = CategorySelect;
                        string text = obj as string;
                        if(!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(category))
                        {
                            IsLoading = Visibility.Visible;
                            string sql = $"Select * From [{Table}] Where ";
                            if(category == "*")
                            {
                                for(int i = 0; i < MeTable.Columns.Count; i++)
                                {
                                    if (i != MeTable.Columns.Count - 1)
                                        sql += $"{MeTable.Columns[i].ColumnName} LIKE '%{text}%' OR ";
                                    else
                                        sql += $"{MeTable.Columns[i].ColumnName} LIKE '%{text}%'";
                                }
                            }
                            else
                            {
                                sql += $"{category} LIKE '%{text}%'";
                            }
                            if (ApplicationDB.Connection is MySqlConnector.MySqlConnection)
                                sql = sql.Replace("[", "").Replace("]", "");
                            var ime = ApplicationDB.Connection.CreateCommand();
                            ime.CommandText = sql;
                            var tablle = new DataTable();
                            var reader = ime.ExecuteReader();
                            tablle.Load(reader);
                            Grid.ItemsSource = tablle.AsDataView();
                            OnPropertyChanged("MeTableGrid");
                            IsLoading = Visibility.Hidden;
                            reader.Close();
                        }
                    }
                }));
            }
        }

        public RelayCommand InvokeSeeAll
        {
            get
            {
                return (new RelayCommand(obj =>
                {
                    Grid.ItemsSource = MeTable.AsDataView();
                }));
            }
        }
        /// <summary>
        /// Команда для спец. копирования таблицы (символ указан в CommandParametr)
        /// </summary>
        public RelayCommand CopyIn
        {
            get
            {
                return (new RelayCommand(obj =>
                {
                    string symbolSplit = obj.ToString();

                    string textForBuffer = "";
                    foreach(var vC in Grid.SelectedItems)
                    {
                        DataRow row = (vC as DataRowView).Row;
                        string text = "";
                        for(int i = 0; i < row.ItemArray.Length; i++)
                        {
                            text += row.ItemArray[i].ToString() + symbolSplit;
                        }
                        textForBuffer += text.TrimEnd(symbolSplit.ToCharArray()) + Environment.NewLine;
                        //Debugger.Log(0, "Debug", text + "\n");
                    }


                    if (!string.IsNullOrWhiteSpace(textForBuffer))
                        Clipboard.SetText(textForBuffer);

                }));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(prop, new PropertyChangedEventArgs(prop));
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using Newtonsoft.Json;
using MySqlConnector;

namespace SuxininKirill
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {


            base.OnExit(e);
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            int checkSum = 0;
            foreach (var v in Application.Current.Windows)
            {
                if ((v as Window).Visibility == Visibility.Hidden)
                {
                    checkSum++;
                }
            }
            System.IO.File.AppendAllText($"OnDeactivated_Log",$"{System.DateTime.Now}{Environment.NewLine}CheckSUM: {checkSum}{Environment.NewLine} из {Application.Current.Windows.Count}");

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Json.JsonSqlSetting.RootSettingServer dataConnections = JsonConvert.DeserializeObject<Json.JsonSqlSetting.RootSettingServer>(File.ReadAllText($"SqlConnectDataSet.json"));
            
            if(dataConnections.Server != null)
            {
                if (dataConnections.Server.Invoke == "Windows")
                {
                    if (dataConnections.Server.Windows != null)
                    {
                        if(dataConnections.Server.Windows.DataSource == null || dataConnections.Server.Windows.InitialCatalog == null || dataConnections.Server.Windows.IntegratedSecurity == null)
                        {
                            AdonisUI.Controls.MessageBox.Show("Don't found MySql string connect for Windows!", "Error", AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Error);
                        }


                        System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder()
                        {
                            DataSource = dataConnections.Server.Windows.DataSource,
                            InitialCatalog = dataConnections.Server.Windows.InitialCatalog,
                            IntegratedSecurity = bool.Parse(dataConnections.Server.Windows.IntegratedSecurity)
                        };
                        ApplicationDB.Connection = new System.Data.SqlClient.SqlConnection(builder.ConnectionString);

                    }
                }
                else if(dataConnections.Server.Invoke == "Ubuntu/Linux")
                {
                    if(dataConnections.Server.UbuntuLinux != null)
                    {
                        if(dataConnections.Server.UbuntuLinux.Server == null || dataConnections.Server.UbuntuLinux.Password == null || dataConnections.Server.UbuntuLinux.IdUser == null)
                        {
                            AdonisUI.Controls.MessageBox.Show("Don't found MySql string connect for Ubuntu/Linux!", "Error", AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Error);
                        }

                        ApplicationDB.Connection = new MySqlConnection($"Server={dataConnections.Server.UbuntuLinux.Server};User Id={dataConnections.Server.UbuntuLinux.IdUser};Password={dataConnections.Server.UbuntuLinux.Password};Database={dataConnections.Server.UbuntuLinux.Database}");

                    }
                }
            }

            base.OnStartup(e);
        }
    }
}

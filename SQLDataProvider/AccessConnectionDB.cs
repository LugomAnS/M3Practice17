using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataProvider
{
    public class AccessConnectionDB
    {
        private OleDbConnectionStringBuilder connectionString;
        private OleDbDataAdapter accessData;
        public string AccessConnectionsString { get => connectionString.ConnectionString.ToString(); }
        
        private OleDbConnection dbConnection;

        public event Action<string> ConnectionState;

        #region Запросы к БД
        private string selectAll = @"SELECT * FROM Purchases";

        #endregion


        // Provider=Microsoft.ACE.OLEDB.12.0;
        // Data Source=F:\SkillboxUnityCourse\Module3\Practice16\AdoNet\AdoNet\bin\Debug\net7.0-windows\AccessDB.mdb
        // Provider=Microsoft.ACE.OLEDB.12.0;
        // Data Source = F:\SkillboxUnityCourse\Module3\Practice16\AdoNet\AdoNet\bin\Debug\net7.0-windows\AccessDB.accdb;
        // Persist Security Info=True

        public AccessConnectionDB()
        {
            connectionString = new OleDbConnectionStringBuilder()
            {
                Provider = "Microsoft.ACE.OLEDB.12.0",
                DataSource = @"AccessDB.accdb",
                PersistSecurityInfo = true
            };

            accessData = new OleDbDataAdapter();
        }

        private void SelectCurentClientPurchasesCommand(string email, OleDbConnection con)
        {
            string sql = @$"SELECT * FROM Purchases WHERE Purchases.eMail = '{email}'";

            accessData.SelectCommand = new OleDbCommand(sql, con);
        }

        private void AddNewPurchaseCommand(OleDbConnection con)
        {
            string sql = @"INSERT INTO Purchases (eMail, itemCode, itemName)
                                  VALUES (@eMail, @itemCode, @itemName);";

            accessData.InsertCommand = new OleDbCommand(sql, con);
            accessData.InsertCommand.Parameters.Add("@eMail", OleDbType.WChar, 20, "eMail");
            accessData.InsertCommand.Parameters.Add("@itemCode", OleDbType.Integer, 4, "itemCode");
            accessData.InsertCommand.Parameters.Add("@itemName", OleDbType.WChar, 4, "itemName");
        }

        #region проверка соединения
        public void OpenConnection()
        {
            using (dbConnection = new OleDbConnection(connectionString.ToString()))
            {
                try
                {
                    dbConnection.Open();
                    ConnectionState?.Invoke(dbConnection.State.ToString());
                    Thread.Sleep(2000);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }

            ConnectionState?.Invoke(dbConnection.State.ToString());
        }

        public async void OpenConnectionAsync()
        {
            await Task.Factory.StartNew(OpenConnection);
        }

        #endregion

        #region Получить все записи покупок
        public DataTable GetAllPurchases()
        {
            DataTable dt = new DataTable();
            using (dbConnection = new OleDbConnection(connectionString.ConnectionString))
            {
                accessData.SelectCommand = new OleDbCommand(this.selectAll, dbConnection);
                accessData.Fill(dt);
            }
            return dt;
        }

        #endregion

        #region Получить записи о покупках по клиенту
        public DataTable GetClientPurchases(string clientEmail)
        {
            DataTable dt = new DataTable();
            using (dbConnection = new OleDbConnection(connectionString.ConnectionString))
            {
                SelectCurentClientPurchasesCommand(clientEmail, dbConnection);
                accessData.Fill(dt);
            }

            return dt;
        }

        #endregion

        #region Добавить запись
        public void AddNewPurchase(object purchases)
        {
            using (dbConnection = new OleDbConnection(connectionString.ConnectionString))
            {
                AddNewPurchaseCommand(dbConnection);
                accessData.Update(purchases as DataTable);
            }
        }

        #endregion
    }
}

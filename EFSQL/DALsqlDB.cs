using EFSQL.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFSQL
{
    public class DALsqlDB
    {
        private readonly string connection;
        public string Connection
        {
            get => connection;
        }
        private EntityFrameworkTestDBContext sqlDB;

        public event Action<string> SqlConnectionStateChange;

        public DALsqlDB()
        {
            sqlDB = new EntityFrameworkTestDBContext();
            sqlDB.Clients.Load();

            connection = sqlDB.Database.GetDbConnection().ConnectionString;
            sqlDB.Database.GetDbConnection().StateChange += DALsqlDB_StateChange;
        }

        private void DALsqlDB_StateChange(object sender, System.Data.StateChangeEventArgs e)
        {
            SqlConnectionStateChange?.Invoke(e.CurrentState.ToString());
        }

        public DbSet<Clients> CetAllClients()
        {
            return sqlDB.Clients;
        }

        public void SaveChanges()
        {
            sqlDB.SaveChanges();
        }

        public void DeleteClient(Clients clientTodelete)
        {
            sqlDB.Clients.Remove(clientTodelete);
            sqlDB.SaveChanges();
        }

        public void AddNewClient(Clients newClient)
        {
            sqlDB.Add(newClient);
            sqlDB.SaveChanges();
        }
    }
}

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
        public DALsqlDB()
        {
            sqlDB = new EntityFrameworkTestDBContext();
            sqlDB.Clients.Load();

            connection = sqlDB.Database.GetDbConnection().ConnectionString;
        }

        public DbSet<Clients> CetAllClients()
        {
            return sqlDB.Clients;
        }

        public void SaveChanges()
        {
            sqlDB.SaveChanges();
        }
    }
}

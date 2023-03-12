using EFAccess.Contexts;
using EFAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EFAccess
{
    public class DALAccessDB
    {
        private readonly string connection;
        public string ConnectionString
        {
            get => connection;
        }

        public event Action<string> ConnectionStatus;

        private ModelContext accessDB;

        public DALAccessDB()
        {
            accessDB = new ModelContext();
            accessDB.Purchases.Load();
            connection = accessDB.Database.GetDbConnection().ConnectionString;

            accessDB.Database.GetDbConnection().StateChange += ConnectionsStatusChanged;
        }
        private void ConnectionsStatusChanged(object sender, System.Data.StateChangeEventArgs e)
        {
            ConnectionStatus?.Invoke(e.CurrentState.ToString());
        }

        public DbSet<Purchases> GetAllPurchases()
        {
            return accessDB.Purchases;
        }

        public void AddNewPurchase(Purchases purchases)
        {
            accessDB.Purchases.Add(purchases);
            accessDB.SaveChanges();
        }
    }
}

using EFAccess.Contexts;
using EFAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFAccess
{
    public class DALAccessDB
    {
        private ModelContext accessDB;

        public DALAccessDB()
        {
            accessDB = new ModelContext();
            accessDB.Purchases.Load();
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

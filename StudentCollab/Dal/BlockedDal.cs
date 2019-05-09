using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using StudentCollab.Models;

namespace StudentCollab.Dal
{
    public class BlockedDal : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<BlockedDal>(null);
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Blocked>().ToTable("dbBlocked");
        }

        public DbSet<Blocked> Blockeds { get; set; }
    }
}
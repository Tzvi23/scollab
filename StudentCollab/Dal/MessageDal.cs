using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using StudentCollab.Models;

namespace StudentCollab.Dal
{
    public class MessageDal : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<MessageDal>(null);
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Message>().ToTable("dbMsg");
        }

        public DbSet<Message> Messages { get; set; }
    }
}
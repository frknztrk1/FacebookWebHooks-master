using FacebookWebHooks.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacebookWebHooks
{
    public class AppDbContext : DbContext
    {
        public DbSet<FacebookAdsLog> FacebookAdsLogs;
        public DbSet<FacebookAdsFormData> FacebookAdsFormDatas;

        private IDbContextTransaction _transaction;

        public void BeginTransaction()
        {
            _transaction = Database.BeginTransaction();
        }

        public void Commit()
        {
            try
            {
                SaveChanges();
                _transaction.Commit();
            }
            finally
            {
                _transaction.Dispose();
            }
        }

        public void Rollback()
        {
            _transaction.Rollback();
            _transaction.Dispose();
        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FacebookAdsLog>().ToTable("FacebookAdsLogs");
            modelBuilder.Entity<FacebookAdsFormData>().ToTable("FacebookAdsFormDatas");


            base.OnModelCreating(modelBuilder);
        }
    }
}

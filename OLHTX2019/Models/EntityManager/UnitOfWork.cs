using OLHTX2019.Models.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OLHTX2019.Models.EntityManager
{
    public class UnitOfWork : IDisposable
    {
        private OLHTX2019Entities context = new OLHTX2019Entities();

        public void SetProxyCreationEnabled(bool SetTo)
        {
            //https://stackoverflow.com/questions/26206288/entity-to-json-error-a-circular-reference-was-detected-while-serializing-an-ob
            context.Configuration.ProxyCreationEnabled = SetTo;
        }

        #region Repository Members
        
        private GenericRepository<Registration> registration;
        private GenericRepository<Setting> setting;
        private GenericRepository<Administrator> administrator;
        private GenericRepository<AuditLog> auditLog;

        #endregion

        #region Repository Get Function
        public GenericRepository<Registration> RegistrationRepository
        {
            get
            {
                return this.registration ?? new GenericRepository<Registration>(context);
            }
        }
        public GenericRepository<Setting> SettingRepository
        {
            get
            {
                return this.setting ?? new GenericRepository<Setting>(context);
            }
        }

        public GenericRepository<Administrator> AdministratorRepository
        {
            get
            {
                return this.administrator ?? new GenericRepository<Administrator>(context);
            }
        }
        public GenericRepository<AuditLog> AuditLog
        {
            get
            {
                return this.auditLog ?? new GenericRepository<AuditLog>(context);
            }
        }


        #endregion

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
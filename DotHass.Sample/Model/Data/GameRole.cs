using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
using DotHass.Repository;
using DotHass.Repository.DB;
using DotHass.Repository.Entity;

namespace DotHass.Sample.Model.Data
{
    [RepositoryAction(DbType = typeof(DataDbRepository<GameRole>))]
    [Serializable]
    public class GameRole
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [RepositoryPrimaryKey(Order = 0)]
        public long Id { get; set; }


        public string RoleName { get; set; }

        public string HeadId { get; set; }


        public Int32 Money { get; set; }


        public Int32 Gold { get; set; }


        public Int32 Experience { get; set; }


        [JsonIgnore]
        private readonly object _lockFlag = new object();
        public void ModifyLocked(Action action)
        {
            if (action != null)
            {
                try
                {
                    EnterLock();
                    action();
                }
                finally
                {
                    ExitLock();
                }
            }
        }

        public void EnterLock()
        {
            Monitor.Enter(_lockFlag);
        }

        /// <summary>
        /// Exit lock
        /// </summary>
        public void ExitLock()
        {
            Monitor.Exit(_lockFlag);
        }
    }

}

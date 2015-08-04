using Orkidea.Pioneer.DataAccesssEF;
using Orkidea.Pioneer.Entities;
using Orkidea.Pioneer.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orkidea.Pioneer.Business
{
    public static class ActivityLogBiz
    {
        /*CRUD Users*/

        /// <summary>
        /// Retrieve User list without parameters
        /// </summary>
        /// <returns></returns>
        public static List<ActivityLog> GetActivityLogList()
        {

            List<ActivityLog> lstAL = new List<ActivityLog>();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstAL = ctx.ActivityLog.ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstAL;
        }

        public static List<ActivityLog> GetActivityLogList(string accion)
        {

            List<ActivityLog> lstAL = new List<ActivityLog>();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstAL = ctx.ActivityLog.Where(x => x.accion == accion).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstAL;
        }

        /// <summary>
        /// Retrieve User information based in the primary key
        /// </summary>
        /// <param name="activityLog"></param>
        /// <returns></returns>       
        public static ActivityLog GetActivityLogByKey(ActivityLog activityLog)
        {
            ActivityLog oAL = new ActivityLog();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oAL = ctx.ActivityLog.Where(x => x.id.Equals(activityLog.id)).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return oAL;
        }

        /// <summary>
        /// Create or update a User
        /// </summary>
        /// <param name="activityLog"></param>
        public static void SaveActivityLog(ActivityLog activityLog)
        {
            try
            {
                using (var ctx = new pioneerEntities())
                {
                    //verify if the User exists
                    ActivityLog oAL = GetActivityLogByKey(activityLog);

                    if (oAL != null)
                    {
                        // if exists then edit 
                        ctx.ActivityLog.Attach(oAL);
                        EntityFrameworkHelper.EnumeratePropertyDifferences(oAL, activityLog);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create
                        if (activityLog.accion.Length >= 50)
                            activityLog.accion = activityLog.accion.Substring(0, 50);

                        ctx.ActivityLog.Add(activityLog);
                        ctx.SaveChanges();
                    }
                }

            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Delete a User
        /// </summary>
        /// <param name="UserTarget"></param>
        public static void DeleteActivityLog(ActivityLog activityLog)
        {
            try
            {
                using (var ctx = new pioneerEntities())
                {
                    //verify if the User exists
                    ActivityLog oAL = GetActivityLogByKey(activityLog);

                    if (oAL != null)
                    {
                        // if exists then edit 
                        ctx.ActivityLog.Attach(oAL);
                        ctx.ActivityLog.Remove(oAL);
                        ctx.SaveChanges();
                    }
                }

            }
            catch (Exception ex) { throw ex; }
        }
    }
}

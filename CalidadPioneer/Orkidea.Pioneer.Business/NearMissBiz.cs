using Orkidea.Pioneer.DataAccesssEF;
using Orkidea.Pioneer.Entities;
using Orkidea.Pioneer.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orkidea.Pioneer.Business
{
    public class NearMissBiz
    {
        /*CRUD NearMiss*/

        /// <summary>
        /// Retrieve process list 
        /// </summary>
        /// <param name="schoolTarget"></param>
        /// <returns></returns>
        public List<NearMiss> GetNearMissList()
        {

            List<NearMiss> lstNearMiss = new List<NearMiss>();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstNearMiss = ctx.NearMiss.ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstNearMiss;
        }

        public List<NearMiss> GetNearMissList(bool estado)
        {

            List<NearMiss> lstNearMiss = new List<NearMiss>();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    if (estado)
                        lstNearMiss = ctx.NearMiss.Where(x => x.idUsuarioCierra == null).ToList();
                    else
                        lstNearMiss = ctx.NearMiss.Where(x => x.idUsuarioCierra != null).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstNearMiss;
        }

        public List<NearMiss> GetNearMissList(bool estado, DateTime desde, DateTime hasta)
        {

            List<NearMiss> lstNearMiss = new List<NearMiss>();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    if (estado)
                        lstNearMiss = ctx.NearMiss.Where(x => x.idUsuarioCierra == null && x.fechaApertura >= desde && x.fechaApertura<= hasta).ToList();
                    else
                        lstNearMiss = ctx.NearMiss.Where(x => x.idUsuarioCierra != null && x.fechaApertura >= desde && x.fechaApertura<= hasta).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstNearMiss;
        }

        public IEnumerable GetReporteGenerico(bool estado)
        {
            List<ReporteGenericoNearMissHallazgo> lsRep = new List<ReporteGenericoNearMissHallazgo>();
            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    if (estado)
                        lsRep = ctx.Database.SqlQuery<ReporteGenericoNearMissHallazgo>("Select * from vwNearMissAbiertas").ToList();
                    else
                        lsRep = ctx.Database.SqlQuery<ReporteGenericoNearMissHallazgo>("Select * from vwNearMissCerradas").ToList();
                }
            }
            catch (Exception) { }

            return lsRep;
        }

        /// <summary>
        /// Retrieve course information based in the primary key
        /// </summary>
        /// <param name="nearMissTarget"></param>
        /// <returns></returns>
        public NearMiss GetNearMissbyKey(NearMiss nearMissTarget)
        {
            NearMiss oNearMiss = new NearMiss();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oNearMiss =
                        ctx.NearMiss.Where(x => x.id.Equals(nearMissTarget.id)).FirstOrDefault();
                }
            }
            catch (Exception) { }

            return oNearMiss;
        }

        /// <summary>
        /// Create or update a course
        /// </summary>
        /// <param name="nearMiss"></param>
        public int SaveNearMiss(NearMiss nearMiss)
        {

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    NearMiss oNearMiss = GetNearMissbyKey(nearMiss);

                    //verify if the student exists
                    if (oNearMiss != null)
                    {
                        // if exists then edit 
                        ctx.NearMiss.Attach(oNearMiss);
                        EntityFrameworkHelper.EnumeratePropertyDifferences(oNearMiss, nearMiss);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create
                        ctx.NearMiss.Add(nearMiss);
                        ctx.SaveChanges();
                    }

                    return nearMiss.id;
                }

            }
            catch (DbEntityValidationException e)
            {
                StringBuilder oError = new StringBuilder();
                foreach (var eve in e.EntityValidationErrors)
                {
                    oError.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State));

                    foreach (var ve in eve.ValidationErrors)
                    {
                        oError.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage));
                    }
                }
                string msg = oError.ToString();
                throw new Exception(msg);
            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Delete a course
        /// </summary>
        /// <param name="nearMissTarget"></param>
        public void DeleteNearMiss(NearMiss nearMissTarget)
        {
            try
            {
                using (var ctx = new pioneerEntities())
                {
                    //verify if the school exists
                    NearMiss oNearMiss = GetNearMissbyKey(nearMissTarget);

                    if (oNearMiss != null)
                    {
                        // if exists then edit 
                        ctx.NearMiss.Attach(oNearMiss);
                        ctx.NearMiss.Remove(oNearMiss);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }
    }
}

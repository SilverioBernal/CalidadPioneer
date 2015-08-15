using Orkidea.Pioneer.DataAccesssEF;
using Orkidea.Pioneer.Entities;
using Orkidea.Pioneer.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orkidea.Pioneer.Business
{
    public class DrillBiz
    {
        /*CRUD Drill*/

        /// <summary>
        /// Retrieve process list 
        /// </summary>
        /// <param name="schoolTarget"></param>
        /// <returns></returns>
        public List<Drill> GetDrillList()
        {

            List<Drill> lstDrill = new List<Drill>();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstDrill = ctx.Drill.ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstDrill;
        }

        /// <summary>
        /// Retrieve course information based in the primary key
        /// </summary>
        /// <param name="drillTarget"></param>
        /// <returns></returns>
        public Drill GetDrillbyKey(Drill drillTarget)
        {
            Drill oDrill = new Drill();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oDrill =
                        ctx.Drill.Where(x => x.id.Equals(drillTarget.id)).FirstOrDefault();
                }
            }
            catch (Exception) { }

            return oDrill;
        }

        /// <summary>
        /// Create or update a course
        /// </summary>
        /// <param name="drill"></param>
        public void SaveDrill(Drill drill)
        {

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    Drill oDrill = GetDrillbyKey(drill);
                    //verify if the student exists
                    
                    if (oDrill!=null)
                    {
                        // if exists then edit 
                        ctx.Drill.Attach(oDrill);
                        EntityFrameworkHelper.EnumeratePropertyDifferences(oDrill, drill);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create
                        ctx.Drill.Add(drill);
                        ctx.SaveChanges();
                    }
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
        /// <param name="drillTarget"></param>
        public void DeleteDrill(Drill drillTarget)
        {
            try
            {
                using (var ctx = new pioneerEntities())
                {
                    //verify if the school exists
                    Drill oDrill = GetDrillbyKey(drillTarget);

                    if (oDrill != null)
                    {
                        // if exists then edit 
                        ctx.Drill.Attach(oDrill);
                        ctx.Drill.Remove(oDrill);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }
    }
}

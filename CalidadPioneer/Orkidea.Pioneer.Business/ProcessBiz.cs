
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
    public class ProcessBiz
    {
        /*CRUD Process*/

        /// <summary>
        /// Retrieve process list 
        /// </summary>
        /// <param name="schoolTarget"></param>
        /// <returns></returns>
        public List<Process> GetProcessList()
        {

            List<Process> lstProcess = new List<Process>();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstProcess = ctx.Process.ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstProcess;
        }

        /// <summary>
        /// Retrieve course information based in the primary key
        /// </summary>
        /// <param name="ProcessTarget"></param>
        /// <returns></returns>
        public Process GetProcessbyKey(Process ProcessTarget)
        {
            Process oProcess = new Process();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oProcess =
                        ctx.Process.Where(x => x.id.Equals(ProcessTarget.id)).FirstOrDefault();
                }
            }
            catch (Exception ) { }

            return oProcess;
        }

        /// <summary>
        /// Create or update a course
        /// </summary>
        /// <param name="Process"></param>
        public void SaveProcess(Process Process)
        {

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    //verify if the student exists
                    if (Process.id != 0)
                    {
                        Process oProcess = GetProcessbyKey(Process);

                        // if exists then edit 
                        ctx.Process.Attach(oProcess);
                        EntityFrameworkHelper.EnumeratePropertyDifferences(oProcess, Process);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create
                        ctx.Process.Add(Process);
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
        /// <param name="ProcessTarget"></param>
        public void DeleteProcess(Process ProcessTarget)
        {
            try
            {
                using (var ctx = new pioneerEntities())
                {
                    //verify if the school exists
                    Process oProcess = GetProcessbyKey(ProcessTarget);

                    if (oProcess != null)
                    {
                        // if exists then edit 
                        ctx.Process.Attach(oProcess);
                        ctx.Process.Remove(oProcess);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }
    }
}

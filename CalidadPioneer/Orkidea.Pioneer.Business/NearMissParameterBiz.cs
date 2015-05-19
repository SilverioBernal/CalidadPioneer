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
    public class NearMissParameterBiz
    {
        /*CRUD NearMissParameter*/

        /// <summary>
        /// Retrieve process list 
        /// </summary>
        /// <param name="schoolTarget"></param>
        /// <returns></returns>
        public List<NearMissParameter> GetNearMissParameterList()
        {

            List<NearMissParameter> lstNearMissParameter = new List<NearMissParameter>();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstNearMissParameter = ctx.NearMissParameter.ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstNearMissParameter;
        }

        /// <summary>
        /// Retrieve course information based in the primary key
        /// </summary>
        /// <param name="nearMissParameterTarget"></param>
        /// <returns></returns>
        public NearMissParameter GetNearMissParameterbyKey(NearMissParameter nearMissParameterTarget)
        {
            NearMissParameter oNearMissParameter = new NearMissParameter();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oNearMissParameter =
                        ctx.NearMissParameter.Where(x => x.parametro== nearMissParameterTarget.parametro).FirstOrDefault();
                }
            }
            catch (Exception) { }

            return oNearMissParameter;
        }

        /// <summary>
        /// Create or update a course
        /// </summary>
        /// <param name="nearMissParameter"></param>
        public void SaveNearMissParameter(NearMissParameter nearMissParameter)
        {

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    //verify if the student exists
                    if (!string.IsNullOrEmpty(nearMissParameter.parametro))
                    {
                        NearMissParameter oNearMissParameter = GetNearMissParameterbyKey(nearMissParameter);

                        // if exists then edit 
                        ctx.NearMissParameter.Attach(oNearMissParameter);
                        EntityFrameworkHelper.EnumeratePropertyDifferences(oNearMissParameter, nearMissParameter);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create
                        //ctx.NearMissParameter.Add(NearMissParameter);
                        //ctx.SaveChanges();
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
        /// <param name="nearMissParameterTarget"></param>
        public void DeleteNearMissParameter(NearMissParameter nearMissParameterTarget)
        {
            try
            {
                using (var ctx = new pioneerEntities())
                {
                    //verify if the school exists
                    NearMissParameter oNearMissParameter = GetNearMissParameterbyKey(nearMissParameterTarget);

                    if (oNearMissParameter != null)
                    {
                        // if exists then edit 
                        ctx.NearMissParameter.Attach(oNearMissParameter);
                        ctx.NearMissParameter.Remove(oNearMissParameter);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }
    }
}

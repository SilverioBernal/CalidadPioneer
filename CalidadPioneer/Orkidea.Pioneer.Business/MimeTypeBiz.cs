using Orkidea.Pioneer.DataAccesssEF;
using Orkidea.Pioneer.Entities;
using Orkidea.Pioneer.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orkidea.Pioneer.Business
{
    public static class MimeTypeBiz
    {
        /*CRUD mimetypes*/

        /// <summary>
        /// Retrieve mimetypes list without parameters
        /// </summary>
        /// <returns></returns>
        public static List<mimetype> GetmimetypeList()
        {

            List<mimetype> lstmimetype = new List<mimetype>();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstmimetype = ctx.mimetype.ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstmimetype;
        }

        /// <summary>
        /// Retrieve mimetype information based in the primary key
        /// </summary>
        /// <param name="mimetypeTarget"></param>
        /// <returns></returns>
        public static mimetype GetMimeType(int id)
        {
            mimetype omimetype = new mimetype();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    omimetype = ctx.mimetype.Where(x => x.Id.Equals(id)).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return omimetype;
        }

        /// <summary>
        /// Retrieve mimetype information based in the primary key
        /// </summary>
        /// <param name="mimetypeTarget"></param>
        /// <returns></returns>
        public static mimetype GetMimeType(string extension)
        {
            mimetype omimetype = new mimetype();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    omimetype = ctx.mimetype.Where(x => x.extension == extension).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return omimetype;
        }

        /// <summary>
        /// Create or update a mimetype
        /// </summary>
        /// <param name="mimetypeTarget"></param>
        public static void Savemimetype(mimetype mimetypeTarget)
        {

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    //verify if the mimetype exists
                    mimetype omimetype = GetMimeType(mimetypeTarget.Id);

                    if (omimetype != null)
                    {
                        // if exists then edit 
                        ctx.mimetype.Attach(omimetype);
                        EntityFrameworkHelper.EnumeratePropertyDifferences(omimetype, mimetypeTarget);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create
                        ctx.mimetype.Add(mimetypeTarget);
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
        /// Delete a mimetype
        /// </summary>
        /// <param name="mimetypeTarget"></param>
        public static void Deletemimetype(mimetype mimetypeTarget)
        {
            try
            {
                using (var ctx = new pioneerEntities())
                {
                    //verify if the school exists
                    mimetype omimetype = GetMimeType(mimetypeTarget.Id);

                    if (omimetype != null)
                    {
                        // if exists then edit 
                        ctx.mimetype.Attach(omimetype);
                        ctx.mimetype.Remove(omimetype);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("REFERENCE constraint"))
                {
                    throw new Exception("No se puede eliminar este grado porque existe información asociada a este.");
                }
            }
            catch (Exception ex) { throw ex; }
        }
    }
}

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
    public class FileUploadBiz
    {
        /// <summary>
        /// Retrieve FileUploads list without parameters
        /// </summary>
        /// <returns></returns>
        public List<FileUpload> GetFileUploadList()
        {

            List<FileUpload> lstFileUpload = new List<FileUpload>();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstFileUpload = ctx.FileUpload.ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstFileUpload;
        }

        public List<FileUpload> GetFileUploadList(bool active)
        {

            List<FileUpload> lstFileUpload = new List<FileUpload>();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstFileUpload = ctx.FileUpload.ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstFileUpload;
        }

        /// <summary>
        /// Retrieve FileUpload information based in the primary key
        /// </summary>
        /// <param name="FileUploadTarget"></param>
        /// <returns></returns>
        public FileUpload GetFileUploadByKey(FileUpload FileUploadTarget)
        {
            FileUpload oFileUpload = new FileUpload();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oFileUpload = ctx.FileUpload.Where(x => x.id.Equals(FileUploadTarget.id)).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return oFileUpload;
        }

        /// <summary>
        /// Create or update a FileUpload
        /// </summary>
        /// <param name="FileUploadTarget"></param>
        public void SaveFileUpload(FileUpload FileUploadTarget)
        {

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    //verify if the FileUpload exists
                    FileUpload oFileUpload = GetFileUploadByKey(FileUploadTarget);

                    if (oFileUpload != null)
                    {
                        // if exists then edit 
                        ctx.FileUpload.Attach(oFileUpload);
                        EntityFrameworkHelper.EnumeratePropertyDifferences(oFileUpload, FileUploadTarget);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create
                        ctx.FileUpload.Add(FileUploadTarget);
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
        /// Delete a FileUpload
        /// </summary>
        /// <param name="FileUploadTarget"></param>
        public void DeleteFileUpload(FileUpload FileUploadTarget)
        {
            try
            {
                using (var ctx = new pioneerEntities())
                {
                    //verify if the school exists
                    FileUpload oFileUpload = GetFileUploadByKey(FileUploadTarget);

                    if (oFileUpload != null)
                    {
                        // if exists then edit 
                        ctx.FileUpload.Attach(oFileUpload);
                        ctx.FileUpload.Remove(oFileUpload);
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

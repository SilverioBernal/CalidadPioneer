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
    public class ProcessDocumentBiz
    {
        /*CRUD ProcessDocument*/

        /// <summary>
        /// Retrieve process list 
        /// </summary>
        /// <param name="schoolTarget"></param>
        /// <returns></returns>
        public List<ProcessDocument> GetProcessDocumentList()
        {

            List<ProcessDocument> lstProcess = new List<ProcessDocument>();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstProcess = ctx.ProcessDocument.ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstProcess;
        }

        /// <summary>
        /// Retrieve process list 
        /// </summary>
        /// <param name="schoolTarget"></param>
        /// <returns></returns>
        public List<ProcessDocument> GetProcessDocumentListByProcessDocumentType(ProcessDocument processDocument)
        {

            List<ProcessDocument> lstProcess = new List<ProcessDocument>();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstProcess = ctx.ProcessDocument.Where(x => x.idProceso.Equals(processDocument.idProceso) && x.idTipoDocumento.Equals(processDocument.idTipoDocumento)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstProcess;
        }

        /// <summary>
        /// Retrieve process list 
        /// </summary>
        /// <param name="schoolTarget"></param>
        /// <returns></returns>
        public List<DocumentType> GetDocumentTypeByProcess(Process process)
        {
            DocumentTypeBiz docTypeBiz = new DocumentTypeBiz();
            List<DocumentType> lstDocumentType = new List<DocumentType>();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    var processTypeDocument = (from x in ctx.ProcessDocument where x.idProceso.Equals(process.id) select x.idTipoDocumento).Distinct().ToList();

                    foreach (int item in processTypeDocument)
                    {
                        lstDocumentType.Add(docTypeBiz.GetDocumentTypeByKey(new DocumentType() { id = item }));
                    }
                }
            }
            catch (Exception ex) { throw ex; }

            return lstDocumentType;
        }

        /// <summary>
        /// Retrieve course information based in the primary key
        /// </summary>
        /// <param name="processDocumentTarget"></param>
        /// <returns></returns>
        public ProcessDocument GetProcessDocumentbyKey(ProcessDocument processDocumentTarget)
        {
            ProcessDocument oProcess = new ProcessDocument();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oProcess =
                        ctx.ProcessDocument.Where(x => x.id.Equals(processDocumentTarget.id)).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return oProcess;
        }

        /// <summary>
        /// Create or update a course
        /// </summary>
        /// <param name="ProcessDocument"></param>
        public void SaveProcessDocument(ProcessDocument ProcessDocument)
        {

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    //verify if the student exists
                    ProcessDocument oProcess = GetProcessDocumentbyKey(ProcessDocument);

                    if (oProcess != null)
                    {
                        // if exists then edit 
                        ctx.ProcessDocument.Attach(oProcess);
                        EntityFrameworkHelper.EnumeratePropertyDifferences(oProcess, ProcessDocument);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create
                        ctx.ProcessDocument.Add(ProcessDocument);
                        ctx.SaveChanges();
                    }
                }

            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Delete a course
        /// </summary>
        /// <param name="ProcessTarget"></param>
        public void DeleteProcessDocument(ProcessDocument ProcessTarget)
        {
            try
            {
                using (var ctx = new pioneerEntities())
                {
                    //verify if the school exists
                    ProcessDocument oProcess = GetProcessDocumentbyKey(ProcessTarget);

                    if (oProcess != null)
                    {
                        // if exists then edit 
                        ctx.ProcessDocument.Attach(oProcess);
                        ctx.ProcessDocument.Remove(oProcess);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }
    }
}

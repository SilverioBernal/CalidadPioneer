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
    public class ProjectDocumentBiz
    {
        /*CRUD ProjectDocument*/

        /// <summary>
        /// Retrieve process list 
        /// </summary>
        /// <param name="schoolTarget"></param>
        /// <returns></returns>
        public List<ProjectDocument> GetProjectDocumentList()
        {

            List<ProjectDocument> lstProcess = new List<ProjectDocument>();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstProcess = ctx.ProjectDocument.ToList();
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
        public List<ProjectDocument> GetProjectDocumentListByProject(Project project)
        {

            List<ProjectDocument> lstProcess = new List<ProjectDocument>();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstProcess = ctx.ProjectDocument.Where(x => x.idProyecto.Equals(project.id)).ToList();
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
        public List<DocumentType> GetProjectDocumentTypeListByProject(Project project)
        {
            DocumentTypeBiz dpBiz = new DocumentTypeBiz();

            List<DocumentType> lstTD = dpBiz.GetDocumentTypeList();
            List<DocumentType> lstProcessDocTypess = new List<DocumentType>();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    var DocTypes = ctx.ProjectDocument.Where(x => x.idProyecto.Equals(project.id)).Select(x => x.idTipoDocumento).Distinct().ToList();

                    foreach (var item in DocTypes)
                    {
                        lstProcessDocTypess.Add(lstTD.Where(x => x.id.Equals((int)item)).FirstOrDefault());
                    }
                }
            }
            catch (Exception ex) { throw ex; }

            return lstProcessDocTypess;
        }

        /// <summary>
        /// Retrieve course information based in the primary key
        /// </summary>
        /// <param name="projectDocumentTarget"></param>
        /// <returns></returns>
        public ProjectDocument GetProjectDocumentbyKey(ProjectDocument projectDocumentTarget)
        {
            ProjectDocument oProcess = new ProjectDocument();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oProcess =
                        ctx.ProjectDocument.Where(x => x.id.Equals(projectDocumentTarget.id)).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return oProcess;
        }

        /// <summary>
        /// Create or update a course
        /// </summary>
        /// <param name="projectDocument"></param>
        public void SaveProjectDocument(ProjectDocument projectDocument)
        {

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    //verify if the student exists
                    ProjectDocument oProcess = GetProjectDocumentbyKey(projectDocument);

                    if (oProcess != null)
                    {
                        // if exists then edit 
                        ctx.ProjectDocument.Attach(oProcess);
                        EntityFrameworkHelper.EnumeratePropertyDifferences(oProcess, projectDocument);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create
                        ctx.ProjectDocument.Add(projectDocument);
                        ctx.SaveChanges();
                    }
                }

            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Delete a course
        /// </summary>
        /// <param name="ProjectTarget"></param>
        public void DeleteProjectDocument(ProjectDocument ProjectTarget)
        {
            try
            {
                using (var ctx = new pioneerEntities())
                {
                    //verify if the school exists
                    ProjectDocument oProcess = GetProjectDocumentbyKey(ProjectTarget);

                    if (oProcess != null)
                    {
                        // if exists then edit 
                        ctx.ProjectDocument.Attach(oProcess);
                        ctx.ProjectDocument.Remove(oProcess);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }
    }
}

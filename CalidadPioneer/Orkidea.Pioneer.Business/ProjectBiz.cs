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
    public class ProjectBiz
    {
        /*CRUD Project*/

        /// <summary>
        /// Retrieve process list 
        /// </summary>
        /// <param name="schoolTarget"></param>
        /// <returns></returns>
        public List<Project> GetProjectList()
        {

            List<Project> lstProject = new List<Project>();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstProject = ctx.Project.ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstProject;
        }

        /// <summary>
        /// Retrieve course information based in the primary key
        /// </summary>
        /// <param name="ProjectTarget"></param>
        /// <returns></returns>
        public Project GetProjectbyKey(Project ProjectTarget)
        {
            Project oProject = new Project();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oProject =
                        ctx.Project.Where(x => x.id.Equals(ProjectTarget.id)).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return oProject;
        }

        /// <summary>
        /// Create or update a course
        /// </summary>
        /// <param name="Project"></param>
        public void SaveProject(Project Project)
        {

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    //verify if the student exists
                    Project oProject = GetProjectbyKey(Project);

                    if (oProject != null)
                    {
                        // if exists then edit 
                        ctx.Project.Attach(oProject);
                        EntityFrameworkHelper.EnumeratePropertyDifferences(oProject, Project);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create
                        ctx.Project.Add(Project);
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
        public void DeleteProject(Project ProjectTarget)
        {
            try
            {
                using (var ctx = new pioneerEntities())
                {
                    //verify if the school exists
                    Project oProject = GetProjectbyKey(ProjectTarget);

                    if (oProject != null)
                    {
                        // if exists then edit 
                        ctx.Project.Attach(oProject);
                        ctx.Project.Remove(oProject);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }
    }
}

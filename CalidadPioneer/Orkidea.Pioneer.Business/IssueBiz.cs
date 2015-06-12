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
    public class IssueBiz
    {
        /*CRUD Issue*/

        /// <summary>
        /// Retrieve process list 
        /// </summary>
        /// <param name="schoolTarget"></param>
        /// <returns></returns>
        public List<Issue> GetIssueList()
        {

            List<Issue> lstIssue = new List<Issue>();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstIssue = ctx.Issue.ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstIssue;
        }

        public List<Issue> GetIssueList(bool estado)
        {

            List<Issue> lstIssue = new List<Issue>();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    if (estado)
                        lstIssue = ctx.Issue.Where(x => x.idUsuarioCierre == null).ToList();
                    else
                        lstIssue = ctx.Issue.Where(x => x.idUsuarioCierre != null).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstIssue;
        }

        public List<Issue> GetIssueList(bool estado, DateTime desde, DateTime hasta)
        {

            List<Issue> lstIssue = new List<Issue>();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    if (estado)
                        lstIssue = ctx.Issue.Where(x => x.idUsuarioCierre == null && x.fechaCreacion >= desde && x.fechaCreacion <= hasta).ToList();
                    else
                        lstIssue = ctx.Issue.Where(x => x.idUsuarioCierre != null && x.fechaCreacion >= desde && x.fechaCreacion <= hasta).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstIssue;
        }

        /// <summary>
        /// Retrieve course information based in the primary key
        /// </summary>
        /// <param name="issueTarget"></param>
        /// <returns></returns>
        public Issue GetIssuebyKey(Issue issueTarget)
        {
            Issue oIssue = new Issue();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oIssue =
                        ctx.Issue.Where(x => x.id.Equals(issueTarget.id)).FirstOrDefault();
                }
            }
            catch (Exception) { }

            return oIssue;
        }

        /// <summary>
        /// Create or update a course
        /// </summary>
        /// <param name="issue"></param>
        public int SaveIssue(Issue issue)
        {

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    Issue oIssue = GetIssuebyKey(issue);

                    //verify if the student exists
                    if (oIssue != null)
                    {
                        // if exists then edit 
                        ctx.Issue.Attach(oIssue);
                        EntityFrameworkHelper.EnumeratePropertyDifferences(oIssue, issue);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create
                        ctx.Issue.Add(issue);
                        ctx.SaveChanges();
                    }

                    return issue.id;
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
        /// <param name="issueTarget"></param>
        public void DeleteIssue(Issue issueTarget)
        {
            try
            {
                using (var ctx = new pioneerEntities())
                {
                    //verify if the school exists
                    Issue oIssue = GetIssuebyKey(issueTarget);

                    if (oIssue != null)
                    {
                        // if exists then edit 
                        ctx.Issue.Attach(oIssue);
                        ctx.Issue.Remove(oIssue);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }




        public List<IssueDetail> getIssueDetailList(int id)
        {

            List<IssueDetail> lsIssueDetail = new List<IssueDetail>();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lsIssueDetail = ctx.IssueDetail.Where(x => x.idHallazgo.Equals(id)).OrderBy(x => x.fechaRegistro).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lsIssueDetail;
        }

        public void SaveIssueDetail(IssueDetail issueDetail)
        {

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    // else create
                    ctx.IssueDetail.Add(issueDetail);
                    ctx.SaveChanges();
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

        public void DeleteIssueDetail(int issueDetailId)
        {
            try
            {
                using (var ctx = new pioneerEntities())
                {
                    //verify if the school exists
                    IssueDetail issue = ctx.IssueDetail.Where(x => x.id.Equals(issueDetailId)).First();

                    if (issue != null)
                    {
                        // if exists then edit 
                        ctx.IssueDetail.Attach(issue);
                        ctx.IssueDetail.Remove(issue);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
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
                        lsRep = ctx.Database.SqlQuery<ReporteGenericoNearMissHallazgo>("Select * from vwIssuesAbiertas").ToList();
                    else
                        lsRep = ctx.Database.SqlQuery<ReporteGenericoNearMissHallazgo>("Select * from vwIssuesCerradas").ToList();
                }
            }
            catch (Exception) { }

            return lsRep;
        }

        public IEnumerable GetReporteGenerico()
        {
            List<ReporteComparativoIssue> lsRep = new List<ReporteComparativoIssue>();
            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    lsRep = ctx.Database.SqlQuery<ReporteComparativoIssue>("Select * from vwIssuesAbiertasCerradas").ToList();
                }
            }
            catch (Exception) { }

            return lsRep;
        }

        public IEnumerable GetReporteFuentes()
        {
            List<ReporteGenericoNearMissHallazgo> lsRep = new List<ReporteGenericoNearMissHallazgo>();
            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lsRep = ctx.Database.SqlQuery<ReporteGenericoNearMissHallazgo>("select fuente rig, count(1) cantidad from issue group by fuente").ToList();                    
                }
            }
            catch (Exception) { }

            return lsRep;
        }
    }
}

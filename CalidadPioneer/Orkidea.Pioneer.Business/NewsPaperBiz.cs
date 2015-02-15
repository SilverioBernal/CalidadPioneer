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
    public class NewsPaperBiz
    {
        /*CRUD DocumentType*/

        /// <summary>
        /// Retrieve DocumentType list 
        /// </summary>
        /// <param name="schoolTarget"></param>
        /// <returns></returns>
        public List<NewsPaper> GetNewsList()
        {

            List<NewsPaper> lstNews = new List<NewsPaper>();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstNews = ctx.NewsPaper.ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstNews.OrderByDescending(x => x.fecha).ToList();
        }

        /// <summary>
        /// Retrieve course information based in the primary key
        /// </summary>
        /// <param name="newsTarget"></param>
        /// <returns></returns>
        public NewsPaper GetNewsByKey(NewsPaper newsTarget)
        {
            NewsPaper oNews = new NewsPaper();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oNews =
                        ctx.NewsPaper.Where(x => x.id.Equals(newsTarget.id)).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return oNews;
        }

        /// <summary>
        /// Create or update a course
        /// </summary>
        /// <param name="news"></param>
        public void SaveNews(NewsPaper news)
        {

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    //verify if the student exists
                    NewsPaper oNews = GetNewsByKey(news);

                    if (oNews != null)
                    {
                        // if exists then edit 
                        ctx.NewsPaper.Attach(oNews);
                        EntityFrameworkHelper.EnumeratePropertyDifferences(oNews, news);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create
                        ctx.NewsPaper.Add(news);
                        ctx.SaveChanges();
                    }
                }

            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Delete a course
        /// </summary>
        /// <param name="newsTarget"></param>
        public void DeleteNews(NewsPaper newsTarget)
        {
            try
            {
                using (var ctx = new pioneerEntities())
                {
                    //verify if the school exists
                    NewsPaper oNews = GetNewsByKey(newsTarget);

                    if (oNews != null)
                    {
                        // if exists then edit 
                        ctx.NewsPaper.Attach(oNews);
                        ctx.NewsPaper.Remove(oNews);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }
    }
}

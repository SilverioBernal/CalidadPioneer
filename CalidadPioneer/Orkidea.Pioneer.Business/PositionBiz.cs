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
    public class PositionBiz
    {
        /*CRUD Position*/

        /// <summary>
        /// Retrieve process list 
        /// </summary>
        /// <param name="schoolTarget"></param>
        /// <returns></returns>
        public List<Position> GetPositionList()
        {

            List<Position> lstPosition = new List<Position>();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstPosition = ctx.Position.ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstPosition;
        }

        /// <summary>
        /// Retrieve course information based in the primary key
        /// </summary>
        /// <param name="positionTarget"></param>
        /// <returns></returns>
        public Position GetPositionbyKey(Position positionTarget)
        {
            Position oPosition = new Position();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oPosition =
                        ctx.Position.Where(x => x.id.Equals(positionTarget.id)).FirstOrDefault();
                }
            }
            catch (Exception) { }

            return oPosition;
        }

        /// <summary>
        /// Create or update a course
        /// </summary>
        /// <param name="position"></param>
        public void SavePosition(Position position)
        {

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    Position oPosition = GetPositionbyKey(position);
                    //verify if the student exists
                    if (oPosition != null)
                    {
                        // if exists then edit 
                        ctx.Position.Attach(oPosition);
                        EntityFrameworkHelper.EnumeratePropertyDifferences(oPosition, position);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        //else create
                        ctx.Position.Add(position);
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
        /// <param name="positionTarget"></param>
        public void DeletePosition(Position positionTarget)
        {
            try
            {
                using (var ctx = new pioneerEntities())
                {
                    //verify if the school exists
                    Position oPosition = GetPositionbyKey(positionTarget);

                    if (oPosition != null)
                    {
                        // if exists then edit 
                        ctx.Position.Attach(oPosition);
                        ctx.Position.Remove(oPosition);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }
    }
}

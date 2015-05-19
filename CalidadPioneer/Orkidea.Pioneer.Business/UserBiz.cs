using Orkidea.Pioneer.DataAccesssEF;
using Orkidea.Pioneer.Entities;
using Orkidea.Pioneer.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orkidea.Pioneer.Business
{
    public class UserBiz
    {
        /*CRUD Users*/

        /// <summary>
        /// Retrieve User list without parameters
        /// </summary>
        /// <returns></returns>
        public List<User> GetUserList()
        {

            List<User> lstUser = new List<User>();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstUser = ctx.User.ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstUser;
        }

        /// <summary>
        /// Retrieve User information based in the primary key
        /// </summary>
        /// <param name="UserTarget"></param>
        /// <returns></returns>
        public User GetUserbyName(User UserTarget)
        {
            User oUser = new User();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oUser = ctx.User.Where(x => x.usuario.Equals(UserTarget.usuario)).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return oUser;
        }

        public User GetUserbyKey(User UserTarget)
        {
            User oUser = new User();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oUser = ctx.User.Where(x => x.id.Equals(UserTarget.id)).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return oUser;
        }

        /// <summary>
        /// Create or update a User
        /// </summary>
        /// <param name="UserTarget"></param>
        public void SaveUser(User UserTarget, string rootPath)
        {
            try
            {
                using (var ctx = new pioneerEntities())
                {
                    Cryptography oCrypto = new Cryptography();
                    
                    User oUser = GetUserbyKey(UserTarget);

                    //UserTarget.clave = oCrypto.Encrypt(PasswordHelper.Generate());
                    //UserTarget.usuario = oUser.usuario;

                    string accion = "";
                    if (oUser != null)
                    {
                        oUser.clave = "";
                        // if exists then edit 
                        ctx.User.Attach(oUser);
                        EntityFrameworkHelper.EnumeratePropertyDifferences(oUser, UserTarget);
                        ctx.SaveChanges();
                        accion = "modificación";
                    }
                    else
                    {
                        // else create
                        UserTarget.clave = oCrypto.Encrypt(PasswordHelper.Generate());
                        ctx.User.Add(UserTarget);
                        ctx.SaveChanges();
                        accion = "creación";
                    }

                    // send notification
                    List<System.Net.Mail.MailAddress> to = new List<System.Net.Mail.MailAddress>();
                    if (ConfigurationManager.AppSettings["testMail"].ToString() == "N")
                        to.Add(new System.Net.Mail.MailAddress(UserTarget.usuario));
                    else
                        to.Add(new System.Net.Mail.MailAddress("silverio.bernal@orkidea.co"));

                    Dictionary<string, string> dynamicValues = new Dictionary<string, string>();
                    dynamicValues.Add("[accion]", accion);
                    dynamicValues.Add("[usuario]", UserTarget.usuario);
                    dynamicValues.Add("[clave]", oCrypto.Decrypt(UserTarget.clave));
                    dynamicValues.Add("[urlSitio]", ConfigurationManager.AppSettings["UrlApp"].ToString());
                    MailingHelper.SendMail(to, string.Format("Notificación de {0} de usuario", accion),
                    rootPath + ConfigurationManager.AppSettings["emailNewUserNotificationTemplateHTML"].ToString(),
                    rootPath + ConfigurationManager.AppSettings["emailNewUserNotificationTemplateText"].ToString(),
                    rootPath + ConfigurationManager.AppSettings["emailLogoPath"].ToString(), dynamicValues);
                }
            }
            catch (Exception ex) { throw ex; }
        }

        public void SaveUser(User UserTarget)
        {
            try
            {
                using (var ctx = new pioneerEntities())
                {
                    Cryptography oCrypto = new Cryptography();

                    User oUser = GetUserbyKey(UserTarget);
                    UserTarget.clave = oUser.clave;
                    if (oUser != null)
                    {                        
                        // if exists then edit 
                        ctx.User.Attach(oUser);
                        EntityFrameworkHelper.EnumeratePropertyDifferences(oUser, UserTarget);
                        ctx.SaveChanges();                        
                    }                    
                }
            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Delete a User
        /// </summary>
        /// <param name="UserTarget"></param>
        public void DeleteUser(User UserTarget)
        {
            try
            {
                using (var ctx = new pioneerEntities())
                {
                    //verify if the User exists
                    User oUser = GetUserbyKey(UserTarget);

                    if (oUser != null)
                    {
                        // if exists then edit 
                        ctx.User.Attach(oUser);
                        ctx.User.Remove(oUser);
                        ctx.SaveChanges();
                    }
                }

            }
            catch (Exception ex) { throw ex; }
        }
    }
}

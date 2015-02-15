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
    public class DocumentTypeBiz
    {
        /*CRUD DocumentType*/

        /// <summary>
        /// Retrieve DocumentType list 
        /// </summary>
        /// <param name="schoolTarget"></param>
        /// <returns></returns>
        public List<DocumentType> GetDocumentTypeList()
        {

            List<DocumentType> lstDocumentType = new List<DocumentType>();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstDocumentType = ctx.DocumentType.ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstDocumentType;
        }

        /// <summary>
        /// Retrieve course information based in the primary key
        /// </summary>
        /// <param name="documentTypeTarget"></param>
        /// <returns></returns>
        public DocumentType GetDocumentTypeByKey(DocumentType documentTypeTarget)
        {
            DocumentType oDocumentType = new DocumentType();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oDocumentType =
                        ctx.DocumentType.Where(x => x.id.Equals(documentTypeTarget.id)).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return oDocumentType;
        }

        /// <summary>
        /// Create or update a course
        /// </summary>
        /// <param name="documentType"></param>
        public void SaveDocumentType(DocumentType documentType)
        {

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    //verify if the student exists
                    DocumentType oDocumentType = GetDocumentTypeByKey(documentType);

                    if (oDocumentType != null)
                    {
                        // if exists then edit 
                        ctx.DocumentType.Attach(oDocumentType);
                        EntityFrameworkHelper.EnumeratePropertyDifferences(oDocumentType, documentType);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create
                        ctx.DocumentType.Add(documentType);
                        ctx.SaveChanges();
                    }
                }

            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Delete a course
        /// </summary>
        /// <param name="DocumentTypeTarget"></param>
        public void DeleteDocumentType(DocumentType DocumentTypeTarget)
        {
            try
            {
                using (var ctx = new pioneerEntities())
                {
                    //verify if the school exists
                    DocumentType oDocumentType = GetDocumentTypeByKey(DocumentTypeTarget);

                    if (oDocumentType != null)
                    {
                        // if exists then edit 
                        ctx.DocumentType.Attach(oDocumentType);
                        ctx.DocumentType.Remove(oDocumentType);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }
    }

}

using Orkidea.Pioneer.DataAccesssEF;
using Orkidea.Pioneer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orkidea.Pioneer.Business
{
    public class FileTypeBiz
    {
        public FileType GetFileTypebyKey(FileType fileType)
        {
            FileType oFileType = new FileType();

            try
            {
                using (var ctx = new pioneerEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oFileType = ctx.FileType.Where(x => x.tipoMIME.Equals(fileType.tipoMIME)).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return oFileType;
        }
    }
}

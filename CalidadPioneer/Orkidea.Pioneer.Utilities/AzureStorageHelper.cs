using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orkidea.Pioneer.Utilities
{
    public static class AzureStorageHelper
    {
        public static bool uploadFile(Stream fileStream, string fileName, string destiny)
        {
            bool res = false;

            try
            {
                string connString = ConfigurationManager.ConnectionStrings["AzureStorageAccount"].ConnectionString;
                string destinyContainer = ConfigurationManager.AppSettings[destiny];

                //Get a reference to the storage account
                CloudStorageAccount sa = CloudStorageAccount.Parse(connString);
                CloudBlobClient bc = sa.CreateCloudBlobClient();

                //Get a reference to the container
                CloudBlobContainer container = bc.GetContainerReference(destinyContainer);

                CloudBlockBlob cbb = container.GetBlockBlobReference(fileName);

                cbb.UploadFromStream(fileStream);

                res = true;
            }
            catch (Exception)
            {
                res = false;
            }

            return res;
        }

        public static MemoryStream getFile(string fileName, string origin)
        {
            MemoryStream res = new MemoryStream();

            try
            {
                string connString = ConfigurationManager.ConnectionStrings["AzureStorageAccount"].ConnectionString;
                string destinyContainer = ConfigurationManager.AppSettings[origin];

                //Get a reference to the storage account
                CloudStorageAccount sa = CloudStorageAccount.Parse(connString);
                CloudBlobClient bc = sa.CreateCloudBlobClient();

                //Get a reference to the container
                CloudBlobContainer container = bc.GetContainerReference(destinyContainer);

                CloudBlockBlob cbb = container.GetBlockBlobReference(fileName);

                cbb.DownloadToStream(res);
            }
            catch (Exception)
            {
                throw;
            }

            return res;
        }
    }
}

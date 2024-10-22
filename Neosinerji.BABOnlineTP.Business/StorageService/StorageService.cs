using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Text;
using storage = Microsoft.WindowsAzure.Storage;

namespace Neosinerji.BABOnlineTP.Business {
	public class StorageService : IStorageService
    {
        private static storage.CloudStorageAccount _cloudStorageAccount;

        public StorageService(string containerName)
        {
            _ContainerName = containerName;
            _IsContainerOpen = false;
            _BlobContainer = null;
        }

        protected bool OpenContainer()
        {
            if (this.IsContainerOpen)
                return true;

            if (_cloudStorageAccount == null)
            {
                string connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
                connectionString = "DefaultEndpointsProtocol=https;AccountName=neoonlinestrg;AccountKey=wrM/Fs9YFcU5FQlVw7KiMtuY2nY6XsjvQ7SCB29qhK6i3jEcbH1r1NuP0zCm2r8IXun1kc4gAiYYqib4RW23ww==;EndpointSuffix=core.windows.net";

                _cloudStorageAccount = storage.CloudStorageAccount.Parse(connectionString);
            }

            if (_BlobContainer == null)
            {
                CloudBlobClient blobClient = _cloudStorageAccount.CreateCloudBlobClient();

                _BlobContainer = blobClient.GetContainerReference(this.ContainerName);

                if (!_BlobContainer.Exists())
                {
                    _BlobContainer.CreateIfNotExists();
                    _BlobContainer.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                }
            }

            this._IsContainerOpen = true;

            return true;
        }

        private bool _IsContainerOpen;
        public bool IsContainerOpen
        {
            get
            {
                return _IsContainerOpen;
            }
        }

        private CloudBlobContainer _BlobContainer;
        protected CloudBlobContainer BlobContainer
        {
            get { return _BlobContainer; }
        }


        private string _ContainerName;
        public string ContainerName
        {
            get { return _ContainerName; }
        }

        public string UploadFile(string fileName, byte[] content)
        {
            if (!this.OpenContainer())
                throw new StorageContainerException();

            CloudBlockBlob blockBlob = BlobContainer.GetBlockBlobReference(fileName);

            using (MemoryStream ms = new MemoryStream(content))
            {
                blockBlob.UploadFromStream(ms);
            }

            return blockBlob.Uri.AbsoluteUri;
        }

        public string UploadFile(string directory, string fileName, byte[] content)
        {
            if (!this.OpenContainer())
                throw new StorageContainerException();

            string blobName = String.Format("{0}\\{1}", directory, fileName);
            CloudBlockBlob blockBlob = BlobContainer.GetBlockBlobReference(blobName);

            using (MemoryStream ms = new MemoryStream(content))
            {
                blockBlob.UploadFromStream(ms);
            }

            return blockBlob.Uri.AbsoluteUri;
        }

        public string UploadFile(string fileName, Stream stream)
        {
            if (!this.OpenContainer())
                throw new StorageContainerException();

            CloudBlockBlob blockBlob = BlobContainer.GetBlockBlobReference(fileName);

            blockBlob.UploadFromStream(stream);

            return blockBlob.Uri.AbsoluteUri;
        }

        public string UploadFile(string directory, string fileName, Stream stream)
        {
            if (!this.OpenContainer())
                throw new StorageContainerException();

            string blobName = String.Format("{0}\\{1}", directory, fileName);
            CloudBlockBlob blockBlob = BlobContainer.GetBlockBlobReference(blobName);

            blockBlob.UploadFromStream(stream);

            return blockBlob.Uri.AbsoluteUri;
        }
       
        public string UploadFile(string directory, string fileName, string contents)
        {
            if (!this.OpenContainer())
                throw new StorageContainerException();

            string blobName = String.Format("{0}\\{1}", directory, fileName);
            CloudBlockBlob blockBlob = BlobContainer.GetBlockBlobReference(blobName);

            byte[] buffer = Encoding.UTF8.GetBytes(contents);
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                blockBlob.UploadFromStream(ms);
            }

            return blockBlob.Uri.AbsoluteUri;
        }

        public string UploadXml(string directory, string contents)
        {
            if (!this.OpenContainer())
                throw new StorageContainerException();

            string fileName = System.Guid.NewGuid().ToString() + ".xml";
            string blobName = String.Format("{0}\\{1}", directory, fileName);
            CloudBlockBlob blockBlob = BlobContainer.GetBlockBlobReference(blobName);

            byte[] buffer = Encoding.UTF8.GetBytes(contents.Trim());
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                blockBlob.UploadFromStream(ms);
            }

            return blockBlob.Uri.AbsoluteUri;
        }

        public byte[] DownloadFile(string fileName)
        {
            //if (!this.OpenContainer())
            //    throw new StorageContainerException();

            //if (fileName.Contains("http"))                
            //    fileName = fileName.Replace(BlobContainer.Uri.AbsoluteUri + "/", "");

            //CloudBlockBlob blockBlob = BlobContainer.GetBlockBlobReference(fileName);


            //using (MemoryStream ms = new MemoryStream())
            //{
            //    blockBlob.DownloadToStream(ms);
            //    content = ms.ToArray();
            //}

            try
            {
                if (!String.IsNullOrEmpty(fileName))
                {
                    using (System.Net.WebClient client = new System.Net.WebClient())
                    {
                        return client.DownloadData(fileName);
                    }
                }
            }
            catch (Exception)
            {

            }

            return null;
        }

        public byte[] DownloadFile(string directory, string fileName)
        {
            if (!this.OpenContainer())
                throw new StorageContainerException();

            string blobName = String.Format("{0}\\{1}", directory, fileName);
            CloudBlockBlob blockBlob = BlobContainer.GetBlockBlobReference(blobName);

            byte[] content;
            using (MemoryStream ms = new MemoryStream())
            {
                blockBlob.DownloadToStream(ms);
                content = ms.ToArray();
            }

            return content;
        }

       
    }

    public class StorageContainerException : ApplicationException
    {
    }
}

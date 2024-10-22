using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text; 
namespace Neosinerji.BABOnlineTP.Business
{
    public class MockStroageService : IMusteriDokumanStorage, IKullaniciFotografStorage, IWEBServiceLogStorage, ITVMDokumanStorage, ITeklifPDFStorage, IPolicePDFStorage, IPoliceTransferStorage
    { 
        public string ContainerName
        {
            get { return "mock"; }
        }

        public bool IsContainerOpen
        {
            get { return true; }
        }

        public string UploadFile(string fileName, byte[] content)
        {
            return "mockurl";
        }

        public string UploadFile(string directory, string fileName, byte[] content)
        {
            return "mockurl";
        }

        public string UploadFile(string fileName, System.IO.Stream stream)
        {
            return "mockurl";
        }

        public string UploadFile(string directory, string fileName, System.IO.Stream stream)
        {
            return "mockurl";
        }

        public string UploadFile(string directory, string fileName, string contents)
        {
            return "mockurl";
        }

        public string UploadXml(string directory, string contents)
        {
            return "mockurl";
        }

        public byte[] DownloadFile(string directory, string fileName)
        {
            return new byte[] { 0x0 };
        }

        public byte[] DownloadFile(string fileName)
        {
            return new byte[] { 0x0 };
        }

        public string DownloadToFiles(string url)
        {
            return "mockurl";
        }

        
    }
}

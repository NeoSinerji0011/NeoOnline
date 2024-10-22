using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IStorageService
    {
        /// <summary>
        /// Blob storage container name
        /// </summary>
        string ContainerName { get; }

        bool IsContainerOpen { get; }

        /// <summary>
        /// Uploads file to blob storage container
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="content">Contents of the file</param>
        /// <returns>
        /// Url
        /// </returns>
        string UploadFile(string fileName, byte[] content);

        /// <summary>
        /// Uploads file to blob storage container
        /// </summary>
        /// <param name="directory">Directory to upload file</param>
        /// <param name="fileName">File name</param>
        /// <param name="content">Contents of the file</param>
        /// <returns>
        /// Url
        /// </returns>
        string UploadFile(string directory, string fileName, byte[] content);

        /// <summary>
        /// Uploads file to blob storage container
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="stream">Contents of the file</param>
        /// <returns>
        /// Url
        /// </returns>
        string UploadFile(string fileName, Stream stream);

        /// <summary>
        /// Uploads file to blob storage container
        /// </summary>
        /// <param name="directory">Directory to upload file</param>
        /// <param name="fileName">File name</param>
        /// <param name="stream">Contents of the file</param>
        /// <returns>
        /// Url
        /// </returns>
        string UploadFile(string directory, string fileName, Stream stream);
        
        /// <summary>
        /// Uploads file to blob storage container
        /// </summary>
        /// <param name="directory">Directory to upload file</param>
        /// <param name="fileName">File name</param>
        /// <param name="contents">Contents of the file</param>
        /// <returns>
        /// Url
        /// </returns>
        string UploadFile(string directory, string fileName, string contents);

        /// <summary>
        /// Uploads xml to blob storage with a random file name
        /// </summary>
        /// <param name="directory">Directory to upload file</param>
        /// <param name="contents">Contents of the file</param>
        /// <returns>
        /// Url
        /// </returns>
        string UploadXml(string directory, string contents);

        /// <summary>
        /// Downloads file from blob storage container
        /// </summary>
        /// <param name="directory">directory of file to download from</param>
        /// <param name="fileName">file name to download</param>
        /// <returns>file contents in byte array</returns>
        byte[] DownloadFile(string directory, string fileName);

        /// <summary>
        /// Downloads file from blob storage container
        /// </summary>
        /// <param name="fileName">file name to download</param>
        /// <returns>file contents in byte array</returns>
        byte[] DownloadFile(string fileName);
    }
}

using System;
using System.Net;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Nexus;
using System.IO;

namespace FSS.Omnius.Modules.Nexus.Service
{
    public class WebDavFileSyncService : IFileSyncService
    {
        public void DownloadFile(FileMetadata file)
        {
            Uri downloadUri = getUri(file.WebDavServer.UriBasePath, file.AppFolderName, file.Filename);
            AsyncCallback callback = new AsyncCallback(finishDownload);
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(downloadUri);
            httpWebRequest.Method = WebRequestMethods.Http.Get.ToString();
            if (!file.WebDavServer.AnonymousMode)
                httpWebRequest.Credentials = new NetworkCredential(file.WebDavServer.AuthUsername, file.WebDavServer.AuthPassword);
            httpWebRequest.BeginGetResponse(callback, new WebDavAsyncState { Metadata = file, Request = httpWebRequest });
        }
        private void finishDownload(IAsyncResult result)
        {
            WebDavAsyncState downloadAsyncState = (WebDavAsyncState)result.AsyncState;
            HttpWebRequest httpWebRequest = downloadAsyncState.Request;

            using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.EndGetResponse(result))
            {
                int contentLength = int.Parse(response.GetResponseHeader("Content-Length"));
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (var context = new DBEntities())
                    {
                        FileMetadata currentMetadataItem;
                        try
                        {
                            currentMetadataItem = context.FileMetadataRecords.Find(downloadAsyncState.Metadata.Id);
                        }
                        catch(InvalidOperationException)
                        {
                            currentMetadataItem = downloadAsyncState.Metadata;
                            context.FileMetadataRecords.Add(currentMetadataItem);
                        }
                        if (currentMetadataItem.CachedCopy == null)
                            currentMetadataItem.CachedCopy = new FileSyncCache();

                        byte[] buffer = new byte[16 * 1024];
                        using (MemoryStream ms = new MemoryStream())
                        {
                            int read;
                            while ((read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                ms.Write(buffer, 0, read);
                            }
                            currentMetadataItem.CachedCopy.Blob = ms.ToArray();
                        }
                        context.SaveChanges();
                    }
                }
            }
        }
        public void UploadFile(FileMetadata file)
        {
            Uri uploadUri = getUri(file.WebDavServer.UriBasePath, file.AppFolderName, file.Filename);
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uploadUri);
            httpWebRequest.Method = WebRequestMethods.Http.Put.ToString();
            if (!file.WebDavServer.AnonymousMode)
                httpWebRequest.Credentials = new NetworkCredential(file.WebDavServer.AuthUsername, file.WebDavServer.AuthPassword);

            if (file.CachedCopy == null)
                throw new InvalidOperationException("Cannot upload file: no cached data found for this file.");
            httpWebRequest.ContentLength = file.CachedCopy.Blob.Length;
            Stream requestStream = httpWebRequest.GetRequestStream();
            requestStream.Write(file.CachedCopy.Blob, 0, file.CachedCopy.Blob.Length);
            httpWebRequest.GetResponse();
        }
        private Uri getUri(string baseUrl, string appFolder, string filename)
        {
            string remotePath = String.Format("{0}/{1}/{2}", baseUrl.TrimEnd('/'), appFolder.Trim('/'), filename.TrimStart('/'));
            return new Uri(remotePath);
        }
    }
    class WebDavAsyncState
    {
        public FileMetadata Metadata;
        public HttpWebRequest Request;
    }
}

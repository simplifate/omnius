using System;
using System.Net;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Nexus;
using FSS.Omnius.Modules.Compass;
using System.IO;
using FSS.Omnius.Modules.Compass.Service;
using System.Collections.Generic;

namespace FSS.Omnius.Modules.Nexus.Service
{
    public class WebDavFileSyncService : IFileSyncService
    {

        private HttpWebRequest CreateWebRequest(FileMetadata file, string method)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            Uri downloadUri = getUri(file.WebDavServer.UriBasePath, file);
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(downloadUri);
            httpWebRequest.Method = method; // WebRequestMethods.Http.Get.ToString();
            if (!file.WebDavServer.AnonymousMode)
                httpWebRequest.Credentials = new NetworkCredential(file.WebDavServer.AuthUsername, file.WebDavServer.AuthPassword);

            return httpWebRequest;
        }

        public void BeginDownloadFile(FileMetadata file, FileSyncServiceDownloadedEventHandler downloadedHandler = null)
        {
            HttpWebRequest httpWebRequest = this.CreateWebRequest(file, WebRequestMethods.Http.Get);
            AsyncCallback callback = new AsyncCallback(finishDownload);
            httpWebRequest.BeginGetResponse(callback, new WebDavAsyncState { Metadata = file, Request = httpWebRequest, DownloadedHandler = downloadedHandler });
        }

        public void DownloadFile(FileMetadata file)
        {
            HttpWebRequest httpWebRequest = this.CreateWebRequest(file, WebRequestMethods.Http.Get);
            using (var response = httpWebRequest.GetResponse())
            {
                ProcessResponse((HttpWebResponse)response, ref file);
            }
        }

        public void DeleteFile(FileMetadata file)
        {
            var context = DBEntities.instance;
            HttpWebRequest httpWebRequest = this.CreateWebRequest(file, "DELETE");
            context.CachedFiles.Remove(file.CachedCopy);
            context.FileMetadataRecords.Remove(file);
            context.SaveChanges();
            httpWebRequest.GetResponse();
        }

        private void ProcessResponse(HttpWebResponse response, ref FileMetadata currentMetadataItem)
        {
            int contentLength = int.Parse(response.GetResponseHeader("Content-Length"));
            using (Stream responseStream = response.GetResponseStream())
            {
                //using (var context = DBEntities.instance)
                using (var context = new DBEntities())
                {

                    try
                    {
                        var foundRec = context.FileMetadataRecords.Find(currentMetadataItem.Id);
                        currentMetadataItem = foundRec;
                    }
                    catch (InvalidOperationException)
                    {
                        //currentMetadataItem = currentMetadataItem;
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

        private void finishDownload(IAsyncResult result)
        {
            try
            { 
                WebDavAsyncState downloadAsyncState = (WebDavAsyncState)result.AsyncState;
                HttpWebRequest httpWebRequest = downloadAsyncState.Request;

                FileMetadata currentMetadataItem = downloadAsyncState.Metadata;

                using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.EndGetResponse(result))
                {
                    ProcessResponse(response, ref currentMetadataItem);
                }

                if (downloadAsyncState.DownloadedHandler != null)
                    downloadAsyncState.DownloadedHandler(this, new FileSyncServiceDownloadedEventArgs() { FileMetadata = currentMetadataItem, Result = FileSyncServiceDownloadedResult.Success });
            }
            catch(Exception exc)
            {
                WebDavAsyncState downloadAsyncState = (WebDavAsyncState)result.AsyncState;
                if (downloadAsyncState.DownloadedHandler != null)
                    downloadAsyncState.DownloadedHandler(this, new FileSyncServiceDownloadedEventArgs() { FileMetadata = null, Result = FileSyncServiceDownloadedResult.Error });
                throw exc;
            }
        }

        public void UploadFile(FileMetadata file)
        {
            if (file.Id == default(int))
                throw new ArgumentException("Add FileMetadata to context and save to generate Id");

            var httpWebRequest = CreateWebRequest(file, WebRequestMethods.Http.Put);

            if (file.CachedCopy == null)
                throw new InvalidOperationException("Cannot upload file: no cached data found for this file.");
            httpWebRequest.ContentLength = file.CachedCopy.Blob.Length;
            Stream requestStream = httpWebRequest.GetRequestStream();
            requestStream.Write(file.CachedCopy.Blob, 0, file.CachedCopy.Blob.Length);
            httpWebRequest.GetResponse();

            Uri uploadUri = getUri(file.WebDavServer.UriBasePath, file);
            using (var context = DBEntities.instance)
            {
                context.FileMetadataRecords.Find(file.Id).AbsoluteURL = uploadUri.AbsoluteUri;
                context.SaveChanges();
            }
        }

        private Uri getUri(string baseUrl, FileMetadata fmd)
        {
            return getUri(baseUrl, fmd.AppFolderName, fmd.ModelEntityName, fmd.Tag, fmd.Filename, fmd.Id);
        }

        private Uri getUri(string baseUrl, string appFolder, string modelEntityName, string tag, string filename, int id)
        {
            string remotePath = String.Format("{0}/{1}/{4}", baseUrl.TrimEnd('/'), appFolder.Trim('/'), modelEntityName.Trim('/'), tag.Trim('/'), id + "_" + filename.TrimStart('/'));
            return new Uri(remotePath);
        }
    }
    class WebDavAsyncState
    {
        public FileMetadata Metadata;
        public HttpWebRequest Request;
        public FileSyncServiceDownloadedEventHandler DownloadedHandler;
    }
}

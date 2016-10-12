using System;
using System.Linq;
using System.Net;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Nexus;
using System.IO;
using System.Collections.Generic;
using Elasticsearch.Net;
using Nest;
using FSS.Omnius.Modules.Nexus.Service;
using System.Web.Configuration;
using Newtonsoft.Json;

namespace FSS.Omnius.Modules.Compass.Service
{
    public class ElasticService : IElasticService
    {
        public void Index(List<FileMetadata> files)
        {
            CheckMapping(); //TODO: provádět pouze jednou, dát pryč podmínku na exists

            IFileSyncService webDavService = new WebDavFileSyncService();

            foreach(FileMetadata file in files)
            {
                webDavService.DownloadFile(file, OnDownloaded); //TODO?: podmínka na blob == null ?
            }
        }

        private string GetConfig(string key)
        {
            string value = WebConfigurationManager.AppSettings[key];

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException($"{key} setting not found in the web.config");
            }

            return value;
        }

        private ElasticLowLevelClient GetClient()
        {
            var node = new Uri(GetConfig("ElasticSearchServerUri"));
            var config = new ConnectionConfiguration(node);
            var client = new ElasticLowLevelClient(config);
            
            return client;
        }

        private void CheckMapping()
        {
            string indexName = GetConfig("ElasticSearchServerIndexName");
            var client = GetClient();

            if(client.IndicesExists<byte[]>(indexName).HttpStatusCode == 200)   //200 existuje, 404 neexistuje - https://www.elastic.co/guide/en/elasticsearch/reference/current/indices-exists.html
            {
                return;
            }
            
            var indexResult = client.IndicesCreate<byte[]>(indexName, null, null);
            if (!indexResult.Success)
                throw new ElasticServiceException(indexResult.DebugInformation);

            var result = client.IndicesPutMapping<byte[]>(indexName, "document", new
            {
                document = new
                {
                    properties = new
                    {
                        file = new
                        {
                            type = "attachment",
                            fields = new
                            {
                                content = new
                                {
                                    type = "string",
                                    term_vector = "with_positions_offsets",
                                    store = true
                                }
                            }
                        },
                        AppFolderName = new { type = "string" },
                        FileName = new { type = "string" },
                        Id = new { type = "integer" },
                        TimeCreated = new { type = "date" },
                        TimeChanged = new { type = "date" },
                        Version = new { type = "integer" }
                    }
                }
            });

            if (!result.Success)
                throw new ElasticServiceException(result.DebugInformation);
        }

        private void OnDownloaded(object sender, FileSyncServiceDownloadedEventArgs args)
        {
            if (args.Result != FileSyncServiceDownloadedResult.Success)
                return;

            string indexName = GetConfig("ElasticSearchServerIndexName");
            var client = GetClient();

            var result = client.Index<byte[]>(indexName, "document", args.FileMetadata.Id.ToString(), new
            {
                file = Convert.ToBase64String(args.FileMetadata.CachedCopy.Blob),
                AppFolderName = args.FileMetadata.AppFolderName,
                FileName = args.FileMetadata.Filename,
                Id = args.FileMetadata.Id,
                TimeChanged = (long)(args.FileMetadata.TimeChanged - new DateTime(1970, 1, 1)).TotalMilliseconds,
                TimeCreated = (long)(args.FileMetadata.TimeCreated - new DateTime(1970, 1, 1)).TotalMilliseconds,
                Version = args.FileMetadata.Version,
            });

            if (!result.Success)
                throw new ElasticServiceException(result.DebugInformation);
        }

        public ElasticServiceFoundDocument[] Search(string query, string appName = null)
        {
            string indexName = GetConfig("ElasticSearchServerIndexName");
            var client = GetClient();

            string appNameQuery = string.Empty;
            if (!string.IsNullOrWhiteSpace(appName))
#warning TODO: filtr na aplikaci
                appNameQuery = "";

            var result = client.Search<string>(
                indexName,
                "document",
                "{\"fields\": [\"AppFolderName\", \"FileName\", \"Id\", \"TimeChanged\", \"TimeCreated\", \"Version\"],\"query\": {\"match\": {\"file.content\": \"" + query.Replace("\"", " ") + "\"}},\"highlight\": {\"fields\": {\"file.content\": {\"pre_tags\" : [\" <b> \"], \"post_tags\" : [\" </b> \"]}}}}"
                );
            
            if (!result.Success)
                throw new ElasticServiceException(result.DebugInformation);

            SearchResult sResult;
            Newtonsoft.Json.JsonSerializer ser = new JsonSerializer();
            using (TextReader reader = new StringReader(result.Body))
            {
                using (JsonReader jr = new JsonTextReader(reader))
                {
                    sResult = ser.Deserialize<SearchResult>(jr);
                }
            }

            return sResult.hits.hits.Select(a => new ElasticServiceFoundDocument() {
                Highlights = a.highlight.FileContent,
                AppFolderName = a.Fields.AppFolderName[0],
                Filename = a.Fields.FileName[0],
                Id = a.Fields.Id[0],
                TimeChanged = new DateTime(1970,1,1).AddMilliseconds(a.Fields.TimeChanged[0]),
                TimeCreated = new DateTime(1970,1,1).AddMilliseconds(a.Fields.TimeCreated[0]),
                Version = a.Fields.Version[0]
            }).ToArray();
        }

        private class SearchResult
        {
            public SearchResultHits hits { get; set; }
        }

        private class SearchResultHits
        {
            public SearchResultHit[] hits { get; set; }
        }

        private class SearchResultHitFields
        {
            public string[] AppFolderName { get; set; }
            public string[] FileName { get; set; }
            public int[] Id { get; set; }
            public long[] TimeChanged { get; set; }
            public long[] TimeCreated { get; set; }

            public int[] Version { get; set; }
        }

        private class SearchResultHit
        {
            public int _Id { get; set; }

            public SearchResultHitFields Fields { get; set; }

            public SearchResultHitHighlight highlight { get; set; }
        }

        private class SearchResultHitHighlight
        {
            [JsonProperty("file.content")]
            public string[] FileContent { get; set; }
        }
    }

    public class ElasticServiceFoundDocument
    {
        public int Id { get; set; }
        public string Filename { get; set; }
        public string AppFolderName { get; set; }
        public DateTime TimeCreated { get; set; }
        public DateTime TimeChanged { get; set; }
        public int Version { get; set; }
        public string[] Highlights { get; set; }
    }

    public class ElasticServiceException: Exception
    {
        public ElasticServiceException(string message): base(message)
        {

        }
    }
}

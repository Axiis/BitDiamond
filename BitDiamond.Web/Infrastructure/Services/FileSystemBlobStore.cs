using Axis.Luna;
using Axis.Luna.Extensions;
using BitDiamond.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;

namespace BitDiamond.Web.Infrastructure.Services
{
    using Metadata = Dictionary<string, string>;

    public class FileSystemBlobStore: IBlobStore
    {
        public string RootDirecotry => Path.Combine(HostingEnvironment.MapPath("~/"), "Content/Blob"); // ~/Content/Blob
        
        public IAppUrlProvider UrlProvider { get; private set; }

        public IUserContext UserContext { get; private set; }
        
        public FileSystemBlobStore(IAppUrlProvider urlProvider, IUserContext userContextService)
        {
            UrlProvider = urlProvider;
            UserContext = userContextService;

            //create the root dir
            var dinfo = new DirectoryInfo(RootDirecotry);
            if (!dinfo.Exists) dinfo.Create();
        }
        
        public Operation Delete(string blobUri)
        => Operation.Try(() =>
        {
            if (string.IsNullOrWhiteSpace(blobUri)) return;

            //delete blob file
            var finfo = new FileInfo(GetLocalPath(blobUri));
            if (finfo.Exists) finfo.Delete();

            //delete meta file
            finfo = new FileInfo(finfo.FullName + ".meta");
            if (finfo.Exists) finfo.Delete();
        });
        
        public Operation<EncodedBinaryData> GetBlob(string blobUri)
        => Operation.Try(() =>
        {
            var finfo = new FileInfo(GetLocalPath(blobUri));
            return new EncodedBinaryData(finfo.OpenRead() as Stream, MimeMap.ToMime(finfo.Extension));
        });
        
        public Operation<string> Persist(EncodedBinaryData blob)
        => Operation.Try(() =>
        {
            var mime = blob.MimeObject();
            var fileName = GenerateUniqueName(mime);

            using (var stream = new FileInfo(fileName).OpenWrite())
                stream.Write(blob.Data, 0, blob.Data.Length);

            if (!string.IsNullOrWhiteSpace(blob.Metadata))
                using (var writer = new StreamWriter(new FileInfo(fileName + ".meta").OpenWrite()))
                {
                    blob.MetadataTags().ForAll((cnt, next) => writer.WriteLine(next));
                    writer.Flush();
                }

            return UrlProvider.GenerateBlobUrl(new FileInfo(fileName).Name);
        });

        public Operation<Metadata> GetMetadata(string blobUri)
        => Operation.Try(() =>
        {
            var finfo = new FileInfo(GetLocalPath(blobUri) + ".meta");

            if (finfo.Exists)
                return new StreamReader(finfo.OpenRead())
                    .Using(reader => reader.ReadToEnd())
                    .Pipe(_tags => TagBuilder.Parse(_tags))
                    .Select(_tag => _tag.Name.ValuePair(_tag.Value))
                    .Pipe(_tags => new Dictionary<string, string>().AddAll(_tags));

            else return new Dictionary<string, string>();
        });


        internal string GetLocalPath(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri)) return null;

            var path = uri.StartsWith("/") ? uri : new Uri(uri).PathAndQuery;

            if (!path.ToLower().StartsWith("/content/blob")) return null;

            path = Regex.Replace(path, "\\/Content\\/Blob\\/", "", RegexOptions.IgnoreCase);
            return Path.Combine(RootDirecotry, path);
        }

        private string GenerateUniqueName(Mime mime)
        {
            var dinfo = new DirectoryInfo(RootDirecotry);
            string fname = null;
            do
            {
                fname = Path.Combine(dinfo.FullName, $"{RandomAlphaNumericGenerator.RandomAlphaNumeric(40)}{mime.Extension}");
            }
            while (File.Exists(fname));
            return fname;
        }
    }
}
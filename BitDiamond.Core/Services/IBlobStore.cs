using Axis.Luna;
using System.Collections.Generic;

namespace BitDiamond.Core.Services
{
    using Metadata = Dictionary<string, string>;

    public interface IBlobStore
    {
        Operation<string> Persist(EncodedBinaryData blob);
        Operation Delete(string blobUri);
        Operation<EncodedBinaryData> GetBlob(string blobUri);

        Operation<Metadata> GetMetadata(string blobUri);
    }
}

using Axis.Luna;

namespace BitDiamond.Core.Services
{
    public interface IBlobStore
    {
        Operation<string> Persist(EncodedBinaryData blob);
        Operation Delete(string blobUri);
        Operation<EncodedBinaryData> GetBlob(string blobUri);
    }
}

using System;
using System.Threading.Tasks;

namespace WsaPartner.APKViewer
{
    public interface IFileDecoder
    {
        event Action decodeProgressCallbackEvent;

        void SetFilePath(Uri fileUri);
        Task DecodeAsync();

        PackageDataModel GetDataModel();
    }
}

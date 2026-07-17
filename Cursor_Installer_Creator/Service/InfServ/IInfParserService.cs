using System.Text;

using Cursor_Installer_Creator.Data;

namespace Cursor_Installer_Creator.Service.InfServ;

public interface IInfParserService
{
    InfFile Parse(string content);
    Task<InfFile> ParseAsync(Stream stream, Encoding? encoding = null, CancellationToken cancellationToken = default);
    Task<InfFile> ParseAsync(string content, CancellationToken cancellationToken = default);
    string Serialize(InfFile infFile);
}

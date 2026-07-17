using Cursor_Installer_Creator.Data;

using Microsoft.Extensions.Logging;

namespace Cursor_Installer_Creator.Service.CursorServ;

public sealed class CursorService : ICursorService
{
    private readonly ILogger<CursorService> _logger;

    public CursorService(ILogger<CursorService> logger)
    {
        _logger = logger;
    }

    public CursorType GetCursorTypeFromFile(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);

        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        var type = extension switch
        {
            ".cur" => CursorType.cur,
            ".ani" => CursorType.ani,
            _ => throw new NotSupportedException($"Unsupported cursor file type: {extension}"),
        };
        _logger.LogDebug("Resolved cursor type: {Extension} \u2192 {Type}", extension, type);
        return type;
    }

    public bool IsValidCursorFile(string fileName)
    {
        var ext = Path.GetExtension(fileName);
        return ext.Equals(".cur", StringComparison.OrdinalIgnoreCase)
            || ext.Equals(".ani", StringComparison.OrdinalIgnoreCase);
    }
}

using System.Globalization;

using Avalonia.Platform;

using CsvHelper;
using CsvHelper.Configuration;

using Cursor_Installer_Creator.Data;

using Microsoft.Extensions.Logging;

namespace Cursor_Installer_Creator.Repository.CursorAssignmentRepo;

public sealed class CursorAssignmentRepository : ICursorAssignmentRepository
{
    private readonly CursorAssignment[] _assignments;
    private readonly ILogger<CursorAssignmentRepository> _logger;

    public CursorAssignmentRepository(ILogger<CursorAssignmentRepository> logger)
    {
        _logger = logger;

        using var stream = AssetLoader.Open(new Uri($"avares://{nameof(Cursor_Installer_Creator)}/Resources/cursor-assignment.csv"));
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true
        });

        _assignments = [.. csv.GetRecords<CursorAssignment>()];
        _logger.LogDebug("Loaded {Count} cursor assignments from CSV", _assignments.Length);
    }

    public CursorAssignment[] GetAllAssignments()
        => _assignments;

    public CursorAssignment? GetAssignmentFromName(string name, CursorAssignmentType type)
    {
        return type switch
        {
            CursorAssignmentType.Name => _assignments.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)),
            CursorAssignmentType.DisplayName => _assignments.FirstOrDefault(x => string.Equals(x.DisplayName, name, StringComparison.OrdinalIgnoreCase)),
            CursorAssignmentType.WindowsDefault => _assignments.FirstOrDefault(x => string.Equals(x.WindowsDefault, name, StringComparison.OrdinalIgnoreCase)),
            CursorAssignmentType.WindowsReg => _assignments.FirstOrDefault(x => string.Equals(x.WindowsReg, name, StringComparison.OrdinalIgnoreCase)),
            CursorAssignmentType.WindowsInstall => _assignments.FirstOrDefault(x => string.Equals(x.WindowsInstall, name, StringComparison.OrdinalIgnoreCase)),
            CursorAssignmentType.Avalonia => _assignments.FirstOrDefault(x => string.Equals(x.Avalonia, name, StringComparison.OrdinalIgnoreCase)),
            _ => null,
        };
    }

    public CursorAssignment? GetAssignmentFromName(string name, CursorAssignmentType[]? order = null)
    {
        if (order is not null && order.Length != 0)
            return GetAssignmentFromNameInOrder(name, order);

        var assignment = GetAssignmentFromName(name, CursorAssignmentType.Name);
        assignment ??= GetAssignmentFromName(name, CursorAssignmentType.DisplayName);
        assignment ??= GetAssignmentFromName(name, CursorAssignmentType.WindowsDefault);
        assignment ??= GetAssignmentFromName(name, CursorAssignmentType.WindowsReg);
        assignment ??= GetAssignmentFromName(name, CursorAssignmentType.WindowsInstall);
        assignment ??= GetAssignmentFromName(name, CursorAssignmentType.Avalonia);
        return assignment;
    }

    private CursorAssignment? GetAssignmentFromNameInOrder(string name, CursorAssignmentType[] order)
    {
        foreach (var type in order)
        {
            var assignment = GetAssignmentFromName(name, type);
            if (assignment is not null)
                return assignment;
        }
        return null;
    }
}

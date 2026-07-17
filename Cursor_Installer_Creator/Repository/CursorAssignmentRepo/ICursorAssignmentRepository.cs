using Cursor_Installer_Creator.Data;

namespace Cursor_Installer_Creator.Repository.CursorAssignmentRepo;

public interface ICursorAssignmentRepository
{
    CursorAssignment? GetAssignmentFromName(string name, CursorAssignmentType type);
    CursorAssignment? GetAssignmentFromName(string name, CursorAssignmentType[]? order = null);
    CursorAssignment[] GetAllAssignments();
}

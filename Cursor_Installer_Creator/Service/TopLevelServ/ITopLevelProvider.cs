using Avalonia.Controls;

namespace Cursor_Installer_Creator.Service.TopLevelServ;

public interface ITopLevelProvider
{
    void SetTopLevel(TopLevel topLevel);
    TopLevel GetTopLevel();
}

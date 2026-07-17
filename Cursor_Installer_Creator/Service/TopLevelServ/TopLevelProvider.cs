using Avalonia.Controls;

namespace Cursor_Installer_Creator.Service.TopLevelServ;

public sealed class TopLevelProvider : ITopLevelProvider
{
    private TopLevel? _topLevel;

    public void SetTopLevel(TopLevel topLevel) => _topLevel = topLevel;

    public TopLevel GetTopLevel()
        => _topLevel ?? throw new InvalidOperationException(
            "TopLevel has not been initialized. Ensure SetTopLevel is called after the root view is attached.");
}

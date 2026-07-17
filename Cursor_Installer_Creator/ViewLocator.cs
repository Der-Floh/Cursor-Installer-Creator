using System.Diagnostics.CodeAnalysis;

using Avalonia.Controls;
using Avalonia.Controls.Templates;

using Cursor_Installer_Creator.ViewModels;

namespace Cursor_Installer_Creator;

[RequiresUnreferencedCode("Default implementation of ViewLocator involves reflection which may be trimmed away.", Url = "https://docs.avaloniaui.net/docs/concepts/view-locator")]
public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        if (type is not null)
        {
            return (Control?)App.Services.GetService(type)
                ?? throw new InvalidOperationException($"View {type.Name} is not registered in DI.");
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data) => data is IViewModelBase;
}
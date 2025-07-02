using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Exp.Avalonia.ViewModels;

namespace Exp.Avalonia;

public class ViewLocator : IDataTemplate
{
    /// <inheritdoc />
    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    /// <inheritdoc />
    public bool Match(object? data) => data is ViewModelBase;
}

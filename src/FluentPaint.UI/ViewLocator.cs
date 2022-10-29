using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using FluentPaint.UI.ViewModels;

namespace FluentPaint.UI;

/// <summary>
/// Generated class by Avalonia that is used to match Views with ViewModels.
/// </summary>
public class ViewLocator : IDataTemplate
{
    public IControl Build(object data)
    {
        var name = data.GetType().FullName!.Replace("ViewModel", "View");
        var type = Type.GetType(name);

        if (type != null)
        {
            return (Control) Activator.CreateInstance(type)!;
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object data)
    {
        return data is ViewModelBase;
    }
}
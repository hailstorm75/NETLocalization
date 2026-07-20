using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Localization.Shared;
using Localization.Shared.Models;

namespace Localization.Avalonia;

public class TrEnumExtension : AvaloniaObject
{
    private static readonly TrEnumConverter CONVERTER = new();

    internal static readonly AttachedProperty<Enum?> ItemsSourceExProperty =
        AvaloniaProperty.RegisterAttached<TrEnumExtension, Interactive, Enum?>(
            "ItemsSourceEx",
            defaultBindingMode: BindingMode.OneTime);

    public static readonly AttachedProperty<Enum?> SelectedEnumProperty =
        AvaloniaProperty.RegisterAttached<TrEnumExtension, Interactive, Enum?>(
            "SelectedEnum",
            defaultBindingMode: BindingMode.TwoWay);

    static TrEnumExtension()
    {
        SelectedEnumProperty.Changed.AddClassHandler<Interactive>(HandleSelectedEnumChanged);
    }

    private static void HandleSelectedEnumChanged(Interactive interactive, AvaloniaPropertyChangedEventArgs args)
    {
        if (interactive is not SelectingItemsControl selectingControl)
            return;

        HandleItemsSourceExChanged(selectingControl, args);

        if (args.NewValue is not Enum value)
            return;

        var localized = CONVERTER.Convert(value, typeof(LEnum), null, CultureInfo.CurrentCulture);
        selectingControl.SetValue(SelectingItemsControl.SelectedItemProperty, localized);
    }

    private static void HandleItemsSourceExChanged(SelectingItemsControl selectingItemsControl, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.NewValue is not Enum value)
        {
            CultureManager.LanguageChanged -= HandleChange;

            return;
        }

        if (ReferenceEquals(args.NewValue, args.OldValue))
            return;

        var fields = value.GetType().GetLocalizedValues();
        selectingItemsControl.SetValue(ItemsControl.ItemsSourceProperty, fields);

        if (args.OldValue is not Enum)
            CultureManager.LanguageChanged += HandleChange;

        void HandleChange(object? sender, Language args)
        {
            var selection = selectingItemsControl.GetValue<object?>(SelectingItemsControl.SelectedItemProperty);
            if (selection is not LEnum localized)
                return;

            var fields = localized.EnumField.GetType().GetLocalizedValues();
            selectingItemsControl.SetValue(ItemsControl.ItemsSourceProperty, fields);
            selectingItemsControl.SetValue(SelectingItemsControl.SelectedValueProperty, localized.String);
            selectingItemsControl.SetValue(SelectingItemsControl.SelectedItemProperty, localized);
        }
    }

    /// <summary>
    /// Accessor for Attached property <see cref="CommandProperty"/>.
    /// </summary>
    public static void SetItemsSourceEx(AvaloniaObject element, Enum? value) => element.SetValue(ItemsSourceExProperty, value);

    public static Enum? GetSelectedEnum(AvaloniaObject element) => element.GetValue(SelectedEnumProperty);

    public static void SetSelectedEnum(AvaloniaObject element, Enum? value) => element.SetValue(SelectedEnumProperty, value);
}

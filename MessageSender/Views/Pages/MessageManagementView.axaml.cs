using ActiproSoftware.UI.Avalonia.Themes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using AvaloniaEdit.Indentation.CSharp;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;

namespace MessageSender.Views.Pages;

public partial class MessageManagementView : UserControl
{
    public MessageManagementView()
    {
        InitializeComponent();
        Application.Current!.ActualThemeVariantChanged += Current_ActualThemeVariantChanged;
        InitiateEditors(GetThemeVariant(Application.Current!.ActualThemeVariant));
    }

    private void InitiateEditors(ThemeName themeName)
    {
        Color textSelectionColor;
        Color textSearchSectionColor;
        Color searchPanelColor;

        if (themeName == ThemeName.DarkPlus)
        {
            textSelectionColor = Color.Parse("#23405a");
            textSearchSectionColor = Color.Parse("#952354");
            searchPanelColor = Color.Parse("#F8F8F9");
        }
        else
        {
            textSelectionColor = Color.Parse("#ADD6FF");
            textSearchSectionColor = Color.Parse("#F8C9AB");
            searchPanelColor = Color.Parse("#F8F8F9");
        }

        var registryOptions = new RegistryOptions(themeName);
        Language jsonSyntax = registryOptions.GetLanguageByExtension(".json");

        MessageBodyEditor.ShowLineNumbers = true;
        MessageBodyEditor.Options.ConvertTabsToSpaces = true;
        MessageBodyEditor.Options.AllowScrollBelowDocument = true;
        MessageBodyEditor.Options.HighlightCurrentLine = true;
        MessageBodyEditor.Options.EnableEmailHyperlinks = false;
        MessageBodyEditor.Options.EnableHyperlinks = false;
        MessageBodyEditor.SearchResultsBrush = new SolidColorBrush(textSearchSectionColor);
        MessageBodyEditor.TextArea.IndentationStrategy = new CSharpIndentationStrategy(MessageBodyEditor.Options);
        MessageBodyEditor.TextArea.RightClickMovesCaret = true;
        MessageBodyEditor.TextArea.SelectionBrush = new SolidColorBrush(textSelectionColor);

        TextMate.Installation messageBodyEditorTextMateInstallation = MessageBodyEditor.InstallTextMate(registryOptions);
        messageBodyEditorTextMateInstallation.SetGrammar(registryOptions.GetScopeByLanguageId(jsonSyntax.Id));

        MessagePropertiesEditor.ShowLineNumbers = true;
        MessagePropertiesEditor.Options.ConvertTabsToSpaces = true;
        MessagePropertiesEditor.Options.AllowScrollBelowDocument = true;
        MessagePropertiesEditor.Options.HighlightCurrentLine = true;
        MessagePropertiesEditor.Options.EnableEmailHyperlinks = false;
        MessagePropertiesEditor.Options.EnableHyperlinks = false;
        MessagePropertiesEditor.SearchResultsBrush = new SolidColorBrush(textSearchSectionColor);
        MessagePropertiesEditor.TextArea.IndentationStrategy = new CSharpIndentationStrategy(MessageBodyEditor.Options);
        MessagePropertiesEditor.TextArea.RightClickMovesCaret = true;
        MessagePropertiesEditor.TextArea.SelectionBrush = new SolidColorBrush(textSelectionColor);

        TextMate.Installation messageHeadersEditorTextMateInstallation = MessagePropertiesEditor.InstallTextMate(registryOptions);
        messageHeadersEditorTextMateInstallation.SetGrammar(registryOptions.GetScopeByLanguageId(jsonSyntax.Id));
    }

    private void Current_ActualThemeVariantChanged(object? sender, System.EventArgs e)
    {
        InitiateEditors(Application.Current!.ActualThemeVariant.IsDark()
            ? ThemeName.DarkPlus
            : ThemeName.LightPlus);
    }

    private ThemeName GetThemeVariant(ThemeVariant themeVariant)
    {
        return themeVariant.IsDark()
            ? ThemeName.DarkPlus
            : ThemeName.LightPlus;
    }
}
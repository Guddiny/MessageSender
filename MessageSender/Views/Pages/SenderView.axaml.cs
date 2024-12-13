using ActiproSoftware.UI.Avalonia.Themes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using AvaloniaEdit.Indentation.CSharp;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;
namespace MessageSender.Views.Pages;

public partial class SenderView : UserControl
{
    public SenderView()
    {
        InitializeComponent();
        Application.Current!.ActualThemeVariantChanged += Current_ActualThemeVariantChanged;
        InitiateEditors(GetThemeVariant(Application.Current.ActualThemeVariant));
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

        MessageHeadersEditor.ShowLineNumbers = true;
        MessageHeadersEditor.Options.ConvertTabsToSpaces = true;
        MessageHeadersEditor.Options.AllowScrollBelowDocument = true;
        MessageHeadersEditor.Options.HighlightCurrentLine = true;
        MessageHeadersEditor.Options.EnableEmailHyperlinks = false;
        MessageHeadersEditor.Options.EnableHyperlinks = false;
        MessageHeadersEditor.SearchResultsBrush = new SolidColorBrush(textSearchSectionColor);
        MessageHeadersEditor.TextArea.IndentationStrategy = new CSharpIndentationStrategy(MessageBodyEditor.Options);
        MessageHeadersEditor.TextArea.RightClickMovesCaret = true;
        MessageHeadersEditor.TextArea.SelectionBrush = new SolidColorBrush(textSelectionColor);

        TextMate.Installation messageHeadersEditorTextMateInstallation = MessageHeadersEditor.InstallTextMate(registryOptions);
        messageHeadersEditorTextMateInstallation.SetGrammar(registryOptions.GetScopeByLanguageId(jsonSyntax.Id));

        SelectedIncomingMessageEditor.ShowLineNumbers = true;
        SelectedIncomingMessageEditor.Options.ConvertTabsToSpaces = true;
        SelectedIncomingMessageEditor.Options.AllowScrollBelowDocument = true;
        SelectedIncomingMessageEditor.Options.HighlightCurrentLine = true;
        SelectedIncomingMessageEditor.Options.EnableEmailHyperlinks = false;
        SelectedIncomingMessageEditor.Options.EnableHyperlinks = false;
        SelectedIncomingMessageEditor.SearchResultsBrush = new SolidColorBrush(textSearchSectionColor);
        SelectedIncomingMessageEditor.TextArea.IndentationStrategy = new CSharpIndentationStrategy(MessageBodyEditor.Options);
        SelectedIncomingMessageEditor.TextArea.RightClickMovesCaret = true;
        SelectedIncomingMessageEditor.TextArea.SelectionBrush = new SolidColorBrush(textSelectionColor);

        TextMate.Installation selectedIncomingMessageEditorTextMateInstallation = SelectedIncomingMessageEditor.InstallTextMate(registryOptions);
        selectedIncomingMessageEditorTextMateInstallation.SetGrammar(registryOptions.GetScopeByLanguageId(jsonSyntax.Id));
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
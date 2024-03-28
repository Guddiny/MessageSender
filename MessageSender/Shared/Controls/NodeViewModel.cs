using MessageSender.ViewModels;
using System.Collections.ObjectModel;

namespace MessageSender.Shared.Controls;

public partial class NodeViewModel : ViewModelBase
{
    public ObservableCollection<NodeViewModel>? SubNodes { get; } = [];

    public string Title { get; }

    public NodeViewModel(string title)
    {
        Title = title;
    }

    public NodeViewModel(string title, ObservableCollection<NodeViewModel> subNodes)
    {
        Title = title;
        SubNodes = subNodes;
    }
}
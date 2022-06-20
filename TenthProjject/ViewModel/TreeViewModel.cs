using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Models;
using Ookii.Dialogs.Wpf;
using TenthProject.DataProvider;
using TenthProject.ViewModel;
using Directory = Models.Directory;
using File = Models.File;

namespace TenthProject.ViewModels;

public class TreeViewModel : Observable
{
    static readonly TreeViewModel DummyChild = new TreeViewModel();

    readonly ObservableCollection<TreeViewModel> _children;
    readonly TreeViewModel _parent;
    public IFileSystemObject _fileSystemObject;

    public string Name => _fileSystemObject.Name;
    public long Size => _fileSystemObject.Size;

    bool _isExpanded;

    protected TreeViewModel(TreeViewModel parent, bool lazeLoadChildren)
    {
        _parent = parent;

        _children = new ObservableCollection<TreeViewModel>();

        if (lazeLoadChildren)
        {
            _children.Add(DummyChild);
        }
    }
    private TreeViewModel()
    {

    }
    public ObservableCollection<TreeViewModel> Children
    {
        get { return _children; }
    }
    public bool HasDummyChild
    {
        get { return this.Children.Count == 1 && this.Children[0] == DummyChild; }
    }
    public bool IsExpanded
    {
        get { return _isExpanded; }
        set
        {
            if (value != _isExpanded)
            {
                _isExpanded = value;
                this.OnPropertyChanged("IsExpanded");
            }

            if (_isExpanded && _parent != null)
            {
                _parent.IsExpanded = true;
            }

            if (this.HasDummyChild)
            {
                this.Children.Remove(DummyChild);
                this.LoadChildren();
            }
        }
    }
    protected virtual void LoadChildren()
    {

    }
    public TreeViewModel Parent
    {
        get { return _parent; }
    }
}
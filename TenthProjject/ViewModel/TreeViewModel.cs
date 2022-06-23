using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenthProject.ViewModel
{
    public class TreeViewModel : Observable
    {
        static readonly TreeViewModel DummyChild = new TreeViewModel();

        readonly ObservableCollection<TreeViewModel> _children;
        private TreeViewModel _parent;
        public IFileSystemObject FileSystemObject;

        public string Name;
        public long Size;
        public string Path;

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
        public TreeViewModel()
        {
            _children = new ObservableCollection<TreeViewModel>();
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
            set { _parent = value; }
        }
    }
}

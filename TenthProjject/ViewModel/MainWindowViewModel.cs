using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TenthProject.DataProvider;

namespace TenthProject.ViewModel
{
    public enum DisplayedUnit
    {
        Kilobyte,
        Megabyte,
        Gigabyte
    }
    public class MainWindowViewModel
    {
        public static long size;
        private AlalyzeProvider dataProvider = new AlalyzeProvider();
        public MainWindowViewModel()
        {
            KB = new DelegateCommand.DelegateCommand(OnClick_KB);
            MB = new DelegateCommand.DelegateCommand(OnClick_MB);
            GB = new DelegateCommand.DelegateCommand(OnClick_GB);
            ChooseDrive = new DelegateCommand.DelegateCommand(OnClick_ChooseDrive);
        }

        public DisplayedUnit unit { get; set; }
        public ICommand ChooseDrive { get; set; }
        public ICommand KB { get; private set; }
        public ICommand MB { get; private set; }
        public ICommand GB { get; private set; }

        public void OnClick_ChooseDrive(object obj)
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            dialog.ShowDialog();
            var DriveData = dataProvider.ScanDirectory(dialog.SelectedPath);
            size = DriveData.Size / 1024;
            unit = DisplayedUnit.Kilobyte;
            MessageBox.Show(size.ToString());
        }
        public void OnClick_KB(object obj)
        {
            if (unit == DisplayedUnit.Gigabyte)
            {
                size = size * 1024 * 1024;
            }
            else if (unit == DisplayedUnit.Megabyte)
            {
                size = size * 1024;
            }
            unit = DisplayedUnit.Kilobyte;
            MessageBox.Show(size.ToString("#.##") + " kb");
        }
        public void OnClick_MB(object obj)
        {
            if (unit == DisplayedUnit.Gigabyte)
            {
                size = size * 1024;
            }
            else if (unit == DisplayedUnit.Kilobyte)
            {
                size = size / 1024;
            }
            unit = DisplayedUnit.Megabyte;
            MessageBox.Show(size.ToString("#.##") + " mb");
        }
        public void OnClick_GB(object obj)
        {
            if (unit == DisplayedUnit.Megabyte)
            {
                size = size / 1024;
            }
            else if (unit == DisplayedUnit.Kilobyte)
            {
                size = size / 1024 / 1024;
            }
            unit = DisplayedUnit.Gigabyte;
            MessageBox.Show(size.ToString("#.##") + " gb");
        }
    }
}

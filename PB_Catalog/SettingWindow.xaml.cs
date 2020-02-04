using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace PB_Catalog
{
    //This class is a setting window for user to set a upload path.
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();
            UploadPathBox.Text = MainWindow.getUploadPath();
        }    

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.setUploadPath(UploadPathBox.Text);
            Close();
        }

        //Browse Button Click event handler to display file browser and return the file path.
        private void Browse_Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowDialog();
            UploadPathBox.Text = folderBrowserDialog.SelectedPath;
        }

        //Cancle Button Click event handler to close the setting window.
        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}

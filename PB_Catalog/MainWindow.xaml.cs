using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Image = System.Drawing.Image;
using Point = System.Windows.Point;

namespace PB_Catalog
{
    public partial class MainWindow : Window
    {
        private static string uploadPath = "";
        private static BitmapImage bimap;
        public MainWindow()
        {
            InitializeComponent();
            duplicationWarning.Visibility = Visibility.Hidden;
            uploadPath = validateUploadPath();
            openDuplicate.Visibility = Visibility.Hidden;
        }

        protected override void OnClosed(EventArgs e)
        {
            Application.Current.Shutdown();
        }

        //Read path to upload from registry, if not exists, create key
        private string validateUploadPath()
        {
            string regPath=null;
            try
            {
                regPath = (string)Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("CEB").OpenSubKey("Catalog").GetValue("Path");
            }
            catch
            {
                RegistryKey WriteCEB = Registry.CurrentUser.CreateSubKey("Software").CreateSubKey("CEB").CreateSubKey("Catalog");
                WriteCEB.SetValue("Path", "N/A");
                WriteCEB.Close();
            }
            return regPath;
        }

        //Check if registry path is configured.
        private bool checkUploadPath()
        {
            string regPath = regPath = (string)Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("CEB").OpenSubKey("Catalog").GetValue("Path");
            if(regPath==@"N/A")
            {
                MessageBox.Show("You have to set the upload destination folder.");
                return false;
            }
            else
            {
                return true;
            }
        }

        //Setting upload path in registry.
        public static void setUploadPath(string path)
        {
            RegistryKey WriteCEB = Registry.CurrentUser.CreateSubKey("Software").CreateSubKey("CEB").CreateSubKey("Catalog");
            WriteCEB.SetValue("Path", path);
            WriteCEB.Close();
            uploadPath = (string)Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("CEB").OpenSubKey("Catalog").GetValue("Path");
            MessageBox.Show("Upload path has been updated to " + uploadPath);
        }

        //Getting upload path.
        public static string getUploadPath()
        {
            return uploadPath;
        }

        //Upload button Event hanlder when upload button is clicked.
        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            if (checkUploadPath())
            {
                MessageBox.Show("you have to set the location to upload before upload.");
            }
        }

        //Check if file or link is image file through checking extention.
        private bool checkValidPath(string text)
        {
            string[] picture = { "jpg", "jpeg", "gif", "png", "tiff", "bmp" };
            string extension = text.Substring(text.LastIndexOf(".") + 1);
            if ((text.StartsWith("http") || File.Exists(text)) && picture.Contains(extension.ToLower()))
            {
                return true;
            }
            else
            {
                MessageBox.Show("Please verify the path of source.");
                return false;
            }
        }

        //Clear Button event handler.
        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            pathBox.Clear();
            ItemIDbox.Clear();
            imgbox.Source = null;
        }

        //Open File Dialog to set the file
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.jpg ; *.jpeg ;*.gif ;*.png ;*.tiff ;*.bmp)|*.jpg; *.JPG; *.jpeg ; *.JPEG; *.gif; *.GIF ;*.png; *.PNG; *.tiff; *.TIFF; *.bmp; *.BMP" ;
            if (openFileDialog.ShowDialog() == true)
                pathBox.Text = openFileDialog.FileName;
        }

        //Setting Button click Event handler to open setting window
        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow settingWindow = new SettingWindow();
            settingWindow.Show();
        }

        //Read Event Handler to load image from path field.
        private void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            if (checkValidPath(pathBox.Text))
            {
                ImageSource imageSource = new BitmapImage(new Uri(pathBox.Text));
                imgbox.Source = imageSource;
            }
        }

        //Check if picture exists, and warn with red color.
        private void itemIDbox_Validation(object sender, TextChangedEventArgs e)
        {
            if (File.Exists(uploadPath + @"\" + ItemIDbox.Text.Trim() + @".png"))
            {
                duplicationWarning.Visibility = Visibility.Visible;
                openDuplicate.Visibility = Visibility.Visible;
                TooltipItemID.Content = ItemIDbox.Text.Trim();
                bimap = new BitmapImage();
                bimap.BeginInit();
                bimap.UriSource = new Uri(uploadPath + @"\" + ItemIDbox.Text.Trim() + @".png");
                bimap.CacheOption = BitmapCacheOption.OnLoad;
                bimap.EndInit();
                CurrentImage.Source = bimap;
            }
            else
            {
                duplicationWarning.Visibility = Visibility.Hidden;
                openDuplicate.Visibility = Visibility.Hidden;
            }
        }

        //Read image from file, resize, and save to to uploadpath
        private void upload()
        {
            Image bitmap = Image.FromFile(pathBox.Text);
            Image targetIMG = new Bitmap(200, 200);
            using (Graphics graphicsHandle = Graphics.FromImage(targetIMG))
            {
                graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(bitmap, 0, 0, 200, 200);
            }
            FileStream fs = File.Create(uploadPath + @"\" + ItemIDbox.Text.Trim() + @".png");
            targetIMG.Save(fs, ImageFormat.Png);
            fs.Close();
        }

        //Clear all fields.
        private void reset()
        {
            pathBox.Clear();
            ItemIDbox.Clear();
            imgbox.Source = null;
        }

        //Upload button click event.
        private void Upload_Button_Click(object sender, RoutedEventArgs e)
        {
            if (checkUploadPath()) {
                if (checkValidPath(pathBox.Text))
                {
                    ImageSource imageSource = new BitmapImage(new Uri(pathBox.Text));
                    imgbox.Source = imageSource;
                    if (ItemIDbox.Text.Trim().Length > 0)
                    {
                        try
                        {
                            if (File.Exists(uploadPath + @"\" + ItemIDbox.Text.Trim() + @".png"))
                            {
                                MessageBoxResult result = MessageBox.Show("File already exists, do you want to overwrite?", "OverWrite", MessageBoxButton.YesNo);
                                switch (result)
                                {
                                    case MessageBoxResult.Yes:
                                        bimap = null;
                                        CurrentImage.Source = null;
                                        File.Delete(uploadPath + @"\" + ItemIDbox.Text.Trim() + @".png");
                                        CurrentImage.Source = new BitmapImage(new Uri(uploadPath + @"\" + ItemIDbox.Text.Trim() + @".png"));
                                        upload();
                                        MessageBox.Show("Upload completed");
                                        reset();
                                        break;
                                    case MessageBoxResult.No:
                                        MessageBox.Show("Uploaded has been cancelled", "OverWrite");
                                        break;
                                }
                            }
                            else
                            {
                                upload();
                                MessageBox.Show("Upload completed");
                                reset();
                            }
                        }
                        catch (IOException ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Upload failed.");
                        }

                    }
                    else
                    {
                        MessageBox.Show("Item ID field is required.");
                    }
                }
            }
        }

        //pathbox drop event handler to display file path in path box.
        private void pathBox_Drop(object sender, DragEventArgs e)
        {
            
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                pathBox.Text = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            }
        }

        //PreviewDrageOver event to display picture.
        private void pathBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }
    }
}

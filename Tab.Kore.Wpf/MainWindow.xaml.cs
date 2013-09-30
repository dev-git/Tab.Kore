using System;
using System.Collections.Generic;
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

using System.IO;

namespace Tab.Kore.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string startDir = "C:\\";
        private int directoryCount = 0;
        private int fileCount = 0;
        private string fullPath = String.Empty;
        private FileInfo[] fis = null;

        public MainWindow()
        {
            InitializeComponent();

            PopulateTree();
        }

        private void PopulateTree()
        {
            /*var item1 = new TreeViewItem() { Header = "Item 1", Name = "Item1" };
            trvDirectory.Items.Add(item1);
            item1.RegisterName("Item1", item1);

            var item2 = new TreeViewItem() { Header = "Item 2", Name = "Item2" };
            trvDirectory.Items.Add(item2);
            item1.RegisterName("Item2", item2);

            var item3 = new TreeViewItem() { Header = "Item 3", Name = "Item3" };
            trvDirectory.Items.Add(item3);
            item1.RegisterName("Item3", item3);

            var i2 = trvDirectory.FindName("Item2") as TreeViewItem;
            var subitem = new TreeViewItem() { Header = "SubItem 1" };
            i2.Items.Add(subitem);

            return;*/


            
            /*TreeViewItem item1 = new TreeViewItem() { Header = "C:", Name = "Directory" };
            trvDirectory.Items.Add(item1);
            item1.RegisterName("Directory", item1);*/

            GetDirectoryList(startDir, String.Empty);
        }
        

        private void GetDirectoryList(string pathName, string searchName)
        {
            
            try
            {
                DirectoryInfo[] di = new DirectoryInfo(pathName).GetDirectories();
                string dirName = String.Empty;
                foreach (DirectoryInfo dir in di)
                {
                    dirName = "dir" + directoryCount.ToString();
                    

                    var subDir = trvDirectory.FindName(searchName) as TreeViewItem;
                    if (subDir != null)
                    {
                        if (subDir.Items.Count != di.Length)
                        {
                            TreeViewItem trv =
                                new TreeViewItem() { Header = String.Format("{0} - {1:N0} MB ", dir.Name, GetDirectorySize(dir) / 1000000), Name = dirName, Tag = dir.FullName + System.IO.Path.DirectorySeparatorChar };
                            subDir.Items.Add(trv);
                            subDir.RegisterName(dirName, trv);
                            subDir.ExpandSubtree();
                        }

                    }
                    else
                    {
                        TreeViewItem trv = new TreeViewItem() { Header = dir.Name, Name = dirName, Tag = dir.FullName + System.IO.Path.DirectorySeparatorChar };
                        trvDirectory.Items.Add(trv);
                        trvDirectory.RegisterName(dirName, trv);
                    }
                    directoryCount++;
                }
            }
            catch
            {
            }
            
        }

        private void trvDirectory_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            string givenDirName = ((TreeViewItem)e.NewValue).Name;
            GetDirectoryList(((TreeViewItem)e.NewValue).Tag.ToString(), givenDirName);
        }

        /// <summary>
        /// Gets the size of the directory.
        /// </summary>
        /// <param name="d">The command.</param>
        /// <returns></returns>
        public static long GetDirectorySize(DirectoryInfo d)
        {
            long dirSize = 0;
            try
            {
                // Add file sizes.
                FileInfo[] files = d.GetFiles();
                foreach (FileInfo fi in files)
                {
                    dirSize += fi.Length;
                }
                // Add subdirectory sizes.
                DirectoryInfo[] dis = d.GetDirectories();
                foreach (DirectoryInfo di in dis)
                {
                    dirSize += GetDirectorySize(di);
                }
      
            }
            catch (UnauthorizedAccessException uaex)
            {
                dirSize = -1000000;
            }
            catch
            {
                dirSize = -2000000;
            }
            return (dirSize) ;
        }

        private void btnGetFiles_Click(object sender, RoutedEventArgs e)
        {
            LoadFiles();
        }

        private void LoadFiles()
        {
            lstFiles.Items.Clear();
            fullPath = ((TreeViewItem)trvDirectory.SelectedItem).Tag.ToString();
            DirectoryInfo di = new DirectoryInfo(fullPath);
            fis = di.GetFiles().OrderBy(f => f.Extension).ToArray();
            /*var sort = from fn in fis
                       orderby new FileInfo(fn).Length descending
                       select fn;
            fis.OrderBy(FileInfo*/
            foreach (FileInfo fi in fis)
            {
                //File.AppendAllText(@"C:\Data\Material_errors.csv", fi.Name + "\r\n");
                lstFiles.Items.Add(fi.Name);
            }
        }

        private void lstFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewFile();
        }

        private void ViewFile()
        {
            
            ViewWindow vw = new ViewWindow(fullPath + lstFiles.SelectedItem.ToString());
            vw.Show();
            
        }

        private void scvFiles_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

        }

        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            ScrollForward();
        }

        private void ScrollForward()
        {
            if (fileCount <= fis.Length)
            {
                string fileName = fis[fileCount].FullName;
                txtView.Text = String.Empty;
                string[] content = File.ReadAllLines(fileName);
                foreach (string line in content)
                {
                    txtView.Text += line + "\r\n";
                }
                fileCount++;
            }

        }

        private void btnParse_Click(object sender, RoutedEventArgs e)
        {
            ParseFile();
        }

        private void ParseFile()
        {
            string searchText = "Feature Request";
            string fileName = fis[2].FullName;
            txtView.Text = String.Empty;
            string htmlContent = File.ReadAllText(fileName);
            string[] content = htmlContent.Split(new string[] { "> " + searchText +" </font></font>" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in content)
            {
                
                //txtView.Text += line + "\r\n";
                try
                {
                    if (line.StartsWith(" <html") == false)
                    {
                        string title = line.Substring(0, line.IndexOf("<f", 0));
                        string date = line.Substring(line.IndexOf("nowrap>"), line.IndexOf("<tr bgcolor=\"#E8EEF") - line.IndexOf("nowrap>")).Replace("&nbsp;", " ").Replace("nowrap>", " ");
                        txtView.Text += String.Format("{0},{1}\r\n", title, date);
                        File.AppendAllText(@"C:\Temp\" + searchText + ".csv", String.Format("{0},{1}\r\n", title, date));
                    }
                }
                catch { }
            }
           
        }

        private void btnGroup_Click(object sender, RoutedEventArgs e)
        {
            
            //GroupPerfFiles();
        }


        /// <summary>
        /// Deprectated -- Group a set of Jakaar Perf emails in one flat file
        /// </summary>
        private void GroupPerfFiles()
        {
            int xx = 0;
            if (fis != null)
            {
                foreach (FileInfo fi in fis)
                {
                    if (fi.Extension != ".xml")
                    {
                        string txtContents = File.ReadAllText(fi.FullName);
                        if (txtContents.StartsWith("28941"))
                        {
                            File.AppendAllText("C:\\Aspect PT\\Jakaar_4162_files\\Prod_req_28491.csv", String.Format("{0}|{1}", fi.Name, txtContents));
                            xx++;
                        }
                    }
                }
            }
            MessageBox.Show(String.Format("{0} matching files.", xx));
        }
    }
}

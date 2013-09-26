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
using System.Windows.Shapes;

using System.IO;

namespace Tab.Kore.Wpf
{
    /// <summary>
    /// Interaction logic for ViewWindow.xaml
    /// </summary>
    public partial class ViewWindow : Window
    {
        public ViewWindow()
        {
            InitializeComponent();
        }

        public ViewWindow(string fileName)
        {
            InitializeComponent();

            LoadFile(fileName);

            this.Title += " - " + fileName;
        }

        private void LoadFile(string fileName)
        {
            string[] content = File.ReadAllLines(fileName);
            foreach (string line in content)
            {
                txtContent.Text += line + "\r\n";
            }
        }
    }
}

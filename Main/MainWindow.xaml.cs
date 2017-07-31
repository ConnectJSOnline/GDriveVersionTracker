using GDriveClient;
using Google.Apis.Drive.v3.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public  int LatestRevID { get; set; }
        public DateTime LastModifiedTime { get; set; }
        string fileID = string.Empty;
        GDrive g;
        ObservableCollection<Revision> FileRevisions;
        winNotify win;
        int delay = 0;

        public delegate void UpdateRevisionListCallback(Revision item);
        public void UpdateRevisionList(Revision item)
        {
            item.ETag = item.ModifiedTime.Value.ToString("dd-MM-yyyy, hh:mm tt");
            FileRevisions.Insert(0, item);
            LatestRevID = int.Parse(FileRevisions.FirstOrDefault().Id);
            LastModifiedTime = FileRevisions.FirstOrDefault().ModifiedTime.Value;
            if (item.Kind == "notify")
            {
                win.lblPerson.Content = item.LastModifyingUser.DisplayName;
                win.lblTime.Content = item.ETag;
                win.Show();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            g = new GDrive(ServiceType.Drive);
            FileRevisions = new ObservableCollection<Revision>();
            win = new winNotify();
            XmlDocument doc = new XmlDocument();
            doc.Load("conf.xml");
            fileID = doc.GetElementsByTagName("file-id")[0].InnerText.Trim();
            delay = int.Parse(doc.GetElementsByTagName("delay")[0].InnerText.Trim());
        }

        private void winMain_Loaded(object sender, RoutedEventArgs e)
        {
            lstRevisions.ItemsSource = FileRevisions;
            GetRevisions();
            RevisionPooling();
        }

        public async void GetRevisions()
        {
            await Task.Run(() =>
            {
                List<Revision> Rs = g.GetRevisions(fileID);
                foreach (Revision item in Rs)
                    this.lstRevisions.Dispatcher.Invoke(new UpdateRevisionListCallback(this.UpdateRevisionList), item);
            });
        }

        public async void RevisionPooling()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    if(FileRevisions.Count > 0)
                    {
                        Revision r = g.GetLatestRevision(fileID);
                        if (r != null && r.ModifiedTime > LastModifiedTime)
                        {
                            r.Kind = "notify";
                            this.lstRevisions.Dispatcher.Invoke(new UpdateRevisionListCallback(this.UpdateRevisionList), r);
                        }
                    }
                    System.Threading.Thread.Sleep(delay);
                }
            });
        }

        private void winMain_Unloaded(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}

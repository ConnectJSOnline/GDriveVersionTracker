using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDriveClient;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace GDriveVersionTracker
{
    class Program
    {
        public static int LatestRevID { get; set; }
        static string fileID = "1XrhDKZtHWMkPmPI8YsaQVsUcdAWpraozHhNB_39dnwM";
        static GDrive g;
        static List<Revision> Revisions;
        static void Main(string[] args)
        {
            g = new GDrive(ServiceType.Drive);
            Revisions = new List<Revision>();
            GetRevisions();
            RevisionPooling();
            Console.ReadKey();
        }

        public static async void GetRevisions()
        {
            await Task.Run(() =>
            {
                List<Revision> Revisions = g.GetRevisions(fileID);
                foreach (Revision item in Revisions)
                {
                    LatestRevID = LatestRevID == 0 ? int.Parse(item.Id) : LatestRevID;
                    Console.WriteLine(item.ModifiedTime + " - " + item.LastModifyingUser.DisplayName);
                }
                System.Threading.Thread.Sleep(2000);
            });
        }

        public static async void RevisionPooling()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    Revision r = g.GetLatestRevision(fileID);
                    if(r!=null && int.Parse(r.Id) > LatestRevID)
                    {
                        LatestRevID = int.Parse(r.Id);
                        Revisions.Insert(0, r);
                        Console.Clear();

                        foreach (Revision item in Revisions)
                            Console.WriteLine(item.ModifiedTime + " - " + item.LastModifyingUser.DisplayName);
                    }
                    System.Threading.Thread.Sleep(2000);
                }
            });
        }
    }
}

using Google.Apis.Drive.v3;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System.Security.Cryptography;
using Google.Apis.Drive.v3.Data;

namespace GDriveClient
{
    public enum ServiceType
    {
        Drive,
        Spreadsheet
    }
    public class GDrive
    {
        public string APIKey { get; set; }
        public DriveService driveService { get; set; }
        public SheetsService sheetService { get; set; }
        public GDrive(ServiceType service)
        {
            var certificate = new X509Certificate2(Directory.GetCurrentDirectory() + "\\ConnectJS-4f5b836be640.p12", "notasecret", X509KeyStorageFlags.Exportable);
            if (service == ServiceType.Drive)
            {
                string[] scopes = new string[] { DriveService.Scope.Drive };

                var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer("connectjs-admin@connectjs-175408.iam.gserviceaccount.com")
                {
                    Scopes = scopes
                }.FromCertificate(certificate));



                driveService = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "ConnectJS",
                    ApiKey = "AIzaSyAseLqm2MH01eTiFbrEE_9WeY5lkl7TcDQ"
                });

            }

            if (service == ServiceType.Spreadsheet)
            {
                string[] scopes = new string[] { SheetsService.Scope.Spreadsheets, SheetsService.Scope.Drive };

                var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer("connectjs-admin@connectjs-175408.iam.gserviceaccount.com")
                {
                    Scopes = scopes
                }.FromCertificate(certificate));

                sheetService = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "ConnectJS",
                    ApiKey = "AIzaSyAseLqm2MH01eTiFbrEE_9WeY5lkl7TcDQ"
                });





            }

        }
        public GDrive()
        {

        }

        public List<Revision> GetRevisions(string fileID)
        {
            if (driveService != null)
            {
                var RevisionRequest = driveService.Revisions.List(fileID);
                RevisionRequest.Fields = "*";
                RevisionList changes = RevisionRequest.Execute();
                return changes.Revisions.OrderByDescending(m=>int.Parse(m.Id)).ToList();
            }
            return new List<Revision>();
        }

        public Revision GetLatestRevision(string fileID)
        {
            if (driveService != null)
            {
                return GetRevisions(fileID).FirstOrDefault();
            }
            return null;
        }

    }
}
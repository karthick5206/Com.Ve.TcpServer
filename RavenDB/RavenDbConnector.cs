using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Operations.Indexes;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Com.Ve.ServerDataReceiver.RavenDB
{
    public static class RavenDbConnector
    {
        static RavenDbConnector()
        {
            try
            {
                Console.WriteLine($"Initializing Raven Db..");
                using (IDocumentSession session = DocumentStoreHolder.Store.OpenSession())
                {
                    Console.WriteLine($"Connected Raven Db.. Opened Session...");

                    string[] indexNames = DocumentStoreHolder.Store.Maintenance.Send(new GetIndexNamesOperation(0, int.MaxValue));
                    Console.WriteLine($"Index count...{indexNames?.Length}");

                    if (indexNames?.Any(i => i == nameof(GpsDataDateTimeIndex)) ?? false)
                        new GpsDataDateTimeIndex().Execute(DocumentStoreHolder.Store);

                    Console.WriteLine($"Index inserted...{nameof(GpsDataDateTimeIndex)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while initializing ...{ex}");
            }
        }

        public static void Add(GpsData gpsData)
        {
            try
            {
                Console.WriteLine($"New data received - {DateTime.Now}");

                using (IDocumentSession session = DocumentStoreHolder.Store.OpenSession())
                {
                    session.Store(gpsData);

                    session.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error while adding ...{ex}");
            }
        }
    }

    public class GpsDataDateTimeIndex : AbstractIndexCreationTask<GpsData>
    {
        public GpsDataDateTimeIndex()
        {
            Map = gpsData =>
              from o in gpsData
              select new
              {
                  o.Data,
                  o.DateTime,
              };
        }
    }

    public static class DocumentStoreHolder
    {
        private static X509Certificate2 clientCertificate = new X509Certificate2(@$"{Directory.GetCurrentDirectory()}/RavenDB/Pfix/free.vibhav.client.certificate.pfx");

        private static readonly Lazy<IDocumentStore> LazyStore =
            new Lazy<IDocumentStore>(() =>
            {
                var store = new DocumentStore
                {
                    Certificate = clientCertificate,
                    Urls = new[] { "https://a.free.vibhav.ravendb.cloud" },
                    Database = "GPSRawData"
                };

                return store.Initialize();
            });

        public static IDocumentStore Store => LazyStore.Value;
    }
    public class GpsData
    {
        public string Id { get; set; }
        public string Data { get; set; }
        public DateTime DateTime => DateTime.Now;
    }
}

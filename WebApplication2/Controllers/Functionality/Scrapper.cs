using HtmlAgilityPack;
using WebApplication2.Models;
using MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace WebApplication2.Controllers.Functionality
{
    public class Scrapper
    {
         public void ScrapeWebPage(string urlToScan,out string[] results)
        {
            ScannedPage currentPage = GetElementsFromPage(urlToScan);
            SaveResultsToDB(currentPage);
            results = getCounters(currentPage);

        }

        /**
         * Get Data from the page using HTMLAgilityPack Library. 
        **/
        private ScannedPage GetElementsFromPage(string urlToScan)
        {
            var Webget = new HtmlWeb();
            //get entire page
            var doc = Webget.Load(urlToScan);

            //get span, link and div elements.
            var spanTags = doc.DocumentNode.SelectNodes("//span");
            var linkTags = doc.DocumentNode.SelectNodes("//a");
            var divTags = doc.DocumentNode.SelectNodes("//div");

            ScannedPage page = new ScannedPage
            {
                SpanCount = spanTags.Count,
                LinkCount = linkTags.Count,
                DivCount = divTags.Count,
                UrlToSave = urlToScan
            };
            return page;

        }


        /*
         * Check if mongoDB Connetion succeeded 
         */
        private static bool ProbeForMongoDbConnection(string connectionString, string dbName)
        {
            var probeTask =
                    Task.Run(() =>
                    {
                        var isAlive = false;
                        var client = new MongoDB.Driver.MongoClient(connectionString);

                        for (var k = 0; k < 6; k++)
                        {
                            client.GetDatabase(dbName);
                            var server = client.Cluster.Description.Servers.FirstOrDefault();
                            isAlive = (server != null &&
                                   server.HeartbeatException == null &&
                                   server.State == MongoDB.Driver.Core.Servers.ServerState.Connected);
                            if (isAlive)
                            {
                                break;
                            }
                            System.Threading.Thread.Sleep(300);
                        }
                        return isAlive;
                    });
            probeTask.Wait();
            return probeTask.Result;
        }


        private  void SaveResultsToDB(ScannedPage pageToAdd)
        {
            //open connection
            string connectionString = WebConfigurationManager.ConnectionStrings["ConnStringDb1"].ToString();
            var isAlive = ProbeForMongoDbConnection(connectionString, "admin");
            
            //for debugging
            Console.WriteLine("Connection to mongodb " + (isAlive ? "successful!" : "NOT successful!"));


            var client = new MongoClient(connectionString);
            IMongoDatabase db = client.GetDatabase("scrapperdata");

            var collection = db.GetCollection<BsonDocument>("scrapedpages");


            //add new item: URL, all three counters
            var documnt = new BsonDocument
            {
                {"URL",pageToAdd.UrlToSave},
                {"Div Count",pageToAdd.DivCount},
                {"Span Count",pageToAdd.SpanCount},
                {"Link Count",pageToAdd.LinkCount},
            };
             collection.InsertOne(documnt);
          


        }

        //get number of urls in DB
        public static int countDBDocs()
        {
            string connectionString = WebConfigurationManager.ConnectionStrings["ConnStringDb1"].ToString();
            var client = new MongoClient(connectionString);
            IMongoDatabase db = client.GetDatabase("scrapperdata");
            var collection = db.GetCollection<BsonDocument>("scrapedpages");          
            var allItems = collection.AsQueryable();
            return  allItems.Count();


        }



     
        //to add the to view later
        public string[] getCounters(ScannedPage curPage)
        {
            string []  result={
                    curPage.UrlToSave,
                    curPage.DivCount.ToString(),
                    curPage.SpanCount.ToString(),
                    curPage.LinkCount.ToString()
            };

            return result;

        }

    }
}
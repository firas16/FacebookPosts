using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using System.Threading;
using System.Data;
using System.Globalization;
using System.Net.Cache;
using System.IO;
using System.Runtime.Serialization.Json;
using CoursUpdate.Domain;
using System.Net.Http;

namespace CoursUpdate
{
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            //Trace.TraceInformation("Mise en route");
            //List<Cours> Liste_cours = new List<Cours>();
            while (true)
            {
                ReadGold();
                Trace.TraceInformation("Sleep");
                System.Threading.Thread.Sleep(1000 * 60);
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }

        public static void ReadGold()
        {
            string urltoken = "https://graph.facebook.com/oauth/access_token?type=client_cred&client_id={158409194584205}&client_secret={6c7155ee0365a983db8ce50cb07a97ab}&fb_exchange_token={EAACQEoPFuI0BALawc7JZAEP4Y7LHYlHZAz5tbSDViEZARNcrNntu54TrrWKxZAP7ONAtY1tkZCQ0pNdWKXw6sOoEnYLGniHJAht8NUkv1biVdrjfhl1utYN6VZAdAZAFHChKS3GTxa2VFEI2Den5xAiguZAEsRYn1a1vGcwIQ8vZCiAZDZD}";


            string url = "https://graph.facebook.com/v2.7/6815841748?fields=posts.limit(5)&access_token=EAACEdEose0cBAJrKIrVCFArIvDM5mh9EzRBhYgdy2f92IsadKFVJMR2CERvKZBxwgANEHPejStZCZBo4NZAqjmunBAXjqlc0ezdRgdQN32tSrV6lLxfRsx4iigN18ZBXUXs8rEMUGYvZCRzx7vJZC9cZBsundkz6kX3yZBUdybCZCyaAZDZD";
            HttpWebRequest rssFeed = (HttpWebRequest)WebRequest.Create(url);
            //var policy = new System.Net.Cache.HttpRequestCachePolicy(HttpCacheAgeControl.MaxAge,
            //                            TimeSpan.FromMinutes(0));
            rssFeed.ContentType = "test/json";
            //httpWebRequest.ContentType = "text/json";
            //rssFeed.CachePolicy = policy;
            HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
           
            rssFeed = (HttpWebRequest)WebRequest.Create(url);

            rssFeed.CachePolicy = noCachePolicy;
            //rssFeed.Headers.Set("Cache-Control", "no-cache");
            //rssFeed.Headers.Set("Cache-Control", "must-revalidate");
            try
            {
                HttpWebRequest request = WebRequest.Create(urltoken) as HttpWebRequest;
                request.ContentType = "application/json; charset=utf-8";


                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(DataResponse));
                    string data = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    DataResponse objResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<DataResponse>(data);
                    DataResponse jsonResponse = objResponse as DataResponse;
                    CassandraApplication CassApp = new CassandraApplication();
                    CassApp.Connect("127.0.0.1");
                    foreach (Post p in jsonResponse.posts.data)
                    {
                        CassApp.PosteAInserer = p;
                        CassApp.CreateSchema();
                    }
        
                    
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string result = reader.ReadToEnd();
                        result += "123";
                    }
                        
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
               
            }
            
            


            //using (Goldy2Entities ge = new Goldy2Entities())
            //{
            //    using (DataSet rssData = new DataSet())
            //    {
                  
            //        rssData.ReadXml(rssFeed.GetResponse().GetResponseStream());
            //        //List<zEnumSources> Listecours = ge.zEnumSources.Join(ge.Cours,t=> t.Cours,t1 => t1.date,(t,t1)=>t). .GroupBy(x=> x.zEnumSources.nom);
            //        foreach (DataRow dataRow in rssData.Tables["item"].Rows)
            //        {
            //            string idSource = null;
            //            DateTime date = DateTime.Today;
            //            try
            //            {
            //                idSource = Convert.ToString(dataRow["id"]);
            //                int i;
            //                if (idSource == "B Napoléon")
            //                    i = 0;
            //                     date = Convert.ToDateTime(dataRow["date"]);
            //                    Cours cours = null;
                        
            //                    cours = ge.Cours.SingleOrDefault(x => x.zEnumSources.nom == idSource && x.date == date);
            //                    if (cours == null)
            //                    {
            //                        try
            //                        {
            //                            zEnumSources source = ge.zEnumSources.SingleOrDefault(x => x.nom == idSource);
            //                            if (source == null)
            //                                source = new zEnumSources { nom = idSource };
            //                            Cours cour = new Cours
            //                            {
            //                                date = date,
            //                                zEnumSources = source,
            //                                valeur = Decimal.Parse(dataRow["value"].ToString(), CultureInfo.InvariantCulture),
            //                                timestamp = DateTime.Now
            //                            };
            //                            ge.Cours.Add(cour);
            //                        }
            //                        catch (InvalidOperationException)
            //                        {
            //                            Trace.TraceError(String.Format("La source {0} existe plusieurs fois", idSource));
            //                        }
            //                        catch (Exception ex)
            //                        {
            //                            Trace.TraceError(ex.ToString());
            //                        }

            //                    }
            //                }
            //                catch (Exception e)
            //                {
            //                    Trace.TraceError(String.Format("Le cours de la source {0} existe plusieurs fois pour la date {1: dd/MM/yy hh:mm:ss}", idSource, date));
            //                }

            //             ge.SaveChanges();


            //                ////UAT database
            //                //Goldy2EntitiesUAT ge1 = new Goldy2EntitiesUAT();
            //                //try
            //                //{
            //                //    Cours cours = ge1.Cours.SingleOrDefault(x => x.zEnumSources.nom == idSource && x.date == date);
            //                //    if (cours == null)
            //                //    {
            //                //        try
            //                //        {
            //                //            zEnumSources source = ge1.zEnumSources.SingleOrDefault(x => x.nom == idSource);
            //                //            if (source == null)
            //                //                source = new zEnumSources { nom = idSource };
            //                //            Cours cour = new Cours
            //                //            {
            //                //                date = date,
            //                //                zEnumSources = source,
            //                //                valeur = Decimal.Parse(dataRow["value"].ToString(), CultureInfo.InvariantCulture),
            //                //                timestamp = DateTime.Now
            //                //            };
            //                //            ge1.Cours.Add(cour);
            //                //        }
            //                //        catch (InvalidOperationException)
            //                //        {
            //                //            Trace.TraceError(String.Format("La source {0} existe plusieurs fois", idSource));
            //                //        }
            //                //        catch (Exception ex)
            //                //        {
            //                //            Trace.TraceError(ex.ToString());
            //                //        }

            //                //    }
            //                //}
            //                //catch (InvalidOperationException)
            //                //{
            //                //    Trace.TraceError(String.Format("Le cours de la source {0} existe plusieurs fois pour la date {1: dd/MM/yy hh:mm:ss}", idSource, date));
            //                //}


                           
            //                // ge1.SaveChanges();

            //            //}
            //            //catch (Exception ex)
            //            //{
            //            //    Trace.TraceError(String.Format("Erreur - id:{0}, date: {1}\n{2}", Convert.ToString(dataRow["id"]), Convert.ToString(dataRow["date"]), ex.Message));
            //            //}
            //        }
            //    }
            //    ge.SaveChanges();
            //}
        }
    }
}

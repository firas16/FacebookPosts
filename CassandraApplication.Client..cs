using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassandra;
using CoursUpdate.Domain;
 
namespace CoursUpdate
{
    public class CassandraApplication
    {
        public Cluster Cluster { get; private set; }
        public ISession Session { get; private set; }
        public object datetime { get; private set; }

        public Post PosteAInserer;
        public void Connect(String node)
        {
            Cluster = Cluster.Builder()
             .AddContactPoint(node)
             .Build();
            Console.WriteLine("Connected to cluster: " + Cluster.Metadata.ClusterName.ToString());
            foreach (var host in Cluster.Metadata.AllHosts())
            {
                Console.WriteLine("Data Center: " + host.Datacenter + ", " +
                    "Host: " + host.Address + ", " +
                    "Rack: " + host.Rack);
            }
            Session = Cluster.Connect();
        }
        public void Close()
        {
            Cluster.Shutdown();
        }
        public void CreateSchema()
        {
            Session.ChangeKeyspace("keyspace1");
            //Session.Execute(
            //    "CREATE KEYSPACE simplex WITH replication " +
            //    "= {'class':'SimpleStrategy', 'replication_factor':3};");
            //Session.Execute(
            //    "CREATE TABLE keyspace1.Post (" +
            //    "id uuid PRIMARY KEY," +
            //    "title text," +
            //    "album text," +
            //    "artist text," +
            //    "tags set<text>," +
            //    "" +
            //    ");");

            Session.Execute(
                "INSERT INTO table2 (id,message) " +
                "VALUES (" + System.Guid.NewGuid().ToString()  +",'"+
                PosteAInserer.Message + "'" +
                
                ");");
        }
        public virtual void LoadData() { }
    }
    
}

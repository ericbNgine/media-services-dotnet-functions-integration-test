using System.IO;
using System.Net.Http;
using System.Text;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace media_functions_for_logic_app.functions
{
    public static class blob_trigger2
    {
        //private static string logicAppUri = @"https://prod-05.westus.logic.azure.com:443/.........";
        private static string logicAppUri = @"https://prod-03.westeurope.logic.azure.com:443/workflows/b9b2808d2cdd4e03b6556bf4b3b7f8b9/triggers/manual/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=t7-GeSlUqaPf5orp34ztzwLjn_FAhLMfZCLTs9g087Y";
        [FunctionName("blob_trigger2")]
        public static void Run([BlobTrigger("samples-workitems/{name}", Connection = "")]Stream myBlob, string name, TraceWriter log)
        {
            log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
         
            var json = new Rootobject();
            json.properties = new Properties();
            json.properties.name = new Name();
            json.properties.name.value = name;
            json.properties.name.type = "string";
            string jsonStr = JsonConvert.SerializeObject(json);
            log.Info($"C# Blob trigger function Processed json data is  : \n {jsonStr} ");
            using (var client = new HttpClient())
            {        
                //var response = client.PostAsync(logicAppUri, new StringContent(name, Encoding.UTF8, "application/json")).Result;
                var response = client.PostAsync(logicAppUri, new StringContent(jsonStr, Encoding.UTF8, "application/json")).Result;
            }
        }
    }
    /* TODO move this in separate class :*/
    public class Rootobject
    {
        public Properties properties { get; set; }
        public string type { get; set; }
    }

    public class Properties
    {
        public Name name { get; set; } 
    }

    public class Name
    {
        public string value { get; set; }
        public string type { get; set; }
    }
}

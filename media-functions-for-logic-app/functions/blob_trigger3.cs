using System.IO;
using System.Net.Http;
using System.Text;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using static media_functions_for_logic_app.SharedLibs.jsonRequestSchemeHelper;

namespace media_functions_for_logic_app.functions
{
    public static class blob_trigger3
    {
        //private static string logicAppUri = @"https://prod-05.westus.logic.azure.com:443/.........";
        private static string logicAppUri = @"https://prod-03.westeurope.logic.azure.com:443/workflows/b9b2808d2cdd4e03b6556bf4b3b7f8b9/triggers/manual/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=t7-GeSlUqaPf5orp34ztzwLjn_FAhLMfZCLTs9g087Y";
        [FunctionName("blob_trigger3")]
        public static void Run([BlobTrigger("csblob1/{name}", Connection = "")]Stream myBlob, string name, TraceWriter log)
        {
            //To set working folder dyna : Specify your container name in local.settings.json locally or in Application settings on Azure.
            //{            "IsEncrypted": false,    "Values": {        ....         "MyBlobContainer":"samples-workitems"    }        }
            //public static void Run([BlobTrigger("%MyBlobContainer%/{name}", Connection = "")]Stream myBlob, string name, TraceWriter log)

            /*
            //we could also try sthg like : public static void Run(CloudBlockBlob myBlob, TraceWriter log) from actionUpload3 so that we can get full Path of Blob
            CloudBlockBlob yBlob;
            string uri = yBlob.Uri.AbsolutePath;
            string uriS = yBlob.StorageUri;
            */
        log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
         
            var json = new Rootobject();
            json.properties = new Properties();

            json.properties.name = new Name();
            json.properties.name.value = name;
            json.properties.name.type = "string";

            json.properties.pathToItem = new PathToItem();
            json.properties.pathToItem.value = "https://skysignagestorage.blob.core.windows.net/csblob1/" + name;//{"properties":{"name":{"value":"folder1/pitufo.png","type":"string"}},"type":null}
            
            json.properties.sourceContainerPath = new SourceContainerPath();
            json.properties.sourceContainerPath.value = "csblob1"; 


            string jsonStr = JsonConvert.SerializeObject(json);
            log.Info($"C# Blob trigger function Processed json data is  : \n {jsonStr} ");
            using (var client = new HttpClient())
            {        
                //var response = client.PostAsync(logicAppUri, new StringContent(name, Encoding.UTF8, "application/json")).Result;
                var response = client.PostAsync(logicAppUri, new StringContent(jsonStr, Encoding.UTF8, "application/json")).Result;
            }
        }
    }
    
}

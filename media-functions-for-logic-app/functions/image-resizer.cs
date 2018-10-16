using System;
using System.IO;
using System.Net.Http;
using System.Text;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using ImageResizer;
using ImageResizer.ExtensionMethods;
using Newtonsoft.Json;
using static media_functions_for_logic_app.SharedLibs.jsonRequestSchemeHelper;
using System.Drawing;

namespace media_functions_for_logic_app.functions
{
    public static class image_resizer
    {
        //private static string logicAppUri = @"https://prod-05.westus.logic.azure.com:443/.........";
        private static string logicAppUri = @"https://prod-03.westeurope.logic.azure.com:443/workflows/b9b2808d2cdd4e03b6556bf4b3b7f8b9/triggers/manual/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=t7-GeSlUqaPf5orp34ztzwLjn_FAhLMfZCLTs9g087Y";
        [FunctionName("image_resizer")]
        public static void Run([BlobTrigger("gallery/{name}", Connection = "")]Stream myBlob, string name, Stream outputBlob, TraceWriter log)
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
            Image image = Image.FromStream(myBlob);
            int h = image.Height;
            int w = image.Width;
        log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Image Height : {h}  \n Image Width : {w} ");


            var instructions = new Instructions
            {
                Width = 200,
                Mode = FitMode.Carve,
                Scale = ScaleMode.Both
            };
            ImageBuilder.Current.Build(new ImageJob(myBlob, outputBlob, instructions));

            /* >>>>>>>>>>>>>>>>>>>>>>> 
            var json = new Rootobject();
            json.properties = new Properties();

            json.properties.name = new Name();
            json.properties.name.value = name;
            json.properties.name.type = "string";

            json.properties.pathToItem = new PathToItem();
            json.properties.pathToItem.value = "https://skysignagestorage.blob.core.windows.net/csblob1/" + name;//{"properties":{"name":{"value":"folder1/pitufo.png","type":"string"}},"type":null}
            
            json.properties.sourceContainerPath = new SourceContainerPath();
            json.properties.sourceContainerPath.value = "csblob1"; 

            if (IsMediaFile(json.properties.pathToItem.value))
            {
                log.Info($"C# Blob trigger image_resizer function Media Is Image go on ... \n  ");
                string jsonStr = JsonConvert.SerializeObject(json);
                log.Info($"C# Blob trigger function Processed json data is  : \n {jsonStr} ");
                using (var client = new HttpClient())
                {        
                    //var response = client.PostAsync(logicAppUri, new StringContent(name, Encoding.UTF8, "application/json")).Result;
                    var response = client.PostAsync(logicAppUri, new StringContent(jsonStr, Encoding.UTF8, "application/json")).Result;
                }
            }*/
        }
        static string[] mediaExtensions = {
             ".PNG", ".JPG", ".GIF"
        };
        static bool IsMediaFile(string path)
        {
            return -1 != Array.IndexOf(mediaExtensions, Path.GetExtension(path).ToUpperInvariant());
        }
    }
    
}

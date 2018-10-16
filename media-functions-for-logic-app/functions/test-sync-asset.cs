/*

Azure Media Services REST API v2 Function
 
This function declares the asset files in the AMS asset based on the blobs in the asset container.

Input:
{
    "assetId" : "the Id of the asset"
}

Output:
{}

*/

using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.MediaServices.Client;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Collections.Generic;
using System.IO;

namespace media_functions_for_logic_app
{
    public static class test_sync_asset
    {
        // Field for service context.
        private static CloudMediaContext _context = null;

        [FunctionName("sync-asset-new")]
        public static async Task<object> Run([HttpTrigger(WebHookType = "genericJson")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"Webhook was triggered!");

            
        }
    }
}

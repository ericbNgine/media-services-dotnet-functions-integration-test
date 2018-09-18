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
    public static class sync_asset_new
    {
        // Field for service context.
        private static CloudMediaContext _context = null;

        [FunctionName("sync-asset-new")]
        public static async Task<object> Run([HttpTrigger(WebHookType = "genericJson")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"Webhook was triggered!");

            string jsonContent = await req.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(jsonContent);

            log.Info(jsonContent);

            var attachedstoragecred =  KeyHelper.ReturnStorageCredentials();

            if (data.assetId == null)
            {
                // for test
                // data.Path = "/input/WP_20121015_081924Z.mp4";

                return req.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    error = "Please pass assetId in the input object"
                });
            }

            MediaServicesCredentials amsCredentials = new MediaServicesCredentials();
            log.Info($"Using Azure Media Service Rest API Endpoint : {amsCredentials.AmsRestApiEndpoint}");

            try
            {
                AzureAdTokenCredentials tokenCredentials = new AzureAdTokenCredentials(amsCredentials.AmsAadTenantDomain,
                                                new AzureAdClientSymmetricKey(amsCredentials.AmsClientId, amsCredentials.AmsClientSecret),
                                                AzureEnvironments.AzureCloudEnvironment);

                AzureAdTokenProvider tokenProvider = new AzureAdTokenProvider(tokenCredentials);

                _context = new CloudMediaContext(amsCredentials.AmsRestApiEndpoint, tokenProvider);



                // Step 1:  Copy the Blob into a new Input Asset for the Job
                // ***NOTE: Ideally we would have a method to ingest a Blob directly here somehow. 
                // using code from this sample - https://azure.microsoft.com/en-us/documentation/articles/media-services-copying-existing-blob/

                // Get the asset
                string assetid = data.assetId;
                var asset = _context.Assets.Where(a => a.Id == assetid).FirstOrDefault();

                if (asset == null)
                {
                    log.Info($"Asset not found {assetid}");

                    return req.CreateResponse(HttpStatusCode.BadRequest, new
                    {
                        error = "Asset not found"
                    });
                }

                log.Info("Asset found, ID: " + asset.Id);


                string storname = amsCredentials.StorageAccountName;
                string storkey = amsCredentials.StorageAccountKey;
                if (asset.StorageAccountName != amsCredentials.StorageAccountName)
                {
                    if (attachedstoragecred.ContainsKey(asset.StorageAccountName)) // asset is using another storage than default but we have the key
                    {
                        storname = asset.StorageAccountName;
                        storkey = attachedstoragecred[storname];
                    }
                    else // we don't have the key for that storage
                    {
                        log.Info($"Face redaction Asset is in {asset.StorageAccountName} and key is not provided in MediaServicesAttachedStorageCredentials application settings");
                        return req.CreateResponse(HttpStatusCode.BadRequest, new
                        {
                            error = "Storage key is missing"
                        });
                    }
                }
                

                CloudBlobContainer assetContainer = CopyBlobHelpers.GetCloudBlobContainer(storname, storkey, asset.Uri.Segments[1]);
                foreach(String seg in asset.Uri.Segments)
                {
                    log.Info($"> asset.Uri.Segments :" + seg);
                }
              
                /*IEnumerable<CloudBlobContainer> containers = blobClient.ListContainers();
                foreach (CloudBlobContainer item in containers)
                {
                    foreach (IListBlobItem blob in item.ListBlobs())
                    {
                        blobs.Add(string.Format("{0}", blob.Uri.Segments[2]));
                    }
                }
                */
                /*
                //Get a reference to the storage account that is associated with the Media Services account. 
                StorageCredentials mediaServicesStorageCredentials =
                    new StorageCredentials(_storageAccountName, _storageAccountKey);
                var _destinationStorageAccount = new CloudStorageAccount(mediaServicesStorageCredentials, false);

                CloudBlobClient destBlobStorage = _destinationStorageAccount.CreateCloudBlobClient();

                // Get the destination asset container reference
                string destinationContainerName = asset.Uri.Segments[1];
                log.Info($"destinationContainerName : {destinationContainerName}");

                CloudBlobContainer assetContainer = destBlobStorage.GetContainerReference(destinationContainerName);
                */

                log.Info($"assetContainer retrieved");

                // Get hold of the destination blobs
                var blobsPseudo = assetContainer.ListBlobs();
                log.Info($"blobsPseudo retrieved");

                log.Info($"blobsPseudo count : {blobsPseudo.Count()}");
                 
                var aflist = asset.AssetFiles.ToList().Select(af => af.Name);

                /*   OLD WAY 
                 *   foreach (CloudBlockBlob blob in blobs)
                   {
                       if (aflist.Contains(blob.Name))
                       {
                           var assetFile = asset.AssetFiles.Where(af => af.Name == blob.Name).FirstOrDefault();
                           assetFile.ContentFileSize = blob.Properties.Length;
                           assetFile.Update();
                           log.Info($"Asset file updated : {assetFile.Name}");
                       }
                       else
                       {
                           var assetFile = asset.AssetFiles.Create(blob.Name);
                           assetFile.ContentFileSize = blob.Properties.Length;
                           assetFile.Update();
                           log.Info($"Asset file created : {assetFile.Name}");
                       }
                   }
                   */
                log.Info($"Witness 2 ");
                //Adding mecanic to list all sub dir (original just lists elements in given folder and consider them as blob even if they are subdirs)
                //var folders = blobs.Where(b => b as CloudBlobDirectory != null).ToList();

                //this is supposedly a 2 levels scenario... might not suits our case yet (one more level?)
                foreach (IListBlobItem blobItem in blobsPseudo)
                {
                    log.Info($"blobItem found : ");
                    if (blobItem is CloudBlobDirectory)
                    {
                        CloudBlobDirectory directory = (CloudBlobDirectory)blobItem;
                        IEnumerable<IListBlobItem> blobs = directory.ListBlobs();
                        //ICloudBlob bi;
                        foreach (var item in blobs)
                        {
                            log.Info($"item StorageUri : " + item.StorageUri);
                            CloudBlockBlob blob = (CloudBlockBlob)item;
                            blob.FetchAttributes();
                            log.Info($"Blob found uri : " + blob.Uri);
                            log.Info($"Blob found Name : " + blob.Name);
                            String potentialName = Path.GetFileName(blob.Name);
                            log.Info($"Blob potentialName : " + potentialName);
                            if (aflist.Contains(blob.Name))
                            {
                                log.Info($" aflist.Contains Blob found Name : " + blob.Name);
                                var assetFile = asset.AssetFiles.Where(af => af.Name == blob.Name).FirstOrDefault();
                                assetFile.ContentFileSize = blob.Properties.Length;
                                assetFile.Update();
                                log.Info($"Asset file updated : {assetFile.Name}");
                            }
                            else
                            {
                                log.Info($"Create Blob found Name : " + blob.Name);

                                var assetFile = asset.AssetFiles.Create(blob.Name);
                                assetFile.ContentFileSize = blob.Properties.Length;
                                assetFile.Update();
                                log.Info($"Asset file created : {assetFile.Name}");
                            }
                        }
                    } else {
                        CloudBlockBlob blob = (CloudBlockBlob)blobItem;
                        blob.FetchAttributes();
                        log.Info($"Blob2 found uri : " + blob.Uri);
                        log.Info($"Blob2 found Name : " + blob.Name);
                        if (aflist.Contains(blob.Name))
                        {
                            var assetFile = asset.AssetFiles.Where(af => af.Name == blob.Name).FirstOrDefault();
                            assetFile.ContentFileSize = blob.Properties.Length;
                            assetFile.Update();
                            log.Info($"Asset2 file updated : {assetFile.Name}");
                        }
                        else
                        {
                            var assetFile = asset.AssetFiles.Create(blob.Name);
                            assetFile.ContentFileSize = blob.Properties.Length;
                            assetFile.Update();
                            log.Info($"Asset2 file created : {assetFile.Name}");
                        }
                    }
                }
                  
                asset.Update();
                MediaServicesHelper.SetAFileAsPrimary(asset);

                log.Info("Asset updated");
            }
            catch (Exception ex)
            {
                string message = ex.Message + ((ex.InnerException != null) ? Environment.NewLine + MediaServicesHelper.GetErrorMessage(ex) : "");
                log.Info($"ERROR: Exception {message}");
                return req.CreateResponse(HttpStatusCode.InternalServerError, new { error = message });
            }

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}

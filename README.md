media-services-dotnet-functions-test
---
services: media-services,functions
platforms: dotnet
author: johndeu
author2: Ngine Test
---
<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FericbNgine%2Fmedia-services-dotnet-functions-integration-test%2Fmaster%2Fazuredeploy.json" target="_blank">
    <img src="http://azuredeploy.net/deploybutton.png"/>
</a>

# Media Services: Integrating Azure Media Services with Azure Functions and Logic Apps
This project contains examples of using Azure Functions with Azure Media Services.

The project includes several folders of sample Azure Functions for use with Azure Media Services that show workflows related
to ingesting content directly from blob storage, encoding, and writing content back to blob storage. It also includes examples of
how to monitor job notifications via WebHooks and Azure Queues.

## Deploying to Azure
It is **REQUIRED** that you first fork the project and update the "sourceCodeRepositoryURL" in the [azuredeploy.json](azuredeploy.json) template parameters
when deploying to your own Azure account.  That way you can more easily update, experiment and edit the code and see changes
reflected quickly in your own Functions deployment.

We are doing this to save you from our future updates that could break your functions due to continuous integration.

**WARNING**: If you attempt to deploy from the public samples Github repo, and not your own fork, you will see an Error during deployment with a "BadRequest" and an OAuth exception. 
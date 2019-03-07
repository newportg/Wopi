# Web Application Open Platform Interface

## Description

WOPI or to give it its official name, Web Application Open Platform Interface, is an interface protocol that is used between Microsoft Office Online Server and a compatible file store. It was originally defined so Office Online could interact with SharePoint documents, and provide users with a consistent online office experience.

## Requirement

The HUB application generates marketing material in word document format, and stores the files within a Azure Blob store. From time to time the user may need to view or change the document, so will need to be able to read and write back changes. The users have also expressed a preference for using Word to do the editing as it is the word processor they are most familiar with.

## Implementation

Probably the best way to describe the infrastructure of this system is to build it a block at a time.

Step 1,
    Update the hub deployment to make the storage RA-GRS, this will automatically copy data contained in the storage account to a secondary data centre. 
* Make initial steps towards making the solution resistive to disaster conditions.
  * The hub will need to be updated to make use of the circuit breaker pattern so it can read from the secondary data centre if we encounter transient read failures on the primary.
  * If Microsoft determines there is a issue with the primary DC then it will flip writes to the secondary, and will manage the data correction when the primary DC is available again.
* The key here is changing the storage account type to RA-GRS, changing the HUB Web Application to take advantage of this feature is not a necessary change at this point.
 

![Wopi Simple](http://www.plantuml.com/plantuml/proxy?cache=no&src=https://raw.github.com/Kf-GaryNewport/Wopi/master/puml/WopiHubStorageRAGRS.puml)

Step 2,
    If we just focus on Northern Europe, we can see how the WOPI server connects to the hub. 
    
* The Hub Web Application creates documents and stores them in the storage
* The Hub Web Application would include a link to the document in the file store on its web pages. 
* When the use wishes to open the file then the filename and path should be passed to the Office Online Server (OOS) via a IFrame. 
* The OOS calls the WOPI Host requesting the file.
* The Wopi Host validates that it can process the document by viewing the OOS Discovery document.
* The Wopi Host retrieves the document from the storage and returns it to the OSS server to display.

### Shared Components
* On the diagram the shared components, for administrative reasons, are all within the Northern Europe Subscription or the Wopi Host Resource group, but are global components and are accessible to both NE and WE WOPI resource groups.
* CosmosDb
  * Used to store file meta data
* Application insights 
  * Collects log information and can generate simple dashboard charts
  * Log Analytics can provide deeper analysis of Application Insights data
* Azure Active Directory (AAD)
  * All the application components are registered to the Enterprise application via Managed Service Identity (MSI)
  * All users should be assigned to the AAD and given appreciate permissions, preferable via groups.
* Key Vault
  * Keeper of secrets and connection strings.
  * This resource contains the unique values for this environment, enables the resource to redeployable

![Wopi Simple](http://www.plantuml.com/plantuml/proxy?cache=no&src=https://raw.github.com/Kf-GaryNewport/Wopi/master/puml/WopiSimple.puml)

## Final redundant, balanced deployment

This setup enables requests to be routed to either datacentre and provides a level of redundancy and recovery.
The WOPI resource group would be separately deployable to the rest of the HUB application, although it will use the HUB storage account for its file store.

![Wopi Balanced](http://www.plantuml.com/plantuml/proxy?cache=no&src=https://raw.github.com/Kf-GaryNewport/Wopi/master/puml/WopiBalanced2.puml)


## Current State of the POC

The POC has been ongoing for some time on a adhoc basis due to issues that have been encountered and the reliance on other teams to help solve them.

The solution includes a project which implements the Patterns and Practice demo application and an ARM Template deployment script.
The solution has been designed so it can be torn down and redeployed at any time, and relies on the build agent to supply the relevant environmental secrets and connections. Building the application in this way allows us to build the application once and deploy it to many environments, the only difference being the injected environmental values.

### Current issue
Currently the Antares-Alpha-NEU-RG VNet Gateway can see the wopi Office Online Server, but unfortunately the WOPIHost RG cannot.
The issue lies in the routing, James is working through the issues.

![Wopi Balanced](http://www.plantuml.com/plantuml/proxy?cache=no&src=https://raw.github.com/Kf-GaryNewport/Wopi/master/puml/WopiCurrentState.puml)

### 
After commenting out the OOS interaction I have been able to see how the WopiHost handles its files. 
The demo interface consists of a file list, as there are no files in the repository the list is empty.
The headdings bar is active, so you can drag any file onto the heading and it will instigate a file upload.
With the OOS connections disabled the host is not doing any validation, so it will take anything and store it.
The file information is stored in the cosmos db and a encrypted file is stored in the file repository.
One thing to note, as this is storing a new file, it creates a container(directory) in the stroage which is named after the user.

![Wopi filestore](https://raw.github.com/Kf-GaryNewport/Wopi/master/puml/filestore.PNG)

![Wopi filestore](https://raw.github.com/Kf-GaryNewport/Wopi/master/puml/cosmosdb.PNG)

The JSON record that is in the database looks like this.
Most of the elements a null, or default, as it hasnt been able to populate it with data from discovery.

```JSON
{
    "UserId": null,
    "CloseUrl": null,
    "HostEditUrl": null,
    "HostViewUrl": null,
    "SupportsCoauth": false,
    "SupportsExtendedLockLength": false,
    "SupportsFileCreation": false,
    "SupportsFolders": false,
    "SupportsGetLock": true,
    "SupportsLocks": true,
    "SupportsRename": true,
    "SupportsScenarioLinks": false,
    "SupportsSecureStore": false,
    "SupportsUpdate": true,
    "SupportsUserInfo": true,
    "LicensesCheckForEditIsEnabled": true,
    "ReadOnly": false,
    "RestrictedWebViewOnly": false,
    "UserCanAttend": false,
    "UserCanNotWriteRelative": false,
    "UserCanPresent": false,
    "UserCanRename": true,
    "UserCanWrite": true,
    "WebEditingDisabled": false,
    "Actions": null,
    "id": "40f424a1-3aed-498c-b929-92a35152a16e",
    "LockValue": null,
    "LockExpires": null,
    "OwnerId": "gary.newport@knightfrank.com",
    "BaseFileName": "120027 - [Tech] EPC Graph.docx",
    "Container": "gary-newport-knightfrank-com",
    "Size": 1505043,
    "Version": 1,
    "UserInfo": null,
    "_rid": "UjJIANtWPJoBAAAAAAAAAA==",
    "_self": "dbs/UjJIAA==/colls/UjJIANtWPJo=/docs/UjJIANtWPJoBAAAAAAAAAA==/",
    "_etag": "\"5b003fb8-0000-0000-0000-5c812a770000\"",
    "_attachments": "attachments/",
    "_ts": 1551968887
}
```





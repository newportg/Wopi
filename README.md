# Web Application Open Platform Interface

## Description

WOPI or to give it its official name, Web Application Open Platform Interface, is an interface protocol that is used between Microsoft Office Online Server and a compatible file store. It was originally defined so Office Online could interact with SharePoint documents, and provide users with a consistent online office experience.

## Requirement

The HUB application generates marketing material in word document format, and stores the files within a Azure Blob store. From time to time the user may need to view or change the document, so will need to be able to read and write back changes. The users have also expressed a preference for using Word to do the editing as it is the word processor they are most familiar with.

## Implementation

Probably the best way to descibe the infrastructure of this system is to build it a block at a time.

Step 1,
    Update the hub deployment to make the storage RA-GRS, this will automatically copy data contained in the storage account to a secondary data center. 
* Make initial steps towards making the solution resistive to disaster conditions.
  * The hub will need to be updated to make use of the circuit breaker pattern so it can read from the secondary data center if we encounter transient read failures on the primary.
  * If Microsoft determines there is a issue with the primary DC then it will flip writes to the secondary, and will manage the data correction when the primary DC is available again.
* The key here is changing the storage account type to RA-GRS, changing the HUB Web Application to take advantage of this feature is not a necessary change at this point.
 

![Wopi Simple](http://www.plantuml.com/plantuml/proxy?cache=no&src=https://raw.github.com/Kf-GaryNewport/Wopi/master/WopiHubStorageRAGRS.puml)


Step 2,
    If we just focus on Nortern Europe, we can see how the WOPI server connects to the hub. 
    
* The Hub Web Application creates documents and stores them in the storage
* The Hub Web Application would include a link to the document in the file store. 
* When the use wishes to open the file then the filename and path should be passed to the Office Online Server (OOS) via a IFrame. 
* The OOS calls the WOPI Host requesting the file.
* The Wopi Host validates that it can process the document by viewing the OOS Discovery document.
* The Wopi Host retrieves the document from the storage and returns it to the OSS server to display.

### Shared Components
* On the diagram the shared components, for adminster reasons are all within the Northern Europe Subscription or the Wopi Host Resource group, but are global components that are accessible to both NE and WE WOPI resoiurce groups.


![Wopi Simple](http://www.plantuml.com/plantuml/proxy?cache=no&src=https://raw.github.com/Kf-GaryNewport/Wopi/master/WopiSimple.puml)


![Wopi Balanced](http://www.plantuml.com/plantuml/proxy?cache=no&src=https://raw.github.com/Kf-GaryNewport/Wopi/master/WopiBalanced2.puml)


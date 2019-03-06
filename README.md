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
  * The hub will need to be updated to make use of the circuit breaker pattern so it can read from the secondary data center if get transient read failures on the primary.
  * If Microsoft determines there is a issue with the primary DC then it will flip writes to the secondary, and will manage the data correction when the primary dc is available again.
 

![Wopi Simple](http://www.plantuml.com/plantuml/proxy?cache=no&src=https://raw.github.com/Kf-GaryNewport/Wopi/master/WopiHubStorageRAGRS.puml)




![Wopi Simple](http://www.plantuml.com/plantuml/proxy?cache=no&src=https://raw.github.com/Kf-GaryNewport/Wopi/master/WopiSimple.puml)


![Wopi Balanced](http://www.plantuml.com/plantuml/proxy?cache=no&src=https://raw.github.com/Kf-GaryNewport/Wopi/master/WopiBalanced2.puml)


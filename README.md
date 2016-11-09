# field-finder
Relativity Application: The Field Finder solution allows you to extract certain text from a document's OCR text field and populate that text in specified workspace fields. 

While this is an open source project on the kCura github account, there is no support available through kCura for this solution or code.  You are welcome to use the code and solution as you see fit within the confines of the license it is released under. However, if you are looking for support or modifications to the solution, we suggest reaching out to the Project Champion listed below.

# Project Champion 
![TSD Services](http://www.tsdservices.com/wp-content/uploads/2015/03/TSD_Logo-TM-for-website.png "TSD Services")

TSD Services is a major contributor to this project.  If you are interested in having modifications made to this project, please reach out to [TSD Services](http://www.tsdservices.com) for an estimate. 


# Project Setup
This project requires references to kCura's Relativity® SDK dlls.  These dlls are not part of the open source project and must be obtained through kCura.  In the "packages" folder under "Source" you will need to create a "Relativity" folder if one does not exist.  You will need to add the following dlls:
- kCura.Agent.dll
- kCura.EventHandler.dll
- kCura.Relativity.Client.dll
- Relativity.API.dll
- Relativity.CustomPages.dll

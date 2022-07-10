# UPDATask
 Simple Task tom manage data using identity , dataTables, SweetAlert2
##How to run the application locally:
1.	Download and install.Net5 SDK from  https://dotnet.microsoft.com/en-us/download/dotnet/5.0
2.	Restore packages.
3.	Update the connection string in appsetting.json 
4.	Open package manager console 
5.	Navigate to the infrastructure project and run Ef Commands to create DB and pass the project has a connection string using –s ../WEBUI/WEBUI.csproj at the end of each commands 
### Example :
`dotnet ef migrations add migrationName s- ..\WebUI\WebUI.csproj` 
`dotnet ef database update s- ..\WebUI\WebUI.csproj`

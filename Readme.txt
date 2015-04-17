* Solution Projects

1. Khalaf - is main website project
2. Server2 - Does calculation with web api services
3. Server3 - Stores calculated data by Server2

* Khalaf 
1. Install "JSON.NET" and "RestSharp" nuget libraries to send REST requests
2. Set Server2 and Server3 URL addreses in web.config files. For Example:
   <appSettings>   
    <add key="Server2Url" value="http://localhost:11214/"/>
    <add key="Server3Url" value="http://localhost:11227/"/>
  </appSettings>
 

* Server 2
WebApiConfig.cs - Set JSON Media formatter (Line 11) to receive json request

* Server 3
WebApiConfig.cs - Set JSON Media formatter (Line 11) to receive json request
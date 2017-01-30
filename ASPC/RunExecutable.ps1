param 
( 
    [string]$exepathcmd = "C:\Users\Administrator\Desktop\ASPC\ASPC\bin\Debug", #Path to exe file
	[string]$exeurl = "https://pedersundboe1.sharepoint.com/sites/aspc ", #Url to site 
	[string]$exeusr = "pedersundboe@pedersundboe1.onmicrosoft.com", #Username with domain
	[string]$exepass = "Sundbosundbo321", #Password for user
	[string]$environment= "cloud" #dev or cloud
	
)

Set-Location $exepathcmd
Start-Process -FilePath .\ASPC.exe -ArgumentList "$exeurl $exeusr $exepass $environment";

param 
( 
    [string]$exepathcmd = "C:\Users\Administrator\Desktop\ASPC\ASPC\bin\Debug", #Path to exe file
	[string]$exeurl = "URL", #Url to site 
	[string]$exeusr = "USERNAME", #Username with domain
	[string]$exepass = "PASSWORD", #Password for user
	[string]$environment= "dev" #dev or cloud
	
)

Set-Location $exepathcmd
Start-Process -FilePath .\ASPC.exe -ArgumentList "$exeurl $exeusr $exepass $environment";

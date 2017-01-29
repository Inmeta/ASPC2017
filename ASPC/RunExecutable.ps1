param 
( 
    [string]$exepathcmd = "C:\Users\Administrator\Desktop\ASPC\ASPC\bin\Debug", #Path to exe file
	[string]$exeurl = "url", #Url to site 
	[string]$exeusr = "administrator", #Username with domain
	[string]$exepass = "Password01", #Password for user
	[string]$environment= "dev" #dev or cloud
	
)

Set-Location $exepathcmd
Start-Process -FilePath .\ASPC.exe -ArgumentList "$exeurl $exeusr $exepass $environment";

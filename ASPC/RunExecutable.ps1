param 
( 
    [string]$exepathcmd = "PATH", #Path to exe file
	[string]$exeurl = "URL", #Url to site 
	[string]$exeusr = "USERNAME", #Username with domain
	[string]$exepass = "PASSWORD ", #Password for user
	[string]$environment= "cloud", #dev or cloud
	[string]$fulldeploy= "true" #dev or cloud	
)

Set-Location $exepathcmd
Start-Process -FilePath .\ASPC.exe -ArgumentList "$exeurl $exeusr $exepass $environment $fulldeploy";

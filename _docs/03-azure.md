# Azure

## CLI

Listing commands should support the `-o table` display option.
If that does not work well, use `az group list | convertfrom-json`

After its in JSON format, you can select specific fields that you are interested in:
`az group list | convertfrom-json | select name, tags`


List Account 
`az account list`
The id field = subscription id

List AppServices:
`az webapp list  -o table`

Get list of AppService plans:
`az appservice plan list -o table`

Resource group (`az group`)
`az group create --name mini-tools-rg --location southeastasia`

Storage accounts (`az storage account`)
`az storage account create --name minitools --location southeastasia --resource-group mini-tools-rg --sku Standard_LRS`

Display deployment profile (w/out-file to save to file)
`az webapp deployment list-publishing-profiles --resource-group mini-tools-rg --name mini-recep --xml | out-file webapp-publishing-profile.xml`

Create WebApp
`az webapp create -g mini-tools-rg -p mini-tools-appservice-plan -n mini-recep`

Azure AD Service Principal (`az ad sp`)
`az ad sp create-for-rbac --name "MiniRecepSP" --sdk-auth --role contributor --scopes /subscriptions/f5ee2a9b-c2ff-4e67-85ea-6ede57eafd5f/resourceGroups/mini-tools-rg/providers/Microsoft.Web/sites/mini-recep`


## Deployment (w/github actions using Publish Profile)

1.  Create WebApp
2.  Download publish profile
3.  Set up Github Actions


## Deployment (w/github actions using Service Provider)
This is the more generic method.

1.  Create WebApp
2.  Create Credentials
3.  Set up Github Actions


## Troubleshooting

If your application cannot start up, (500.30), 
try running the site via kudu debug shell (PowerShell / CMD).
In `..site\wwwroot` run `dotnet .\Recep.dll` 


## Configuration

Configuration variables:

Recep
RECEP_SIGNING_KEY


```ps1 Generate Private/Public keys
$t4 = [System.Security.Cryptography.RSA]::Create()
$t4.ToXmlString($true) | Out-File C:\data\rsa\RECEP-PrivateKey.xml
$t4.ToXmlString($false) | Out-File C:\data\rsa\RECEP-PublicKey.xml
```

## PKI note

Public keys are used to:

1.  Encrypt a message to other individuals on the system
2.  Confirm a signature signed by someone???s private key

Private keys are used to:

1.  decrypt a message secured by your public key
2.  sign your message with your private key so that the recipients know the message could only have come from you.



# Reference

https://github.com/Azure/webapps-deploy


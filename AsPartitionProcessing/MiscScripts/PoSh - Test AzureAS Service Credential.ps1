Install-Module -Name Azure.AnalysisServices
Install-Module -Name SqlServer

Update-Module -Name Azure.AnalysisServices
Update-Module -Name SqlServer


$AppId = "9964d45f-ab47-4b2e-804d-16ccc0743e62"
$PlainPWord = "vFnC2OQ5bouGK3yiteKkBv1DXKwG2ZTZ+9Jfso8wi34="
$TenantId = "54fd453c-3cc6-41e8-8200-fa568f04164a"

$PWord = ConvertTo-SecureString -String $PlainPWord -AsPlainText -Force

$Credential = New-Object -TypeName "System.Management.Automation.PSCredential" -ArgumentList $AppId, $PWord

Login-AzureAsAccount -Credential $Credential -ServicePrincipal -TenantId $TenantId -RolloutEnvironment "southcentralus.asazure.windows.net"

Invoke-ProcessTable -Server "asazure://southcentralus.asazure.windows.net/opifexdemotabular" -TableName "Date" -Database "ASPP_AdventureWorksDW1200" -RefreshType "Full"
Login-AzureRmAccount 
 Set-AzureRmContext -SubscriptionId 121c53d3-e96d-4890-af9a-58b104da7215 -Tenant 55a71488-bbff-4451-a18d-a1bfa479293b
 Set-AzureRmKeyVaultAccessPolicy -VaultName wopihost-kv-dev-ne -ServicePrincipalName abfa0a7c-a6b6-4736-8310-5855508787cd -PermissionsToSecrets get

/*

$pwd = 'kXj1OfX5'
$file = '.\hub_wildcard_cert_12_2018.pfx'
$fullPath = Get-ChildItem $file
$certCollection = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2Collection
$certCollection.Import($fullPath.FullName, $pwd, [System.Security.Cryptography.X509Certificates.X509KeyStorageFlags]::Exportable)
$certExport = $certCollection.Export([System.Security.Cryptography.X509Certificates.X509ContentType]::Pkcs12)
$certb64 = [System.Convert]::ToBase64String($certExport)
$certSecureString = ConvertTo-SecureString -String $certb64 -AsPlainText –Force
*/
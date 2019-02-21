Login-AzureRmAccount 
 Set-AzureRmContext -SubscriptionId 121c53d3-e96d-4890-af9a-58b104da7215 
 Set-AzureRmKeyVaultAccessPolicy -VaultName wopihost-kv-dev-ne -ServicePrincipalName abfa0a7c-a6b6-4736-8310-5855508787cd -PermissionsToSecrets get
 var idClient = ConfidentialClientApplicationBuilder.Create("038f9c30-e496-40ed-ba3b-36d4eb41c390")
                           .WithAuthority(AzureCloudInstance.AzurePublic, "7fbe498c-e540-4556-b198-7b2e16bff9b0")
                           .WithClientSecret("1lC-f0u26bVzg-v6o-LnfbW0ql.qc8_4~8")
                           .Build();

                string[] scopes = new string[] { "https://graph.microsoft.com/.default" };
                var result = idClient.AcquireTokenForClient(scopes)
                           .ExecuteAsync().Result;
                return ResultApi(new { result.AccessToken, result.ExpiresOn, result.IdToken, result.ExtendedExpiresOn });
 public static async Task GetATokenForGraph()
        {
            string authority = "https://fs.epm.com.co/adfs/";
            string[] scopes = new string[] { "openid profile email" };
            IPublicClientApplication app;
            app = PublicClientApplicationBuilder.Create("<IdClient>")
                                              .WithAuthority(authority)
                                              .Build();
            var accounts = await app.GetAccountsAsync();

            AuthenticationResult result = null;
            if (accounts.Any())
            {
                result = await app.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                                  .ExecuteAsync();
            }
            else
            {
                try
                {
                    var securePassword = new SecureString();
                    foreach (char c in "<ClaveUsuario>")        
                        securePassword.AppendChar(c); 

                    result = await app.AcquireTokenByUsernamePassword(scopes,
                                                                     "<CorreoUsuario>",
                                                                     securePassword)
                                     .ExecuteAsync();
                }
                catch (MsalUiRequiredException ex) when (ex.Message.Contains("AADSTS65001"))
                {
                }
                catch (MsalServiceException ex) when (ex.ErrorCode == "invalid_request")
                {
                   

                }
                catch (MsalServiceException ex) when (ex.ErrorCode == "unauthorized_client")
                {
                   
                }
                catch (MsalServiceException ex) when (ex.ErrorCode == "invalid_client")
                {
                   
                }
                catch (MsalServiceException)
                {
                    throw;
                }

                catch (MsalClientException ex) when (ex.ErrorCode == "unknown_user_type")
                {
                 
                    throw new ArgumentException("U/P: Wrong username", ex);
                }
                catch (MsalClientException ex) when (ex.ErrorCode == "user_realm_discovery_failed")
                {
                   
                    throw new ArgumentException("U/P: Wrong username", ex);
                }
                catch (MsalClientException ex) when (ex.ErrorCode == "unknown_user")
                {
                  
                    throw new ArgumentException("U/P: Wrong username", ex);
                }
                catch (MsalClientException ex) when (ex.ErrorCode == "parsing_wstrust_response_failed")
                {
                  
                }
            }

            Console.WriteLine("Token -> " + result.AccessToken);
        }
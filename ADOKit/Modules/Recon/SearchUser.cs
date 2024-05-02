using System;
using System.Net;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Collections.Generic;

namespace ADOKit.Modules.Recon
{
    class SearchUser
    {
        public static async Task execute(string credential, string url, string search)
        {
            // Generate module header
            Console.WriteLine(Utilities.ArgUtils.GenerateHeader("searchuser", credential, url, search, "", "", ""));

            // ignore SSL errors
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // check if credentials provided are valid
            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Checking credentials provided");
            Console.WriteLine("");

            // if creds valid, then provide message and continue
            if (await Utilities.WebUtils.credsValid(credential, url))
            {
                Console.WriteLine("[+] SUCCESS: Credentials provided are VALID.");
                Console.WriteLine("");

                try
                {

                    // create table header
                    string tableHeader = string.Format("{0,50} | {1,50} | {2,50}", "Username", "Display Name", "UPN");
                    Console.WriteLine(tableHeader);
                    Console.WriteLine(new String('-', tableHeader.Length));

                    // get a listing of all users in the Azure DevOps instance
                    List<Objects.User> userList = await Utilities.UserUtils.getAllUsers(credential, url);

                    // iterate through the list of users and list them
                    foreach (Objects.User user in userList)
                    {

                        // list the user if there is a match for what is being searched for
                        if (user.directoryAlias.ToLower().Contains(search.ToLower()) || user.principalName.ToLower().Contains(search.ToLower()) )
                        {

                            Console.WriteLine("{0,50} | {1,50} | {2,50}", user.directoryAlias, user.displayName, user.principalName);
                        }

                    }

                    Console.WriteLine("");


                }
                catch (Exception ex)
                {
                    Console.WriteLine("");
                    Console.WriteLine("[-] ERROR: " + ex.Message);
                    Console.WriteLine("");
                }

            }

            // if creds not valid, display message and return
            else
            {
                Console.WriteLine("[-] ERROR: Credentials provided are INVALID. Check the credentials again.");
                Console.WriteLine("");
                return;
            }

        }



    }
}

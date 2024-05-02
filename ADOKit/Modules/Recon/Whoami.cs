using System;
using System.Net;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Collections.Generic;

namespace ADOKit.Modules.Recon
{
    class Whoami
    {

        public static async Task execute(string credential, string url)
        {
            // Generate module header
            Console.WriteLine(Utilities.ArgUtils.GenerateHeader("whoami", credential, url, "", "", "", ""));

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

                    // get the current user that is being authenticated as
                    string currentUser = await Utilities.UserUtils.getCurrentUser(credential, url);
                    string[] splitCurrentUser = currentUser.Split(':');


                    // get a listing of all users in the Azure DevOps instance
                    List<Objects.User> userList = await Utilities.UserUtils.getAllUsers(credential, url);

                    // iterate through the list of users and find our user
                    foreach (Objects.User user in userList)
                    {


                        // if we have found our current user we are authenticating as, list user details
                        if (user.principalName.ToLower().Equals(splitCurrentUser[1].ToLower()))
                        {
                            Objects.User ourUser = await Utilities.UserUtils.getUserDetails(credential, url, user.descriptor, user.principalName);
                            Console.WriteLine("{0,50} | {1,50} | {2,50}", ourUser.directoryAlias, ourUser.displayName, ourUser.principalName);

                            Console.WriteLine("");
                            Console.WriteLine("");
                            Console.WriteLine("[*] INFO: Listing group memberships for the current user");
                            Console.WriteLine("");
                            Console.WriteLine("");

                            // create table header
                            string tableHeaderGroups = string.Format("{0,70} | {1,50} | {2,50}", "Group UPN", "Display Name", "Description");
                            Console.WriteLine(tableHeaderGroups);
                            Console.WriteLine(new String('-', tableHeaderGroups.Length));


                            // get a listing of all groups in the Azure DevOps instance
                            List<Objects.Group> groupList = await Utilities.GroupUtils.getAllGroups(credential, url);

                            // iterate through the list of groups
                            foreach (Objects.Group group in groupList)
                            {

                                // get group member list based on the group descriptor
                                List<Objects.GroupMember> groupMemberList = await Utilities.GroupUtils.getGroupMembers(credential, url, group.descriptor);

                                // go through each group member in the group and if our user matches, display it
                                foreach (Objects.GroupMember member in groupMemberList)
                                {
                                    //Console.WriteLine("whoami Checking user " + user.principalName.ToLower() + "against group " + group.principalName + ", member " + member.mailAddress.ToLower());
                                    if (member.mailAddress.ToLower().Equals(user.principalName.ToLower()))
                                    {

                                        Console.WriteLine("{0,70} | {1,50} | {2,50}", group.principalName, group.displayName, group.description);
                                    }

                                }

                            }


                        }


                    }

                    Console.WriteLine("");


                }
                catch (Exception ex)
                {
                    Console.WriteLine("");
                    Console.WriteLine("[-] ERROR: " + ex.Message);
                    Console.WriteLine("[-] ERROR: " + ex.StackTrace);
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

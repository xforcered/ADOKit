using System;
using System.Net;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Collections.Generic;

namespace ADOKit.Modules.Privesc
{
    class RemoveCollectionBuildSvc
    {

        public static async Task execute(string credential, string url, string username)
        {

            // Generate module header
            Console.WriteLine(Utilities.ArgUtils.GenerateHeader("removecollectionbuildsvc", credential, url, "", "", "", username));

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
                    // these 2 values are needed to remove a user from a group
                    string userDescriptor = "";
                    string groupDescriptor = "";

                    // get a listing of all users in the Azure DevOps instance
                    List<Objects.User> userList = await Utilities.UserUtils.getAllUsers(credential, url);

                    // iterate through the list of users and find our user. this is way to get the user descriptor.
                    foreach (Objects.User user in userList)
                    {
                        // if we have found our user, keep going to get the user descriptor and group descriptor
                        if (user.directoryAlias.ToLower().Equals(username.ToLower()))
                        {

                            // fetch the user details so we can get the descriptor
                            Objects.User ourUser = await Utilities.UserUtils.getUserDetails(credential, url, user.descriptor, user.principalName);
                            userDescriptor = ourUser.descriptor;

                            // get a listing of all groups in the Azure DevOps instance
                            List<Objects.Group> groupList = await Utilities.GroupUtils.getAllGroups(credential, url);

                            // iterate through the list of groups and grab the desriptor for the collection build service accounts group
                            foreach (Objects.Group group in groupList)
                            {
                                if (group.displayName.ToLower().Equals("project collection build service accounts"))
                                {
                                    groupDescriptor = group.descriptor;

                                }

                            }

                        }


                    }

                    if (groupDescriptor == "")
                    {
                        Console.WriteLine("[*] ERROR We didn't find a group descriptor - there wasn't a match. Stopping.");
                        return;
                    }
                    if (userDescriptor == "")
                    {
                        Console.WriteLine("[*] ERROR We didn't find a user descriptor - there wasn't a match. Stopping.");
                        return;
                    }

                    Console.WriteLine("");
                    Console.WriteLine("[*] INFO: Attempting to remove " + username + " from the Project Collection Build Service Accounts group.");
                    Console.WriteLine("");

                    // remove user from group now that we have the user descriptor and group descriptor
                    bool userRemoved = await Utilities.GroupUtils.removeUserFromGroup(credential, url, userDescriptor, groupDescriptor);

                    // if user was removed successfully, display message and list the members of the project collection build service accounts group
                    if (userRemoved)
                    {
                        Console.WriteLine("[+] SUCCESS: User successfully removed");
                        Console.WriteLine("");

                        // get a listing of all groups in the Azure DevOps instance
                        List<Objects.Group> groupList = await Utilities.GroupUtils.getAllGroups(credential, url);

                        // iterate through the list of groups for the project and list the members of the project collection build service accounts group
                        foreach (Objects.Group group in groupList)
                        {
                            // if the group is project collection build service accounts
                            if (group.displayName.ToLower().Equals("project collection build service accounts"))
                            {
                                // get group member list based on the group descriptor
                                List<Objects.GroupMember> groupMemberList = await Utilities.GroupUtils.getGroupMembers(credential, url, group.descriptor);

                                // create table header for listing group members
                                string tableHeaderMembers = string.Format("{0,70} | {1,50} | {2,50}", "Group", "Mail Address", "Display Name");
                                Console.WriteLine(tableHeaderMembers);
                                Console.WriteLine(new String('-', tableHeaderMembers.Length));

                                // go through each group member in the group and list them
                                foreach (Objects.GroupMember member in groupMemberList)
                                {
                                    Console.WriteLine("{0,70} | {1,50} | {2,50}", group.principalName, member.mailAddress, member.displayName);
                                }

                            }

                        }

                    }
                    else
                    {
                        Console.WriteLine("[-] ERROR: User was NOT successfully removed");
                        Console.WriteLine("");
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

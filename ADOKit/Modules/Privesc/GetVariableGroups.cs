using System;
using System.Net;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Collections.Generic;
using ADOKit.Objects;

namespace ADOKit.Modules.Privesc
{
    class GetVariableGroups
    {

        public static async Task execute(string credential, string url, string project)
        {
            // Generate module header
            Console.WriteLine(Utilities.ArgUtils.GenerateHeader("getvariablegroups", credential, url, "", project, "", ""));

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

                    // if user wants to dump variable groups for all projects
                    if (project.ToLower().Equals("all"))
                    {

                        // create table header
                        string tableHeader = string.Format("{0,30} | {1,30} | {2,30} | {3,50}", "Project Name", "Variable Group Name", "Variable Name", "Variable Value");
                        Console.WriteLine(tableHeader);
                        Console.WriteLine(new String('-', tableHeader.Length));

                        // get a listing of all projects in the Azure DevOps instance
                        List<Project> projectList = await Utilities.ProjectUtils.getAllProjects(credential, url);

                        // iterate through the list of projects
                        foreach (Project proj in projectList)
                        {
                            // get a list of all variable groups and their variables
                            List<GroupVariable> variableGroups = await Utilities.PipelineUtils.getVariableGroups(credential, url, proj.projectName);
                            foreach (GroupVariable var in variableGroups)
                            {
                                Console.WriteLine("{0,30} | {1,30} | {2,30} | {3,50}", proj.projectName, var.groupName, var.name, var.value);
                            }
                        }

                        Console.WriteLine("");

                    }

                    // if user wants to only dump pipeline variables for a certain project
                    else
                    {
                        // create table header
                        string tableHeader = string.Format("{0,30} | {1,30} | {2,50}", "Variable Group Name", "Variable Name", "Variable Value");
                        Console.WriteLine(tableHeader);
                        Console.WriteLine(new String('-', tableHeader.Length));

                        // get a list of all variable groups and their variables
                        List<GroupVariable> variableGroups = await Utilities.PipelineUtils.getVariableGroups(credential, url, project);
                        foreach (GroupVariable var in variableGroups)
                        {
                            Console.WriteLine("{0,30} | {1,30} | {2,50}", var.groupName, var.name, var.value);
                        }

                        Console.WriteLine("");

                    }



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

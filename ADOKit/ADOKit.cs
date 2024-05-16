using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOKit
{
    class ADOKit
    {

        // variables to be used
        private static string module = "";
        private static string credential = "";
        private static string url = "";
        private static string project = "";
        private static string group = "";
        private static string user = "";
        private static string search = "";
        private static string id = "";
        private static string sshKey = "";
        private static List<string> approvedModules = new List<string> { "check", "whoami", "listrepo", "searchrepo", "listproject", "searchproject", "searchcode", "searchfile", "listuser", "searchuser", "listgroup", "searchgroup", "getgroupmembers", "getpermissions", "createpat", "removepat", "listpat", "createsshkey", "removesshkey", "listsshkey", "addprojectadmin", "removeprojectadmin", "addbuildadmin", "removebuildadmin", "addcollectionadmin", "removecollectionadmin", "addcollectionbuildadmin", "removecollectionbuildadmin", "addcollectionbuildsvc", "removecollectionbuildsvc", "addcollectionsvc", "removecollectionsvc", "getpipelinevars", "getpipelinesecrets", "getvariablegroups", "getserviceconnections" };



        static async Task Main(string[] args)
        {

            try
            {

                Dictionary<string, string> argDict = Utilities.ArgUtils.ParseArguments(args); // dictionary to hold arguments

                // if no arguments given, display help and return
                if ((args.Length > 0 && argDict.Count == 0) || argDict.ContainsKey("help"))
                {
                    Utilities.ArgUtils.HelpMe();
                    return;
                }

                module = args[0].ToLower(); // get the module by the first argument given

                // if url is not set, display message and exit
                if (!argDict.ContainsKey("url"))
                {
                    Console.WriteLine("");
                    Console.WriteLine("[-] ERROR: Must supply a URL. See the README.");
                    return;
                }

                // if both module and credential are not given, display message and exit
                if (module.Equals("") && !argDict.ContainsKey("credential"))
                {
                    Console.WriteLine("");
                    Console.WriteLine("[-] ERROR: Must supply both a module and credential. See the README.");
                    return;
                }


                // initialize variables

                // credential
                if (argDict.ContainsKey("credential"))
                {

                    credential = argDict["credential"];

                }

                // url
                if (argDict.ContainsKey("url"))
                {

                    url = argDict["url"];

                }

                // search
                if (argDict.ContainsKey("search"))
                {

                    search = argDict["search"];

                }

                // project
                if (argDict.ContainsKey("project"))
                {

                    project = argDict["project"];

                }

                // user
                if (argDict.ContainsKey("user"))
                {

                    user = argDict["user"];

                }

                // group
                if (argDict.ContainsKey("group"))
                {

                    group = argDict["group"];

                }

                // id
                if (argDict.ContainsKey("id"))
                {

                    id = argDict["id"];

                }

                // SSH key
                if (argDict.ContainsKey("sshkey"))
                {

                    sshKey = argDict["sshkey"];

                }

                // determine if invalid module was given
                if (!approvedModules.Contains(module))
                {
                    Console.WriteLine("");
                    Console.WriteLine("[-] ERROR: Invalid module given. Please see the README for approved modules.");
                    return;
                }

                // if all is good, proceed to run appropriate module
                switch (module)
                {

                    case "check":
                        await Modules.Recon.Check.execute(credential, url);
                        break;
                    case "whoami":
                        await Modules.Recon.Whoami.execute(credential, url);
                        break;
                    case "listrepo":
                        await Modules.Recon.ListRepo.execute(credential, url);
                        break;
                    case "searchrepo":
                        await Modules.Recon.SearchRepo.execute(credential, url, search);
                        break;
                    case "listproject":
                        await Modules.Recon.ListProject.execute(credential, url);
                        break;
                    case "searchproject":
                        await Modules.Recon.SearchProject.execute(credential, url, search);
                        break;
                    case "searchcode":
                        await Modules.Recon.SearchCode.execute(credential, url, search);
                        break;
                    case "searchfile":
                        await Modules.Recon.SearchFile.execute(credential, url, search);
                        break;
                    case "listuser":
                        await Modules.Recon.ListUser.execute(credential, url);
                        break;
                    case "searchuser":
                        await Modules.Recon.SearchUser.execute(credential, url,search);
                        break;
                    case "listgroup":
                        await Modules.Recon.ListGroup.execute(credential, url);
                        break;
                    case "searchgroup":
                        await Modules.Recon.SearchGroup.execute(credential, url,search);
                        break;
                    case "getgroupmembers":
                        await Modules.Recon.GetGroupMembers.execute(credential, url, group);
                        break;
                    case "getpermissions":
                        await Modules.Recon.GetPermissions.execute(credential, url, project);
                        break;
                    case "createpat":
                        await Modules.Persistence.CreatePAT.execute(credential, url);
                        break;
                    case "listpat":
                        await Modules.Persistence.ListPAT.execute(credential, url);
                        break;
                    case "removepat":
                        await Modules.Persistence.RemovePAT.execute(credential, url, id);
                        break;
                    case "createsshkey":
                        await Modules.Persistence.CreateSSHKey.execute(credential, url,sshKey);
                        break;
                    case "listsshkey":
                        await Modules.Persistence.ListSSHKey.execute(credential, url);
                        break;
                    case "removesshkey":
                        await Modules.Persistence.RemoveSSHKey.execute(credential, url, id);
                        break;
                    case "addprojectadmin":
                        await Modules.Privesc.AddProjectAdmin.execute(credential, url, project, user);
                        break;
                    case "removeprojectadmin":
                        await Modules.Privesc.RemoveProjectAdmin.execute(credential, url, project, user);
                        break;
                    case "addbuildadmin":
                        await Modules.Privesc.AddBuildAdmin.execute(credential, url, project, user);
                        break;
                    case "removebuildadmin":
                        await Modules.Privesc.RemoveBuildAdmin.execute(credential, url, project, user);
                        break;
                    case "addcollectionadmin":
                        await Modules.Privesc.AddCollectionAdmin.execute(credential, url, user);
                        break;
                    case "removecollectionadmin":
                        await Modules.Privesc.RemoveCollectionAdmin.execute(credential, url, user);
                        break;
                    case "addcollectionbuildadmin":
                        await Modules.Privesc.AddCollectionBuildAdmin.execute(credential, url, user);
                        break;
                    case "removecollectionbuildadmin":
                        await Modules.Privesc.RemoveCollectionBuildAdmin.execute(credential, url, user);
                        break;
                    case "addcollectionbuildsvc":
                        await Modules.Privesc.AddCollectionBuildSvc.execute(credential, url, user);
                        break;
                    case "removecollectionbuildsvc":
                        await Modules.Privesc.RemoveCollectionBuildSvc.execute(credential, url, user);
                        break;
                    case "addcollectionsvc":
                        await Modules.Privesc.AddCollectionSvc.execute(credential, url, user);
                        break;
                    case "removecollectionsvc":
                        await Modules.Privesc.RemoveCollectionSvc.execute(credential, url, user);
                        break;
                    case "getpipelinevars":
                        await Modules.Privesc.GetPipelineVars.execute(credential, url, project);
                        break;
                    case "getpipelinesecrets":
                        await Modules.Privesc.GetPipelineSecrets.execute(credential, url, project);
                        break;
                    case "getvariablegroups":
                        await Modules.Privesc.GetVariableGroups.execute(credential, url, project);
                        break;
                    case "getserviceconnections":
                        await Modules.Privesc.GetServiceConnections.execute(credential, url, project);
                        break;
                    default:
                        Console.WriteLine("");
                        Console.WriteLine("[-] ERROR: That module is not supported. Please see README");
                        Console.WriteLine("");
                        Environment.Exit(1);
                        break;

                }



            } // end try
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.WriteLine("[-] ERROR : {0}", ex.Message);
            }


        }
    }
}

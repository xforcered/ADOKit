﻿using System;
using System.Net;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Collections.Generic;

namespace ADOKit.Modules.Recon
{
    class ListTeam
    {
        public static async Task execute(string credential, string url)
        {
            // Generate module header
            Console.WriteLine(Utilities.ArgUtils.GenerateHeader("listteam", credential, url, "", "", "", ""));

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
                    string tableHeader = string.Format("{0,50} | {1,30} | {2,50}", "Team Name", "Project", "Description");
                    Console.WriteLine(tableHeader);
                    Console.WriteLine(new String('-', tableHeader.Length));

                    // get a listing of all teams in the Azure DevOps instance
                    List<Objects.Team> teamList = await Utilities.TeamUtils.getAllTeams(credential, url);

                    // iterate through the list of teams and list them
                    foreach (Objects.Team team in teamList)
                    {

                        Console.WriteLine("{0,50} | {1,30} | {2,50}", team.name, team.project, team.description);

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

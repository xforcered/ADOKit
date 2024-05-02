using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using ADOKit.Objects;


namespace ADOKit.Utilities
{
    class RepoUtils
    {

        // get a list of all repos for a project
        public static async Task<List<Repo>> getAllReposForProject(string credentials, string projectURL)
        {
            // this is the list of repos to return
            List<Repo> reposList = new List<Repo>();


            // ignore SSL errors
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {


                // web request to get all repos in a given project
                HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(projectURL + "/_apis/git/repositories?includeHidden=true&api-version=7.0");
                if (webRequest != null)
                {

                    // set header values
                    webRequest.Method = "GET";
                    webRequest.ContentType = "application/json";
                    webRequest.UserAgent = "ADOKit-21e233d4334f9703d1a3a42b6e2efd38";

                    // if cookie was provided
                    if (credentials.ToLower().Contains("userauthentication="))
                    {
                        webRequest.Headers.Add("Cookie", "X-VSS-UseRequestRouting=True; " + credentials);

                    }

                    // otherwise PAT was provided
                    else
                    {
                        webRequest.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(":" + credentials)));
                    }

                    // get web response
                    HttpWebResponse myWebResponse = (HttpWebResponse)await webRequest.GetResponseAsync();
                    string content;
                    var reader = new StreamReader(myWebResponse.GetResponseStream());
                    content = reader.ReadToEnd();


                    // parse the JSON output and display results
                    JsonTextReader jsonResult = new JsonTextReader(new StringReader(content));

                    string propName = "";
                    string link = "";
                    string repoName = "";

                    // read the json results
                    while (jsonResult.Read())
                    {
                        switch (jsonResult.TokenType.ToString())
                        {
                            case "StartObject":
                                break;
                            case "EndObject":

                                // if repo already doesn't exist in our list, add it
                                if (!doesRepoAlreadyExistInOurList(link, reposList))
                                {

                                    // add the repo to our list as long as it has URL and name
                                    if (repoName != "" && link != "")
                                    {
                                        reposList.Add(new Repo(repoName, link));
                                    }

                                }

                                break;
                            case "StartArray":
                                break;
                            case "EndArray":
                                break;
                            case "PropertyName":
                                propName = jsonResult.Value.ToString();
                                break;
                            case "String":

                                // grab the repo URL
                                if (propName.ToLower().Equals("weburl"))
                                {
                                    link = jsonResult.Value.ToString();

                                    // get the repo name based on the link
                                    repoName = link.Substring(link.LastIndexOf('/') + 1, link.Length - link.LastIndexOf('/') - 1);


                                }
                                break;
                            case "Integer":
                                break;
                            default:
                                break;

                        }

                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.WriteLine("[-] ERROR: " + ex.Message);
                Console.WriteLine("");
            }


            return reposList;
        }


        // get a list of all repos for a project to include just the GUID URL
        public static async Task<List<string>> getAllReposWithGUIDURL(string credentials, string projectURL)
        {
            // this is the list of repos to return
            List<string> reposList = new List<string>();


            // ignore SSL errors
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {


                // web request to get all repos in a given project
                HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(projectURL + "/_apis/git/repositories?api-version=7.0");
                if (webRequest != null)
                {

                    // set header values
                    webRequest.Method = "GET";
                    webRequest.ContentType = "application/json";
                    webRequest.UserAgent = "ADOKit-21e233d4334f9703d1a3a42b6e2efd38";

                    // if cookie was provided
                    if (credentials.ToLower().Contains("userauthentication="))
                    {
                        webRequest.Headers.Add("Cookie", "X-VSS-UseRequestRouting=True; " + credentials);

                    }

                    // otherwise PAT was provided
                    else
                    {
                        webRequest.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(":" + credentials)));
                    }

                    // get web response
                    HttpWebResponse myWebResponse = (HttpWebResponse)await webRequest.GetResponseAsync();
                    string content;
                    var reader = new StreamReader(myWebResponse.GetResponseStream());
                    content = reader.ReadToEnd();


                    // parse the JSON output and display results
                    JsonTextReader jsonResult = new JsonTextReader(new StringReader(content));

                    string propName = "";
                    string link = "";
                    string repoName = "";

                    // read the json results
                    while (jsonResult.Read())
                    {
                        switch (jsonResult.TokenType.ToString())
                        {
                            case "StartObject":
                                break;
                            case "EndObject":

                                // if repo already doesn't exist in our list, add it
                                if (!doesRepoGUIDAlreadyExistInOurList(link, reposList))
                                {

                                    // add the repo to our list as long as it has URL and name
                                    if (link != "")
                                    {
                                        reposList.Add(link);
                                    }

                                }

                                break;
                            case "StartArray":
                                break;
                            case "EndArray":
                                break;
                            case "PropertyName":
                                propName = jsonResult.Value.ToString();
                                break;
                            case "String":

                                // grab the repo GUID URL
                                if (propName.ToLower().Equals("url"))
                                {

                                    if (jsonResult.Value.ToString().ToLower().Contains("/_apis/git/repositories"))
                                    {

                                        // get the GUID piece of the repository
                                        string formingLink = jsonResult.Value.ToString();
                                        formingLink = formingLink.Substring(formingLink.IndexOf("/_apis/git/repositories") + 1, formingLink.Length - formingLink.IndexOf("/_apis/git/repositories") - 1);

                                        // for the full link add the project URL plus the GUID piece for the repository
                                        link = projectURL + "/" + formingLink;

                                    }

                                }
                                break;
                            case "Integer":
                                break;
                            default:
                                break;

                        }

                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.WriteLine("[-] ERROR: " + ex.Message);
                Console.WriteLine("");
            }


            return reposList;
        }




        // determine whether we already have a repo in our list by the given unique URL for that repo
        public static bool doesRepoAlreadyExistInOurList(string url, List<Repo> repoList)
        {
            bool doesItExist = false;

            foreach (Objects.Repo repo in repoList)
            {
                if (repo.repoURL.Equals(url))
                {
                    doesItExist = true;
                }
            }

            return doesItExist;
        }



        // determine whether we already have a repo in our list by the given unique URL for that repo
        public static bool doesRepoGUIDAlreadyExistInOurList(string url, List<string> repoList)
        {
            bool doesItExist = false;

            foreach (string repo in repoList)
            {
                if (repo.Equals(url))
                {
                    doesItExist = true;
                }
            }

            return doesItExist;
        }


    }



}

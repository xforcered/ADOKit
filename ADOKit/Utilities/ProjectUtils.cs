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
    class ProjectUtils
    {

        // translate project URL guid to the actual repo URL
        public static async Task<string> translateGUIProjectURL(string credentials, string url)
        {

            string translatedRepoURL = "";

            // ignore SSL errors
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {


                // web request to get details for the project
                HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(url);
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

                    // read the json results
                    while (jsonResult.Read())
                    {
                        switch (jsonResult.TokenType.ToString())
                        {
                            case "StartObject":
                                break;
                            case "EndObject":
                                break;
                            case "StartArray":
                                break;
                            case "EndArray":
                                break;
                            case "PropertyName":
                                propName = jsonResult.Value.ToString();
                                break;
                            case "String":

                                // grab the translated repo URL
                                if (propName.ToLower().Equals("href"))
                                {
                                    if (!jsonResult.Value.ToString().Contains("/_apis/project"))
                                    {
                                        translatedRepoURL = jsonResult.Value.ToString();
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

            return translatedRepoURL;
        }


        // get a list of all projects
        public static async Task<List<Project>> getAllProjects(string credentials, string url)
        {
            List<Objects.Project> projectList = new List<Objects.Project>();

            try
            {

                // ignore SSL errors
                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                // web request to get list of repos(aka projects)
                HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(url + "/_apis/projects?api-version=7.0");
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

                    // get web response and status code
                    HttpWebResponse myWebResponse = (HttpWebResponse)await webRequest.GetResponseAsync();
                    string content;
                    var reader = new StreamReader(myWebResponse.GetResponseStream());
                    content = reader.ReadToEnd();


                    // parse the JSON output and display results
                    JsonTextReader jsonResult = new JsonTextReader(new StringReader(content));

                    string name = "";
                    string link = "";
                    string visibility = "";
                    string propName = "";

                    // read the json results
                    while (jsonResult.Read())
                    {
                        switch (jsonResult.TokenType.ToString())
                        {
                            case "StartObject":
                                break;
                            case "EndObject":

                                // if project already doesn't exist in our list, add it
                                if (!doesProjectAlreadyExistInList(link, projectList))
                                {
                                    projectList.Add(new Objects.Project(name, link, visibility));
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

                                if (propName.ToLower().Equals("name"))
                                {
                                    name = jsonResult.Value.ToString();
                                }
                                if (propName.ToLower().Equals("url"))
                                {
                                    link = jsonResult.Value.ToString();
                                }
                                if (propName.ToLower().Equals("visibility"))
                                {
                                    visibility = jsonResult.Value.ToString();
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



            return projectList;
        }

        


        // determine whether we already have a project in our list by the given unique URL for that project
        public static bool doesProjectAlreadyExistInList(string url, List<Project> projectList)
        {
            bool doesItExist = false;

            foreach (Objects.Project proj in projectList)
            {
                if (proj.projectURL.Equals(url))
                {
                    doesItExist = true;
                }
            }

            return doesItExist;
        }

    }
}

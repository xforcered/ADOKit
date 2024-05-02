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
    class FileUtils
    {

        // get a listing of all files in a repo
        public static async Task<List<Objects.File>> getAllFilesInRepo(string credentials, string repoURL)
        {


            // this is the list of repos to return
            List<Objects.File> filesList = new List<Objects.File>();


            // ignore SSL errors
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {


                // web request to get all files in a given repo recursively
                HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(repoURL + "/items?recursionLevel=Full&api-version=7.0");
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
                    string path = "";

                    // read the json results
                    while (jsonResult.Read())
                    {
                        switch (jsonResult.TokenType.ToString())
                        {
                            case "StartObject":
                                break;
                            case "EndObject":

                                // if file already doesn't exist in our list, add it
                                if (!doesFileAlreadyExistInOurList(link, filesList))
                                {

                                    // add the file to our list as long as it has URL
                                    if (link != "")
                                    {
                                        filesList.Add(new Objects.File(path, link));
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

                                // grab the path for file
                                if (propName.ToLower().Equals("path"))
                                {
                                    path = jsonResult.Value.ToString();

                                }

                                // grab the repo GUID URL
                                if (propName.ToLower().Equals("url"))
                                {
                                    link = jsonResult.Value.ToString();

                                    // clean up the URL so it is browsable
                                    link = link.Replace("_apis/git/repositories", "_git");
                                    link = link.Substring(0, link.IndexOf("/items"));
                                    link = link + "?path=" + path;

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

            return filesList;

        }


        // get the file contents at a given file URL
        public static async Task<string> getFileContents(string credentials, string fileURL)
        {

            // this is the file content to return
            string fileContents = "";


            // ignore SSL errors
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {


                // web request to get the file contents
                HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(fileURL + "&api-version=7.0");
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

                    // get web response and store
                    HttpWebResponse myWebResponse = (HttpWebResponse)await webRequest.GetResponseAsync();
                    var reader = new StreamReader(myWebResponse.GetResponseStream());
                    fileContents = reader.ReadToEnd();

                }

            }
            catch (Exception ex)
            {

            }

            return fileContents;


        }



        // determine whether we already have a file in our list by the given unique URL for that file
        public static bool doesFileAlreadyExistInOurList(string url, List<Objects.File> fileList)
        {
            bool doesItExist = false;

            foreach (Objects.File file in fileList)
            {
                if (file.fileURL.ToLower().Equals(url.ToLower()))
                {
                    doesItExist = true;
                }
            }

            return doesItExist;
        }


    }
}

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
    class ServiceConnectionUtils
    {


        // get a list of service connection objects for a project
        public static async Task<List<Objects.ServiceConnection>> getServiceConnectionsForProject(string credentials, string url, string project)
        {
            // this is the list of service connections to return
            List<ServiceConnection> serviceConnectionList = new List<ServiceConnection>();


            // ignore SSL errors
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {

                // web request to get all service connections for a user
                HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(url + "/" + project + "/_apis/serviceendpoint/endpoints?includeFailed=true&api-version=7.0");
                if (webRequest != null)
                {

                    // set header values
                    webRequest.Method = "GET";
                    webRequest.ContentType = "application/json;api-version=5.0-preview.1";
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
                    string name = "";
                    string type = "";
                    string id = "";
            

                    // read the json results
                    while (jsonResult.Read())
                    {
                        switch (jsonResult.TokenType.ToString())
                        {
                            case "StartObject":
                                break;
                            case "EndObject":


                                // add the service connection to our list if it has the required attributes
                                if (name != "" && type != "" && id != "")
                                {

                                    if (!doesServiceConnectionAlreadyExistInList(id, serviceConnectionList))
                                    {
                                        serviceConnectionList.Add(new ServiceConnection(name, type, id));
                                        name = "";
                                        type = "";
                                        id = "";
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

                                // grab the name
                                if (propName.ToLower().Equals("name"))
                                {
                                    name = jsonResult.Value.ToString();
                                }

                                // grab the type
                                if (propName.ToLower().Equals("type"))
                                {
                                    type = jsonResult.Value.ToString();
                                }

                                // grab the id
                                if (propName.ToLower().Equals("id"))
                                {
                                    id = jsonResult.Value.ToString();
                                }

                                break;
                            case "Boolean":
                                break;
                            case "Date":
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


            return serviceConnectionList;
        }


        // determine whether we already have a service connection in our list by the given unique id
        public static bool doesServiceConnectionAlreadyExistInList(string id, List<ServiceConnection> serviceConnectionList)
        {
            bool doesItExist = false;

            foreach (Objects.ServiceConnection connection in serviceConnectionList)
            {
                if (connection.id.Equals(id))
                {
                    doesItExist = true;
                }
            }

            return doesItExist;
        }




    }
}

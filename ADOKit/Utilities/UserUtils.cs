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
    class UserUtils
    {

        // get a list of all users
        public static async Task<List<User>> getAllUsers(string credentials, string url)
        {
            // this is the list of users to return
            List<User> userList = new List<User>();

            // ignore SSL errors
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // parse the JSON output and display results
            JsonTextReader jsonResult;

            string propName = "";
            string directoryAlias = "";
            string displayName = "";
            string principalName = "";
            string descriptor = "";


            try
            {

                url = url.Replace("dev.azure.com", "vssps.dev.azure.com");

                // web request to get all users
                string contToken = "";
                string content = "";

                HttpWebRequest webRequest = null;

                // loop until we don't have a continuationToken in the response. if we do, fetch more 
                do
                {
                    content = ""; // empty our buffer 

                    if (contToken != "")
                    {
                        webRequest = (HttpWebRequest)System.Net.WebRequest.Create(url + "/_apis/graph/users?api-version=7.0-preview.1&continuationToken=" + contToken);
                    }
                    else
                    {
                        webRequest = (HttpWebRequest)System.Net.WebRequest.Create(url + "/_apis/graph/users?api-version=7.0-preview.1");
                    }

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

                        var reader = new StreamReader(myWebResponse.GetResponseStream());
                        content += reader.ReadToEnd();

                        // if there's a continuationToken header we need to send the request to get
                        // a bit more ... and a bit more until there isn't a continuation token 

                        contToken = "";
                        // get the X-ms-continuationtoken header value
                        for (int i = 0; i < myWebResponse.Headers.Count; i++)
                        {

                            // grab the header value
                            if (myWebResponse.Headers.Keys[i].ToString().ToLower().Equals("x-ms-continuationtoken"))
                            {
                                //Console.WriteLine("[+] Response header : " + myWebResponse.Headers.Keys[i].ToString() + " / " + myWebResponse.Headers[i]);

                                contToken = myWebResponse.Headers[i];
                            }
                        }

                    }

                    // parse the JSON output and display results
                    jsonResult = new JsonTextReader(new StringReader(content));

                    propName = "";
                    directoryAlias = "";
                    displayName = "";
                    principalName = "";
                    descriptor = "";


                    // read the json results 
                    while (jsonResult.Read())
                    {
                        switch (jsonResult.TokenType.ToString())
                        {
                            case "StartObject":
                                break;
                            case "EndObject":

                                // add the user to the list if it has all the needed attributes
                                if (displayName != "" && principalName != "" && descriptor != "")

                                {

                                    if (!doesUserAlreadyExistInOurList(principalName, userList))
                                    {

                                        userList.Add(new User(directoryAlias, displayName, principalName, descriptor));
                                        descriptor = "";
                                        directoryAlias = "";
                                        displayName = "";
                                        principalName = "";

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

                                // grab the display name
                                if (propName.ToLower().Equals("displayname"))
                                {
                                    displayName = jsonResult.Value.ToString();

                                }

                                // grab the user directory alias
                                if (propName.ToLower().Equals("directoryalias"))
                                {
                                    directoryAlias = jsonResult.Value.ToString();
                                }

                                // grab the UPN
                                if (propName.ToLower().Equals("principalname"))
                                {
                                    principalName = jsonResult.Value.ToString();
                                }

                                // grab the descriptor
                                if (propName.ToLower().Equals("descriptor"))
                                {
                                    descriptor = jsonResult.Value.ToString();
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
                } while (contToken != "") ;

            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.WriteLine("[-] ERROR: " + ex.Message);
                Console.WriteLine("[-] ERROR: " + ex.StackTrace);
                Console.WriteLine("");
            }


            return userList;
        }


        // get details for a specific user
        public static async Task<User> getUserDetails(string credentials, string url, string userDescriptor, string ourUser)
        {
            // this is the user to return
            User user = null;


            // ignore SSL errors
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {

                url = url.Replace("dev.azure.com", "vssps.dev.azure.com");

                // web request to get details for a specific user
                HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(url + "/_apis/graph/users/" + userDescriptor + "?api-version=7.0-preview.1");
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
                    string directoryAlias = "";
                    string displayName = "";
                    string principalName = "";
                    string descriptor = "";

                    // read the json results
                    while (jsonResult.Read())
                    {
                        switch (jsonResult.TokenType.ToString())
                        {
                            case "StartObject":
                                break;
                            case "EndObject":

                                // add the user to the list if it has all the needed attributes
                                if (directoryAlias != "" && displayName != "" && principalName != "" && descriptor != "")
                                {

                                    user = new User(directoryAlias, displayName, principalName, descriptor);
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

                                // grab the display name
                                if (propName.ToLower().Equals("displayname"))
                                {
                                    displayName = jsonResult.Value.ToString();

                                }

                                // grab the user directory alias
                                if (propName.ToLower().Equals("directoryalias"))
                                {
                                    directoryAlias = jsonResult.Value.ToString();
                                }

                                // grab the UPN
                                if (propName.ToLower().Equals("principalname"))
                                {
                                    principalName = jsonResult.Value.ToString();
                                }

                                // grab the descriptor
                                if (propName.ToLower().Equals("descriptor"))
                                {
                                    descriptor = jsonResult.Value.ToString();
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
                Console.WriteLine("[-] ERROR: " + ex.StackTrace);
                Console.WriteLine("");
            }


            return user;
        }




        // get a list of all user memberships
        public static async Task<List<string>> getUserMemberships(string credentials, string url, string userDescriptor)
        {
            // this is the list of memberships to return
            List<string> membershipList = new List<string>();


            // ignore SSL errors
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {

                url = url.Replace("dev.azure.com", "vssps.dev.azure.com");

                // web request to get all memberships for a user
                HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(url + "/_apis/Graph/Memberships/" + userDescriptor + "?api-version=7.0-preview.1");
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
                    string membershipLink = "";


                    // read the json results
                    while (jsonResult.Read())
                    {
                        switch (jsonResult.TokenType.ToString())
                        {
                            case "StartObject":
                                break;
                            case "EndObject":

                                // add the the membership URL to the list
                                if (membershipLink != "" && membershipLink.Contains("/Graph/Groups/"))
                                {


                                    membershipList.Add(membershipLink);
                                    membershipLink = "";

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

                                // grab the membership link
                                if (propName.ToLower().Equals("href"))
                                {

                                    membershipLink = jsonResult.Value.ToString();

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
                Console.WriteLine("[-] ERROR: " + ex.StackTrace);
                Console.WriteLine("[-] ERROR: " + ex.Message);
                Console.WriteLine("");
            }


            return membershipList;
        }

        // get who the current user is
        public static async Task<string> getCurrentUser(string credentials, string url)
        {
            // the user string to return
            string theUser = "";

            // ignore SSL errors
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {

                // web request to check auth and whether organization uses Azure DevOps
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

                    // get web response and status code
                    HttpWebResponse myWebResponse = (HttpWebResponse)await webRequest.GetResponseAsync();

                    // get the X-Vss-Userdata header value
                    for (int i = 0; i < myWebResponse.Headers.Count; i++)
                    {
			//DEBUG
                        //Console.WriteLine("[+] Response header : " + myWebResponse.Headers.Keys[i].ToString() + " / " + myWebResponse.Headers[i]);

                        // grab the header value
                        if (myWebResponse.Headers.Keys[i].ToString().ToLower().Equals("x-vss-userdata"))
                        {
                            theUser = myWebResponse.Headers[i];
                        
                        }


                    }


                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Exception " + ex.StackTrace);
                return theUser;
            }

            return theUser;


        }



        // determine whether we already have a user in our list by the given unique principal name for that user
        public static bool doesUserAlreadyExistInOurList(string principalName, List<User> userList)
        {
            bool doesItExist = false;

            foreach (Objects.User user in userList)
            {
                if (user.principalName.ToLower().Equals(principalName.ToLower()))
                {
                    doesItExist = true;
                }
            }

            return doesItExist;
        }

    }
}

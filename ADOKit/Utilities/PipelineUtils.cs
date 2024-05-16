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
    class PipelineUtils
    {
        public static async Task<List<string>> getBuildDefinitionURLs(string credentials, string url, string project)
        {

            // this is the list of URLs to return
            List<string> buildDefURLs = new List<string>();


            // ignore SSL errors
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {


                // web request to get all build definitions for a project
                HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(url + "/" + project + "/_apis/build/definitions?api-version=5.0");
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
                    string buildDefURL = "";


                    // read the json results
                    while (jsonResult.Read())
                    {
                        switch (jsonResult.TokenType.ToString())
                        {
                            case "StartObject":
                                break;
                            case "EndObject":

                                // add the URL to our list
                                if (buildDefURL != "" && buildDefURL.Contains("/_apis/build/Definitions"))
                                {

                                    buildDefURLs.Add(buildDefURL);
                                    buildDefURL = "";

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
                                // grab the url
                                if (propName.ToLower().Equals("href"))
                                {
                                    buildDefURL = jsonResult.Value.ToString();

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


            return buildDefURLs;

        }



        // get a list of build variables from a project build definition
        public static async Task<List<BuildVariable>> getBuildVars(string credentials, string buildDefURL)
        {

            // this is the list of build variables to return
            List<BuildVariable> buildVariables = new List<BuildVariable>();


            // ignore SSL errors
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {


                // web request to get all build variables for a build definition
                HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(buildDefURL);
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
                    string name = "";
                    string value = "";
                    string propVarName = ""; // used to track whether we are in variables section


                    // read the json results
                    while (jsonResult.Read())
                    {
                        switch (jsonResult.TokenType.ToString())
                        {
                            case "StartObject":
                                break;
                            case "EndObject":

                                // add build variable to the list if it has all the attributes needed and we didn't already add it
                                if (!doesBuildVariableAlreadyExist(name, value, buildVariables) && value != "" && name != "")
                                {
                                    buildVariables.Add(new BuildVariable(name, value));

                                    // reset values after build variable item has been added
                                    buildDefURL = "";
                                    name = "";
                                    value = "";
                                }

                                break;
                            case "StartArray":
                                break;
                            case "EndArray":
                                break;
                            case "PropertyName":

                                // at this point we know we are at start of variables section
                                if (propName.Equals("variables"))
                                {
                                    propVarName = jsonResult.Value.ToString();

                                }

                                // at this point we know we are at end of variables so we can set it back to blank
                                if (propName.Equals("properties"))
                                {
                                    propVarName = "";

                                }

                                propName = jsonResult.Value.ToString();

                                // only get a variable name if we are in the variables block and it isn't a value
                                if (propVarName != "")
                                {

                                    if(propName != "value")
                                    {
                                        name = jsonResult.Value.ToString();
                                    }

                                }
                                break;
                            case "String":

                                // only print if we are in the variables block
                                if (propVarName != "")
                                {
                                    value = jsonResult.Value.ToString();
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


            return buildVariables;

        }


        // get a list of build secrets from a project build definition
        public static async Task<List<BuildVariable>> getBuildSecrets(string credentials, string buildDefURL)
        {

            // this is the list of build variables to return
            List<BuildVariable> buildVariables = new List<BuildVariable>();


            // ignore SSL errors
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {


                // web request to get all build variables for a build definition
                HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(buildDefURL);
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
                    string name = "";
                    string value = "";
                    string propVarName = ""; // used to track whether we are in variables section


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

                                // at this point we know we are at start of variables section
                                if (propName.Equals("variables"))
                                {
                                    propVarName = jsonResult.Value.ToString();

                                }

                                // at this point we know we are at end of variables so we can set it back to blank
                                if (propName.Equals("properties"))
                                {
                                    propVarName = "";

                                }

                                propName = jsonResult.Value.ToString();

                                // only get a variable name if we are in the variables block and it isn't a value
                                if (propVarName != "")
                                {

                                    // if we have gotten to a build secret, then assign it appropriatelly
                                    if(propName == "isSecret")
                                    {
                                        value = ""; // value is going to be null, so assign blank string

                                        // add build secret to the list if it has all the attributes needed and we didn't already add it
                                        if (!doesBuildVariableAlreadyExist(name, value, buildVariables) && value == "" && name != "")
                                        {
                                            buildVariables.Add(new BuildVariable(name, value));

                                            // reset values after build secret item has been added
                                            buildDefURL = "";
                                            name = "";
                                            value = "";
                                        }
                                    }

                                    if (propName != "value" && propName != "isSecret")
                                    {
                                        name = jsonResult.Value.ToString();

                                    }

                                }
                                break;
                            case "String":

                                // only print if we are in the variables block
                                if (propVarName != "")
                                {
                                    value = jsonResult.Value.ToString();
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


            return buildVariables;

        }


        // determine whether we already have that build variable
        public static bool doesBuildVariableAlreadyExist(string name, string value, List<BuildVariable> buildList)
        {
            bool doesItExist = false;

            foreach (Objects.BuildVariable variable in buildList)
            {
                if (variable.name.Equals(name) && variable.value.Equals(value))
                {
                    doesItExist = true;
                }
            }

            return doesItExist;
        }


        public static async Task<List<GroupVariable>> getVariableGroups(string credentials, string url, string project)
        {

            // this is the list of group IDs to return
            List<GroupVariable> groupList = new List<GroupVariable>();


            // ignore SSL errors
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {

                // web request to get all variable groups for a project
                // https://learn.microsoft.com/en-us/rest/api/azure/devops/distributedtask/variablegroups/get-variable-groups
                HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(url + "/" + project + "/_apis/distributedtask/variablegroups?api-version=7.2-preview.2");
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
                    dynamic jsonResult = JsonConvert.DeserializeObject(content);

                    string groupName = "";
                    string variableName = "";
                    string variableValue = "";

                    // read the json results, first all groups
                    foreach (var group in jsonResult.value)
                    {
                        groupName = group.name;

                        // then all variables for this group
                        foreach(var variable in group.variables)
                        {
                            variableName = variable.Name;
                            variableValue = variable.Value.isSecret=="true" ? "[HIDDEN]" : variable.Value.value;
                            groupList.Add(new GroupVariable(variableName, variableValue, groupName));
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


            return groupList;

        }

    }
}

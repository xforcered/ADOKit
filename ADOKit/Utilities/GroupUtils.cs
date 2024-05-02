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
    class GroupUtils
    {

        // get a list of all groups
        public static async Task<List<Group>> getAllGroups(string credentials, string url)
        {
            // this is the list of groups to return
            List<Group> groupList = new List<Group>();


            // ignore SSL errors
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

	    // parse the JSON output and display results
	    JsonTextReader jsonResult;
	    
	    string propName = "";
	    string description = "";
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

                // web request to get all users
		do {

		    content="";

		    if (contToken != "")
		    {
			webRequest = (HttpWebRequest)System.Net.WebRequest.Create(url + "/_apis/graph/groups?api-version=7.0-preview.1&continuationToken=" + contToken);
		    }
		    else
		    {
			webRequest = (HttpWebRequest)System.Net.WebRequest.Create(url + "/_apis/graph/groups?api-version=7.0-preview.1");			
		    }
		    
		
		    if (webRequest != null) {

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
			content = reader.ReadToEnd();


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


			// parse the JSON output and display results
			jsonResult = new JsonTextReader(new StringReader(content));

			propName = "";
			description = "";
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
                                if (description != "" && displayName != "" && principalName != "" && descriptor != "")
                                {

                                    if (!doesGroupAlreadyExistInOurList(principalName, groupList))
                                    {

                                        groupList.Add(new Group(principalName, displayName, description, descriptor));
                                        descriptor = "";
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

                                // grab the description
                                if (propName.ToLower().Equals("description"))
                                {
                                    description = jsonResult.Value.ToString();
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
		
		} while (contToken!="") ;
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.WriteLine("[-] ERROR: " + ex.Message);
                Console.WriteLine("[-] ERROR: " + ex.StackTrace);
                Console.WriteLine("");
            }

            return groupList;
        }

        // get details for a group
        public static async Task<Group> getGroupDetails(string credentials, string url)
        {
            // this is the group to return
            Group group = null;


            // ignore SSL errors
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {

                // web request to get all users
                HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(url + "?api-version=7.0-preview.1");
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
                    string description = "";
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

                                // get the group if it has all attributes needed
                                if (description != "" && displayName != "" && principalName != "" && descriptor != "")
                                {

                                    group = new Group(principalName, displayName, description, descriptor);
                                    descriptor = "";

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

                                // grab the description
                                if (propName.ToLower().Equals("description"))
                                {
                                    description = jsonResult.Value.ToString();
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
                Console.WriteLine("");
            }


            return group;
        }



        // add a user to a group
        public static async Task<bool> addUserToGroup(string credentials, string url, string userDescriptor, string groupDescriptor)
        {
            // whether addition was completed successfully
            bool success = false;


            // ignore SSL errors
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {

                url = url.Replace("dev.azure.com", "vssps.dev.azure.com");

                // web request to add user to a group in a project
                HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(url + "/_apis/graph/memberships/" + userDescriptor + "/" + groupDescriptor + "?api-version=7.0-preview.1");
                if (webRequest != null)
                {

                    // set header values
                    webRequest.Method = "PUT";
                    webRequest.ContentType = "application/json";
                    webRequest.UserAgent = "ADOKit-21e233d4334f9703d1a3a42b6e2efd38";
                    webRequest.ContentLength = 0;

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

                    // get web response and determin if successful or not
                    HttpWebResponse myWebResponse = (HttpWebResponse)await webRequest.GetResponseAsync();


                    /*
                    for (int i = 0; i < myWebResponse.Headers.Count; i++)
                    {
                        Console.WriteLine("[+] Response header : " + myWebResponse.Headers.Keys[i].ToString() + " / " + myWebResponse.Headers[i]);

                    } */

                    if (myWebResponse.StatusCode.ToString().ToLower().Equals("created"))
                    {
                        success = true;
                    }
              

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.WriteLine("[-] ERROR: " + ex.Message);
                Console.WriteLine("");
            }


            return success;
        }



        // remove user from a group
        public static async Task<bool> removeUserFromGroup(string credentials, string url, string userDescriptor, string groupDescriptor)
        {
            // whether removal was completed successfully
            bool success = false;


            // ignore SSL errors
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {

                url = url.Replace("dev.azure.com", "vssps.dev.azure.com");

                // web request to add user to remove user from a group in a project
                HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(url + "/_apis/graph/memberships/" + userDescriptor + "/" + groupDescriptor + "?api-version=7.0-preview.1");
                if (webRequest != null)
                {

                    // set header values
                    webRequest.Method = "DELETE";
                    webRequest.ContentType = "application/json";
                    webRequest.UserAgent = "ADOKit-21e233d4334f9703d1a3a42b6e2efd38";
                    webRequest.ContentLength = 0;

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

                    // get web response and determin if successful or not
                    HttpWebResponse myWebResponse = (HttpWebResponse)await webRequest.GetResponseAsync();

                    if (myWebResponse.StatusCode.ToString().ToLower().Equals("ok"))
                    {
                        success = true;
                    }


                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.WriteLine("[-] ERROR: " + ex.Message);
                Console.WriteLine("");
            }


            return success;
        }






        // get a listing of groups that have permissions for a given project
        public static async Task<List<Group>> getGroupPermissionsForProject(string credentials, string url, string projectName)
        {
            // this is the list of groups to return
            List<Group> groupList = new List<Group>();


            // ignore SSL errors
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {

                // web request to get all users
                HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(url + "/_apis/Contribution/HierarchyQuery?api-version=7.0-preview.1");
                if (webRequest != null)
                {

                    // set header values
                    webRequest.Method = "POST";
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



                    // set body and send request
                    using (var streamWriter = new StreamWriter(webRequest.GetRequestStream()))
                    {

                        string json = "{\"contributionIds\":[\"ms.vss-admin-web.org-admin-groups-data-provider\"],\"dataProviderContext\":{\"properties\":{\"sourcePage\":{\"routeValues\":{\"project\":\"" + projectName + "\",\"adminPivot\":\"permissions\",\"controller\":\"ContributedPage\",\"action\":\"Execute\"}}}}}";

                        streamWriter.Write(json);
                    }

                    // get web response
                    HttpWebResponse myWebResponse = (HttpWebResponse)await webRequest.GetResponseAsync();
                    string content;
                    var reader = new StreamReader(myWebResponse.GetResponseStream());
                    content = reader.ReadToEnd();


                    // parse the JSON output and display results
                    JsonTextReader jsonResult = new JsonTextReader(new StringReader(content));

                    string propName = "";
                    string description = "";
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
                                if (description != "" && displayName != "" && principalName != "" && descriptor != "")
                                {

                                    if (!doesGroupAlreadyExistInOurList(principalName, groupList))
                                    {

                                        groupList.Add(new Group(principalName, displayName, description, descriptor));
                                        descriptor = "";
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

                                // grab the description
                                if (propName.ToLower().Equals("description"))
                                {
                                    description = jsonResult.Value.ToString();
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
                Console.WriteLine("");
            }


            return groupList;
        }


        // get all group members for a given group
        public static async Task<List<GroupMember>> getGroupMembers(string credentials, string url, string groupDescriptor)
        {
            // this is the list of group members to return
            List<GroupMember> groupMemberList = new List<GroupMember>();


            // ignore SSL errors
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {

                // web request to get all group members
                HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(url + "/_apis/Contribution/HierarchyQuery?api-version=7.0-preview.1");
                if (webRequest != null)
                {

                    // set header values
                    webRequest.Method = "POST";
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



                    // set body and send request
                    using (var streamWriter = new StreamWriter(webRequest.GetRequestStream()))
                    {

                        string json = "{\"contributionIds\":[\"ms.vss-admin-web.org-admin-group-members-data-provider\"],\"dataProviderContext\":{\"properties\":{\"subjectDescriptor\":\"" + groupDescriptor + "\",\"sourcePage\":{\"routeValues\":{\"adminPivot\":\"permissions\",\"controller\":\"ContributedPage\",\"action\":\"Execute\"}}}}}";

                        streamWriter.Write(json);
                    }

                    // get web response
                    HttpWebResponse myWebResponse = (HttpWebResponse)await webRequest.GetResponseAsync();
                    string content;
                    var reader = new StreamReader(myWebResponse.GetResponseStream());
                    content = reader.ReadToEnd();


                    // parse the JSON output and display results
                    JsonTextReader jsonResult = new JsonTextReader(new StringReader(content));

                    string propName = "";
                    string mailAddress = "";
                    string displayName = "";


                    // read the json results
                    while (jsonResult.Read())
                    {
                        switch (jsonResult.TokenType.ToString())
                        {
                            case "StartObject":
                                break;
                            case "EndObject":

                                // add the user to the list if it has all the needed attributes
                                if (mailAddress != "" && displayName != "")
                                {

                                    if (!doesGroupMemberAlreadyExistInOurList(mailAddress, groupMemberList))
                                    {

                                        groupMemberList.Add(new GroupMember(mailAddress, displayName));
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

                                // grab the mail address
                                if (propName.ToLower().Equals("mailaddress"))
                                {
                                    mailAddress = jsonResult.Value.ToString();
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

            return groupMemberList;
        }



        // determine whether we already have a group in our list by the given unique UPN for that group
        public static bool doesGroupAlreadyExistInOurList(string upn, List<Group> groupList)
        {
            bool doesItExist = false;

            foreach (Objects.Group group in groupList)
            {
                if (group.principalName.Equals(upn))
                {
                    doesItExist = true;
                }
            }

            return doesItExist;
        }


        // determine whether we already have a group member in our list by the given unique mail address for that group
        public static bool doesGroupMemberAlreadyExistInOurList(string mailAddress, List<GroupMember> groupMemberList)
        {
            bool doesItExist = false;

            foreach (Objects.GroupMember member in groupMemberList)
            {
                if (member.mailAddress.Equals(mailAddress))
                {
                    doesItExist = true;
                }
            }

            return doesItExist;
        }

    }
}

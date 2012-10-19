An mvc4 classifieds project to demonstrate use of RavenDB, IOC and various other technologies.

I will update this ReadMe as the project is built out.

# Step 2

##Add Raven

firstly you should get http://ravendb.org , simply download version 1.2 then run start.bat located in the server folder.

When it launches its interface, create a database called 'AdminUsers', in raven, seperating logical documents into different databases is encouraged.

Add the client library (at this point it is pre-release for raven 1.2) to your project using NuGet

````
Install-Package RavenDB.Client -Pre
````

Now we have raven server running, and the client library. 

Working with raven, we're best to start with data model, So create an AdminAccount object in /models

###AdminAccount
* Email
* Password

We'll leave the form validation stuff (copy from auto-created accountModel before deleting that class)

Fix references to accountModel to use your AdminAccount, remove all the externallogin methods, we'll be doing our own here.

There are some fixes required to Login.cshtml to support the new type as well.

Time to create a Raven DocumentStore factory, I have one I like, which was made with help from the community (https://groups.google.com/forum/?fromgroups=#!topic/ravendb/9aPuWSajPGk)

So, I've addded /service/data/RavenStore.cs

## Implement the Membership provider ValidateUser method
Our first data access code! using this raven singleton is as easy as

````csharp
var sess = RavenStore.Instance("AdminUsers").OpenSession()
````

and then a simple link query

Now, were going to use a simple hashing for passwords, because that's good practice, with a salt of email+salt, storing salt in the web.config

Here I've created our first unitTest to test out the encryption stuff, and make sure its' fine, also we can use this to get a salted password for our first admin user.

Using our unitTest (TestSha1IsRepeatable), I got the encrypted password for the string 'password', using the same salt as I put in the web.config.

Now go to Raven, and create a document in the AdminUsers database matching our AdminAccount schema
with an Id of AdminAccount/[email], so I made

AdminAccount/bill@bigmojo.net

````json
{
  "Email": "bill@bigmojo.net",
  "Password": "uFhSnjNFwVe2bz5IKuABW/b4/iAzDiNKFNO6hWDY81E="
}
````

Ok, were ready to go! fire up the project and try and login.


* Step 2 Notes: Implemented membership provider using RavenDB, and SHA passwording

# Step 1

###  Links
Commit

* SHA: 37f9088aa71ef092f3bd19776fb5e0eb8ebb0460

Tag
https://github.com/billCreativeD/Classifieds/zipball/Step1

## Create The Project
````VS 2012 File->New->MVC4````

* Template is 'Internet Application'
* Check the 'Create Unit Test' option as well
* Run it and bask!

We will also get our own membership provider implemented as part of this step.

* Add a custom Membership Provider RavenMembershipProvider
We will be using RavenDB later on, so we will name our provider appropriately (despite not having raven yet)
This has to extend ExtendedMembershipProvider, implement the methods (you are using Resharper I hope)
For now we will simply allow 'anyone' to login, so in ValidateUser, return true;
* Tell MVC to use your membership provider
Add this to your System.Web config node in web.config

````xml
    <membership defaultProvider="AccountMembershipProvider">
      <providers>
        <clear/>
        <add name="AccountMembershipProvider"
             type="Classifieds.services.UserServices.RavenMembershipProvider" />
      </providers>
    </membership>
 ````

* Run the project and login, you should get through, and bounce to the homepage with the username you entered at the top, as you are logged in.

##First Commit Notes: Project running with custom membership provider
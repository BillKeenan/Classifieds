An mvc4 classifieds project to demonstrate use of RavenDB, IOC and various other technologies.

I will update this ReadMe as the project is built out.

##Step 1 Links
Commit

* SHA: 37f9088aa71ef092f3bd19776fb5e0eb8ebb0460

Tag
https://github.com/billCreativeD/Classifieds/zipball/Step1

# Create The Project
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
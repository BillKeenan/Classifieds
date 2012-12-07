An mvc4 classifieds project to demonstrate use of RavenDB, IOC and various other technologies.

I will update this ReadMe as the project is built out.

The Steps (and tags) will be


* Step 1
Project running with custom membership provider
SHA:37f9088aa7

* Step 2
Implemented membership provider using RavenDB, and SHA passwording
SHA:3802b58821

* Step 3
Create DataModel and sample data for classifieds items


* Step 4
Testing Views

* Step 5
Add Facebook Auth to allow users to create accounts

* Step 6
Allow users to post items

* Step 7
RSS feeds for categories and Search






# Build Notes
# Step 3

Ok, time to get to business, we want to be able to store any number of things, so lets not be too specific with our object, Off the cuff I think having items simply contain an array of tags and a basic category (auto/electronics/etc..), and categories exist in a hierarchy.. So a product could be in multiple categories.

So as a start

## Item
* Name
* Description
* Price
* Location
* User 
* Tags

** Category
* Name
* Children

** User
* Nickname
* Email

For Canonical names I'm using a baseclass with a Guid Implementation that each object will implement, we'll see this when we call store shortly.

With These Created, lets try out creating some categories and items, 

## CHanged my mind!
Category wasn't working out, and thinking about raven, we don't really need it. It simply creates another data structure to manage, and really is subserviant to objects existing in those categories in the first place.

So now we have just items, with an array of strings that are categories
## Item
* Name
* Description
* Price
* Location
* User 
* Tags
* Categories

So for now I've created a number of sample items (cars) and they look like this (check sampledatatests.CreateItems for the code)

{
  "Name": null,
  "Description": " A 2012 Toyota, the model is wagon",
  "Price": null,
  "Location": null,
  "Tags": [
    "wagon",
    "2012",
    "Toyota"
  ],
  "Categories": [
    "Auto",
    "Toyota",
    "2012",
    "wagon"
  ]
}


I'm going to decide that categories can only go 4 levels deep, this is arbitrary, but will be easy to change later.

For searching, we will simply index these items in raven by their categories

 public class FourthCategorySearch : AbstractIndexCreationTask<Item, FourthCategorySearch.Result>
    {
        public class Result
        {
            public string First { get; set; }
            public string Second { get; set; }
            public string Third { get; set; }
            public string Fourth { get; set; }
        }

        public FourthCategorySearch()
        {
            Map = items =>
                  from item in items
                  select new
                  {
                      First = item.Categories[0],
                      Second = item.Categories[1],
                      Third = item.Categories[2],
                      Fourth = item.Categories[3]
                  };
        }
    }

This allows us to search by any category combination of First/Second/Third/Fourth category, and you can see we could add deeper levels and it would be trivial.

I of course immediately found that if an item didnt' have at least 4 categories it would fail! which makes sense, their would be an arrayindexoutofbounds exception somewhere, so adding a safety takes care of that.

 First = item.Categories.Count>0?item.Categories[0]:"",
                      Second = item.Categories.Count > 1 ? item.Categories[1] : "",
                      Third = item.Categories.Count > 2 ? item.Categories[2] : "",
                      Fourth = item.Categories.Count > 3 ? item.Categories[3] : ""


Create that, and then I can now easily in studio do queries like:

First:Auto AND Third:2012

For all 2012 autos

Im not sure if this is better than having a cat1,cat2 properties.. I think it is though.

* Add a simple list and search view
Finally, some output.

Create a SearchController and matching view in /views/search/index.cshtml

We're going to implement the index method, which we will have be a list of top level categories, later on we can add most popular items or something as content.

Our easiest way to get these items, will be to do a mapReduce for all category pats, and then do a select for which we want.

So here we want a reduce, that gives us our first categories, and the count of items.

 public class Result
        {
            public string First { get; set; }
            public int Count { get; set; }
        }

        public CategoryFirstMapReduce(){
                Map=items =>
                       from item in items
                       select new
                                  {
                                      First = item.Categories.Count > 0 ? item.Categories[0] : "",
                                      Count=1
                                  };

            Reduce = results => from result in results
                                group result by result.First
                                into g
                                select new
                                           {
                                               First=g.Key,
                                               Count = g.Sum(x => x.Count)
                                           };


        }

nice and simple, and we can see how easy it would be to add the second level, simply add it to the map, and then the reduce. So we create our index (using our unit test) and pop into studio to see Auto:36 returned from it, lets get that on screen.

Into the search controller, and a simple raven query
    var result = sess.Query<CategoryFirstMapReduce.Result,CategoryFirstMapReduce>();

 drop that into our view
     return View(result) 

 And were off. In the view we declare our model, put a simple iterator, and we can see our result
 So at the top of the search/index.cshtml view
     @model IEnumerable<Classifieds.Indexes.CategoryFirstMapReduce.Result>

And then down in the body, we put our output
    @foreach(var result in Model)
    {
        <div>@result.First:@result.Count</div>
    }

From here it should be simple to see how we could format a landing page, where we see our categories, with totals, and we click in to get second level

@ A digression about Raven
Why would we use this Map-Reduce as opposed to just doing a query, well given that our categories are inferred from our objects, geting a distinct list would require a very expensive sql query, or coding to update an index every time there is an item is stored.. essentially raven is handling that for us, the index, and the map reduce we've created will be updated every time an item is stored, giving us extremely high performance access to these stats, with no ongoing overhead on our parts.

# Step 2

* SHA:3802b58
Tag: Step2

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
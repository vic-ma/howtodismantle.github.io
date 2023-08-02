---
layout: post
title: MS Graph API - Show user's online status by getting it with a POST call
date: 2023-07-21 03:00:00 +0200
tags: msgraph api
image: /assets/2023-07-21/title.png
read_more_links:
  - name: MS Graph API Documentation - Get the presence information for multiple users.
    url: https://learn.microsoft.com/en-us/graph/api/cloudcommunications-getpresencesbyuserid?view=graph-rest-1.0&tabs=http
  - name: MS Graph API - Understand the basis and get started
    url: /MS-Graph-API-Understand-the-basis-and-get-started.html
downloads:
  - name: GraphUsers.pbmx
    url: /assets/2023-07-21/GraphUsers.pbmx
---
This article covers a nice use case for MS Graph: list all the users of a tenant who are currently active or online. At the same time, it's also a nice example on how to combine two different API calls that are dependant on each other.

Please make sure to read through the basics of using MS Graph API in Peakboard: [MS Graph API - Understand the basis and get started]({% post_url 2023-06-09-MS-Graph-API-Understand-the-basis-and-get-started %})

## The calls

For this use case, we have two different API calls.

The first one lists all the users of a tenant, regardless of if they are humans or not, and regardless of if they are online or offline. Every user has an ID—or to be more precise—a GUID. We will need them in the second call.

{% highlight url %}
https://graph.microsoft.com/v1.0/users
{% endhighlight %}

The second call requests the status of a user. 

{% highlight url %}
https://graph.microsoft.com/v1.0/communications/getPresencesByUserId
{% endhighlight %}

It's not necessary to make one call for each user. Instead, we simply send a list of user IDs in the request body. Check out the [spec](https://learn.microsoft.com/en-us/graph/api/cloudcommunications-getpresencesbyuserid?view=graph-rest-1.0&tabs=http) of this call to learn more. Here's an example of how the JSON string looks like when requesting multiple users:

{% highlight json %}
{
	"ids": ["fa8bf3dc-eca7-46b7-bad1-db199b62afc3", "66825e03-7ef5-42da-9069-724602c31f6b"]
}
{% endhighlight %}


## The implementation

The first source we build is the request for the user list. We don't need to provide the URL for this call, as the user list is a built-in function of the Graph extension.

![image](/assets/2023-07-21/010.png)

The second source is a call with a custom URL (see the last paragraph for the URL). The trick is: We use a placeholder within the request body that points to variables called IDs. Thse hold all the users' IDs in the form of `FirstUserID`, `SecondUserID`, etc.

![image](/assets/2023-07-21/020.png)

So how do we fill these variables with the user IDs? We use the `Refereshed` script of the first data source. You can see in the screenshot that there's a loop over the users list. Each ID is wrapped in apostrophes. Unless it's the last ID, a comma is appended, ensuring that the entire string is valid JSON.

![image](/assets/2023-07-21/030.png)

The final thing we must do is make sure that the second data source is only executed after *both* the first data source *and* the `Refereshed` script are executed successfully. For this, we need a reload flow that defines the order of execution. Ideally, this reload flow is maintained in the second data source, as shown in the screenshot.

![image](/assets/2023-07-21/040.png)

The final magic is done with a data flow that is added to the second source. First, we join both sources with the ID.

![image](/assets/2023-07-21/050.png)

Then, we remove all the unnecessary columns, keeping only the online status, email address, and of course, name of the user.

![image](/assets/2023-07-21/060.png)

The last step is to filter for all users that are neither offline nor unknown. That's it!

![image](/assets/2023-07-21/070.png)

Here's the final result:

![image](/assets/2023-07-21/080.png)

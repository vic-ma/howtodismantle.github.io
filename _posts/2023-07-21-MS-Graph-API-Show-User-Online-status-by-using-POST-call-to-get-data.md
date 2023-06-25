---
layout: post
title: MS Graph API - Show User Online status by using POST call to get data
date: 2023-03-01 12:00:00 +0200
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
This article covers a supernice scenario around MS Graph: List all users of a tenant who are currently active or online. At the same time it's also a nice sample on how to combine two different API calls that are dependant on each other.

Please make sure to read through the basics of using MS Graph API in Peakboard: [MS Graph API - Understand the basis and get started]({% post_url 2023-06-09-MS-Graph-API-Understand-the-basis-and-get-started %})

## The Calls

For this use case we two different API calls.

The first one lists all users of tenant, regardless if they are human or technical users and regardless, if they are online or offline. Every user has an ID, to be more precise a GUID. We will need that in the second call.

{% highlight url %}
https://graph.microsoft.com/v1.0/users
{% endhighlight %}

The second call requests the status of a user. 

{% highlight url %}
https://graph.microsoft.com/v1.0/communications/getPresencesByUserId
{% endhighlight %}

It's not necessary to call for every single user, we just send a list of User IDs in the request body. Please check out the [spec](https://learn.microsoft.com/en-us/graph/api/cloudcommunications-getpresencesbyuserid?view=graph-rest-1.0&tabs=http) of this call to learn more about the details. Here's a sample of how the JSon string looks like to request mutliple users:

{% highlight json %}
{
	"ids": ["fa8bf3dc-eca7-46b7-bad1-db199b62afc3", "66825e03-7ef5-42da-9069-724602c31f6b"]
}
{% endhighlight %}


## The implementation

The first source we build is the request for the user list. We don't need to provide the URL for this call, as the user list is a built in function of the Graph extension.

![image](/assets/2023-07-21/010.png)

The second source is a call with a custom URL (see last paragraph for URL). The trick is: we use a placeholder within the request body, that points to variable called IDs. It holds all the user's IDs in the form of "MyFirstUsersID", "MySecondUsersID", etc....

![image](/assets/2023-07-21/020.png)

So how do we fill this variable with the user IDs? This happens in the _Refereshed_ script of the first data source. You can see in the screenshot, that there's a loop over the users list. Every ID is wrapped in apostrophes. Unless it's not the last ID, a comma is added to make sure, that the whole string forms a valid JSon.

![image](/assets/2023-07-21/030.png)

The last thing we must do, is to make sure, that the second data source is only executed after the first data source PLUS the _Refereshed_ script is executed succesfully. For this, we need a Reload Flow, that defines the order of execution. Ideally this Reload flow is maintained in the second data source as shown in the screenshot.

![image](/assets/2023-07-21/040.png)

The final magic is done by a dataflow that is added to the second source. In the first steps we join both sources with the ID.

![image](/assets/2023-07-21/050.png)

The we remove all unnecessary columns except the online status, email adress and of course the name of the user.

![image](/assets/2023-07-21/060.png)

The last step is a filter on all users, that are neither offline nor unknown. That's it!

![image](/assets/2023-07-21/070.png)

Here's the final result:

![image](/assets/2023-07-21/080.png)





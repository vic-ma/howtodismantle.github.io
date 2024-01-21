---
layout: post
title: Reading and writing Sharepoint lists with Graph extension
date: 2024-01-20 12:00:00 +0200
tags: msgraph office365
image: /assets/2024-01-20/title.png
read_more_links:
  - name: MS Graph API - Understand the basis and get started
    url: /MS-Graph-API-Understand-the-basis-and-get-started.html
  - name: MS Graph API findRooms documentation
    url: https://learn.microsoft.com/en-us/graph/api/user-findrooms
  - name: MS Graph API list events documentation
    url: https://learn.microsoft.com/en-us/graph/api/user-list-events
  - name: MS Graph Explorer
    url: https://developer.microsoft.com/en-us/graph/graph-explorer
downloads:
  - name: SharePointListWithGraphAPI.pbmx
    url: /assets/2024-01-20/SharePointListWithGraphAPI.pbmx
---
In this article, we will learn how to read and write a Sharepoint list in an Office 365 environment. Before we start, make sure you understand the [basics of the MS Graph API extension](/MS-Graph-API-Understand-the-basis-and-get-started.html).

Peakboard offers native access to Sharepoint lists.
However, the built-in data source does not support multifactor authentication.
If you don't need multifactor authentication, and you only need to read data, then you can use the built-in data source.
 
But if you do need multifactor authentication, then this article will teach you how to solve the problems with the MS Graph API.

We will use a sample list, like the one shown in the screenshot. Beside the string columns, there is a numeric column for quantities. These are treated a little differently than strings.

![image](/assets/2024-01-20/005.png)

## Get the Sharepoint IDs

Before we can use the MS Graph API for Sharepoint lists in Peakboard,
we first need to get the Sharepoint Site ID and Sharepoint List ID.
This API call gets a list of all Sites in the Sharepoint instance. [Here](https://learn.microsoft.com/en-us/graph/api/site-list?view=graph-rest-1.0&tabs=http) is the documentation.

{% highlight url %}
https://graph.microsoft.com/v1.0/sites?Search=*
{% endhighlight %}

If we execute the call in the Graph Explorer, we will find the ID inside the response.

![image](/assets/2024-01-20/010.png)

We can get all the available lists in the site, with the following call. We just need to plug in the Site ID. [Here]( https://learn.microsoft.com/en-us/graph/api/list-list?view=graph-rest-1.0&tabs=http) is the documentation.

{% highlight url %}
https://graph.microsoft.com/v1.0/sites/{site-id}/lists
{% endhighlight %}

The following screenshot shows how to find the list ID within the response of the call.

![image](/assets/2024-01-20/020.png)

## Set up the data source

In Peakboard Designer, we need to understand the API to get the items of the list. Firstly, you can find the documentation [here](https://learn.microsoft.com/en-us/graph/api/listitem-list?view=graph-rest-1.0&tabs=http). The call looks like this:

{% highlight url %}
https://graph.microsoft.com/v1.0/sites/{site-id}/lists/{list-id}/items?expand=fields(select=Column1,Column2)
{% endhighlight %}

With the given Site ID and List ID, and the columns `MaterialNo`, `Description`, `Color`, and `QuantityOnStock`, this is the actual call:

{% highlight url %}
https://graph.microsoft.com/v1.0/sites/xxx.sharepoint.com,0ca4593b-ac3b-45d3-88aa-xx75a54b93,96c52b38-7eb7-4f20-923c-2e8fe2cb3595
    /lists/276aadc7-77ee-48xxxx-2fd264778fde/
    items?expand=fields(select=MaterialNo,Description,Color,QuantityOnStock)
{% endhighlight %}

Here's how it looks like in Peakboard Designer when using the UserAuthCustomList. Make sure to apply the `Sites.Read.All` permission before authenticating.

The columns we're looking for are all on the right end of the columns list. The first 10-12 columns only contain administrative information (creation date, creation user, etc.).

![image](/assets/2024-01-20/030.png)

Finally, we can build a nice table control and bind the data source to it.

![image](/assets/2024-01-20/040.png)

## Writing to the list

To understand how to write data back to Sharepoint, read [this documentation](https://learn.microsoft.com/en-us/graph/api/listitem-create?view=graph-rest-1.0&tabs=http). The API expects a POST call to this endpoint:

{% highlight url %}
https://graph.microsoft.com/v1.0/sites/{site-id}/lists/{list-id}/items
{% endhighlight %}

In the POST call, we need to send a JSON body that contains the names and values of the columns of the row we want to add to the list:

{% highlight json %}
{
  "fields": {
    "MaterialNo": "0815",
    "Color": "Black",
    "QuantityOnStock": 90,
    "Description": "Lady's Shirt"
  }
}
{% endhighlight %}

Next, we create a MsGraphCustomFunctionsList data source. We add a new function to the list with the correct URL (replace Site ID and List ID with actual values). We also add our JSON object to the body.

Note that we replaced the actual values with placeholders like `$s_materialno$`. Because the quantity column in our list is numeric, the placeholder must start with `d` (for double): `$d_quantity$`.

![image](/assets/2024-01-20/050.png)

Next, we build a small form with input boxes that let the user provide the information for the row they want to add.

![image](/assets/2024-01-20/060.png)

Now, let's have a look at the code behind the **Add...** button. We use an Extension Functions block. Because we used placeholders in the JSON body, the Building Blocks editor automatically offers the right sockets for our dynamic string from the text box. After the submission, we reload the list to update the data. That's it.

![image](/assets/2024-01-20/070.png)

Here's the final result in action:

![image](/assets/2024-01-20/result.gif)
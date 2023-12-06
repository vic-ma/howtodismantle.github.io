---
layout: post
title: Reading and writing Sharepoint lists with Graph extension
date: 2023-03-01 12:00:00 +0200
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
In this article we will learn how to read and write a Sharepoint list in an Office 365 environment. Before we start, make sure to understand the [basics of the MS Graph API extension](/MS-Graph-API-Understand-the-basis-and-get-started.html).

Peakboard offers native access to Sharepoint lists.
However, the built-in data source does not support multifactor authentication.
If you don't need multifactor authentication, and you only want to read data, then you can use the built-in data source. But if you do need multifactor authentication, then this article will teach you how to solve the problems with the MS Graph API.

We will use a sample list like the one shown in the screenshot. Beside the string columns, there is a numeric column for quantities. These are treated a little differently than strings.

![image](/assets/2024-01-20/005.png)

## Get the requirements

Before we can use the MS Graph API for Sharepoint lists in Peakboard,
we first need to get the Sharepoint Site ID and Sharepoint List ID.
This API Call gets a list of all Sites within the Sharepoint instance. We can find the documention [here](https://learn.microsoft.com/en-us/graph/api/site-list?view=graph-rest-1.0&tabs=http).

{% highlight url %}
https://graph.microsoft.com/v1.0/sites?Search=*
{% endhighlight %}

If we execute the call in the Graph Explorer we can find the ID in the result set of the call.

![image](/assets/2024-01-20/010.png)

With the help of Site ID we can get a list of all available lists within this site by using the following call. [Here]( https://learn.microsoft.com/en-us/graph/api/list-list?view=graph-rest-1.0&tabs=http) is the documentation.

{% highlight url %}
https://graph.microsoft.com/v1.0/sites/{site-id}/lists
{% endhighlight %}

The screenshot shows how to find the list ID from the the response of the call.

![image](/assets/2024-01-20/020.png)

## Set up the data source

After getting back to the Peakboard designer we need to understand the API to get the items of the list. First of all [here](https://learn.microsoft.com/en-us/graph/api/listitem-list?view=graph-rest-1.0&tabs=http) you find the documentation. The call looks like this:

{% highlight url %}
https://graph.microsoft.com/v1.0/sites/{site-id}/lists/{list-id}/items?expand=fields(select=Column1,Column2)
{% endhighlight %}

With the given Site ID and List ID and the colums MaterialNo, Description, Color and QuantityOnStock this is the real call:

{% highlight url %}
https://graph.microsoft.com/v1.0/sites/xxx.sharepoint.com,0ca4593b-ac3b-45d3-88aa-xx75a54b93,96c52b38-7eb7-4f20-923c-2e8fe2cb3595
    /lists/276aadc7-77ee-48xxxx-2fd264778fde/
    items?expand=fields(select=MaterialNo,Description,Color, QuantityOnStock)
{% endhighlight %}

Here's how it looks like in designer when using the UserAuthCustomList. Please make sure to apply the Sites.Read.All before authenticating. The columns we're looking for are all on the right end of the columns list. The first 10-12 columns only contain administrative information (creation date, creation user, etc...).

![image](/assets/2024-01-20/030.png)

So finally we can build a nice table control and bind the data source to it.

![image](/assets/2024-01-20/040.png)

## Writing back to the list

To understand how to write data back to Sharepoint we can read [this](https://learn.microsoft.com/en-us/graph/api/listitem-create?view=graph-rest-1.0&tabs=http) documentation. The API is a POST call to this endpoint:

{% highlight url %}
https://graph.microsoft.com/v1.0/sites/{site-id}/lists/{list-id}/items
{% endhighlight %}

In this POST call we need to send a JSon body that looks like the following containing name and value of the columns of the row we want to add to the list:

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

Now we simply build another data source in our project of type MsGraphCustomFunctionsList and add a new function to the list with the correct URL (replace Site ID and List ID with real values), and also add our JSon to the body. Please note, that we replaced the actual values with placeholders like $s_materialno$. As the quantity column in our list is numeric the placeholder must start with d (for double): $d_quantity$.

![image](/assets/2024-01-20/050.png)

Now we build a small form with input boxes and let the user provide the details of the row to be added.

![image](/assets/2024-01-20/060.png)

Now let's have a look an the code behind the 'Add...'' button. We just use an Extension Functions block. As we used placeholders in the JSon body the Bulding Blocks editor automatically offers the right sockets to plug our dynamic string from the text box. After the submit we do a relad of the list to refresh it. That's it.

![image](/assets/2024-01-20/070.png)

Here's the final result in action:

![image](/assets/2024-01-20/result.gif)
---
layout: post
title: Dismantle Hubspot API - Create an interactive form to submit contact data directly to Hubspot by using REST API
date: 2023-03-01 12:00:00 +0200
tags: api
image: /assets/2024-01-12/title.png
read_more_links:
  - name: HubSpot API Docs
    url: https://developers.hubspot.com/docs/api/crm/contacts
downloads:
  - name: HubspotContactGenerator.pbmx
    url: /assets/2024-01-12/HubspotContactGenerator.pbmx
---

Over the past few years, HubSpot has become one of the top tools for marketing automation and sales. A typical use case would be to put all kinds of contacts into HubSpot that are gathered by the company through different ways.

This article shows how to build a self-service form to submit contact data to HubSpot.

Let's assume an ice cream company is using this self-service terminal next to an ice cream stand to ask their customers for their favourite flavor. The customer can also enter a lucky draw to win prices. Of course, the company wants to have this data in HubSpot as fast as possible, to check back with the customer and send them their newsletter and other sales material.

The example in this article shows how to build a JSON string and submit it to a real world API.

## The API

The HubSpot API is not too complicated to use and is based on typical REST web services. The endpoint we use is `https://api.hubapi.com/crm/v3/objects/contacts`. To better understand the API, you can read the [HubSpot API dev guide](https://developers.hubspot.com/docs/api/crm/contacts).

Because we want to create a contact in HubSpot, we need to submit a JSON string in the body of the HTTP call. Here is a very simple example of what the JSON must look like, in order to be understood by the HubSpot server. We provide the name, email address, and a HubSpot custom field called `favourite_ice_cream`, which contains the flavour the customer has chosen in the form.

{% highlight json %}
{
  "properties": {
    "email": "uli1990@gmail.com",
    "firstname": "Ulrike",
    "favourite_ice_cream": "Vanilla"
  }
}
{% endhighlight %}

For authentication, we need to submit an API token. To get a token, we go to the settings section of our HubSpot account and create a private app. We store the token for later usage, when Peakboard is initiating the call.

![image](/assets/2024-01-12/010.png)

## Build the application screen

Now let's build the application in Peakboard.

We want to give the user the option of choosing their flavour from a combo box. To fill the combo box with the appropriate values, we create a variable list with all the possible flavors. It's a simple list with only one column.

![image](/assets/2024-01-12/020.png)

The screen is simple. We chose a nice background image, add the text, and put the interactive elements on the screen. To fill the combo box with values, we connect it to the variable list. We also give all three input controls a proper name, so we can address them from within our code.

![image](/assets/2024-01-12/030.png)

## Build the REST call

Let's have a look now at the code behind the submit button. Here's what happens:

1. The JSON string is stored in a variable with three placeholders within the string. They all begin with a `@` character, to make them easier to identify.
2. The placeholders are replaced by the actual values that come from the three input controls of the screen.
3. This is the actual HTTP call. It's a POST call, according to the documentation. We need to add two headers to make it work.
    * The first header is `Authorization`. Here, we submit he value `Bearer <mytoken>`.
    * The second header is `Content-Type`. We set it to 'application/json', otherwise HubSpot doesn't understand what to do with the string in the HTTP body.

If this would not be a simple example, we would have to interpret the return message for any errors. For keep it simple we don't do this here but just write the response to the log.

![image](/assets/2024-01-12/040.png)

## The result

Here's the board in full swing.

![image](/assets/2024-01-12/050.png)

Let's check the log. We ca see the JSon that is built dynmically. And also the answer from the HubSpot API server.

![image](/assets/2024-01-12/060.png)

And here's the result in HubSpot....

![image](/assets/2024-01-12/070.png)


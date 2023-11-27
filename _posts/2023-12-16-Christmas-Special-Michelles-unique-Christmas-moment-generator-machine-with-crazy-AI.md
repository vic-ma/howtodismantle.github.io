---
layout: post
title: Christmas Special - Michelle's unique Christmas moment generator machine with crazy AI
date: 2023-03-01 12:00:00 +0200
tags: api ai
image: /assets/2024-12-16/title.png
read_more_links:
  - name: dev guide for image generation with Open AI
    url: https://platform.openai.com/docs/guides/images/usage?context=node&lang=curl
downloads:
  - name: ChristmasMomentGeneratorMachine.pbmx
    url: /assets/2023-12-16/ChristmasMomentGeneratorMachine.pbmx
---

With the release of this article this blog exits precisely one year. The first article was published in December 2022. 2023 was very clearly the year of the rise of articficial intelligence AI. That's why we dedicate this article to how to use AI. Unlike the other articles we will first have a look at the result. WHat we're creating is unique Christmas moment generator machine. You can submit your current mood


## The API

The Hubspot API is not too complicated to use and is based on typical REST webservices. The endpoint we're using in our case is https://api.hubapi.com/crm/v3/objects/contacts. To understand the API better, we can look up all details in the [API dev guide](https://developers.hubspot.com/docs/api/crm/contacts).

As we want to create a contact in Hubspot, we need to submit a JSon string in the body of the HTTP call. Here is a very simple example of how the JSon must look like in order by understood be the Hubspot server. We provide the name, emailaddress and a Hubspot custom field called favourite_ice_cream which contains the flavour the customer has chosen in the form.

{% highlight json %}
{
  "properties": {
    "email": "uli1990@gmail.com",
    "firstname": "Ulrike",
    "favourite_ice_cream": "Vanilla"
  }
}
{% endhighlight %}

For the authentification we need to submit an API token. To get an token, we go to the settings section of our hubspot account and create a private app. The token is stored for usage later when Peakboard is initiating the call.

![image](/assets/2024-01-12/010.png)

## Build the application screen

We want to give the user the option of chosing their flavor from a combo box. To fill the combo box with values we need variable list with all potential flavors. It's a simple list with only one column.

![image](/assets/2024-01-12/020.png)

The screen is simple. We chose a nice background image, put the texts and put the interactive elements on the screen. To fill the combo box with values we need to connect it to the variable list. And we also give any of the three input controls a proper name, so we can address them from within our code.

![image](/assets/2024-01-12/030.png)

## Build the REST call

Lets have a look now at the code behind the submit button. Here's what happens:

1. The JSon string is stored in a variable with three placeholders within the string. They all begin with a @ character to make it easier to identify.
2. The placeholders are relaced with the actual values that come from the three input controls of the screen.
3. This is the actual HTTP call. It's a POST call according to the documentation. We need to add two headers to make it work. The first header 'Authorization'. Here we submit he value 'Bearer <mytoken>'. The second header is the content type. We set it to 'application/json', otherwise Hubspot doesn't understand what to do with the string in the HTTP body.

If this would not be a simple example, we would have to interpret the return message for any errors. For keep it simple we don't do this here but just write the response to the log.

![image](/assets/2024-01-12/040.png)

## The result

Here's the board in full swing.

![image](/assets/2024-01-12/050.png)

Let's check the log. We ca see the JSon that is built dynmically. And also the answer from the Hubspot API server.

![image](/assets/2024-01-12/060.png)

And here's the result in Hubspot....

![image](/assets/2024-01-12/070.png)


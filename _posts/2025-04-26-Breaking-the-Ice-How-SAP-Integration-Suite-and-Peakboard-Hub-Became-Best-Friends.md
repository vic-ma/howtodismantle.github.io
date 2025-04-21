---
layout: post
title: Breaking the Ice - How SAP Integration Suite and Peakboard Hub Became Best Friends
date: 2023-03-01 03:00:00 +0200
tags: peakboardhub peakboardhubapi sap sapbtp
image: /assets/2025-04-26/title.png
image_landscape: /assets/2025-04-26/title_landscape.png
bg_alternative: true
read_more_links:
  - name: Peakboard Hub API Swagger portal
    url: https://api.peakboard.com/public-api/index.html
  - name: Peakboard Hub API getting started
    url: /Cracking-the-code-Part-I-Getting-started-with-Peakboard-Hub-API.html
downloads:
  - name: Iflow TransferOrderToPeakboardHub.zip
    url: /assets/2025-04-26/TransferOrderToPeakboardHub.zip
---
The future of SAP is clear: Everything will be cloud based. The age of on-prem SAP systems is over. And when it comes to connecting systems with one another other---both SAP and non-SAP---the best solution is SAP BTP (Bussiness Technology Platform).

A big part of SAP BTP is the SAP Integration Suite. This is where all the API magic happens and where communication with the outside world is built and configured (more precisely, outside-of-SAP world). One of the artifacts we can build in the SAP Integration Suite is an integration flow (iFlow). This is what we will do today.

We will build an iFlow that submits a transfer order to Peakboard Hub, and stores it in the transfer order table. That way, the iFlow can be used like a blueprint for a general SAP-Peakboard-Hub communications.

## Setup

First, we create a simple table in Peakboard Hub to store our transfer orders:

![image](/assets/2025-04-26/010.png)

Next, we get an API key. You can learn how to do this in our [Peakboard Hub API getting started](/Cracking-the-code-Part-I-Getting-started-with-Peakboard-Hub-API.html) article.

We also need a valid SAP BTP account.

Finally, we create a new package and a new Integration Flow. 

## Build the integration flow

We will build an integration flow that does the following:
1. Add a header with our API key.
2. Call Peakboard Hub to get an access token.
3. Convert the response from the access token request into XML.
4. Use XPath to extract the access token from the XML body and store it in a property.
5. Add a header with our access token.
5. Set request body.
6. Call the Peakboard Hub API to store the values of the transfer order into the table.

Here's what the complete integration flow looks like:

![image](/assets/2025-04-26/020.png)

### Add a header with our API key

For our first step, we add an `apiKey` header with our API key.  We need this header in order to get our access token in the next step.

![image](/assets/2025-04-26/030.png)

## Call Peakboard Hub to get an Access Token

The next step is a Request/Response block that calls this external URL:

{% highlight url %}
https://api.peakboard.com/public-api/v1/auth/token
{% endhighlight %}

This Peakboard Hub API call returns an access token.

![image](/assets/2025-04-26/040.png)

## Convert the response from the access token request into XML

We need to get the token from the API response. But processing JSON in an SAP integration flow is tricky. So instead, we will use XPath get the token. In order to use XPath, we first need to convert the response body into XML:

![image](/assets/2025-04-26/050.png)

## Use XPath to extract the access token from the XML body and store it in a property

We apply the XPath command `//AccessToken` to the XML body in order to get the access token and store it in an internal property.

![image](/assets/2025-04-26/060.png)

## Add a header with our access token.

We add an `Authorization` header, in order to authenticate our API calls. We set the header to `Bearer`, combined with our access token.

![image](/assets/2025-04-26/070.png)

For the body we use the JSON that is needed to feed the Peakboard Hub API. In our table we will store four values: "FromLocation", "ToLocation" is the router of the transfer order, "MaterialNo" the Product to be moved, and "Quantity" the amount of products. The actual value are taken from the headers by using an expression (e.g. "${header.FromLocation}"). So the caller of this integration flow can submit his values just by adding this to the query string of the URL later.

![image](/assets/2025-04-26/080.png)

## 6. Call the Peakboard Hub API to store the values of the transfer order into the table

The last step is the actual call of the table store function "https://api.peakboard.com/public-api/v1/lists/items". In this case it's a POST call. The authentication method is set to none, as we're doing the authentication in our header that we created earlier.

![image](/assets/2025-04-26/090.png)

After building these steps, we save and deploy the service.

## result and test

The screenshot shows the actual service as it is called through Postman. The values to be stored are submitted as query parameters. So they will be mapped into the JSON during the process. As we see, our integration service returns the repsonse body ofr the Peakboard Hub API. This is done automatically as this call as last one to be processed in the flow.

![image](/assets/2025-04-26/100.png)

And here's how the data looks like in the Hub table.

![image](/assets/2025-04-26/110.png)



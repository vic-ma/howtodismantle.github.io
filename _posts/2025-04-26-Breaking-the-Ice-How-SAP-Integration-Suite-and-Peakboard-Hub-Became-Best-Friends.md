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
The long term strategy of SAP is very clear: Everything will be cloud based. The time of on-prem SAP systems is over. And when it comes to connecting the systems with each other - both SAP and non-SAP - the ultimate answer will be SAP BTP - Bussiness Technology Platform. One of the big part of SAP BTP is the so called SAP Integation Suite. Here all the API magic happens and the communication with the outside world (or better said outside-SAP world) is built and configured. One of the artifacts we can build in the SAP integration Suite is the Integration FLow, or just iflow. This is what we will do today. We will build an iflow that submits a transfer order to the Peakboard Hub and stores it in the transfer order table. So that iflow can be used as some kind of blueprint for a general SAP-Peakboard-Hub-communication.

## The requirements

First we need a simple table in the Peakboard Hub to store our transfer orders.

![image](/assets/2025-04-26/010.png)

Beside this we will need an API key. How to get that we can find out in our [Peakboard Hub API getting started article](/Cracking-the-code-Part-I-Getting-started-with-Peakboard-Hub-API.html). 

Beside this we will need a valid SAP BTP account and create a new package and a new Integration Flow. 

## The strategy

The Integration FLow we're building will follow these steps:

1. Set The API key to the correct header
2. Call the Peakboard Hub to get an Access Token
3. Convert the answer of the access token call into XML
4. Use XPath to get the access token from the XML body and store it in a property
5. Get the access token from the property and put it into the header, Prepare the body for the next call
6. Call the Peakboard Hub API to store the values of the transfer order into the table

Here's the how the complete integration flow looks like:

![image](/assets/2025-04-26/020.png)

## Set The API key to the correct header

In the first step we set the API key to the header, so that it's submitted in the next call

![image](/assets/2025-04-26/030.png)

## 2. Call the Peakboard Hub to get an Access Token

The next step is a Request/Response block calling the external URL "https://api.peakboard.com/public-api/v1/auth/token" to receive the access token.

![image](/assets/2025-04-26/040.png)

## 3. Convert the answer of the access token call into XML

Processing JSON in SAP Integration Flow is tricky. It's much easier to use XPath later. That's why we convert the response body of the last call into XML.

![image](/assets/2025-04-26/050.png)

## 4. Use XPath to get the access token from the XML body and store it in a property

In this step we apply the XPath command "//AccessToken" to the XML body to get the Access token and store in an internal property.

![image](/assets/2025-04-26/060.png)

## 5. Get the access token from the property and put it into the header, Prepare the body for the next call

We need to prepare header and body for the next call of the Peakboard Hub API. For the header we use the stored access token from the property and store it together with the literal "Bearer".

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



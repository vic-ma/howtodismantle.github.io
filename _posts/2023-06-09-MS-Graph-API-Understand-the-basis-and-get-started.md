---
layout: post
title: MS Graph API - Understand the basis and get started
date: 2023-03-01 12:00:00 +0200
tags: opcua tutorial
image: /assets/2023-06-09/title.png
---
Using calls of the MS Graph API along with Peakboard needs some basic understanding and requirements. This article covers the minimum basics you need to have to go ahead with MS Graph. It's boring, but it's necessary. Sorry for this.

### The extension

As by June 2023 Peakboard doesn't support Graph calls natively, but there's an extension for this. Please check out the extension's [website](https://templates.peakboard.com/extensions/Microsoft-Graph/en) to find out more. You can download and install it like any other extension.

### The lists

After installing there are three types of lists available.

![image](/assets/2023-06-09/010.png)

Here's the difference between these three lists:

# User Delegated Access
The Graph data is accessed in the name and with the privileges of a dedicated user. Before you can use this list, you alsway have to authenticate by doing a authentification process. Most Graph calls require this kind access type.

# App-Only Access
The Graph data is accessed in the name of the application only. That's easier to handle as the first one. 

# User Function
Using a User Function list is mostly done when calling a certain function to do some action rather than querying list. The authentification pattern is the same as with the User Delegated access.

### The Graph explorer

Microsoft offers a quite handy tool to browse and test Graph functionscalled [Graph Explorer](https://developer.microsoft.com/en-us/graph/graph-explorer). The best way to master a new task is to precisely understand a call in explorer first and then use Peakboard to call it.

Also there is an excellent [Documentation](https://learn.microsoft.com/en-us/graph/api/overview?view=graph-rest-1.0) available that offers lots of sample calls.

### The App registration in Azure

For any of the three kinds of access the caller needs to identify himself. For this an App registration is necessary in Azure AD. Provided that you have admin rights, go to the App registration section and create a new app.
You need three things later:

1. The Tenant ID
2. The client ID
3. The client secret (in case you use the App-Only list) 

![image](/assets/2023-06-09/020.png)

Make sure to allow public client flows.

![image](/assets/2023-06-09/030.png)

And also check the API permission section. In the screenshot you see, that this app has a lot of permissions granted. WHhich permission you need for which call is said in the documentation of each Graph call.

![image](/assets/2023-06-09/040.png)

### The first call

We go back to the Peakboard designer and add a simple App-Only list to the data sources and we provide the three values we noted in he earlier steps (IDs plus secret).
In the combo box, there are already a couple of usefull calls available. So you don't have to provide the Graph call URL for the first step. Just use _user_ and refresh the data.

![image](/assets/2023-06-09/050.png)

In case you don't want ot rely on the the prepacked combo box calls, just use the _custom call_ option as shown in the next screenshot. In most of the articles in this blog, we will use this option.

![image](/assets/2023-06-09/060.png)

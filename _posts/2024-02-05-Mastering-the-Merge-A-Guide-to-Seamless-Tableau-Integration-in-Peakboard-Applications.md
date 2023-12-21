---
layout: post
title: Mastering the Merge - A Guide to Seamless Tableau Integration in Peakboard Applications
date: 2023-03-01 12:00:00 +0200
tags: tutorial bestpractice api
image: /assets/2024-02-05/title.png
read_more_links:
  - name: Configure Embedding Objects and Components
    url: https://help.tableau.com/current/api/embedding_api/en-us/docs/embedding_api_configure.html
  - name: Tableau Token Generator Extension
    url: https://templates.peakboard.com/extensions/Tableau-Token-Generator/en
  - name: Best Practice - Use Power BI for integrating maps
    url: /best-practice-powerbi-for-map-integration.html
downloads:
  - name: TableauInPeakboard.pbmx
    url: /assets/2024-02-05/TableauInPeakboard.pbmx
---


Peakboard applications are often used with BI tools. We discussed this topic back in [this article](/best-practice-powerbi-for-map-integration.html) where we embedded a Power BI map. In today's article, we will do something similar, but with Tableau.

Unlike with Power BI, there isn't a dedicated control for Tableau Dashboards. So we use the HTML control and put some dynamic HTML code in. The control processes this code and displays the HTML-based dashboard.

The tricky part here is authenticating ourselves. Tableau provides a token to access the Tableau portal and dashboard as an external app. To get this token, we need a Peakboard extension called the *Tableau Token Generator*. Here's what this extension does:
1. Generate a Tableau token.
2. Generate some dynamic HTML.
3. Inject the newly generated token into the dynamic HTML.

## Set up the Tableau portal

First, we need to create a Tableau connected app. To do this, we go to the Tableau portal settings and click on **Connected Apps**. Then, we create a new connected app. We generate a secret for the connected app, and we copy the *secret ID*, *secret value*, and *client ID*. We will need these later.

![image](/assets/2024-02-05/010.png)

![image](/assets/2024-02-05/020.png)

## Preparing the data source for generating a token

To generate an authentication token, we need the Tableau Token Generator extension. We add it to Peakboard Designer by clicking on **Data Source&nbsp;> Add Data Source&nbsp;> Manage Extension** and then installing the extension.

![image](/assets/2024-02-05/030.png)

The data source needs the following four parameters:
1. Tableau portal username
2. Client ID
3. Client Secret
4. Secret Value

After filling out all the values, we click on the data load button to check if the token was generated properly. The output of the data source has one column and one row with the token. 

![image](/assets/2024-02-05/040.png)

## HTML generation

In the Tableau portal we look up the dashboard or visualisation we want to use later. The important part is the URL.

![image](/assets/2024-02-05/050.png)

Let's now have a look at the HTML we need to generate. In the following code you can see three variable parts: the server, the URL to the Tableau Dashboard and the Token.

{% highlight html %}
<script type="module" src="https://MyServer/javascripts/api/tableau.embedding.3.latest.min.js">
    </script>
<tableau-viz id="tableauViz" src="MyVisURL" width="1920" height="883" toolbar="bottom" iframe-auth token="MyToken">
  </tableau-viz>
{% endhighlight %}

So for later we build two global variables. One for the server and one for the URL. Of course we can also use these parts as fxed values in HTML but it's a good habbit to make them dynamic. So we can chenge it easily later without touching the actual HTML code.

![image](/assets/2024-02-05/055.png)

![image](/assets/2024-02-05/056.png)

The last part is the HTML control to be placed in the middle of the canvas and give it a proper name.

![image](/assets/2024-02-05/060.png)

The actual magic happens in the Refreshed script of the Token generator data source. As you can see in the screenshot we simply build the HTML code and put the three dynamic values in the right places of the code. After concatenating all these we apply the HTML code to the HTML property of the HTML control. That's it....

![image](/assets/2024-02-05/070.png)

And here's the final result:

![image](/assets/2024-02-05/080.png)

## Conclusion and outlook

As soon as we solved the authentification issue with the token all the rest is easy doing. Generating the HTML code is straight forward and not too complicated. What we have not discussed in this article is how to restrict the Tableau dashboard and get rid of the toolbars or tabs and set filters or allow or disallow certain levels of interactivity. All that things can be easily configured within the dynamic HTML. Just check out the [Tableau documentation](https://help.tableau.com/current/api/embedding_api/en-us/docs/embedding_api_configure.html) for more details.

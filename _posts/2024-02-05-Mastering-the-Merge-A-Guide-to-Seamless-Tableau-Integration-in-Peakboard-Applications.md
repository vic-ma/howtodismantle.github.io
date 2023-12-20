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


Peakboard applications are often used together with existing BI tools. We already discussed this topic back in [this article](/best-practice-powerbi-for-map-integration.html) where we embedded a Power BI map. In today's article we want to do something similiar for Tableau. Unlike for Power BI there's no special control for Tableau Dashbaords. We can just use the HTML control, put some dynamic HTML code in there to be interpreted by this control and then show the HTML based Dashboard. The super tricky part during that whole process is proper authentification. Tableau wants us to get a token for accessing the Tableau portal and dashboard as an external app. To get this token we need a Peakboard extension with the only purpose to generate this token and then generate some dynamic HTML and inject the newly generated token there. 

## Set up the Tabelau portal

Before we step into the Peakboard designer we need to create a so called Connected App. For this we go tot the Tableau portal settings -> Connected App and generate a new entry. After generating a new seceret within the connected app, we write down the Secret ID, Secret Value and CLient ID. All three values are needed later.

![image](/assets/2024-02-05/010.png)

![image](/assets/2024-02-05/020.png)

## Preparing the data source for generating a token

To generate the token we need the Tableau Token Generator extension. You can just add it to your designer instance by clicking on Data Source -> Add Data Source -> Manage Extension and find the right extension and install it.

![image](/assets/2024-02-05/030.png)

The data source needs four parameters. The first one is the user name for the Tableau portal. The three others are Client ID, Client Secret and Secret Value as noted from the last paragraph. After filling out all values we can click on the data load button to check, if the token is generated properly. The output of the data source only has one column and one row with the token. 

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

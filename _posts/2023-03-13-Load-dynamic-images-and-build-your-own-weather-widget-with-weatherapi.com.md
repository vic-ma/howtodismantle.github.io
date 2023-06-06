---
layout: post
title: Load dynamic images and build your own weather widget with weatherapi.com
date: 2023-03-13 12:00:00 +0200
tags: api basics
image: /assets/2023-03-13/060.png
---
There are uncountable services on the internet to provide weather information either as a widget or as API. In this article we will learn more about how to use [weatherapi.com](http://weatherapi.com). They offer a free plan to get basic weather information from any location in the world. If you want to follow this tutorial step-by-step make sure to sign up for the free plan and get your own API key.

The [API explorer](https://www.weatherapi.com/api-explorer.aspx) lets you play around with the API call we use. The only parameter is the name of the location (can be just a name, coordinates or zip code). The call we're using is just

{% highlight url %}
http://api.weatherapi.com/v1/current.json?key=XXX&q=Taipei&aqi=no
{% endhighlight %}

while XXX is the API key.

Let's create a new board and add a JSon data source. We don't have to deal with authentication issues, as the API key in the URL does the trick to tell the server who we are.

![image](/assets/2023-03-13/010.png)

Visualizing the most important parameters lcation (column _name_), temperature (column _temp_c_) and the weather description (column _text_) can be easily done with a single textbox with title and subtitle. Please check the screenshot for how to do the formatting of the temperature. In that case we use a custom format with the unit "deg Celsius" to beautify the number.

![image](/assets/2023-03-13/020.png)

Weatherapi.com provides the caller with a link to an image to visualize the weather condition (e.g. cloud or sunshine). To use these icons direcly in our board, we place an image control on the canvas and add a link to a web resource to feed the image control with a default pic. Just use a link to a random generic pic like [a red cross](https://upload.wikimedia.org/wikipedia/commons/thumb/5/5f/Red_X.svg/100px-Red_X.svg.png).  
The actual image is not so important, as we will override the image anyway with the what the API returns to us. 

![image](/assets/2023-03-13/030.png)

Every time the data is queried from the API, we can use the _Refreshed_-event to arrange all proceeding actions, like setting the URL of the image control. 

![image](/assets/2023-03-13/040.png)

Here's the block we're using to build the URL. The value of the API comes with a URL, but the "http:" is missing to make the URL valid. So we just add it and set the resulting value to the _Source_ proerty of the image control. That's it!

![image](/assets/2023-03-13/050.png)

And here's the result:

![image](/assets/2023-03-13/060.png)

* Download [WeatherAPI.pbmx](/assets/2023-03-13/WeatherAPI.pbmx)


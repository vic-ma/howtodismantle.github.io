---
layout: post
title: Rain or Shine - Visualizing Weather Data with openweathermap.org and Peakboard
date: 2023-03-01 00:00:00 +0200
tags: api lua
image: /assets/2025-05-20/title.png
image_landscape: /assets/2025-05-20/title_landscape.png
bg_alternative: true
read_more_links:
- name: Openweathermap.org API current
    url: https://openweathermap.org/current
- name: Openweathermap.org API forecast
    url: https://openweathermap.org/forecast16
  - name: LUA time and date formatting 
    url: https://www.lua.org/pil/22.1.html
downloads:
  - name: WeatherForecast.pbmx
    url: /assets/2025-05-20/WeatherForecast.pbmx
---
Openweathermap.org is a very simple, easy-to-use weather API provider for any place in the world. It's the data backend for lots of website, apps and other applications. And the best thing: Some API endpoints are free of charge as long as we only use a limited number of calls. In this wrticle we will learn, how to use the openweathermap.org API for the current weather but also to build a weather forecast for the next days. Beside the API calls we will discuss some topics around time formatting and how to set an image dynmically by using a LUA script. The screenshot shows the final result of the application we will build.

![image](/assets/2025-05-20/010.png)

## Preparing the account

For getting access to the API we need an account at openweathermap.org. We need to subscribe to a free plan as shown in the screenshot by clicking on the subscribe button of the API we need to use - Current Weather and Daily Forecast. Under the tab "API Keys" we generate a new API key. We will need it later for the API calls. 

![image](/assets/2025-05-20/015.png)

![image](/assets/2025-05-20/020.png)

![image](/assets/2025-05-20/030.png)

## Preparing the Current Weather data source

In the Peakboard project we choose JSON data source for the first API call. The URL to call is "api.openweathermap.org/data/2.5/weather?q={city}&units=metric&appid={APIKey}". "city" is just the name of the city and the appid is the key we noted earlier. As we want metric units we provide the "metric" value for the "units" attribute. The API also support geographical coordinates and other features. The details can be found in the [API doc](https://openweathermap.org/current).

http://api.openweathermap.org/data/2.5/weather?q=Taipei&units=metric&appid=59067774c1363255

The call generates a table with exactly one row that contains the weather data of the choosen place.

![image](/assets/2025-05-20/040.png)

We will need the sunrise and sunset time in a correctly formatted, local time value. But the API provides only unix timestamps. So we need to add a data flow below the original source and add a step for adjusting the time. 

![image](/assets/2025-05-20/050.png)

It's only one line of code in LUA. We adjust the "sunrise" time for the timezone (that is thankfully provided as part of the API response) and format it to hours and minutes. The same is done for the time of sunset.

{% highlight lua %}
return os.date('%H:%M', item.sunrise + item.timezone)
{% endhighlight %}

For the actual visual representation we just choose normal textfields and some beautiful image icons. Of course we must adjust the correct formatting of the numbers, like the unit for the temperature as shown in the screenshot.

![image](/assets/2025-05-20/060.png)

One last point is the weather icon. We just use a regular image control. The API delivers a field called "icon". It's some kind of code. The details of this code can be checked [in the documentation](https://openweathermap.org/weather-conditions). We can build a URL from this code that points to the icon. The URL is "https://openweathermap.org/img/wn/{code}@2x.png", so for example "https://openweathermap.org/img/wn/10d@2x.png" when the code is replaced. We will place a very simple building block to the "refreshed" event of the data source that builds the icon URL and applies ot the source property of the image control.

![image](/assets/2025-05-20/080.png)

For all the LUA lovers, here's the pure LUA code:

{% highlight lua %}
screens['Screen1'].imgCurrentWeather.source = table.concat({'http://openweathermap.org/img/wn/', data.WeatherActual[0].icon, '@2x.png'})
{% endhighlight %}

## Preparing the weather forecast

For the daily forecast we use a different call, it's "forecast" instead of "weather" in the URL. The rest of the logic stays the same. The example shows how to use the geographical coordinates instead of the city name.

http://api.openweathermap.org/data/2.5/forecast/daily?lat=25.0375198&lon=121.5636796&units=metric&appid=59067774c13632559

![image](/assets/2025-05-20/070.png)

We want to present the daily forecast data in a list later, so we need to turn the unix timestamp into the weekday to present it to the viewer. Similiar as with the current weather, we do that for the "dt" column in data flow and use "return os.date('%a', item.dt )" to update the column. The character "%a" repeesents the weekday value.

![image](/assets/2025-05-20/090.png)

For the list, we choose a "styled list" that generates an item for each row. The principle is the same as with the first data source. We have several formatted values and also an image control

![image](/assets/2025-05-20/100.png)

Currently it's not possible to bind the url of an image to an image control like we do the data binding with the other columns of the data source. For setting the image content to the dynamically generated URL we use the refreshed script of the data flow.

![image](/assets/2025-05-20/110.png)

Here's the LUA code. We just loop over the table and set the URL for every instance of a forecasted day.

{% highlight lua %}
local i = 0

for index = 0, data.ForecastWithDate.count - 1 do
	Screens['Screen1'].listForecast[index].imgForecast.source = "http://openweathermap.org/img/wn/" .. data.ForecastWithDate[index].icon .. "@2x.png"
end
{% endhighlight %}

## result and enhancements

As we saw, it's very easy to use and adjust the openweather.org API for our needs. There are many more features that are not mentioned in this article, e.g. different styles of icons, wind, gusts, other weather phenomens, temperature curves and a lot more. There are many other API calls that are all listed in the API documentation.

![image](/assets/2025-05-20/010.png)


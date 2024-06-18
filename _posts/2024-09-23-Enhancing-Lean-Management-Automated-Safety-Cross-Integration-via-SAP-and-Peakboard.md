---
layout: post
title: Enhancing Lean Management - Automated Safety Cross Integration via SAP and Peakboard
date: 2023-03-01 12:00:00 +0200
tags: sap
image: /assets/2024-09-23/title.png
read_more_links:
  - name: SAP on Steroids - 10 Epic Use Cases and how to build them
    url: /SAP-on-Steroids-10-Epic-Use-Case-and-how-to-do-it.html
  - name: How to build a perfect RFC function module to use in Peakboard
    url: /SAP-How-to-build-a-perfect-RFC-function-module-to-be-used-in-Peakboard.html
  - name: Dismantle SAP Production - How to get the next work orders of a workplace by using COOIS transaction
    url: /Dismantle-SAP-Production-How-to-get-the-next-work-orders-of-a-workplace-by-using-COOIS-transaction-in-Peakboard.html
downloads:
  - name: SAPSafetyCross.pbmx
    url: /assets/2024-09-23/SAPSafetyCross.pbmx
---
A [safety cross](https://www.google.com/search?q=what+is+a+sfety+cross&rlz=1C1GEWG_deDE994DE994&oq=what+is+a+sfety+cross&gs_lcrp=EgZjaHJvbWUyBggAEEUYOTILCAEQABgNGBMYgAQyCwgCEAAYDRgTGIAEMgsIAxAAGA0YExiABDILCAQQABgNGBMYgAQyCwgFEAAYDRgTGIAEMgsIBhAAGA0YExiABDILCAcQABgNGBMYgAQyCggIEAAYDRgTGB4yCggJEAAYExgWGB7SAQgzNDMwajBqNKgCALACAQ&sourceid=chrome&ie=UTF-8) is one of the most common methods for visualizing safety in lean manufacturing.

The cross typically consists of 31 rectangles, representing the 31 days in a month. When an accident happens, the corresponding day is colored orange for light accidents, and red for more severe accidents. When there are no incidents, we paint the rectangle green.

So ideally, at the end of the month, the whole cross should only have green rectangles. That's what managers want to see.

Typically, the cross is colored manually. But of course, we can do better. This article is about how to fill a digital safety cross automatically, from data taken from the SAP HR module. And there's absolutely no changes or additions necessary in SAP. Anyone can download the demo PBMX, put in their credentials, and start right away. It's out-of-the-box with literally any SAP system.

## The SAP side

The basic idea is that every accident that happens in production causes an absence record in SAP HR. When its only a minor incident, that employee might be absent for one or two days. And if it's a bigger accident he/she will be absent 3 or more days.

HR data in SAP is organised in so called info types. These info types have numbers. The info type we're are looking for has the number 2001. But we need an additional filter. It's the sub type. In our case the sub stype is 0270, which stands for industrial accidents.

The screenshot shows a list of absence records in transaction PA30. Beside regular vacation days there are are 4 accident records (we keep in mind that this is just sample data. If a single employee records 4 accidents in two weeks something is wrong with this company) 

![image](/assets/2024-09-23/010.png)

The table where info type are stored are named PAXXXX whereas XXXX is the type number. So we're just using the table PA2001 with SUBTY = 0270 for the sub type and also use the date field BEDGA for limiting the data to the current month. That gives us access to the raw data for our saftey cross.

## Set up the data source

In the Peakboard designer we first set up a time data source, because we will need to determine the current month later.
Beside the time data source we need an SAP data source. The XQL to select the data is

{% highlight sql %}
ELECT PERNR, BEGDA, ABWTG FROM PA2001 
where BEGDA >= '20240601' and BEGDA <= '20240631' 
and SUBTY = '0270';
{% endhighlight %}

Now we need to make it dynamic. The month value within the two date values always should be the current month. So we enrich the XQL code with a bit of LUA to determine the current month:

{% highlight lua %}
return "SELECT PERNR, BEGDA, ABWTG FROM PA2001 " ..
"where BEGDA >= '2024" .. data.MyTimer.format('MM') .. "01' ".. 
"and BEGDA <= '2024" .. data.MyTimer.format('MM') .. "31' "..
"and SUBTY = '0270';"
{% endhighlight %}

And here's how the data source dialog looks like. In the preview grid on the right we can see the raw data that exactly corresponds to the the data we saw in transaction PA30 decribed above.

![image](/assets/2024-09-23/020.png)

## Building the cross

On the canvas of our project we use a bunch of text fields to form a cross. The text boxes whould be named according to their day, so we can easily address them later in the script.

![image](/assets/2024-09-23/030.png)

## Scripting the logic

The actual magic to turn the raw data into colored text fields happens in the reload script of the SAP data source. In fact the script is very simple and can be easily done with building blocks. However we use use LUA in this article because we need to treat every day individually, so LUA gives us a more condensed view than building blocks.

![image](/assets/2024-09-23/040.png)

First we check for every single day, if the day is current or in the past. If this is the case we set it to green. If not we just leave it on the default neutral color. In the script only day 1 and day 31 is shown. All others are removed to make the code more clear.

{% highlight lua %}
if data.MyTimer.format('dd') >= '01' then screens['Main'].txt1.background = brushes.green end
[.....]
if data.MyTimer.format('dd') >= '31' then screens['Main'].txt31.background = brushes.green end
{% endhighlight %}

in the next paragraph we just loop over the whole SAP data set. We convert the SAP data value to the curent day by using "string.sub(current['BEGDA'], 7, 9)" and the number of absence days we turn from string to a number: "AbsenceDays = math.tonumber(current['ABWTG'])". Then we check for every entry and the day if the number of absence day indicate a severe or minor accident and set the text field color accordingly. That's it.

{% highlight lua %}
for index = 0, data.SAPAbsenceThisMonth.count - 1 do

	local current = data.SAPAbsenceThisMonth[index]
	local Day = string.sub(current['BEGDA'], 7, 9)
	local AbsenceDays = math.tonumber(current['ABWTG'])

	if Day == '01' and AbsenceDays < 3 then screens['Main'].txt1.background = brushes.orange end
	[.....]
	if Day == '31' and AbsenceDays < 3 then screens['Main'].txt31.background = brushes.orange end
	
	if Day == '01' and AbsenceDays >= 3 then screens['Main'].txt1.background = brushes.red end
	[.....]	
	if Day == '31' and AbsenceDays >= 3 then screens['Main'].txt31.background = brushes.red end
		
end
{% endhighlight %}

## result and conclusion

The screenshot shows the final result according to our sample data introduced earlier. All the colors are set according to the logic of minor and severe accidents. The LUA cript is actually super simple and can be easily adjusted. It would be easily possible to adjust the XQL to limit the data only to certain teams or use other info types.

![image](/assets/2024-09-23/050.png)


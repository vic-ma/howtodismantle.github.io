---
layout: post
title: Dismantle Number Value Formatting
date: 2023-04-10 12:00:00 +0200
tags: bestpractice basics
image: /assets/2023-04-10/040.gif
---
Displaying numbers in a correct format can be considered as one of the ultimate core task when building boards. The basic features of number formatting in Peakboard are quite easy and documented well, please check out [help](https://help.peakboard.com/misc/en-formating-values.html), if you're not familiar with the format drop down that can be found everywhere.
This article digs a bit deeper into how to format a number when options in the combo box are not sufficient.

*Please note: It's on the roadmap to renew the whole way of formatting numbers in PB during the year 2023. This article refers to the current version as available in March 2023*

Beside the standard formatting from the combo box, there is an option to bring your own formatting pattern. A formatting pattern basically consists of the pattern itself (represented by characters like # and 0, etc..) and the so called _locale_. The _locale_ represents the cultural background of the format, e.g. the Germans use a "," as decimal separator, while the Americans prefer a ".".
Let's start with a simple sample. The pattern "##.##" says, that we want two digits before and two digits behind the decimal separator.

![image](/assets/2023-04-10/010.png)

So if he locale is "de-DE" for Germany it will generate a "12,34", if the locale is "en-US" for American it will generate a "12.34".

The next formatting character to understand is "0". "0" is almost the same as "#", but it will print a "0" in case the original values doesn't have enough digits. The number "12" will be turned into "12" when applying "###" and into "012" when applying "000" as format. So you can easily define leading zeros, if you want some.

![image](/assets/2023-04-10/020.png)

In lots of cases it might be necessary to provide a unit along with the number. Feel free to do this, even with unicode characters like a degree sign.

![image](/assets/2023-04-10/030.png)

Here's is our sample in action. BTW: the source is a public OPC UA server, feel free to download the sample pbmx and play around yourself....

![image](/assets/2023-04-10/040.gif)

## Is there even more formatting madness?

Yes, there's a lot more you can do to do the formatting of your numbers as perfect as possible.
The format strings we learned in this article are only a small subset of what is possible. Please check out the [official documentation provided by Microsoft](https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings). All the option are available in Peakboard.







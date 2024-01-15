---
layout: post
title: Taming JSON - How to use JPath in Peakboard scripts
date: 2023-03-01 02:00:00 +0200
tags: api basics
image: /assets/2024-03-07/title.png
read_more_links:
  - name: JPath library on github (check out the README for interesting details)
    url: https://github.com/atifaziz/JSONPath
downloads:
  - name: JSonJPathExamples.pbmx
    url: /assets/2024-03-07/JSonJPathExamples.pbmx
---

JSON is the most popular data format for modern web and cloud environments. Peakboard applications often use JSON. For example, when calling an API and processing the response. A common way to process the JSON string is not to do any basic string operations to extract the needed data from the payload, but rather to use a so called JPath expression. This JPath is a string describing the way to find the needed data within the JSON string. It sounds much more complicated than it is. That's why we use this article to show how to use JPath by examples. The logic how it works will be pretty clear then. The JSON string we're referring to in our examples is shown below. It represents a simple purchase order with a order header and two order items. The order items are arranged in a so called array.

{% highlight json %}
{
    "order_header": {
        "order_no": "ORD123456",
        "date": "2023-12-29",
        "customer_name": "Jane Doe"
    },
    "order_items": [
        {
            "material_no": "MAT001",
            "quantity": 10
        },
        {
            "material_no": "MAT002",
            "quantity": 5
        }
    ]
}
{% endhighlight %}

## Using JPath is Peakboard

There are predefined functions to use JPath within a Peakboard script or the Building Blocks. As you see in the screenshot, A local variable is filled with our sample JSON. Then the JPath block is used. In hat case the JPath expression is "order_header.order_no", so the JPath block extracts the field order_no that is hierarchically bound to the order header "order_header". So this expression returns the value "ORD123456". The third parameter of this JPath block is a string that is returned by the block in case the JPath expression points to a non-exiting data point. So it's easy to cope with the situation where the JSON might not look like as you expected it to be.

![image](/assets/2024-03-07/010.png)

For all users who prefer to do LUA scirpting instead of Bulding Blocks, here we go:

{% highlight lua %}
local myjson = '...'
screens['Screen1'].txt1.text = json.getvaluefrompath(myjson, 'order_header.order_no', '#ERROR#')
{% endhighlight %}

## Addressing Arrays

Beside the simple path shown above by just using "." to connect the hierarchy levels to express the actual path, we can use brackets "[XXX]" to move within an array. The following expression returns the field material_no in the first array entry (so the first item within our order).

{% highlight jpath %}
order_items[0].material_no

returns MAT001
{% endhighlight %}

Let's assume you don't know exactly how many items an array has, but you're interested in the last one. You can use the operator ":" to indicate a range and then count backwards by using the ordinal number "-1"

{% highlight jpath %}
order_items[-1:].material_no

returns MAT002
{% endhighlight %}

If you want to use a filter to find the right entry within an array you can use the "?" character to apply a filter instead of the ordinal number. In this example we filter for items with material_no equals "MAT001" and return the corresponding quantity.

{% highlight jpath %}
order_items[?(@.material_no == 'MAT001')].quantity

returns 10
{% endhighlight %}

Let's try out this filter in the JPath block shown above but try to filter for something that doesn't exists. In that case the block returns the error string "#ERROR#".

{% highlight jpath %}
order_items[?(@.material_no == 'XXX')].quantity

returns #ERROR#
{% endhighlight %}

## Aggregating and looping

Unfortunately JPath doesn't support aggregating array items. That's why we need to build that on our own. Ths following example shows a loop. The counter variable "i" is used to build a dynamic JPath expression that addresses a certain array item. As soon as the block return "#ERROR#" we know, that we have reached the end of the array and exit the loop. So "i" contains the number of array entries. That's the basic pattern for iterating array entries without knowing how many entries an array has. If you want to let's say summarize some values, it work exactly the same. Just loop and do something for every entry until the JPath returns an error string.

![image](/assets/2024-03-07/020.png)

## result and conclusion

JPath is easy to understand and much more practical than doing basic string opration with JSON strings. Feel free to download the attached pbmx and play around with it. All examples explained in this article are available to tested right away...

![image](/assets/2024-03-07/result.gif)


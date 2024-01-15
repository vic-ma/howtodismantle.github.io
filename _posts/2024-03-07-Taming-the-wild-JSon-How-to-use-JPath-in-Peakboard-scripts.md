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

JSON is the most popular data format for modern web and cloud environments. Peakboard applications often use JSON. For example, when calling an API and processing the response.

A good way to extract the desired data from a JSON string is to use a JPath expression, rather than using basic string operations. A JPath is a string that describes how to find the desired data within the JSON string.

It's a lot simpler than it sounds. This article will show you how to use JPath through examples. The logic behind how it works will become clear in the end.

The JSON string we will use in our examples is shown below. It represents a simple purchase order with an order header and two order items. The order items are arranged in an array.

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

## Using JPath in Peakboard

Peakboard scripts and Building Blocks have predefined functions for JPath. In the following screenshot, we fill a local variable with our JSON example. Then, we use the JPath block.

In this case, the JPath expression is `order_header.order_no`. The JPath block extracts the field `order_no`, which is hierarchically bound to the order header `order_header`. So this expression returns the value `ORD123456`.

The third parameter of this JPath block is a string that is returned by the block if the JPath expression points to a non-exiting data point. This handles the situation where the JSON doesn't look like what you expect.

![image](/assets/2024-03-07/010.png)

For users who prefer LUA scripting to Building Blocks, here is the same script in LUA:

{% highlight lua %}
local myjson = '...'
screens['Screen1'].txt1.text = json.getvaluefrompath(myjson, 'order_header.order_no', '#ERROR#')
{% endhighlight %}

## Addressing Arrays

The simple JSON path above uses `.` to connect the hierarchy levels and create the actual path. But we can also use brackets, like `[3]`, to move within an array.

This expression returns the field `material_no` in the first array entry (which is the first item in our order):

{% highlight jpath %}
order_items[0].material_no

returns MAT001
{% endhighlight %}

Now, let's say you want the last item in an array, but you don't know exactly how many items the array has. In this case, you can use the `:` character to indicate a range, and then count backwards by using the ordinal number `-1`:

{% highlight jpath %}
order_items[-1:].material_no

returns MAT002
{% endhighlight %}

If you want to use a filter to find an entry in an array, you can use the `?` character. In this example, we filter for items with a `material_no` that equals `MAT001` and return the corresponding quantity:

{% highlight jpath %}
order_items[?(@.material_no == 'MAT001')].quantity

returns 10
{% endhighlight %}

Now let's try to filter for something that doesn't exist. In this case, the block returns the error string `#ERROR#`:

{% highlight jpath %}
order_items[?(@.material_no == 'XXX')].quantity

returns #ERROR#
{% endhighlight %}

## Aggregating and looping

Unfortunately, JPath doesn't support aggregating array items. That's why we need to do this on our own

The following example shows a loop. We use a counter variable `i` to build a dynamic JPath expression that addresses each array item. As soon as the expression returns `#ERROR#`, we know that we have reached the end of the array, and so we exit the loop.

So `i` contains the index of the array entries. That's the basic pattern for iterating array entries without knowing how many entries the array has. If you want to summarize some values, it works exactly the same. Just loop and do something for each entry, until the JPath returns an error string.

![image](/assets/2024-03-07/020.png)

## Conclusion

JPath is easy to understand and much more practical than doing basic string operations with JSON strings. Feel free to download the attached pbmx and play around with it. All the examples in this article can be tested straight away.

![image](/assets/2024-03-07/result.gif)


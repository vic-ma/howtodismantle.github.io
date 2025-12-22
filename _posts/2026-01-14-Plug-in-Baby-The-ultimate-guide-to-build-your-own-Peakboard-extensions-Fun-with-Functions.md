---
layout: post
title: Plug-in, Baby - The Ultimate Guide to Building Your Own Peakboard Extensions - Fun with Functions
date: 2023-03-01 00:00:00 +0000
tags: dev
image: /assets/2026-01-14/title.png
image_header: /assets/2026-01-14/title.png
bg_alternative: true
read_more_links:
  - name: Developer stuff
    url: /category/dev
downloads:
  - name: Source code for this article
    url: https://github.com/HowToDismantle/howtodismantle.github.io/tree/main/assets/2026-01-14/MeowExtension
  - name: ExtensionCalls.pbmx
    url: /assets/2026-01-14/ExtensionCalls.pbmx
---
This article is part three of our custom Peakboard extensions series:
* [Part I - The Basics](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-The-Basics.html)
* [Part II - Parameters and User Input](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-Parameters-and-User-Input.html)
* [Part III - Custom-made Functions](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-Fun-with-Functions.html)
* [Part IV - Event-triggered data sources](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-Event-triggered-data-sources.html)

In the [first part of this series](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-The-Basics.html), we explained the basics of custom Peakboard extensions. We built a simple Peakboard extension called `MeowExtension`, by creating two classes in .NET:
* One for specifying the extension metadata.
* One for defining the *Cat List* data source.

In the [second part of this series](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-Parameters-and-User-Input.html), we explained how to add configuration options to a custom data source. We added options like *IsItARealCat* and *Age* to our Cat List data source.

In today's article, we'll explain how to create **functions** for a custom data source.

A data source can provide functions, to let you do things beyond simple data querying. For example, the SQL data source lets you read data from a SQL database. But what if you want to insert data into a SQL database instead? In that case, you would use the *Run SQL query* function.

The *Run SQL query* function is provided by the SQL data source, and it lets you run an arbitrary SQL query. So for example, if you want to insert a row whenever the user taps a button on screen, then you would edit the tapped script of the button and add the *Run SQL query* function.

Before continuing, make sure that you have read [part one](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-The-Basics.html) and [part two](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-Parameters-and-User-Input.html) of this series. This article won't make sense otherwise. You can also take a look at the [final code for this article](https://github.com/HowToDismantle/howtodismantle.github.io/tree/main/assets/2026-01-14/MeowExtension), on GitHub.

## Create a simple function with a return parameter

To begin, we'll add a simple **sum function** to our Cat List data source. The function takes in two numbers and returns their sum.

To add a function to a data source, we do the following:
1. **Declare** the function in the data source's `Functions` attribute. This step declares the name, parameters, and return type of the function.
1. **Define** the function in the `ExecuteFunctionOverride` function. This step defines the implementation of the function.

### Declare the function

To declare our function, we define the `Functions` attribute in our `CustomListDefinition`:

{% highlight csharp %}
protected override CustomListDefinition GetDefinitionOverride()
{
    return new CustomListDefinition
    {
        ID = "CatCustomList",
        // ...

        Functions = new CustomListFunctionDefinitionCollection
        {
            new CustomListFunctionDefinition
            {
                Name = "AddMyNumbers",
                Description = "Adds two numbers",
                InputParameters = new CustomListFunctionInputParameterDefinitionCollection
                {
                    new CustomListFunctionInputParameterDefinition
                    {
                        Name = "FirstNumber",
                        Type = CustomListFunctionParameterTypes.Number,
                        Description = "The first number to be added"
                    },
                    new CustomListFunctionInputParameterDefinition
                    {
                        Name = "SecondNumber",
                        Type = CustomListFunctionParameterTypes.Number,
                        Description = "The first number to be added"
                    }
                },
                ReturnParameters = new CustomListFunctionReturnParameterDefinitionCollection
                {
                    new CustomListFunctionReturnParameterDefinition
                    {
                        Name = "Result",
                        Type = CustomListFunctionParameterTypes.Number,
                        Description = "The result of the addition"
                    }
                }
            }
        }
    };
}
{% endhighlight %}

### Define the function

To define our function, we override the `ExecuteFunctionOverride()` function. Our data source's functions are all routed through this function. So, we use an `if` statement to separate the different function implementations (right now, we only have the one function).

To get the arguments for our function, we use `context.Values[i].GetValue()`. And to return the result, we use a `CustomListExecuteReturnContext`.

{% highlight csharp %}
protected override CustomListExecuteReturnContext ExecuteFunctionOverride(CustomListData data, CustomListExecuteParameterContext context)
{
    if (context.FunctionName.Equals("AddMyNumbers", StringComparison.InvariantCultureIgnoreCase))
    {
        Double FirstNumber = (Double)context.Values[0].GetValue();
        Double SecondNumber = (Double)context.Values[1].GetValue();

        var returncontext = new CustomListExecuteReturnContext();
        returncontext.Add(FirstNumber + SecondNumber);   

        return returncontext;
    }
    else
    {
        throw new DataErrorException("Function is not supported in this version.");
    }
}
{% endhighlight %}

### Use the function

Next, we rebuild our extension, and our function is ready to be used. In order to run a custom data source's function, we use one of two Building Blocks:
* *Run function*
* *Run function with return value*

Custom data source functions do not appear as standalone Building Blocks (unlike built-in data source functions), so we must use one of these universal function runners.

In our case, the function has a return value, so we use *Run function with return value*:
![Peakboard Building Block calling the custom function](/assets/2026-01-14/peakboard-custom-function-building-block.png)

## Submitting a complex parameter to an extension function

The values we used to exchange with the extension have been scalar and simple. Let's assume we want to submit a table-like value to the extension function. In our sample our table is supposed to be a list of messages, along with their message type, to be written to a message logger.

For this use case we use `CustomListFunctionParameterTypes.Collection` as parameter type for the definition of the function's metadata. Here's the source code to be added during `GetDefinitionOverride`.

{% highlight csharp %}
new CustomListFunctionDefinition
{
    Name = "PrintMyTableToLog",
    InputParameters = new CustomListFunctionInputParameterDefinitionCollection
    {
        new CustomListFunctionInputParameterDefinition
        {
            Name = "MessageTable",
            Type = CustomListFunctionParameterTypes.Collection,
            Description = "A table containing messages"
        }
    }
},
{% endhighlight %}

For the `ExecuteFunctionOverride` we just use `.CollectionValue` to convert the parameter object into an instance of `CustomListObjectElementCollection` and iterate through it...

{% highlight csharp %}
else if (context.FunctionName.Equals("PrintMyTableToLog", StringComparison.InvariantCultureIgnoreCase))
{
    CustomListObjectElementCollection MyTab = context.Values[0].CollectionValue;

    foreach(CustomListObjectElement row in MyTab)
    {
        this.Log.Info($"{row["MessageType"]}: {row["Message"]}");
    }

    return new CustomListExecuteReturnContext();
}
{% endhighlight %}

On the host side we can't use Building Blocks anymore because, as of early 2026, complex parameters are not yet supported by the Building Blocks. So to use this function we need to switch to LUA coding. The sample shows how to create the table-like object, store data in it and submit it as a parameter to the function of the extension data source.

![LUA script building the table parameter for the extension function](/assets/2026-01-14/lua-table-parameter-function.png)

The screenshot shows the test cockpit in action. The above code is generating the log file entries through the extension.

![Peakboard test cockpit showing log entries generated by the extension](/assets/2026-01-14/peakboard-test-cockpit-logging.png)

## Returning multiple values

In the first example we used a simple, single, scalar return parameter. Lets assume things get more complicated and we need more than one return value. In this case we can use the same trick we already used in the second example. During the construction of the function we mark the return value as type `CustomListFunctionParameterTypes.Object` to allow a more complex data transfer.

{% highlight csharp %}
new CustomListFunctionDefinition
{
    Name = "GetACat",
    ReturnParameters = new CustomListFunctionReturnParameterDefinitionCollection
    {
        new CustomListFunctionReturnParameterDefinition
        {
            Name = "Result",
            Type = CustomListFunctionParameterTypes.Object,
            Description = "A cat object is returned"
        }
    }
}
{% endhighlight %}

Let's have a look at the actual function implementation in `ExecuteFunctionOverride`. We just build a `CustomListObjectElement` instance, which represents a single table row, or to be more precise, a key/value pair collection.

{% highlight csharp %}
else if (context.FunctionName.Equals("GetACat", StringComparison.InvariantCultureIgnoreCase))
{
    var item = new CustomListObjectElement();
    item.Add("Name", "Tom");
    item.Add("Age", 7);

    var returncontext = new CustomListExecuteReturnContext();
    returncontext.Add(item);

    return returncontext;
}
{% endhighlight %}

We switch over to the host side. Here's the LUA code to process this complex object. It's just a generic LUA object that accepts the attributes to be addressed by their key name directly without any hassle. By the way: we can even return table-like objects. In that case we would have to use `CustomListObjectElementCollection` like in the second example.

![LUA script reading the complex return object](/assets/2026-01-14/lua-handle-complex-return.png)

Here's the script in action using the example test cockpit.

![Peakboard test cockpit showing the complex return value](/assets/2026-01-14/peakboard-test-cockpit-return.png)

## Conclusion

Extension functions are super easy to understand and implement once we understand the principle behind it. Even complex parameters like key/value collections or even tables can be exchanged.




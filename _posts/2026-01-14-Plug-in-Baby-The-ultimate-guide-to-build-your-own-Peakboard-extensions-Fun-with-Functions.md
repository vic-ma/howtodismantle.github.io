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

A data source can provide functions, to allow for functionality beyond simple data querying. For example, the SQL data source lets you read data from a SQL database. But what if you want to insert data into a SQL database instead? In that case, you must use the *Run SQL query* function. This function is provided by the SQL data source and it lets you run an arbitrary SQL command.

Our goal for today's article is to add three functions to our Cat List data source:
1. `AddMyNumbers`, which returns the sum of two numbers.
1. `PrintMyTableToLog`, which prints a table to the log.
1. `GetACat`, which returns the data for a cat named Tom.

Each functions demonstrates a different concept about custom data source functions.

Before continuing, make sure that you have read [part one](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-The-Basics.html) and [part two](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-Parameters-and-User-Input.html) of this series. This article won't make sense otherwise. You can also take a look at the [final code for this article](https://github.com/HowToDismantle/howtodismantle.github.io/tree/main/assets/2026-01-14/MeowExtension), on GitHub.

## Create a simple function

First, let's learn the basics of custom functions. To do this, we'll add a simple `AddMyNumbers` function to our Cat List data source. This function takes in two numbers and returns their sum.

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

In our case, `AddMyNumbers` has a return value, so we use the *Run function with return value* Building Block:
![Peakboard Building Block calling the custom function](/assets/2026-01-14/peakboard-custom-function-building-block.png)

## Create a function with a complex parameter

Now, let's create a function with a more complex parameter type. We'll create `PrintMyTableToLog`, which accepts a table argument and prints that table to the log.

### Declare the function

In the function declaration, we set the parameter type to `CustomListFunctionParameterTypes.Collection`:

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

### Define the function

In the function definition, we use `context.Values[0].CollectionValue` to get the table argument. Then, we iterate through the rows of the table and print them to the log.

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

### Use the function

To use this function in a script, we have to use LUA. This is because Building Blocks does not currently support complex parameters like tables (as of January 2026). However, the LUA code is pretty simple.

For our example, we create a table literal in LUA called `MyTab`. Then, we call our `PrintMyTableToLog` function, passing in `MyTab` as the argument.

![LUA script building the table parameter for the extension function](/assets/2026-01-14/lua-table-parameter-function.png)

As you can see, the script correctly prints our `MyTab` table to the log.

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




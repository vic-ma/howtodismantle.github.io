---
layout: post
title: Plug-in, Baby - The ultimate guide to build your own Peakboard extensions - Parameters and User Input
date: 2023-03-01 00:00:00 +0000
tags: dev
image: /assets/2025-12-05/title.png
image_header: /assets/2025-12-05/title.png
bg_alternative: true
read_more_links:
  - name: Developer stuff
    url: /category/dev
  - name: Extension template for Visual Studio
    url: https://marketplace.visualstudio.com/items?itemName=Peakboard.PBEKTEMP
downloads:
  - name: Source code for this article
    url: https://github.com/HowToDismantle/howtodismantle.github.io/tree/main/assets/2025-12-05/MeowExtension
---
This article is part two of our custom Peakboard extensions series:

* [Part I - The Basics](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-The-Basics.html)
* [Part II - Parameters and User Input](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-Parameters-and-User-Input.html)
* [Part III - Custom-made Functions](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-Fun-with-Functions.html)
* [Part IV - Event-triggered data sources](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-Event-triggered-data-sources.html)

In the [first part of this series](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-The-Basics.html), we explained how to build a simple Peakboard extension called `MeowExtension`. We created two classes in .NET: one for specifying the extension metadata, and one for defining the *Cat List* data source.

In today's article, we're going to take things one step further. We're going to add some configuration options to our Cat List data source. These are the options that the user sees when they add a new Cat List data source. (Just like how, for example, the JSON data source has configuration options for the source URL, authentication type, path, etc.)

Make sure that you have read the [first part of this series](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-The-Basics.html). This article won't make sense otherwise. You can also take a look at the [final code for this article](https://github.com/HowToDismantle/howtodismantle.github.io/tree/main/assets/2025-12-05/MeowExtension).

## Add a simple parameter

Now, let's add a simple text parameter for the cat's name, to our Cat List data source. To do this, we need to modify our `CustomCatList` data source class:
1. Set `PropertyInputPossible` to `true`, to indicate that our data source has parameters that the user can set..
1. Create a new `CustomListPropertyDefinition`. This object defines our text parameter.
1. Add the `CustomListPropertyDefinition` object to the `PropertyInputDefaults` collection, to add it to the data source.

Note: In our last article, we used a proper `CustomCatList` class definition. But here, we're just going to use an object initializer inside the `GetDefinitionOverride` function.

{% highlight csharp %}
protected override CustomListDefinition GetDefinitionOverride()
{
    return new CustomListDefinition
    {
        ID = "CatCustomList",
        Name = "Cat List",
        Description = "A custom list for cats with breed and age information",
        PropertyInputPossible = true,
        PropertyInputDefaults =
        {
            new CustomListPropertyDefinition { Name = "CatsName", Value = ""}
        }
    };
}
{% endhighlight %}

Here's what our parameter looks like in Peakboard Designer:

![Peakboard custom list text parameter input](/assets/2025-12-05/peakboard-text-parameter-configuration.png)

### Get the parameter value

Of course, adding a parameter on its own isn't very useful. We also want to use the value of the parameter (either the value that the user entered, or the default).

To get the value of a parameter, we override the `GetItemsOverride` function. This function provides a `data` object that contains a `Properties` collection. This collection contains all the parameter values. So, to get the value for our `CatsName` parameter, we do this:

{% highlight csharp %}
protected override CustomListObjectElementCollection GetItemsOverride(CustomListData data)
{
    this.Log.Info("Generating cat list items for " + data.Properties["CatsName"]);

    // ....
}
{% endhighlight %}

In the above example, we print out the value of `CatsName` by using the `Log` object. It works as you would expect, and it supports all the standard logging levels, like `Info`, `Verbose`, `Error`, `Critical`, etc.

### Validate the parameter value

The extension kit provides a standardized way to validate parameter values. That way, if the user enters an invalid value, Peakboard Designer will let them know and prevent them from saving the data source, until they fix the parameter value.

To validate a parameter value, we override the `CheckDataOverride` function. We process the parameter's value and if there's a problem, we throw an exception. The exception message is shown to the user, to let them know what's wrong with the value they entered.

{% highlight csharp %}
protected override void CheckDataOverride(CustomListData data)
{
    if (string.IsNullOrWhiteSpace(data.Properties["CatsName"]))
    {
        throw new InvalidDataException("Please provide a good name");
    }
    base.CheckDataOverride(data);
}
{% endhighlight %}

## Complex parameters

Besides the simple text parameter we have the option to force the value of a parameter into the other Peakboard data types such as number or bool. All the additional parameter objects are added to the `PropertyInputDefaults` collection.

{% highlight csharp %}
new CustomListPropertyDefinition { Name = "IsItARealCat", Value = "True", TypeDefinition = TypeDefinition.Boolean },
new CustomListPropertyDefinition { Name = "Age", Value = "4", TypeDefinition = TypeDefinition.Number },
{% endhighlight %}

The dialogs to manipulate the data are adjusted automatically according to this meta data.

![Peakboard boolean and number parameter settings](/assets/2025-12-05/peakboard-boolean-number-parameter-settings.png)

Let's assume we want to give the user only a combo box of distinct values to be chosen rather than a free text, we just use the `selectableValues` attribute to restrict the entry to some values.

{% highlight csharp %}
new CustomListPropertyDefinition { Name = "MaximumOfSomething", Value = "5", 
      TypeDefinition = TypeDefinition.Number.With(selectableValues: [ 2, 3, 5, 10, 20, 50, 100]) },
{% endhighlight %}

Here's the corresponding view for the user of the extension to provide the distinct values they can choose from:

![Peakboard parameter selectable values dropdown](/assets/2025-12-05/peakboard-selectable-values-dropdown.png)

For passwords, connection strings or other potentially sensitive data, we can use the `TypeDefinition` attribute `masked: true`

{% highlight csharp %}
new CustomListPropertyDefinition { Name = "MySecretCode", Value = "18899", TypeDefinition = TypeDefinition.String.With(masked: true) },
{% endhighlight %}

Here's a sample screenshot of a masked parameter:

![image](/assets/2025-12-05/peakboard-masked-parameter-input.png)

The last thing we want to discuss are multliline texts. These are typically used for long SQL statement or JSON / XML fragments.

{% highlight csharp %}
new CustomListPropertyDefinition { Name = "MultilineDescription", 
      Value = "Please provide\nsome\nbeautiful SQL", TypeDefinition = TypeDefinition.String.With(multiLine: true) }
{% endhighlight %}

And here's how it looks like as part of the data source dialog UI.

![image](/assets/2025-12-05/peakboard-multiline-parameter.png)

## result

Defining the parameters of an extension with a good UI is a key point to make it as easy as possible for the user to provide his data. We need to choose them wisely :-)

In the next part of our series we learn how to build functions that can be used in both LUA scripts and building blocks: [Part III - Custom-made Functions](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-Fun-with-Functions.html)

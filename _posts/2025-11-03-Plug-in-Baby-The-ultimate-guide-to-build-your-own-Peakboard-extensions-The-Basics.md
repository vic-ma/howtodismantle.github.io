---
layout: post
title: Plug-in, Baby - The Ultimate Guide to Building Your Own Peakboard Extensions - The Basics
date: 2023-03-01 00:00:00 +0000
tags: dev
image: /assets/2025-11-03/title.png
image_header: /assets/2025-11-03/title.png
bg_alternative: true
read_more_links:
  - name: Developer stuff
    url: /category/dev
  - name: Peakboard Extension Kit
    url: https://www.nuget.org/packages/Peakboard.ExtensionKit/
  - name: Get extension template for Visual Studio
    url: https://marketplace.visualstudio.com/items?itemName=Peakboard.PBEKTEMP
downloads:
  - name: Source code for this article
    url: https://github.com/HowToDismantle/howtodismantle.github.io/tree/main/assets/2025-12-05/MeowExtension
---
Peakboard provides built-in data sources for almost all modern hardware and software that you'd find in an office, warehouse, or factory environment. And even when you do have an "unsupported" device or program, you can often  connect to it by using a generic data source---like ODBC for databases or JSON for REST services. We've discussed this method multiple times in this blog, so feel free to browse [the archives](/archive/) to learn more.

But what if you have an unsupported device or program, but you can't (or don't want to) use a generic data source? In that case, you can create your own custom extension, with Peakboard's easy-to-use plug-in system. Here's how it works:
1. You build a DLL for your extension, by writing some lines of .NET code. 
1. You add your extension to Peakboard Designer.
1. You use the custom data source you built, just like other data source!

Peakboard extensions open the door to tailor-made integrations. And in this four-part series on our blog, we'll explain how to build these extensions, from a basic example all the way to a sophisticated example with complex parameters and event-triggered sources:

* [Part I - The Basics](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-The-Basics.html)
* [Part II - Parameters and User Input](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-Parameters-and-User-Input.html)
* [Part III - Custom-made Functions](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-Fun-with-Functions.html)
* [Part IV - Event-Triggered Data Sources](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-Event-triggered-data-sources.html)


In this article, we're going to build a very basic Peakboard extension called `MeowExtension`, which returns a static list of cats (hard-coded). You can download the [source code for `MeowExtension`](https://github.com/HowToDismantle/howtodismantle.github.io/tree/main/assets/2025-12-05/MeowExtension), if you want to follow along.


## Set up the project

In order to build a Peakboard extension, we first need to install a NuGet package called [`Peakboard.ExtensionKit`](https://www.nuget.org/packages/Peakboard.ExtensionKit/).

Then, we create a new .NET project with the `Library` output format and the `.NET 8` target framework.

We create a new file called `extension.xml`, which contains metadata for our extension. It gets copied to the output folder, and Peakboard Designer uses the file to get basic information about the extension.

Here's our `extension.xml` file:
{% highlight xml %}
<?xml version="1.0" encoding="utf-8" ?>
<ExtensionCatalog xmlns="http://schemas.peakboard.com/pbmx/2020/extensions">
  <Extensions>
    <Extension ID="MeowExtension"
               Path="Meow.dll"
               Version="1.2"
               Class="MeowExtension.MeowExtension" />
  </Extensions>
</ExtensionCatalog>
{% endhighlight %}

We also create an icon (ideally a PNG) for our extension. We add the icon to our project as an embedded resource.

Here's what our final project file looks like:
{% highlight xml %}
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Library</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Peakboard.ExtensionKit" Version="4.0.1" />
  </ItemGroup>

  <ItemGroup>
    <None Include="extension.xml">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
    <EmbeddedResource Include="MeowExtension.png" />
  </ItemGroup>

</Project>
{% endhighlight %}


## Create the classes

Next, we create the two classes which define our extension's functionality:
1. The extension class, which represents our extension.
1. The list class, which we use to make our extension return a list of cats.

### Create the extension class

We need to create a class for our extension. To do this, we define a new class called `MeowExtension`, which extends the base extension class, `ExtensionHost`.

Next, we override the `GetDefinitionOverride()` method. This method provides metadata about our extension to the *extension runtime*. This is different from the `extension.xml` file, which provides metadata to *Peakboard Designer*. However, the metadata in this method must match the metadata in `extension.xml`, to ensure consistency.

Next, we override the `GetCustomListsOverride()` method. This method defines all the lists that the extension provides. For example, if you are building an extension for an ERP system, then your extension could provide a customers list and an orders list. Then, in Peakboard Designer, you can use the extension to add a customers-data-source or an orders-data-source. Both data sources are provided by the same extension.

It is supposed to return a collection of lists that are provided by the extension. Let's assume we are building an extension to access an ERP system. We can have different lists like one for products, one for customers, and one for orders all in the same extension. In our example one list is enough, so we add only one instance to the collection of lists to keep things tidy while we learn the basics.

{% highlight csharp %}
public class MeowExtension : ExtensionBase
{
    public MeowExtension(IExtensionHost host) : base(host) { }
    
    protected override ExtensionDefinition GetDefinitionOverride()
    {
        return new ExtensionDefinition
        {
            ID = "MeowExtension",
            Name = "Meow Extension",
            Description = "A Peakboard extension for cats",
            Version = "1.2",
            Author = "Meow Development Team",
            Company = "Meow Inc.",
            Copyright = "Copyright © Meow Inc.",
        };
    }

    protected override CustomListCollection GetCustomListsOverride()
    {
        return new CustomListCollection
        {
            new CatCustomList(),
        };
    }
}
{% endhighlight %}

### Create the list class

In the extension class we already used the class that represents the actual list. Every list within our extension is derived from the base class `CustomListBase`. The overridden function `GetDefinitionOverride` returns some metadata for this dedicated list so it can be handled correctly by the hosting system and have a good UI for the end user, even when the extension grows more complex later on.

{% highlight csharp %}
[CustomListIcon("Meow.MeowExtension.png")]
public class CatCustomList : CustomListBase
{
    protected override CustomListDefinition GetDefinitionOverride()
    {
        return new CustomListDefinition
        {
            ID = "CatCustomList",
            Name = "Cat List",
            Description = "A custom list for cats with breed and age information",
        };
    }

    // [......]

}
{% endhighlight %}

The second function we need to override is `GetColumnsOverride`. It is called by the host system every time it needs the actual columns to represent the data. In our example we have fixed columns, so we just create a collection of columns and return the collection to the caller. Depending on the use case columns can also be returned dynamically depending on certain parameters the user is providing. We will discuss parameters in the second part of the series. Beside the name the only thing that represents a column is the data type: Number, String, or Boolean.

{% highlight csharp %}
protected override CustomListColumnCollection GetColumnsOverride(CustomListData data)
{
    var columns = new CustomListColumnCollection();
    columns.Add(new CustomListColumn("ID", CustomListColumnTypes.Number));
    columns.Add(new CustomListColumn("Name", CustomListColumnTypes.String));
    columns.Add(new CustomListColumn("Breed", CustomListColumnTypes.String));
    columns.Add(new CustomListColumn("Age", CustomListColumnTypes.Number));
    columns.Add(new CustomListColumn("ImageUrl", CustomListColumnTypes.String));
    return columns;
}
{% endhighlight %}

The last function we need to override is `GetItemsOverride(CustomListData data)`. The function delivers the actual data to the caller. This table-like data is represented by a collection of objects called `CustomListObjectElementCollection`. It holds the rows of the table as instances of the class `CustomListObjectElement` that in turn contain a collection of key/value pairs that represent the table cells.

{% highlight csharp %}
protected override CustomListObjectElementCollection GetItemsOverride(CustomListData data)
{
    var items = new CustomListObjectElementCollection();
    
    // Add cat data directly to items
    var item1 = new CustomListObjectElement();
    item1.Add("ID", 1);
    item1.Add("Name", "Fluffy1");
    item1.Add("Breed", "Persian");
    item1.Add("Age", 3);
    item1.Add("ImageUrl", "https://example.com/cat1.jpg");
    items.Add(item1);
    
    var item2 = new CustomListObjectElement();
    item2.Add("ID", 2);
    item2.Add("Name", "Whiskers");
    item2.Add("Breed", "Maine Coon");
    item2.Add("Age", 5);
    item2.Add("ImageUrl", "https://example.com/cat2.jpg");
    items.Add(item2);
    
    return items;
}
{% endhighlight %}

## Packing the extension

The compiled binaries must all be put into one single ZIP file. In our example the binary is only the DLL, but there might be other artifacts that can be packed as well, e.g. debug symbols or other referenced assemblies that might be necessary to run the extension. In addition to these artifacts we need to place our `extension.xml` into the ZIP file to ensure the Designer can read the metadata right away.

All files that are contained in the ZIP are deployed to the Designer and then later deployed to the Peakboard Box or BYOD instance when the application that uses the extension is deployed to its destination.

![image](/assets/2025-11-03/peakboard-extension-zip-contents.png)

## Using the extension in Peakboard Designer

To install the extension in the Peakboard Designer we just add it in the data source / extension dialog, making sure we point to the freshly created ZIP package.

![image](/assets/2025-11-03/peakboard-designer-install-extension-dialog.png)

After restarting the Designer we can add our custom list as data source. All available lists of the extension are listed in the drop-down, so you can immediately select the new list without additional configuration.

![image](/assets/2025-11-03/peakboard-designer-custom-list-selection.png)

The last screenshot shows the preview mode of the data source based on our example extension after hitting the refresh button. So far the extension doesn't offer any parameters for the user. This is what we will discuss in the [next article of the series](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-Parameters-and-User-Input.html), where we will extend the foundation with interactive options.

![image](/assets/2025-11-03/peakboard-designer-data-preview.png)





---
layout: post
title: Plug-in, Baby - The ultimate guide to build your own Peakboard extensions - The Basics
date: 2023-03-01 00:00:00 +0000
tags: dev
image: /assets/2025-11-03/title.png
image_header: /assets/2025-11-03/title.png
bg_alternative: true
read_more_links:
  - name: Developer stuff
    url: /category/dev
  - name: Peakoard Extension Kit
    url: https://www.nuget.org/packages/Peakboard.ExtensionKit/
downloads:
  - name: Source code for this article
    url: https://github.com/HowToDismantle/howtodismantle.github.io/tree/main/assets/2025-12-05/MeowExtension
---
Peakboard offers a lot of built-in data sources for almost every kind of modern it interfaces. Even when it comes to very exotic requirements a lot of connectectivity requirements can be solved with generic data source, like ODBC for databases or JSON for lots of different kind of REST services. We discussed all these options already multiple times in this blog.

Beside these generic options Peakboard also offers a very easy-to-use plug-in concept. We can build a dll in our prefered .NET IDE with only very lines of code and just plug it into our Peakboard application and use it almost like a regular data source. This conecpt is called Peakboard extensions. In our current series we will have a look how to buid these extensions. From a minimal version to more sophisticated option with complex parameters and event triggered sources. Here are the parts:

* [Part I - The Basics](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-The-Basics.html)
* [Part II - Parameters and User Input](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-Parameters-and-User-Input.html)
* [Part III - Custom made Functions](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-Fun-with-Functions.html)
* [Part IV - EVent triggered data sources](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-Event-triggered-data-sources.html)

The extension project needs a nuget package called `Peakboard.ExtensionKit` that can be found [here](https://www.nuget.org/packages/Peakboard.ExtensionKit/). 

The project we're building in this article can be downloaded [here](https://github.com/HowToDismantle/howtodismantle.github.io/tree/main/assets/2025-12-05/MeowExtension).

## Setting up the project

To build our first extension we will need .NET project with output format `Library` and Target Framework `.NET 8`. In the first iteration we will need two classes that are added later. Beside the two classes we will need a file called `extension.xml` that is copied to the output folder. Here's a sample of the `extenson.xml`. It is used later shipped up to the Peakboard designer and is used there to give the designer the chance to read some metadate about the extension. 

{% highlight xml %}
<?xml version="1.0" encoding="utf-8" ?>
<ExtensionCatalog xmlns="http://schemas.peakboard.com/pbmx/2020/extensions">
  <Extensions>
    <Extension ID="MeowExtension"
               Path="Meow.dll"
               version="1.2"
               Class="MeowExtension.MeowExtension" />
  </Extensions>
</ExtensionCatalog>
{% endhighlight %}

Beside the xml file we will need a small icon, prefferebaly as png to be added to the project as enbedded reosurce. So in total here's the project file of our example project:

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
    <PackageReference Include="Peakboard.ExtensionKit" Version="4.1.0" />
  </ItemGroup>

  <ItemGroup>
    <None Include="extension.xml">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
    <EmbeddedResource Include="MeowExtension.png" />
  </ItemGroup>

</Project>
{% endhighlight %}

## Set up the Extension class

The Extension itself is represented by an extension class that is derived from the base class `ExtensionHost`. The method `GetDefinitionOverride()` is overriden to provide more metadata about the extension like a unique ID, version, and decsription. It's important that these values must be aligned with the values we used in the `extension.xml`.

The second function to be overridden is `GetCustomListsOverride()` it is supposed to return a collection of lists that are provided by the extension. Let's assume we are building an extension to access an ERP system. We can have different lists like one for products, one for customers and one for orders all in the same extension. In our our example one list is enough. So we add only one instance to the collection of lists.

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

## Set up the list class

In the extension class we already used the class that represents the actual list. Every list within our extension is derived from the base class `CustomListBase`. The overriden function `GetDefinitionOverride` returns some metadata for this dedicated list so it can be handeld correctly by the hosting system and have a good UI for the end user.

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

The second function we need to override is `GetColumnsOverride`. It is called by the host system every time it needs the actual columns to represent the data. In our example we have fixed columns, so we just create a collection of columns and return the collection to the caller. Depending on the use case columns can be also returned dynamically depending on certain parameters the user is providing. We will discuss parameters in the second part of the series. Beside the name the only thing that represents a column is the data type: Number, String or Boolean.

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

The last function we need to override is `GetItemsOverride(CustomListData data)`. The function delivers the actual data to the caller. This table-like data is repesented by a collection of object called `CustomListObjectElementCollection`. It holds the rows of the table as instance of the class `CustomListObjectElement` that in turn a collection of key/value pairs that represent the table cells.

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

The compiled binaries must be all put into one single zip file. In our example the binary is only the dll, but there might by other artifacts that be packed as well, e.g. debug symbols or other referenced assemblies that might be necessary to run the extension. Beside these artifacts we need to place our `extension.xml` into the zip file.

All files that are contained in the zip are deployed to the designer and then later deployed to the Peakboard Box or BYOD instance when the pplication that uses the extension is deplyoed to its destination. 

![image](/assets/2025-11-03/010.png)

## Using he extension in Peakboard designer

To install the extension in the Peakboard designer we just add in the data source / extenson dialog

![image](/assets/2025-11-03/020.png)

After restarting the designe we can add the our custom list as data source. All available lists of the extension are listed in the drop down.

![image](/assets/2025-11-03/030.png)

The last screenshot shows the preview mode of the data source based on our example extension after hitting the refresh button. So far the extension doesn't offer any parameters for the user. This is what we will discuss in the [next article of the series](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-Parameters-and-User-Input.html).

![image](/assets/2025-11-03/040.png)





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

For our `MeowExtension` example, however, we only provide a single list, which is a static list of cats. So, our `GetCustomListsOverride()` method simply returns a new `CatCustomList` object. (We define the `CatCustomList` class in the next section.)

Here's what our final `MeowExtension` class looks like:
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

Now, we create the class for `CatCustomList`. This class represents the static list of cats that our extension returns. It extends `CustomListBase`, the base class for all custom lists.

First, We override the `GetDefinitionOverride` method. This method provides metadata about the list. That way the list is handled correctly by the host system and has a good UI---even when the extension grows more complex, later on.

Here's what our `CatCustomList` class looks like, with the `GetDefinitionOverride()` method:
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

Next, we override the `GetColumnsOverride` method. This method defines the columns of the table. Each column has a name and data type (`Number`, `String`, or `Boolean`).

The columns in our list never change, so we return a simple collection of columns. But you can also [make the columns dynamic](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-Parameters-and-User-Input.html)---changing based on the parameters that the user provides.

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

The last method we need to override is `GetItemsOverride(CustomListData data)`. This method provides the actual list data to the caller.
* The data is represented by a `CustomListObjectElementCollection` object.
* A `CustomListObjectElementCollection` object contains one `CustomListObjectElement` object for each row.
* Each `CustomListObjectElement` object contains a collection of key-value pairs that represent the table cells.

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


## Pack the extension

We must put all the compiled binaries into a single ZIP file. For our example, all we have is the DLL binary file. But depending on the extension you're building, you may have other artifacts, such as debug symbols or other referenced assemblies that your extension depends on.

We must also put our `extension.xml` file into the ZIP file. That way, Peakboard Designer can read the metadata right away.

All files that are contained in the ZIP are deployed to the Designer and then later deployed to the Peakboard Box or BYOD instance when the application that uses the extension is deployed to its destination.

![image](/assets/2025-11-03/peakboard-extension-zip-contents.png)

## Using the extension in Peakboard Designer

To install the extension in the Peakboard Designer we just add it in the data source / extension dialog, making sure we point to the freshly created ZIP package.

![image](/assets/2025-11-03/peakboard-designer-install-extension-dialog.png)

After restarting the Designer we can add our custom list as data source. All available lists of the extension are listed in the drop-down, so you can immediately select the new list without additional configuration.

![image](/assets/2025-11-03/peakboard-designer-custom-list-selection.png)

The last screenshot shows the preview mode of the data source based on our example extension after hitting the refresh button. So far the extension doesn't offer any parameters for the user. This is what we will discuss in the [next article of the series](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-Parameters-and-User-Input.html), where we will extend the foundation with interactive options.

![image](/assets/2025-11-03/peakboard-designer-data-preview.png)





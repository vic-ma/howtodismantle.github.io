---
layout: post
title: Version Control Without Git - Store our project files in the Hub
date: 2023-03-01 05:00:00 +0300
tags: administration
image: /assets/2025-09-09/title.png
image_header: /assets/2025-09-09/title_landscape.png
bg_alternative: true
read_more_links:
  - name: Organisation & Administration
    url: /category/administration
---
Every Peakboard application and Peakboard Flow is a development artifact that needs to be managed appropriately. This means providing documentation for them, and using a version control system to track changes.

When it comes to documentation, a Peakboard project (whether an application or a Flow) includes built-in documentation, like description texts and comments, which you can bind to controls and other parts of the project.

A [version control](https://about.gitlab.com/topics/version-control/) system lets you track all the changes of your development artifacts---from the beginning of the project to the latest version. The most popular version control system is [Git](https://git-scm.com/). However, Git is a complex program that can be difficult to use. Luckily, you can use Peakboard Hub to version control your Peakboard projects!

[Peakboard Hub](https://www.peakboard.com/en/product/peakboard-hub) provides a lightweight and easy way to version control your Peakboard projects (both applications and Flows). It's available both online and on-prem. In this article, we'll explain how you can use Peakboard Hub to version control your Peakboard projects. 

## File Management in Peakboard Hub

You can access Peakboard Hub's file management through the regular Hub portal. In this article, we will use Hub Online, but it works the same way in the on-prem version.
A common way to organize is to create a directory structure to store PBMX/PFX files along with other artifacts. The screenshot shows a dedicated directory for all PBMX files.

![image](/assets/2025-09-09/010.png)

## Handling the files in Peakboard Designer

In Peakboard Designer, we can choose to store a project file on the local file system — which is what we would do when using versioning with Git — or in the Hub. In the subsequent dialog we select the appropriate directory.

![image](/assets/2025-09-09/020.png)

To load a project we can also choose between the file system and Hub storage.

## Versioning

All documents stored in the Hub are automatically versioned. To access a version other than the current file, we can right-click on a file and then choose `Manage Versions`. Any stored version from the past can be restored.

![image](/assets/2025-09-09/030.png)

![image](/assets/2025-09-09/040.png)

## Permissions

It's a common practice to restrict access, especially write access, to as few users as possible. Assigning rights to certain groups of users works the same as anywhere else in the Hub. We assign the activity to specific user groups. The screenshot shows how to configure the directory for the project files. Everyone can read or download the files, but only the users who are part of the Developer group can write into the directory and change files.

![image](/assets/2025-09-09/050.png)

## Deployment

Of course, we can use the traditional way of deploying projects from the Designer to the box or BYOD instance. However, if we decide to store and version the projects in the Hub, we can deploy them directly from the file management by right-clicking the PBMX file.

![image](/assets/2025-09-09/060.png)

## Conclusion

Today we explored the basic ideas behind using Peakboard Hub's file management to store, organize, and version Peakboard project files. When we compare this method with the traditional use of Git, we can see that it is a very good trade-off for teams that do not have a Git architecture in place yet. However, Git also offers features that cannot be replaced by the Hub, such as "Pull Requests" or other development workflows that go beyond just storing and versioning. This makes it more suitable for small teams with limited need for sophisticated Git features.
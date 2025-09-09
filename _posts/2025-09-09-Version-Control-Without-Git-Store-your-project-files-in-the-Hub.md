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
Every Peakboard application and Peakboard Flow is a development artifact that needs to be managed properly. This means providing documentation for them, and using a version control system to track changes.

When it comes to documentation, a Peakboard project (whether an application or a Flow) includes built-in documentation, like description texts and comments, which you can bind to controls and other parts of the project.

A [version control](https://about.gitlab.com/topics/version-control/) system lets you track all the changes of your development artifacts---from the beginning of the project to the latest version. The most popular version control system is [Git](https://git-scm.com/). However, Git is a complex program that can be difficult to use. Luckily, you can use Peakboard Hub to version control your Peakboard projects!

[Peakboard Hub](https://www.peakboard.com/en/product/peakboard-hub) provides a lightweight and easy way to version control your Peakboard projects (both applications and Flows). It's available online and on-prem. In this article, we'll explain how you can use Peakboard Hub to version control your Peakboard projects. 

## Save projects to Peakboard Hub

Normally, in Peakboard Designer, we save our project locally, to our computer. But in order to version control our projects with Peakboard Hub, we need to save the projects to Peakboard Hub:

![image](/assets/2025-09-09/020.png)

We select a folder within Peakboard Hub to store the project file and then click *OK*:

![image](/assets/2025-09-09/025.png)

This uploads the project file (PBMX for applications and PFX for Flows) to Peakboard Hub, where it is automatically version controlled. 

## Load projects from Peakboard Hub

In order to load a project from Peakboard Hub, we click on *Open from Peakboard Hub*:

![image](/assets/2025-09-09/027.png)

## Manage project files in Peakboard Hub

To see all the project files we uploaded to Peakboard Hub, we open Peakboard Hub and go to the *Files* tab. We're using Peakboard Hub Online here, but it works the same way in the on-prem version.

![image](/assets/2025-09-09/010.png)

You can organize your files however you like, by creating folders. For example, you might have a folder for small miscellaneous projects. But you might also have a dedicated folder for one big project.


## Versioning

All files stored in Peakboard Hub are automatically versioned. To revert to a previous version of a file, right-click on the file and select *Manage Versions*. 

![image](/assets/2025-09-09/030.png)

You can restore any previous version of the file:
![image](/assets/2025-09-09/040.png)

## Permissions

It's a good security practice to restrict access, especially write access, to as few users as possible. You can configure the permissions for specific folders and user groups.

In the following screenshot, we configure the permissions for the `Misc Projects` folder. We let everyone read or download the files. But we only let developers write into the directory and change files.

![image](/assets/2025-09-09/050.png)

## Deployment

You can deploy an application directly from Peakboard Hub to a Peakboard Box. Right click on the PBMX file, and select *Upload to box*. Note that the target Box must be [connected to Peakboard Hub](/Peakboard-Hub-Online-Bring-your-boxes-into-the-cloud.html).

![image](/assets/2025-09-09/060.png)

However, you can still use the traditional way of deploying projects from Peakboard Designer to the Box or BYOD instance.

## Conclusion

Today we explored the basic ideas behind using Peakboard Hub's file management to store, organize, and version Peakboard project files. When we compare this method with the traditional use of Git, we can see that it is a very good trade-off for teams that do not have a Git architecture in place yet. However, Git also offers features that cannot be replaced by the Hub, such as "Pull Requests" or other development workflows that go beyond just storing and versioning. This makes it more suitable for small teams with limited need for sophisticated Git features.
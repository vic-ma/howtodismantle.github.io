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
Every Peakboard application and Peakboard Flow is a development artifact that should be managed properly. This means providing documentation for them and using a version control system to keep track of changes.

When it comes to documentation, a Peakboard project (an application or a Flow) includes built-in documentation, like description texts and comments, which you can bind to controls and other parts of the project.

Version control, on the other hand, is a little more complicated...

## What is version control?

A [version control](https://about.gitlab.com/topics/version-control/) system lets you track all the changes of your development artifacts---from the beginning of the project to the latest version. The most popular version control system is [Git](https://git-scm.com/). However, Git is complex and can be difficult to use. Fortunately, Peakboard Hub provides a simple alternative to Git.

[Peakboard Hub](https://www.peakboard.com/en/product/peakboard-hub) provides a lightweight and easy way to version control your Peakboard projects (both applications and Flows). It's available online and on-prem. In this article, we'll explain how you can use Peakboard Hub to version control your Peakboard projects. 

## Save projects to Peakboard Hub

Normally, in Peakboard Designer, we save our project locally, to our computer. In order to version control our projects with Peakboard Hub, all we need to do is save the projects to Peakboard Hub instead:

![image](/assets/2025-09-09/020.png)

We select a folder within Peakboard Hub to store the project file and then click *OK*:

![image](/assets/2025-09-09/025.png)

This uploads the project file (PBMX for applications and PFX for Flows) to the specified folder in Peakboard Hub, where it is automatically version controlled. 

## Load projects from Peakboard Hub

In order to load a project from Peakboard Hub, we click on *Open from Peakboard Hub*:

![image](/assets/2025-09-09/027.png)

## Manage project files in Peakboard Hub

To see all the project files we uploaded to Peakboard Hub, we open Peakboard Hub and go to the *Files* tab. We're using Peakboard Hub Online here, but it works the same way with the on-prem version.

![image](/assets/2025-09-09/010.png)

You can organize your files however you like, by creating folders. For example, you might have a folder for Peakboard applications that run in the factory floor, and a folder for Peakboard applications that run in the office. Within those folders, you might have additional subfolders. For example, you might have `/office/sap/` for Peakboard apps that integrate with SAP.


## Versioning

All files stored in Peakboard Hub are automatically versioned. To revert to a previous version of a file, right-click on the file and select *Manage Versions*:

![image](/assets/2025-09-09/030.png)

You can restore any previous version of the file:
![image](/assets/2025-09-09/040.png)

## Permissions

It's a good security practice to restrict access---especially write access---to as few users as possible. You can configure the permissions for specific folders and user groups.

In the following screenshot, we configure the permissions for the `Misc Projects` folder. We allow everyone to read and download the files. But we only let developers write to the directory and change files.

![image](/assets/2025-09-09/050.png)

## Deployment

You can deploy an application directly from Peakboard Hub to a Peakboard Box. Right click on the PBMX file and select *Upload to box*. Note that the target Box must be [connected to Peakboard Hub](/Peakboard-Hub-Online-Bring-your-boxes-into-the-cloud.html).

![image](/assets/2025-09-09/060.png)

However, you can always use the traditional way of deploying projects from Peakboard Designer to the Box or BYOD instance.

## Conclusion

We looked at how to use Peakboard Hub's file management system to store, organize, and version control Peakboard project files. It's a great, lightweight option for teams that don't have a Git workflow in place yet.

However, Git also offers features that cannot be replicated by the Hub, such as pull requests and other development tools that go beyond simple storage and versioning. This makes Peakboard Hub's file management system more suitable for small teams that don't need Git's more complex features, and just want a simple way to store and version control Peakboard files.
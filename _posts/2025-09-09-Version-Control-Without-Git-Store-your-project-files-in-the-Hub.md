---
layout: post
title: Version Control Without Git - Store your project files in the Hub
date: 2023-03-01 05:00:00 +0300
tags: administration
image: /assets/2025-09-09/title.png
image_header: /assets/2025-09-09/title_landscape.png
bg_alternative: true
read_more_links:
  - name: Organisation & Administration
    url: /category/administration
---
Every Peakboard application and flow is some kind of development artifact that should be treated like this. In a professional context it means that these artifacts should be treated in way that is aligned with common inustry standards of security and documentation.
When it comes to documentation a Peakboard project (both regular design project or Flow) brings some kind of built-in documentation like description texts and comments that can be bound to controls and other parts of the project.
Beside documentation versioning is a huge topice and crucial requirement in industrial software development. The most commong way to do this, is [Git](https://git-scm.com/), a free and open source tool to exactly track all changes of development artificats from the beginning of the project to the latest version. In this article we will discuss a light weight alternaive to git. The Peakboard Hub (both online and on prem) offers a built-in document management system to store and version pbmx files (design projects) and pfx files (Flo projects) in similiar way as Git would do it.

## File Management in Peakboard Hub

The file management of Peakboard Hub can be accesses through the regular Hub portal. In this article we will Hub Online, but it works the same way in the on prem version.
A typical way of organizing would be to think of a directory structure to store the pbmx/pfx files along with other artifacts. The screenshot shows a dedicated directory for all pbmx files.

![image](/assets/2025-09-09/010.png)

## Handling the files in Peakboard Designer

In Peakboard Designer we can choose to store a project file on the local file system (which what you would do for using the versioning with Git) or in the Hub. In the subsequent dialog the right driectory must be chosen.

![image](/assets/2025-09-09/020.png)

To load a project from the Hub we also can choose for the files system and Hub storage.

## Versioning

All documents that are stored in the Hub are automatically versioned. To access other than the current version of file we can right click on a file and then choose `Manage Versions`. All stored versions from the past can be restored.

![image](/assets/2025-09-09/030.png)

![image](/assets/2025-09-09/040.png)

## Permissions

It's a common practice to restrict access, especially write access, to as few users as possible. The way of assigning rights to certain groups of users works the same as everywhere in the Hub. Just assign the activity to certain user groups. The screenshot shows how to configure the directory for the project files. EVeryone can read / download the files, but only the users who are part of Developer group can write into the directory and change files.

![image](/assets/2025-09-09/050.png)

## Deployment

Of course we can use the tradional way of deploying projects from the designer to the box or BYOD instance. But if we decide to store and version the projects in Hub, we can deploy it directly from the file management by using right click on the pbmx file.

![image](/assets/2025-09-09/060.png)

## conclusion

Today we learned the basic ideas behind using the Peakboard Hub file mangagement to store, organize, and versioning Peakboard project files. When we compare this method of the traditional usage of Git, we can see, that it it's a very good trade off for users and teams who don't have a Git architecture in place yet. However Git also offers features that can't by replaced by the Hub, e.g. "Pull Requests" or other dev related workflows that go beyond just storing and versioning. This amkes it more suitable for small teams with limited need for sophiscated Git features.




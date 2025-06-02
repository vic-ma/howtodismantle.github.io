---
layout: post
title: Digital Walkaround Inspections Part I - How to Build an Audit App
date: 2023-03-01 03:00:00 +0200
tags: bestpractice usecase
image: /assets/2025-06-13/title.png
image_landscape: /assets/2025-06-13/title_landscape.png
bg_alternative: true
read_more_links:
  - name: Digital Walkaround Inspections Part II - How to Build an Audit App
    url: /Digital-Walkaround-Inspections-Part-II-How-to-Build-an-Audit-App.html
downloads:
  - name: MyAudit.pbmx
    url: /assets/2025-06-13/MyAudit.pbmx
---
In this week's article we start we start with a small series about how to build an Audit app with Peakboard. In the first part we will discuss some theroetical background about how to structure an Audit app, how to define the necssary tables to store the metadata and actual data. In [the second part](/Digital-Walkaround-Inspections-Part-II-How-to-Build-an-Audit-App.html) of we will discuss how to build and operate the actual app. 
It's important to understand that the strcuture and process shown in these articles is very simple and might not satify every requirement for an audit process and visual walkaround. The idea is, that the strcuture is simple and flxible, that t can be easy adjusted to literally every special need a company might have around that topic. SO actually it's designed to be easily enhanced and adjusted.

## What is an audit and how does it look like?

In our definition the actual process of an audit or a "digital walkaround" works like this:

1. The auditor opens the app on his tablet
2. From a list of possible audits he selects one he wants to conduct
3. The app guides him through several steps he must mark as done
4. Some steps might involve typing in additional information
5. After having completed all steps the audit is considered as finished, the results are stored to document and persist them

To make it a bit clearer, here are the steps of our sample audit we will use in this article. It's how to conduct a safety check for a CNC machine. Each step is also one screen the user can step through.

1. Welcome the user and explain the purpose of the audit
2. Ask the user to check both energency stops
3. Ask the user to test the safetey door
4. Ask the use to check and write down any problems with the miling head
5. Ask the user the check the level of the lubricant canister and write down the filling level

For the 5 steps here are the 5 screens the user steps through.

![image](/assets/2025-06-13/AuditStep1.png)
![image](/assets/2025-06-13/AuditStep2.png)
![image](/assets/2025-06-13/AuditStep3.png)
![image](/assets/2025-06-13/AuditStep4.png)
![image](/assets/2025-06-13/AuditStep5.png)



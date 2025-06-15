---
layout: post
title: Digital Walk-Around Inspections Part I - How to Build an Audit App
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
This article is the first part of our two-part series on how to build an audit app with Peakboard. In this article, we discuss the theoretical background on how to structure an audit app, and how to define the necessary tables to store the metadata and actual data.

In [the second part](/Digital-Walkaround-Inspections-Part-II-How-to-Build-an-Audit-App.html) of this series, we discuss how to actually build and operate the app. 

Note that the structure and process we show in these articles is very simple and might not satisfy all the requirements of an audit process and visual walk-around. But the structure is simple and flexible, so you can easily adjust it to suit your company's needs.

## What is an audit and what does it look like?

This is how we define the audit process, or "digital walk-around":

1. The auditor opens the auditing app on their tablet.
1. The app presents a list of possible audits.
1. The auditor selects the audit they want to perform.
1. The app gives step-by-step instructions for how to perform the audit. For each step:
  1. The auditor performs the instructions given by the app.
2. They look at the list of possible audits and select the audit they want to conduct.
3. They follow each of the steps that the app 
3. The app guides him through several steps he must mark as done each of them
4. Some steps might involve typing in additional information
5. After having completed all steps the audit is considered as finished, the results are stored to document and persist them

To make it a bit clearer, here are the steps of our sample audit we will use in this article. It's how to conduct a safety check for a CNC machine. Each step is also one screen the user can step through. Step 4 and 5 needs input from the user.

1. Welcome the user and explain the purpose of the audit
![image](/assets/2025-06-13/AuditStep1.png)

2. Ask the user to check both energency stops
![image](/assets/2025-06-13/AuditStep2.png)

3. Ask the user to test the safetey door
![image](/assets/2025-06-13/AuditStep3.png)

4. Ask the use to check and write down any problems with the miling head
![image](/assets/2025-06-13/AuditStep4.png)

5. Ask the user the check the level of the lubricant canister and write down the filling level
![image](/assets/2025-06-13/AuditStep5.png)

## How to store the meta data 

The metadata on how an Audit looks like is stored in two tables: AuditTemplateHeader and AuditTemplateItems. These tables contain the structure and the actual defintion of an audit. In our example we use the Peakboard Hub for all our tables. It would be no problem to use any other data storage that is supported by Peakboard.

The table AuditTemplateHeader has one row per audit definition. It contains a unique name and a simple description.

![image](/assets/2025-06-13/010.png)

Every step of the audit has one corresponding row in the table AuditTemplateItem. It conatins the actual content that appears on the screens later. The "StepNo" columns represents the order of steps starting with 0.

The most important column is "Layout". It will define how this step is represented  when the audit is displayed to used in the app. Our five sample steps shown in the screenshot have three different layouts:

- FT01 -> Is just a normal text in combination with an image. The user can only mark this step as done, no other input options
- ENTRY01 -> Is a text with an image PLUS a text field for the user to enter some freely choosen text 
- CHOICE01 -> Is a text with an image PLUS a multiple choice out of three different options.

The fields Var01 to Var05 are five multipurpose columns that can contain actual content that is used as content on the screen defined by the "Layout" column. Could be text, could be the URL of an image, could be possible choices or the multiple choice screen. It depends on the layout.

For all layouts Var01 is always the headline, Var02 is always multiline text to explain the step. Beside this,

- Var03 is the URL for the image to be shown, but only when layout is FT01 or ENTRY01
- Var03, Var04, Var05 are the three multiple choice options, but only when layout is ENTRY01

It's very important to understand this logic and combination of variables and layout. Because it's the central idea of our data structure.

![image](/assets/2025-06-13/020.png)

The three layouts mentioned above only apply to our sample use case and how audits are conducted in our sample company. In the real world, there might by other or more layouts. It's even possible to extend the number of variables from 5 to 10 or 15, if it's necessary. It depends on the content.

## Data storage for the actual audit transactions

In the last paragraph we desicussed how to store the meta data of an audit. For the actual audit, that is conduected, the data is stored in the table "AuditHeader" and "AuditItem".

- Columns TS, a time stamp that represents as the point in the time the audit was started. It also serves as primary key.
- Name is the name of the audit, it refers to the same audit name as used int he meta data (e.g. CNCSA1 for our sample CNC machine)
- State of audit (A for Active, D for Done). This state is set from A to D when all audit steps are set from A to D.

The screeshot shows "AuditHeader": 

![image](/assets/2025-06-13/030.png)

For the table AuditItem, we have some more columns:

- TS as time stamp and foreign key to the AuditHeader
- STepNo the steps of this audit starting with 0
- State of audit step (A for Active, D for Done)
- TSDone the time stamp when this step was set on Done.
- Input01 - Input05, five possible columns to store the data in that the user has created as input data during audit
- Var01 - Var05, five variables with the same data as the varibales data from meta data. The meaning depends on the layout

![image](/assets/2025-06-13/040.png)

## conclusion

For building the audit application we need to understand the architecture of the data structure, for both the meta data and also the transaction data. That's what we discussed in this article. In [the second part](/Digital-Walkaround-Inspections-Part-II-How-to-Build-an-Audit-App.html) we will see, who we build the actual application and learn how the audit is conducted. 

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

This is what the audit process, or "digital walk-around," looks like:

1. The user opens the auditing app on their tablet.
1. The app presents a list of possible audits.
1. The user selects the audit they want to perform.
1. The app gives step-by-step instructions for how to perform the audit. For each step:
    1. The user performs the instructions given by the app.
    1. If the step requires information from the user, the user enters that information into the app.
    1. The user marks the step as done.
    1. The app shows the next step.
5. The audit is finished. The app saves the results into a document and persists them.

## Example audit

The following is an example of an audit. It tells the user how to perform a safety check for a CNC machine. Steps 4 and 5 require the user to enter information into the app.

1. Welcome the user and explain the purpose of the audit.
![image](/assets/2025-06-13/AuditStep1.png)

2. Tell the user to inspect both emergency stops.
![image](/assets/2025-06-13/AuditStep2.png)

3. Tell the user to test the safety door to make sure it works properly.
![image](/assets/2025-06-13/AuditStep3.png)

4. Tell the user to visually inspect the milling head, and record their observations in the app.
![image](/assets/2025-06-13/AuditStep4.png)

5. Tell the user the inspect the lubricant canister, and record the amount of remaining lubricant in the app.
![image](/assets/2025-06-13/AuditStep5.png)

## How to store metadata 

An audit is defined by its metadata. This metadata is stored in two tables: `AuditTemplateHeader` and `AuditTemplateItem`. These tables specify the structure and steps of the audits.

In our example, we used Peakboard Hub to store our tables. But you can any storage solution that is supported by Peakboard.

### The `AuditTemplateHeader` table
The `AuditTemplateHeader` table is a list of all the audits. It contains one row for each audit. Each row has two columns:
* `Name`, the name of the audit.
* `Description`, a short description of the audit.

![image](/assets/2025-06-13/010.png)


### The `AuditTemplateItem` table

The `AuditTemplateItem` table defines the steps of all the audits. Each row represents a single step in an audit. The following is an overview of all the columns of this table.

![image](/assets/2025-06-13/020.png)

#### `AuditName`

This is the name of the audit that the step belongs to. Remember that the `AuditTemplateItem` table contains the steps for *all* the audits. This column tells Peakboard which steps belong to which audits.

The `AuditName` column must match the `Name` column of the corresponding audit in the `AuditTemplateHeader` table. Notice that `CNCSA1` also appears in our example `AuditTemplateHeader`  table.

#### `StepNo`

This is the number of the step. So the first step has `StepNo` 0, the second step has `StepNo` 1, etc. This column tells Peakboard how to order the steps of each audit.

#### `Layout`

The most important column is "Layout". It will define how this step is represented  when the audit is displayed to used in the app. Our five sample steps shown in the screenshot have three different layouts:

- FT01 -> Is just a normal text in combination with an image. The user can only mark this step as done, no other input options
- ENTRY01 -> Is a text with an image PLUS a text field for the user to enter some freely choosen text 
- CHOICE01 -> Is a text with an image PLUS a multiple choice out of three different options.

The fields Var01 to Var05 are five multipurpose columns that can contain actual content that is used as content on the screen defined by the "Layout" column. Could be text, could be the URL of an image, could be possible choices or the multiple choice screen. It depends on the layout.

For all layouts Var01 is always the headline, Var02 is always multiline text to explain the step. Beside this,

- Var03 is the URL for the image to be shown, but only when layout is FT01 or ENTRY01
- Var03, Var04, Var05 are the three multiple choice options, but only when layout is ENTRY01

It's very important to understand this logic and combination of variables and layout. Because it's the central idea of our data structure.


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

---
layout: post
title: Digital Walkaround Inspections Part II - How to Build an Auditing App
date: 2023-03-01 03:00:00 +0200
tags: bestpractice usecase
image: /assets/2025-06-21/title.png
image_landscape: /assets/2025-06-21/title_landscape.png
bg_alternative: true
read_more_links:
  - name: Digital Walkaround Inspections Part I - How to Build an Auditing App
    url: /Digital-Walkaround-Inspections-Part-I-How-to-Build-an-Audit-App.html
downloads:
  - name: MyAudit.pbmx
    url: /assets/2025-06-13/MyAudit.pbmx
---
In [our last article](/Digital-Walkaround-Inspections-Part-I-How-to-Build-an-Audit-App.html), we explained what an auditing app is and how the underlying tables work. The audit definitions are stored in the `AuditTemplateHeader` and `AuditTemplateItem` tables. And the audit run data is stored in the `AuditHeader` and `AuditItem` tables. We also discussed the sample data that is necessary to conduct a CNC machine walkthrough.

In this second part of our auditing app mini series, we will explain how to build an auditing app in Peakboard, using the tables that we discussed in our last article.

## Main screen

The main screen of our app is very simple. It has two buttons:
* One for creating a new audit from a template.
* One for loading an existing audit.

![image](/assets/2025-06-21/010.png)

## Prepare the data sources and variables

![image](/assets/2025-06-21/020.png)

Here are the data sources we need:

- The `ActiveAuditHeader` data source points to the `AuditHeader` table. The `ActiveAuditItem` data source points to the `AuditItem` table. For both of these data sources, we use the filter `TS == {ActiveTS}`. That way, the data sources only give information for the active audit.
- The `AllAuditHeader` data source contains all the possible audits. We need this for the overview screen, so the user can pick the audit they want to perform. We use the filter `State == {AuditFilter}`. This lets the user filter for active audits only.
- The `AuditTemplateHeader` and `AuditTemplateItem` data sources contain all the audit definitions, and they correspond to the same tables in our Peakboard Hub (or whatever storage solution you're using; see our [last article](/Digital-Walkaround-Inspections-Part-I-How-to-Build-an-Audit-App.html) for more information).

Here are the variables we need:

| Variable        | Description |
| --------------- | ----------- |
| ActiveStep      | The `StepNo` (step number) of the step that's currently being displayed to the user. |
| ActiveStepState | The `State` of the current step (we need this for the UI). |
| ActiveTS        | The `TS` (start timestamp) of the currently active audit. |
| AuditFilter     | A filter that the user can set, to filter the list of past audits by their `State` (`A` or `D`). |

For more details, you can [download the PBMX](/assets/2025-06-13/MyAudit.pbmx).

## Create a new audit

The screenshot show the overview of all possible audits (table AuditTemplateHeader). The user can pick one to create a new audit from the template.

![image](/assets/2025-06-21/030.png)

The procedure behind the create button is shown in the screenshot. The basic idea is that the template header is copied into the new audit header, and the template items are all copied in the the new audit items. This includes all 5 variables of the item template.

We use the time stamp TS as a database key to build a relationship between header and items.

![image](/assets/2025-06-21/040.png)

## Load an audit

For the screen to let the user load an existing audit, we just present a list of available audits to the user. The user can use a simple filter to choose between open and completed audits.

![image](/assets/2025-06-21/050.png)

The actual procedure behind is pretty simple. It just reloads the two corresponding table according to the filter TS. The active step is set to 0, so we always start with the first screen and activate it.

![image](/assets/2025-06-21/060.png)

## Load a single audit step

The actual magic is happening when a new single step is loaded or ativated. This is coded within the function "ActivateStep". First we check, if the step is valid. If not we pop out an error message. In case there are only D steps (D for DOne), we set the overall state of the audit also to D. This case covers the end of the audit when all steps are done.

![image](/assets/2025-06-21/070.png)

The more important branch is a vlaid, active step. In that case we check for the layout. Depening on the layout in the metadata the corresponding screen is loaded and all variables are set to screen elements (monstly but not limited to textboxes). Through this process the layout value determines the screen to be shown for a certain step.

![image](/assets/2025-06-21/071.png)

Let's check a sample screen with layout "FT01". There are two test fields to be filled with Var1 and Var2, and also an image to be filled with an URL which is stored in Var3.

![image](/assets/2025-06-21/080.png)

## Putting a step to done

Let's discuss the procedure behind the "Mark as Done" button which can be found on any layout screen. The screenshots shows another layout. It's the "ENTRY01" layout where the user can or must make an input entry.

![image](/assets/2025-06-21/090.png)

Here's the logic behind the "Mark as done" button. We just write back to the hub list into the table AuditItem and set the state to D for Done, and also we store the user's input  to one of the input variable columns. In that case "Input01". Then we do a reload to make sure the changed data is available on our datasource. Calling the function "ActivateStep" is re-adjusting the UI elements (e.g. set the "Mark as done" button to disabled)

![image](/assets/2025-06-21/100.png)

## result

The animation shows the audit with our example data. First a new audit is generated from a tamplate. Then the audit starts and every step is marked as done. Two of the steps require the user's input.
It's very important to understand that our example only shows a small part od the options. It's easy to use the same architecture and principle to build even very complex audits with lot's of different layouts and much more user input. For the sake of clearity we only did a very simple example here.

![image](/assets/2025-06-21/result.gif)






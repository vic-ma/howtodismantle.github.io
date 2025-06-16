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

In this second part of our auditing app mini-series, we explain how to build an auditing app in Peakboard, using the architecture that we discussed in our last article.

## Main screen

The main screen of our app is simple. It has two buttons:
* One for creating a new audit from a template.
* One for loading an existing audit.

![image](/assets/2025-06-21/010.png)

## Prepare the data sources and variables

![image](/assets/2025-06-21/020.png)

Here are the data sources we need:

- `ActiveAuditHeader` and `ActiveAuditItem`
    * These data sources point to the `AuditHeader` and `AuditItem` tables, respectively. For both of these data sources, we use the filter, `TS == {ActiveTS}`. That way, the data sources only give information for the active audit.
- `AllAuditHeader`
    * This data source contains all the possible audits. We need this for the overview screen, so that the user can select the audit they want to perform. We use the filter, `State == {AuditFilter}`. This lets the UI switch between open and completed audits.
- `AuditTemplateHeader` and `AuditTemplateItem`
    * These data sources contain all the audit definitions, and they correspond to the same tables in our Peakboard Hub (or whatever storage solution you're using).

Here are the variables we need:

| Variable        | Description |
| --------------- | ----------- |
| `ActiveStep`      | The `StepNo` (step number) of the step that's currently being displayed to the user. |
| `ActiveStepState` | The `State` of the current step (we need this for the UI). |
| `ActiveTS`       | The `TS` (start timestamp) of the currently active audit. |
| `AuditFilter`     | The filter that's used to filter the list of past audits. This is either `A` or `D`. |

For more details, you can [download the PBMX](/assets/2025-06-13/MyAudit.pbmx).

## New audit screen

The new-audit screen shows all the possible audits (using the `AuditTemplateHeader` table). The user can select *Create new audit*, in order to start a new audit.

![image](/assets/2025-06-21/030.png)

Here is the Building Blocks script for the *Create new audit* buttons:

![image](/assets/2025-06-21/040.png)

We use `TS` (timestamp) as a database key to build a relationship between headers and items.

We copy the template header into the new audit header. And we copy all the template items into the new audit items. This includes all 5 variables of the item template.

## Load existing audit screen

The load-existing-audit screen shows a list of existing audits. The user can select the *Open Audits* and *Completed Audits* button, in order to change what type of audits are displayed. These buttons change the `{AuditFilter}` variable to either `A` (active) or `D` (done).

![image](/assets/2025-06-21/050.png)

The buttons according to the filter TS. The active step is set to 0, so we always start with the first screen and activate it.

Here is the Building Blocks script for this screen:

![image](/assets/2025-06-21/060.png)

It reloads `ActiveAuditHeader` and `ActiveAuditItems`. It sets `ActiveStep` to 0---that way, we always show the user the first step (even if it's already completed).

## Load a single audit step

The `ActivateStep` function loads a single audit step. This is how the app switches between steps of an audit.

The script is based on an if-statement that checks if the step is valid or not. If the step is not valid (step number is higher than the number of steps in the active audit), then this is what happens:

![image](/assets/2025-06-21/070.png)

1. Send an error message.
2. Check if all the steps of the active audit are completed. If all the steps are completed, set the `State` of the audit to `D` (done).

If the step is valid, this is what happens:

![image](/assets/2025-06-21/071.png)

The script gets the `Layout` of the step. Then, it sets all the appropriate variables and displays the screen for the step. The exact details of how this works depends on which layout the step uses.

Let's check a sample screen with layout "FT01". There are two test fields to be filled with Var1 and Var2, and also an image to be filled with an URL which is stored in Var3.

Here's a screen with the `FT01` layout. As you can see, it displays the text from `Var1` and `Var2`. It also displays an image, using the URL from `Var3`.

![image](/assets/2025-06-21/080.png)

## Mark as done button

Let's discuss the script behind the *Mark as Done* button. Each layout screen has this button. However, the script for the button differs a bit, based on the specific layout. This is because each layout takes different input from the user (no input, text, or multiple choice).

Let's use the `ENTRY01` layout as an example. This layout lets the user enter text:

![image](/assets/2025-06-21/090.png)

Here's the script for the *Mark as Done* button for the `ENTRY01` layout:

![image](/assets/2025-06-21/100.png)

The script adds a row to our `ActiveAuditItems` table, in Peakboard Hub (you may be using a different storage solution). It sets the following rows:
* Set `Input01` to the text that the user entered into the text field.
* Set `State` to  `D`, to mark this step as done.
* Set `TSDone` to the current time, to record the time that the step was completed.

Then, the script reloads the `ActiveAuditItems` list so that our data source can see the updated data.

Finally, the script calls the `ActivateStep` function, to adjust UI elements appropriately (e.g. disable the *Mark as Done* button).

## Result

The following video shows what our audit app looks like. First, we select the CNC safety audit. Then we mark each step of the audit as done. Two of the steps require user input.

![image](/assets/2025-06-21/result.gif)

It's important to note that we only gave a small demonstration of what's possible with this architecture. It's easy to use the same architecture to build highly complex audits with many different layouts and much more user input.
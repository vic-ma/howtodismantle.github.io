---
layout: post
title: How to make the SAP system fit for report execution
date: 2022-12-15 12:00:00 +0200
tags: sap
image: /assets/2022-12-15/title.png
read_more_links:
  - name: How to build a perfect RFC function module to be used in Peakboard
    url: /SAP-How-to-build-a-perfect-RFC-function-module-to-be-used-in-Peakboard.html
downloads:
  - name: Z_XTRACT_IS_REMOTE_REPORT.txt
    url: /assets/2022-12-15/Z_XTRACT_IS_REMOTE_REPORT.txt
---
Peakboard supports a lot of different object types you can access in SAP, beside RFC function modules, queries, tables, MDX commands, it's possible to execute and process ABAP reports. For doing so, it's necessary to install a small, generic function module in the SAP system. How to do that is explained in this article. Please make sure to logon to SAP with a user who has development rights and who knows how to install ABAP code. During the process you will need to provide a development class and eventually a transport request to put your new objects into it. If you don't know how to do this, please ask a coworker or google.

Feel free to adjust the name of the function module and / or the name of the DDIC stucture to your needs (e.g. according to a company's namespace).

## Setting up the DDIC structure

At first please go to transaction SE11 and create a new DDIC structure wit name ZTAB1024 and one component as shown in the screenshot. Save and activate the object.

![image](/assets/2022-12-15/010.png)

## Setting up the function module

In transaction SE37 or transaction SE80 create a new function module with name Z_XTRACT_IS_REMOTE_REPORT.
Make sure that it's marked as RFC enabled.

![image](/assets/2022-12-15/020.png)

Here are the import parameter to be added. Feel free to use copy and paste, so you dont have to type it manually. There might be a warning, that it is not recommended to use LIKE parameters. Just ignore the warning by hitting Enter. 

`PROGRAM_NAME	LIKE	RPY_PROG-PROGNAME	                     
ACTIONID	LIKE	RPY_PROG-PROG_TYPE	'0'
VARIANT	LIKE	AQADEF-VARI	                     
JOBNAME	LIKE	TBTCJOB-JOBNAME	'XTRACT'
SPOOLID	LIKE	TSP01-RQIDENT	                     
JOBCOUNT	LIKE	TBTCP-JOBCOUNT	                     
SPOOLDESTINATION	LIKE	PRI_PARAMS-PDEST	'LOCL'`
                              			                     
![image](/assets/2022-12-15/030.png)

Here are the export parameters.

`JOBNUMBER	LIKE	TBTCJOB-JOBCOUNT
JOBSPOOLID	LIKE	TBTCP-LISTIDENT
JOBSTATUS	LIKE	TBTCO-STATUS`
                              			                     
![image](/assets/2022-12-15/040.png)

And the tables. Please note, that the second parameter refers tothe new DDIC structure.

`SELECTION_TABLE	LIKE	RSPARAMS
LIST_OUTPUT	LIKE	ZTAB1024
TEXTELEMENTS	LIKE	TEXTPOOL`
                              			                     
![image](/assets/2022-12-15/050.png)

And the excepetion:

`REPORT_NOT_FOUND
LIST_FROM_MEMORY_NOT_FOUND
LIST_FROM_MEMORY_OTHERS
LIST_TO_ASCI_EMPTY
LIST_TO_ASCT_INDEX_INVALID
LIST_TO_ASCT_OTHERS
REFRESH_FROM_SELECTOPTIONS
RPY_PROGRAM_READ_FAILURE
NOT_AUTHORIZED
JOB_CLOSE_EXCEPTION
JOB_OPEN_EXCEPTION
JOBID_NOT_FOUND_EXCEPTION
JOBSTATUS_NOT_FOUND_EXCEPTION`
                              			                     
![image](/assets/2022-12-15/060.png)

For the source code please refer to [this file](/assets/2022-12-15/Z_XTRACT_IS_REMOTE_REPORT.txt).

![image](/assets/2022-12-15/070.png)

Finally make sure to save and activate the function module.

## Use the RFC function in XQL

AFter having succesfully installed the function module you can use within your XQL statements. This sample shows how tocall a simpled report with a variant.

{% highlight sql %}
EXECUTE REPORT 'RLT10010' USING 'VAR01'
{% endhighlight %}

![image](/assets/2022-12-15/080.png)

If you have chosen to use your own name or namespace, just add the WITH-OPTION extension as shown in this sample:
 
{% highlight sql %}
EXECUTE REPORT 'RQMELL10' USING 'OFFEN'
With-Options (CustomFunction = 'Z_MY_REPORT_FUNCTIO
{% endhighlight %}


---
layout: post
title: Dismantle SAP Production - How to determine workplace capacity
date: 2023-03-01 12:00:00 +0200
tags: bestpractice
image: /assets/2023-12-09/title.png
read_more_links:
  - name: How to build a perfect RFC function module to use in Peakboard
    url: /SAP-How-to-build-a-perfect-RFC-function-module-to-be-used-in-Peakboard.html
downloads:
  - name: Source Code for RFC - z_pb_get_workplace.txt
    url: /assets/2023-12-09/z_pb_get_workplace.txt
  - name: SAPWorkplaceCapacity.pbmx
    url: /assets/2023-12-09/SAPWorkplaceCapacity.pbmx
---

One of the top 5 use cases for using Peakboard in a production environment is to show the current orders, operations and the load of ne or more workplaces. To determine the load of workplace, we need the capacity that is currently available for this workplace. The best way to get a workplace's capacity is to let an SAP function module do the work and just call it from the Peakboard application. The problem is, that SAP doesn't offer any standard RFC-enabled function module to determine the capacity. The good news is, that there's an internal function module called CR_CAPACITY_AVAILABLE for dertermining the capacity. This article shows to to make this function module accesible from the outside and call it from an Peakboard application.

Please scroll down to download to the bottom to find a link to download the ABAP source code that is used here.
ALso feel free to adjust the naming which is used in this article. If you need to align the naming to your comapnay's convention, no problem. The code is short and easy to understand.

One more note: The RFC function module built in this article is aligned with the [recommendations on how to build an ideal RFC to be used in Peakboard](/SAP-How-to-build-a-perfect-RFC-function-module-to-be-used-in-Peakboard.html).


## Setting up the requirements on the SAP side

For the exchange of the capacity information we use a DDIC structure. This structure can be created by using tranaction SE11.

![image](/assets/2023-12-09/010.png)

The structure only contains standard component types, that are commonly used. After adding the descirption text and the components we must not forget to activate the structure to make it avaiable.

![image](/assets/2023-12-09/020.png)

The next steps is to create the actual function module. It's is very important to mark it as Remote Enabled as we need to call the function from the outside later. The developmemnt package  and function group are mandatory. It depends on the common habits of the SAP system which one to use. It's reocmmended to create a function group and package only for this and all future Peakboard development objects.

![image](/assets/2023-12-09/030.png)

The next screenshot shows all the import parameters. To determine the capavcity we need to know the start and end date, the workplace and also the plant. These four parameters are all Imports.

![image](/assets/2023-12-09/040.png)

The last step is to define a table. As you can see in the screenshot the type of the table referes to the DDIC structure we created earlier.

![image](/assets/2023-12-09/050.png)

After all requirements are done, we copy and paste the ABAP code to the source code editor:

![image](/assets/2023-12-09/060.png)

And finally save and activate the whole function module.

## How the code works

In the first part the workplace name is translated into the ID by looking it up in table CRHD and KAKO.

![image](/assets/2023-12-09/070.png)

The we call the actual function module CR_CAPACITY_AVAILABLE.

![image](/assets/2023-12-09/080.png)

The return values are translated into the elements of the rtrurn structure. Please note the capaity is returned in the unit seconds. We already transfer the seconds into minutes before we return the values to the caller. Depending on the use case it might make sense to use hours here.

![image](/assets/2023-12-09/090.png)

Beside the capacity also the operating time, and the start and end time is calculated.

## Using the RFC function module in Peakboard

The RFC function's interface is idealy designed to be used within Peakboard. Here's the XQL call for the RFC. We can see the four parameters we used when creating the RFC: Start and end date, workplace and plant.

{% highlight sql %}
EXECUTE FUNCTION 'Z_PB_GET_WORKPLACE'
   EXPORTS
      I_STARTDATE = '20231120',
      I_ENDDATE   = '20231125',
      I_ARBPL     = '1320',
      I_WERKS     = '1000'
   TABLES
      T_WORKPLACE INTO @RETVAL
{% endhighlight %}

And here is how it looks like when editing the data source.

![image](/assets/2023-12-09/100.png)

In our sample application we can easily bind text boxes to the data source and also use a custom format to present the values to the end user (e.g. unit for the capacity, time format, etc...). Feel free to download the sample pbmx.

![image](/assets/2023-12-09/110.png)

## code

Here's the whole code of the Z_PB_GET_WORKPLACE, it's also available for download on the link on the bottom.

{% highlight abap %}
FUNCTION z_pb_get_workplace.
*"----------------------------------------------------------------------
*"*"Local Interface:
*"  IMPORTING
*"     VALUE(I_STARTDATE) TYPE  SYST_DATUM DEFAULT SY-DATUM
*"     VALUE(I_ENDDATE) TYPE  SYST_DATUM DEFAULT SY-DATUM
*"     VALUE(I_ARBPL) TYPE  ARBPL
*"     VALUE(I_WERKS) TYPE  WERKS_D
*"  TABLES
*"      T_WORKPLACE STRUCTURE  ZPBWORKPLACE OPTIONAL
*"----------------------------------------------------------------------

  DATA xkapid TYPE kapid.
  DATA xoptime TYPE rc65d-periodint.
  DATA xcapacity TYPE rc65d-periodint.
  DATA xbeginn TYPE sy-uzeit.
  DATA xende TYPE sy-uzeit.

  t_workplace-arbpl = i_arbpl.
  t_workplace-werks = i_werks.

  CLEAR: xoptime.

  SELECT SINGLE * FROM crhd WHERE arbpl = t_workplace-arbpl AND werks = t_workplace-werks.

  IF sy-subrc EQ 0.
    xkapid = crhd-kapid.
    SELECT SINGLE * FROM kako WHERE kapid = crhd-kapid.
    IF sy-subrc EQ 0.
      IF NOT kako-refid IS INITIAL.
        xkapid = kako-refid.
      ENDIF.

      CALL FUNCTION 'CR_CAPACITY_AVAILABLE'
        EXPORTING
          datub                        = i_enddate
          datuv                        = i_startdate
          kapid                        = xkapid
*         PDAYS_ONLY                   = ' '
*         DURATION                     =
        IMPORTING
          optime                       = xoptime
*         PDAYS                        =
          value                        = xcapacity
*         WDAY_FIRST                   =
*         WDAY_LAST                    =
*         OPTIME_WDAY_FIRST            =
          starttime_wday_first         = xbeginn
*         OPTIME_WDAY_LAST             =
          endtime_wday_last            = xende
*         FLAG_OVERMIDNIGHT            =
        EXCEPTIONS
          not_found                    = 1
          missing_parameter            = 2
          no_capacity                  = 3
          date_outside_factorycalendar = 4
          invalid_parameter            = 5
          OTHERS                       = 6.

      xoptime = xoptime / 60.
      xcapacity = xcapacity / 60.

      t_workplace-optime = xoptime.
      t_workplace-capacity = xcapacity.
      t_workplace-starttime = xbeginn.
      t_workplace-endtime = xende.


    ENDIF.

    APPEND t_workplace.

  ENDIF.

ENDFUNCTION.
{% endhighlight %}
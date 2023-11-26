---
layout: post
title: Dismantle SAP Production - How to determine workplace capacity
date: 2023-03-01 12:00:00 +0200
tags: sap
image: /assets/2023-12-09/title.png
read_more_links:
  - name: How to build a perfect RFC function module to use in Peakboard
    url: /SAP-How-to-build-a-perfect-RFC-function-module-to-be-used-in-Peakboard.html
  - name: Dismantle SAP Production - How to get the next work orders of a workplace by using COOIS transaction
    url: /Dismantle-SAP-Production-How-to-get-the-next-work-orders-of-a-workplace-by-using-COOIS-transaction-in-Peakboard.html
downloads:
  - name: Source Code for RFC - z_pb_get_workplace.txt
    url: /assets/2023-12-09/z_pb_get_workplace.txt
  - name: SAPWorkplaceCapacity.pbmx
    url: /assets/2023-12-09/SAPWorkplaceCapacity.pbmx
---

One of the top 5 use cases for Peakboard in production environments is showing the current orders, operations, and load of one or more workplaces.

To determine the load of a workplace, we need the capacity that is currently available for that workplace. The best way to get this is to have an SAP function module do all the work and just call it from the Peakboard application.

The problem is, SAP doesn't offer any standard RFC-enabled function modules to determine the capacity. But the good news is that there's an internal function module called `CR_CAPACITY_AVAILABLE`, which can determine the capacity.

This article shows how you can make this function module accessible externally and call it from a Peakboard application.

Please scroll to the bottom of this article to find a link to download the ABAP source code that is used here. Also, feel free to adjust the naming which is used in this article. If you need to align the naming to your company's conventions, that's no problem. The code is short and easy to understand.

One more note: The RFC function module built in this article is aligned with the [recommendations on how to build an ideal RFC to be used in Peakboard](/SAP-How-to-build-a-perfect-RFC-function-module-to-be-used-in-Peakboard.html).


## Setting up the requirements on the SAP side

For the exchange of the capacity information, we use a DDIC structure. This structure can be created by using transaction SE11.

![image](/assets/2023-12-09/010.png)

The structure only contains standard component types which are commonly used. After adding the description text and the components, we must not forget to activate the structure to make it available.

![image](/assets/2023-12-09/020.png)

The next step is to create the actual function module. It's very important to mark it as *Remote-Enabled*, as we need to call the function externally, later. 

The development package and function group are mandatory. Which one to use depends on the common habits of the SAP system. It's recommended to create a function group and package specifically for this and any future Peakboard development objects.

![image](/assets/2023-12-09/030.png)

This screenshot shows all the import parameters. To determine the workplace capacity, we need to know the start date, the end date, the workplace, and also the plant. These four parameters are all imports.

![image](/assets/2023-12-09/040.png)

The last step is to define a table. As you can see in the screenshot, the type of the table refers to the DDIC structure we created earlier.

![image](/assets/2023-12-09/050.png)

After all the above steps are done, we copy and paste the ABAP code into the source code editor:

![image](/assets/2023-12-09/060.png)

And finally, we save and activate the entire function module.

## How the code works

Now let's take a look at how the ABAP code actually works.

In the first part, the workplace name is translated into the capacity id by looking it up in the CRHD and KAKO tables.

![image](/assets/2023-12-09/070.png)

Then we call the actual function module `CR_CAPACITY_AVAILABLE`.

![image](/assets/2023-12-09/080.png)

The return values are translated into the elements of the return structure. Please note that the capacity is returned as seconds. We already convert the seconds into minutes before we return the values to the caller. Depending on the use case it might make sense to use hours here.

![image](/assets/2023-12-09/090.png)

Besides the capacity, the operating time, daily start time, and daily end time are calculated.

## Using the RFC function module in Peakboard

The RFC function's interface is perfectly designed to be used within Peakboard. Here's the XQL call for the RFC. We can see the four parameters we used when creating the RFC: Start and end date, workplace and plant.

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

In our sample application we can easily bind text boxes to the data source and also use a custom format to present the values to the end user (e.g. unit for the capacity, time format, etc.). Feel free to download the sample PBMX.

![image](/assets/2023-12-09/110.png)

## The code

Here's the entire code of `Z_PB_GET_WORKPLACE`. It's also available for download at the bottom of this page.

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
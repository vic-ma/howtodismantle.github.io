---
layout: post
title: The Great Data Tango - Mastering PowerBI and Peakboard Integration with Filters
date: 2023-09-07 00:00:00 +0200
tags: office365 
image: /assets/2024-09-07/title.png
read_more_links:
  - name: Best Practice - Use Power BI for integrating maps
    url: /best-practice-powerbi-for-map-integration.html
downloads:
  - name: CorporateDesignTemplate.pbmx
    url: /assets/2024-10-01/CorporateDesignTemplate.pbmx
---



![image](/assets/2024-09-07/010.png)




https://help.peakboard.com/controls/Extended/en-power-bi.html


Requirements for embedding Power BI artifacts
In order to embed Power BI reports and tiles in external apps, you first need a registered App in your Azure AD. To learn how to do this, please check the Peakboard documentation. Each step is explained there, especially how to get the client ID and the tenant ID which we will need later.

Besides this, you will need to provide user credentials for a user that has access to the workspace we created earlier. You need a real user beside the app registration. It’s doesn’t make much sense, but that’s just how Microsoft designed it, so please complain to them. Another unfortunate thing is that this process doesn’t support 2FA. So in case you have a 2FA policy in place, please deactivate it for this specific user and restrict their permissions as much as possible, to maintain an acceptable level of security.



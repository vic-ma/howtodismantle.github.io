---
layout: post
title: Peakboard UI Hacks - No Match, No Entry - Regex Validation Demystified
date: 2023-03-01 05:00:00 +0300
tags: ui bestpractice
image: /assets/2025-10-02/title.png
image_header: /assets/2025-10-02/title_landscape.png
bg_alternative: true
read_more_links:
  - name: UI - User Interface
    url: /category/ui
downloads:
  - name: TextInputCheck.pbmx
    url: /assets/2025-10-02/TextInputCheck.pbmx
---
Many Peakboard applications let the user give text input. However, accepting text input isn't as simple as adding a label control and calling it a day. You should expect users to occasionally give bad inputs---inputs that don't match the format that your app expects.

So, it's important to validate the format of the user's input. Luckily, you can easily do this by using [*regular expressions*](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Guide/Regular_expressions), also known as *regex*. Regex provides a simple, efficient, and standardized way for checking if a string matches a specified pattern. And in this article, we'll explain how regex works, and how to use it in Peakboard, to validate user input.

## What is Regex?

Simply spoken, a regex is a single string that contains the rule another string (the payload) is checked against. For example, let's take the regex `^.{0,6}$`. It defines the rule that the payload's length must not exceed a maximum length of six characters. So the payload "Hello" is aligned with the rule `^.{0,6}$`, but the payload "Hello world" is not. How these regex strings or rules are defined we will learn in the next paragraph.

## Crash course regex

Every regex starts with the character `^` and ends with character `$`. So for example `^abc$` only matches if the text is exactly "abc".

The brackets `[]` are used to define what is allowed:

* `[0-9]` → a digit between 0 and 9
* `[a-z]` → lowercase letters
* `[A-Za-z0-9]` → letters and digits
* `\d` → any single digit
* `[^...]` → NOT these characters

The brackets `{}` are used to define the allowed length:

* `{n}` → exactly n times
* `{n,}` → at least n times
* `{n,m}` → between n and m times (inclusive)

So let's take the typical serial numbers of Peakboard boxes. They usually look like "PB1020", so they start with "PB" and then are followed by a 4–5 digit string. The correct regex would be `^PB\d{4,5}$`:

* `^` → start of the string
* `PB` → the literal characters PB (fixed)
* `\d{4,5}` → 4 or 5 digits (0–9)
* `$` → end of the string

Things can get really complicated when the requirements go up, e.g. `^(?=.*[A-Za-z])(?=.*\d).{8,}$` is a password with at least one letter, one number, and a minimum of eight characters.

## Regex in Peakboard applications

The text boxes in Peakboard offer a validation attribute that can be switched on. It comes along with the actual Regex expression to be checked against and also a dedicated border color. As long as the user entry doesn't match the regex, the border is automatically set to the color.

![image](/assets/2025-10-02/010.png)

Besides the changing border color we can use building blocks in our processing procedure to check for validity and react accordingly if the user entry doesn't match the requirement. We can just use the "IsValid" property and in case it's not valid inform the user (e.g. by shaking the text box) or start the processing in case the entry is ok.

![image](/assets/2025-10-02/020.png)

## Result

The animation shows the check in action for the regex of the Peakboard serial numbers `PBXXXX` with `XXXX` being 4–5 digits. So the regex is `^PB\d{4,5}$`. First we try to submit a wrong entry; there are two digits missing. After the number of digits is correct, the entry is accepted.

![image](/assets/2025-10-02/result.gif)

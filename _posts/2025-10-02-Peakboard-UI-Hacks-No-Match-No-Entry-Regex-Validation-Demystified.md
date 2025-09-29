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
Many Peakboard apps have text boxes that let the user submit some text. However, accepting text input isn't as simple as adding a text box control and calling it a day. You should expect users to occasionally give bad inputs---text that doesn't match the format that your app expects.

So, it's important to validate the user input. An easy way to do this is by using [*regular expressions*](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Guide/Regular_expressions), also known as *regex*. Regex provides a simple, efficient, and standardized way for checking if some text matches a specified pattern. In this article, we'll explain how regex works, and how to use it in Peakboard, to validate user input.

## What is Regex?

A regular expression (regex) is a string that describes a text pattern. For example, 
consider this regex: `^.{0,6}$`. It describes a string that has between 0 and 6 characters. So the string `Hello` is matches the regex, but the string `Hello world` does not.

Now, let's go over how the syntax for regexes work.

## How to create a regex

Every regex starts with the character `^` and ends with character `$`. So for example, the regex `^abc$` only matches the string *abc*.

You can use brackets `[]` to allow different characters:

| Component | Description |
| --- | --- |
| `[0-9]` | Matches a single digit from 0 to 9. |
| `[a-z]` | Matches a single lowercase letter. |
| `[A-Za-z0-9]` | Matches a single letter or a digit. |
| `\d` | Matches any single digit. |
| `[^...]` | Matches any single character NOT in the specified set. (Replace `...` with the characters you want to exclude.) |

You can use curly braces `{}` to require the previous character or group to be repeated a specific number of times (e.g. `a{1-2}` or `[a-z]{1-2}`):

| Component | Description |
|---|---|
| `{n}` | Exactly `n` times. |
| `{n,}` | At least `n` times. |
| `{n,m}` | Between `n` and `m` times (inclusive). |

### An example

Let's try to create a regex for the standard serial numbers of Peakboard Boxes. They usually look something like, `PB1020`. So they start with `PB` and are followed by a 4–5 digit number. The correct regex for this format is this:
```
^PB\d{4,5}$
```

Here's an explanation for how it works:

| Component | Explanation |
| --- | --- |
| `^` | Asserts the start of the string. |
| `PB` | Matches the literal characters "PB". |
| `\d{4,5}` | Matches a sequence of 4 or 5 digits. |
| `$` | Asserts the end of the string. |


## Easier ways to create regexes

Regexes can get really complicated when the requirements go up. For example, here's a regex for a password that contains at least one letter, one number, and a minimum of eight characters:
```
^(?=.*[A-Za-z])(?=.*\d).{8,}$
```

Here are some tools that can make the process of creating regexes easier:
* [regex101](https://regex101.com/) lets you enter a regex and a string and see if the string matches the regex.
* [Regex Generator](https://regex-generator.olafneumann.org/?sampleText=PB1234&flags=i) lets you enter a sample string that you want to match (like `PB1234` for a Peakboard Box serial number). Then, you select the appropriate colored blocks to build the regex, bit by bit.

You can also ask an AI chatbot, like ChatGPT, to generate a regex for you. Just give it a sample text that you want to match, and a plain-English description of what the pattern is. But make sure to verify the regex it gives you. 

![image](/assets/2025-10-02/chatgpt.png)

## Regex in Peakboard applications

Now, let's look at how we can use regex in Peakboard.

The [text box control](https://help.peakboard.com/controls/Input/en-textbox.html) has a data validation option. If you switch this on, you can enter a regex pattern. If the user enters some text that doesn't match the regex, then the border of the text box will change color.

![image](/assets/2025-10-02/010.png)

But what if we want to reject any input that does not match our regex? We can do this with Building Blocks. We get the `IsValid` property of the text box. This returns whether or not the input matches the regex. If the input is invalid, then we can do something like shake the text box, to alert the user. If the input is valid, then we process it as usual.

![image](/assets/2025-10-02/020.png)

## Result

This video shows what a Peakboard app that uses regex to validate user input might look like. The text box accepts a Peakboard Box serial number. So the regex is `^PB\d{4,5}$`. We submit an invalid entry with two missing digits, and the text box shakes. Then, we submit a correct entry, and the entry is accepted.

![image](/assets/2025-10-02/result.gif)

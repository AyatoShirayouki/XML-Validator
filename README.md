# XML Validator Console Application

## Overview

This project is a console application that validates XML strings based on the following rules:
1. Each starting element must have a corresponding ending element.
2. Elements must be well-nested, which means that the element that starts first must end last.

The application outputs `Valid` if the given XML is valid, otherwise `Invalid`. The XML string is passed as the first command-line argument. This project is implemented in C# without using any `System.XML` classes, other XML libraries, or regular expressions.

## Project Structure

### Main Console Application

#### Program.cs

```csharp
using SimpleXMLValidatorLibrary;

class Program
{
    static void Main(string[] args)
    {
#if DEBUG
        // You can use here to test, feel free to modify/add the test cases here.
        // You can also use other ways to test if you want.

        List<(string testCase, bool expectedResult)> testCases = new()
        {
            ("<Design><Code>hello world</Code></Design>",  true),//normal case
            ("<Design><Code>hello world</Code></Design><People>", false),//no closing tag for "People" 
            ("<People><Design><Code>hello world</People></Code></Design>", false),// "/Code" should come before "/People" 
            ("<People age=”1”>hello world</People>", false),//there is no closing tag for "People age=”1”" and no opening tag for "/People"

            // Additional valid cases
            ("<Book><Title>XML Guide</Title><Author>John Doe</Author></Book>", true), // nested valid XML
            ("<Library><Book><Title>XML Guide</Title></Book><Book><Title>Another Book</Title></Book></Library>", true), // multiple nested elements
            ("<note><to>Tove</to><from>Jani</from><heading>Reminder</heading><body>Don't forget me this weekend!</body></note>", true), // typical XML structure
            ("<outer><inner><deep>Deep Value</deep></inner></outer>", true), // deeply nested elements
            ("<a><b><c><d></d></c></b></a>", true), // deeply nested single characters

            // Additional invalid cases
            ("<Book><Title>XML Guide</Title><Author>John Doe</Book>", false), // mismatched closing tag for Author
            ("<Library><Book><Title>XML Guide</Title><Book><Title>Another Book</Title></Library>", false), // missing closing tag for inner Book
            ("<note><to>Tove></to><from>Jani<heading>Reminder</heading><body>Don't forget me this weekend!</body></note>", false), // missing closing tag for from
            ("<outer><inner><deep>Deep Value</inner></deep></outer>", false), // incorrect nesting
            ("<a><b><c></d></c></b></a>", false), // incorrect closing tag order
            ("<root><nested>Text</nested>", false), // missing closing tag for root
            ("<unclosed><nested>Text</nested>", false), // unclosed root tag
            ("<a><b><c><d></c></d></b></a>", false), // mismatched nesting
            ("<root><self-closing/></root>", false), // self-closing tags not allowed

            // Edge cases
            ("", true), // empty string is technically valid (no elements)
            ("<single></single>", true), // single element with proper closing
            ("<a></a><b></b><c></c>", true), // multiple root level elements
            ("<root><a><b></a></b></root>", false), // incorrect nesting
            ("<root></root", false), // missing closing angle bracket
        };
        int failedCount = 0;
        foreach ((string input, bool expected) in testCases)
        {
            bool result = SimpleXmlValidator.DetermineXml(input);
            string resultStr = result ? "Valid" : "Invalid";

            string mark;
            if (result == expected)
            {
                mark = "OK ";
            }
            else
            {
                mark = "NG ";
                failedCount++;
            }
            Console.WriteLine($"{mark} {input}: {resultStr}");
        }
        Console.WriteLine($"Result: {testCases.Count - failedCount}/{testCases.Count}");
#else
        string input = args.FirstOrDefault("");
        bool result = SimpleXmlValidator.DetermineXml(input);
        Console.WriteLine(result ? "Valid" : "Invalid");
#endif
    }
}
```

### XML Validator Library

#### SimpleXmlValidator.cs

```csharp
using System;
using System.Collections.Generic;

namespace SimpleXMLValidatorLibrary
{
    public class SimpleXmlValidator
    {
        public static bool DetermineXml(string xml)
        {
            // Stack to keep track of opened tags
            Stack<string> tags = new Stack<string>();
            // Index to iterate through the XML string
            int i = 0;

            // Iterate through the entire XML string
            while (i < xml.Length)
            {
                // Check if the current character is an opening bracket '<'
                if (xml[i] == '<')
                {
                    // Find the index of the corresponding closing bracket '>'
                    int closeIndex = xml.IndexOf('>', i);
                    // If there is no closing bracket, the XML is invalid
                    if (closeIndex == -1)
                    {
                        return false;
                    }

                    // Extract the tag name between '<' and '>'
                    string tag = xml.Substring(i + 1, closeIndex - i - 1);

                    // Check if it is a closing tag
                    if (tag.StartsWith("/"))
                    {
                        // If there are no tags in the stack, the XML is invalid
                        if (tags.Count == 0)
                        {
                            return false;
                        }

                        // Pop the last opened tag from the stack
                        string lastTag = tags.Pop();
                        // Check if the closing tag matches the last opened tag
                        if (lastTag != tag.Substring(1))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        // Check if the tag contains a space (indicating attributes), which are not allowed
                        if (tag.Contains(" "))
                        {
                            return false;
                        }
                        // Push the opening tag onto the stack
                        tags.Push(tag);
                    }

                    // Move the index to the character after the closing bracket '>'
                    i = closeIndex + 1;
                }
                else
                {
                    // Move to the next character if not a tag
                    i++;
                }
            }

            // If the stack is empty, all tags are properly closed and nested; otherwise, the XML is invalid
            return tags.Count == 0;
        }
    }
}
```

## Usage

### Building the Application

1. Clone the repository.
2. Open the solution in Visual Studio.
3. Build the solution to generate the executable.

### Running in Debug Mode

In debug mode, the application runs predefined test cases to validate the implementation.

## Test Cases

### Valid Test Cases

1. `<Design><Code>hello world</Code></Design>`
2. `<Book><Title>XML Guide</Title><Author>John Doe</Author></Book>`
3. `<Library><Book><Title>XML Guide</Title></Book><Book><Title>Another Book</Title></Book></Library>`
4. `<note><to>Tove</to><from>Jani</from><heading>Reminder</heading><body>Don't forget me this weekend!</body></note>`
5. `<outer><inner><deep>Deep Value</deep></inner></outer>`
6. `<a><b><c><d></d></c></b></a>`
7. `""` (empty string)
8. `<single></single>`
9. `<a></a><b></b><c></c>`

### Invalid Test Cases

1. `<Design><Code>hello world</Code></Design><People>`
2. `<People><Design><Code>hello world</People></Code></Design>`
3. `<People age=”1”>hello world</People>`
4. `<Book><Title>XML Guide</Title><Author>John Doe</Book>`
5. `<Library><Book><Title>XML Guide</Title><Book><Title>Another Book</Title></Library>`
6. `<note><to>Tove></to><from>Jani<heading>Reminder</heading><body>Don't forget me this weekend!</body></note>`
7. `<outer><inner><deep>Deep Value</inner></deep></outer>`
8. `<a><b><c></d></c></b></a>`
9. `<root><nested>Text</nested>`
10. `<unclosed><nested>Text</nested>`
11. `<a><b><c><d></c></d></b></a>`
12. `<root><self-closing/></root>`
13. `<root><a><b></a></b></root>`
14. `<root></root`

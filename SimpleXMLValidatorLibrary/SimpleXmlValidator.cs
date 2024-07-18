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

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
            ("<note><to>Tove</to><from>Jani<heading>Reminder</heading><body>Don't forget me this weekend!</body></note>", false), // missing closing tag for from
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
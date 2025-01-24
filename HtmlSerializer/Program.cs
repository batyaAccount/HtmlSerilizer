using HtmlSerializer;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

var html = await Load("https://hebrewbooks.org/");

var cleanHtml = new Regex("\\t|\\r|\\n").Replace(html, " ");
var lines = Regex.Matches(cleanHtml, @"<\/?([A-Za-z][A-Za-z0-9]*)\b[^>]*>|([^<]+)");
List<string> htmlLines = new List<string>();
foreach (var line in lines)
{
    string l = line.ToString();
    l = l.Replace('<', ' ');
    l = l.Replace('>', ' ');
    l = l.Trim();
    if (!string.IsNullOrWhiteSpace(l))
        htmlLines.Add(l);
}



HtmlElement htmlElement = CreateRoot(htmlLines);
Selector s = Selector.CastingToSelector("input#__VIEWSTATEGENERATOR");

HashSet<HtmlElement> h = htmlElement.FindSelector(s);

foreach (var item in h)
{
    Console.Write($"Tag = {item.Name}, Id = {item.Id}, Classes =  ");
    foreach (var child in item.Classes)
    {
        Console.Write(child + ", ");
    }
    Console.WriteLine();
}
//The function get list of elements in the tree and return the tree in htmlElement type
static HtmlElement CreateRoot(List<string> htmlLines)
{
    HtmlElement htmlElement = new HtmlElement();

    HtmlElement currentHtmlElement = htmlElement;
    HtmlHelper htmlHelper = HtmlHelper.Instance;
    for (int i = 0; i < htmlLines.Count; i++)
    {
        string line = htmlLines[i];
        string element = line.Contains(' ') ? line.Substring(0, line.IndexOf(' ')) : line;

        HtmlElement tmp;
        if (element == "/html")
            return htmlElement;
        if (element.Length >1 && element[0] == '/' && element[0] != '/')
        {
            currentHtmlElement = currentHtmlElement.Parent;
        }
        else
        {
            if (htmlHelper.HtmlTags.Contains(element) || htmlHelper.HtmlVoidTags.Contains(element))
            {
                tmp = new HtmlElement()
                {
                    Name = element,
                    Parent = currentHtmlElement
                };
                currentHtmlElement.Children.Add(tmp);
                currentHtmlElement = tmp;
                List<string> attributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(line).Cast<Match>().Select(m => m.Value).ToList();
                foreach (var attribute in attributes)
                {
                    string part = attribute.Substring(0, attribute.IndexOf("="));
                    if (part == "id")
                    {
                        currentHtmlElement.Id = attribute.Substring(attribute.IndexOf('=') + 1).Trim('"', '\''); ;
                        currentHtmlElement.Attributes.Add("id");
                    }
                    else if (part == "class")
                    {
                        string cls = attribute.Substring(attribute.IndexOf("=") + 1).Trim('"', '\''); ;
                        currentHtmlElement.Classes = cls.Split(" ").ToList();
                        currentHtmlElement.Attributes.Add("class");
                     
                    }

                }
                if (htmlHelper.HtmlVoidTags.Contains(element))
                    currentHtmlElement = currentHtmlElement.Parent;
            }
            else
            {
                currentHtmlElement.InnerHtml = line;
            }
        }
    }
    return htmlElement.Children.FirstOrDefault().Children.FirstOrDefault();
}
async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}

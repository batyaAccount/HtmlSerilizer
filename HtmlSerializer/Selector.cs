using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HtmlSerializer
{
    internal class Selector
    {

        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; }
        public Selector Parent { get; set; }
        public Selector Child { get; set; }
        public Selector()
        {
            Classes = new List<string>();
        }
        //This function get selector by string and return the selector by Selector type
        public static Selector CastingToSelector(string s)
        {
            List<string> selectors = s.Split(' ').ToList();
            Selector firstSelector = new Selector();
            Selector currentSelector = firstSelector;
            bool isValid = true;
            HtmlHelper htmlHelper = HtmlHelper.Instance;
            foreach (string selector in selectors)
            {
                Selector selector1 = new Selector();
                List<string> parts = Regex.Matches(selector, @"[^\s.#]+|[.#][^\s.#]+").Cast<Match>().Select(m => m.Value).ToList();
                foreach (string part in parts)
                {
                    if (part[0] == '#')
                        selector1.Id = part.Substring(1);
                    else if (part[0] == '.')
                        selector1.Classes.Add(part.Substring(1));
                    else
                    {
                        if (htmlHelper.HtmlTags.Contains(part) || htmlHelper.HtmlVoidTags.Contains(part))
                            selector1.TagName = part;
                        else
                            isValid = false;

                    }
                }
                if (isValid)
                {
                    currentSelector.Child = selector1;
                    selector1.Parent = currentSelector;
                    currentSelector = currentSelector.Child;
                }

            }
            return firstSelector.Child;
        }
    }
}

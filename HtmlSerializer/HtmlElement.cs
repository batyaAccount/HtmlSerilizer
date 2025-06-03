using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlSerializer
{
    public class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Attributes { get; set; }
        public List<string> Classes { get; set; }
        public string InnerHtml { get; set; }
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; }

        public HtmlElement(string id, string name, List<string> attributes, List<string> classes, string innerHtml, HtmlElement parent, List<HtmlElement> children)
        {
            Id = id;
            Name = name;
            Attributes = attributes;
            Classes = classes;
            InnerHtml = innerHtml;
            Parent = parent;
            Children = children;
        }

        public HtmlElement()
        {
            Children = new List<HtmlElement>();
            Classes = new List<string>();
            Attributes = new List<string>();

        }
        //Get element and return all the descendants by list
        public IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> q = new Queue<HtmlElement>();
            q.Enqueue(this);
            while (q.Count > 0)
            {
                HtmlElement el = q.Dequeue();
                foreach (HtmlElement child in el.Children)
                {
                    q.Enqueue(child);
                }
                yield return el;

            }
        }

        //Get element and return all the ancestors by list
        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement htmlElement = this;
            while (htmlElement != null)
            {
                yield return htmlElement.Parent;
                htmlElement = htmlElement.Parent;
            }
        }
        //The function enter to the HashSet all the elements that meet the requirement of the selector
        public void RecoursionTree(Selector s, HtmlElement htmlElement, HashSet<HtmlElement> htmlElements)
        {
            if (s == null || htmlElement == null)
                return;
            HashSet<HtmlElement> children = htmlElement.Descendants().ToHashSet();
            if (s.Child == null)
            {
                foreach (HtmlElement child in children)
                {
                    if (child != null && (s.Id == null || s.Id == child.Id) && (s.TagName == null || s.TagName == child.Name) && (s.Classes.Count == 0 || s.Classes.Any(sClass => child.Classes.Contains(sClass))))
                    {
                        htmlElements.Add(child);
                    }
                }
            }

            foreach (HtmlElement child in children)
            {
                if (child != null && (s.Id == null || s.Id == child.Id) && (s.TagName == null || s.TagName == child.Name) && (s.Classes.Count == 0 || s.Classes.Any(sClass => child.Classes.Contains(sClass))))
                {
                    RecoursionTree(s.Child, child, htmlElements);
                }

            }

        }
        public HashSet<HtmlElement> FindSelector(Selector s)
        {
            HashSet<HtmlElement> children = new HashSet<HtmlElement>();

            RecoursionTree(s, this, children);
            return children;
        }
    }
}

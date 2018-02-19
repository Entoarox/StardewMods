using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Microsoft.Xna.Framework;

namespace Entoarox.CustomBooks
{
    public class Bookshelf
    {
        public class Book
        {
            public enum PageType
            {
                Title,
                Text,
                Image
            }
            public class Page
            {
                public PageType Type;
                public string Content;

                public CustomBooks.Page Deserialize(string book)
                {
                    switch(this.Type)
                    {
                        case PageType.Title:
                            return new TitlePage(JsonConvert.DeserializeObject<string[]>(this.Content));
                        case PageType.Text:
                            return new TextPage(this.Content);
                        case PageType.Image:
                            return new ImagePage(book, this.Content);
                    }
                    throw new InvalidOperationException("Unknown page type, cannot deserialize!");
                }
            }
            public List<Page> Pages;
            public string Name;
            public bool Editable = true;
            public Color Color;
            private string Id;

            internal Book(string id)
            {
                this.Id = id;
                this.Name = "(unnamed book)";
                this.Pages = new List<Page>();
                this.Color = new Color(139, 69, 19);
            }
            public Book()
            {

            }

            public List<CustomBooks.Page> GetPages()
            {
                return this.Pages.Select(a => a.Deserialize(this.Id)).ToList();
            }
            public void SetPages(List<CustomBooks.Page> pages)
            {
                this.Pages = pages.Select(a => a.Serialize()).ToList();
            }
        }
        public Dictionary<string, Book> Books = new Dictionary<string, Book>();
        public string CreateBook()
        {
            string id = Guid.NewGuid().ToString();
            this.Books.Add(id, new Book(id));
            return id;
        }
    }
}

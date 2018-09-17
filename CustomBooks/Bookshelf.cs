using System;
using System.Collections.Generic;
using System.Linq;
using Entoarox.CustomBooks.Menu;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Entoarox.CustomBooks
{
    internal class Bookshelf
    {
        /*********
        ** Accessors
        *********/
        public Dictionary<string, Book> Books = new Dictionary<string, Book>();


        /*********
        ** Public methods
        *********/
        public string CreateBook()
        {
            string id = Guid.NewGuid().ToString();
            this.Books.Add(id, new Book(id));
            return id;
        }

        public class Book
        {
            /*********
            ** Fields
            *********/
            private readonly string Id;


            /*********
            ** Accessors
            *********/
            public List<Page> Pages;
            public string Name;
            public Color Color;

            public enum PageType
            {
                Title,
                Text,
                Image
            }


            /*********
            ** Public methods
            *********/
            public Book(string id)
            {
                this.Id = id;
                this.Name = "(unnamed book)";
                this.Pages = new List<Page>();
                this.Color = new Color(139, 69, 19);
            }

            public Book()
            {
            }

            public List<Menu.Page> GetPages()
            {
                return this.Pages.Select(a => a.Deserialize(this.Id)).ToList();
            }

            public void SetPages(List<Menu.Page> pages)
            {
                this.Pages = pages.Select(a => a.Serialize()).ToList();
            }

            public class Page
            {
                public PageType Type;
                public string Content;

                public Menu.Page Deserialize(string book)
                {
                    switch (this.Type)
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
        }
    }
}

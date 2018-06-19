using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

[ProtoContract]
public class Book
{
    [ProtoMember(1)]
    public string author;
    [ProtoMember(2)]
    public List<Fable> stories;
    [ProtoMember(3)]
    public DateTime edition;
    [ProtoMember(4)]
    public int pages;
    [ProtoMember(5)]
    public double price;
    [ProtoMember(6)]
    public bool isEbook;

    public override string ToString()
    {
        StringBuilder s = new StringBuilder();
        s.Append("by "); s.Append(author);
        s.Append(", edition "); s.Append(edition.ToString("dd MM yyyy"));
        s.Append(", pages "); s.Append(pages);
        s.Append(", price "); s.Append(price);
        s.Append(", isEbook"); s.Append(isEbook);
        s.AppendLine();
        if (stories != null) foreach (Fable lFable in stories)
            {
                s.Append("title "); s.Append(lFable.title);
                s.Append(", rating"); s.Append(lFable.customerRatings.Average());
                s.AppendLine();
            }

        return s.ToString();
    }
}

[ProtoContract]
public class Fable
{
    [ProtoMember(1)]
    public string title;
    [ProtoMember(2)]
    public double[] customerRatings;
}

public static class methods
{
    public static Book GetData()
    {
        return new Book
        {
            author = "Aesop",
            price = 1.99,
            isEbook = false,
            edition = new DateTime(1975, 03, 13),
            pages = 203,
            stories = new List<Fable>(new Fable[]
            {
            new Fable { title = "The Fox & the Grapes", customerRatings = new double[] {0.7, 0.7, 0.8}},
            new Fable { title = "The Goose that Laid the Golden Eggs", customerRatings = new double[] {0.6, 0.75, 0.5, 1.0}},
            new Fable { title = "The Cat & the Mice", customerRatings = new double[]{0.1, 0.0, 0.3} },
            new Fable { title = "The Mischievous Dog", customerRatings = new double[] {0.45, 0.5, 0.4, 0.0, 0.5}}
            })
        };
    }
}


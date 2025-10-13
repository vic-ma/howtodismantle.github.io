using Peakboard.ExtensionKit;
using System;
using System.Collections.Generic;

namespace MeowExtension
{
    [CustomListIcon("Meow.MeowExtension.png")]
    public class CatCustomList : CustomListBase
    {
        protected override CustomListDefinition GetDefinitionOverride()
        {
            return new CustomListDefinition
            {
                ID = "CatCustomList",
                Name = "Cat List",
                Description = "A custom list for cats with breed and age information",
            };
        }

        protected override CustomListColumnCollection GetColumnsOverride(CustomListData data)
        {
            var columns = new CustomListColumnCollection();
            columns.Add(new CustomListColumn("ID", CustomListColumnTypes.Number));
            columns.Add(new CustomListColumn("Name", CustomListColumnTypes.String));
            columns.Add(new CustomListColumn("Breed", CustomListColumnTypes.String));
            columns.Add(new CustomListColumn("Age", CustomListColumnTypes.Number));
            columns.Add(new CustomListColumn("ImageUrl", CustomListColumnTypes.String));
            return columns;
        }

        protected override CustomListObjectElementCollection GetItemsOverride(CustomListData data)
        {
            var items = new CustomListObjectElementCollection();
            
            // Add cat data directly to items
            var item1 = new CustomListObjectElement();
            item1.Add("ID", 1);
            item1.Add("Name", "Fluffy1");
            item1.Add("Breed", "Persian");
            item1.Add("Age", 3);
            item1.Add("ImageUrl", "https://example.com/cat1.jpg");
            items.Add(item1);
            
            var item2 = new CustomListObjectElement();
            item2.Add("ID", 2);
            item2.Add("Name", "Whiskers");
            item2.Add("Breed", "Maine Coon");
            item2.Add("Age", 5);
            item2.Add("ImageUrl", "https://example.com/cat2.jpg");
            items.Add(item2);
            
            var item3 = new CustomListObjectElement();
            item3.Add("ID", 3);
            item3.Add("Name", "Shadow");
            item3.Add("Breed", "British Shorthair");
            item3.Add("Age", 2);
            item3.Add("ImageUrl", "https://example.com/cat3.jpg");
            items.Add(item3);
            
            var item4 = new CustomListObjectElement();
            item4.Add("ID", 4);
            item4.Add("Name", "Luna");
            item4.Add("Breed", "Siamese");
            item4.Add("Age", 4);
            item4.Add("ImageUrl", "https://example.com/cat4.jpg");
            items.Add(item4);
            
            var item5 = new CustomListObjectElement();
            item5.Add("ID", 5);
            item5.Add("Name", "Tiger");
            item5.Add("Breed", "Persian");
            item5.Add("Age", 7);
            item5.Add("ImageUrl", "https://example.com/cat5.jpg");
            items.Add(item5);
            
            var item6 = new CustomListObjectElement();
            item6.Add("ID", 6);
            item6.Add("Name", "Mittens");
            item6.Add("Breed", "Ragdoll");
            item6.Add("Age", 1);
            item6.Add("ImageUrl", "https://example.com/cat6.jpg");
            items.Add(item6);
            
            return items;
        }
    }
}

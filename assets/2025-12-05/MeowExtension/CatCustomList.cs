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
                PropertyInputPossible = true,
                PropertyInputDefaults =
                {
                    new CustomListPropertyDefinition { Name = "Breed", Value = "Bengal", TypeDefinition = TypeDefinition.String.With(selectableValues: [ "Abyssinian", "Bengal", "British Shorthair" ] )},
                    new CustomListPropertyDefinition { Name = "IsItACat", Value = "True", TypeDefinition = TypeDefinition.Boolean },
                    new CustomListPropertyDefinition { Name = "Age", Value = "4", TypeDefinition = TypeDefinition.Number },
                    new CustomListPropertyDefinition { Name = "MaximumOfSomething", Value = "5", TypeDefinition = TypeDefinition.Number.With(selectableValues: [ 2, 3, 5, 10, 20, 50, 100]) },
                    new CustomListPropertyDefinition { Name = "MySecretCode", Value = "18899", TypeDefinition = TypeDefinition.String.With(masked: true) },
                    new CustomListPropertyDefinition { Name = "MultilineDescription", Value = "bla\nbla\nbla", TypeDefinition = TypeDefinition.String.With(multiLine: true) }
                }
            };
        }

        protected override void CheckDataOverride(CustomListData data)
        {
            base.CheckDataOverride(data);
        }

        protected override bool IsPropertyInputValidOverride(CustomListData data, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (data.Properties["Breed"] is not string breed || string.IsNullOrWhiteSpace(breed))
            {
                errorMessage = "Breed must be a non-empty string.";
                return false;
            }

            if (data.Properties["IsItACat"] is not bool)
            {
                errorMessage = "IsItACat must be a boolean value.";
                return false;
            }

            if (data.Properties["Age"] is not int age || age < 0)
            {
                errorMessage = "Age must be a non-negative integer.";
                return false;
            }

            if (data.Properties["MaximumOfSomething"] is not int max || max <= 0)
            {
                errorMessage = "MaximumOfSomething must be a positive integer.";
                return false;
            }

            if (data.Properties["MySecretCode"] is not string code || string.IsNullOrWhiteSpace(code))
            {
                errorMessage = "MySecretCode must be a non-empty string.";
                return false;
            }

            if (data.Properties["MultilineDescription"] is not string description || string.IsNullOrWhiteSpace(description))
            {
                errorMessage = "MultilineDescription must be a non-empty string.";
                return false;
            }

            return true;
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

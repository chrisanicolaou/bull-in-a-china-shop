using System.Collections.Generic;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Stock;
using CharaGaming.BullInAChinaShop.UI.Utils;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.Day
{
    public class ShopperReview
    {
        private static List<string> _unhappyImpatientPrompts = new List<string>
        {
            "I just couldn't wait any longer!",
            "I felt like I was queueing for days!",
            "Just can't get the staff nowadays...",
            "I couldn't wait forever - I had places to be!",
            "I couldn't stand around any longer.",
            "Service took FOREVER!",
            "How hard is it to get a bowl nowadays?",
            "How hard is it to get a plate nowadays?",
            "How hard is it to get a jug nowadays?",
            "How hard is it to get a teacup nowadays?",
            "I literally aged waiting in that shop",
            "China shop? More like - Bad Service shop!",
            "Too slow. Gotta go fast.",
        };

        private static List<string> _unhappyNoStockPrompts = new List<string>
        {
            "Looks like grandma won't be getting her {0}...",
            "What a shame - I really liked that {0}.",
            "Eh - that {0} was ugly anyway.",
            "What china shop doesn't have a {0}?!",
            "There was no {0}!",
            "Seriously? Not a single {0}?",
            "I really wanted a {0}!",
        };
        
        private static List<string> _happyPrompts = new List<string>
        {
            "I'm so happy with my new {0}!",
            "Wow, what a shiny {0}.",
            "I got a brilliant, new {0}!",
            "Best china shop in town!",
            "I know where I'm going for my next {0}!",
            "I'm thrilled with my new {0}!",
            "Fantastic service - will be coming again!",
            "I couldn't ask for a better {0}!",
            "This shop cheered me right up!",
            "Wow, what fantastic service!",
            "I can't believe the amazing quality of my {0}!",
            "This is the best shop EVER!",
            "I. Love. This. Shop.",
            "The best darn {0} I ever did see!",
            "My day - nay, my year - has been made.",
            "Thank you so much for my new {0}!"
        };
        
        private static List<string> _shopperSpecificUnhappyPrompts = new List<string>
        {
            "I'm one angry {0}!",
            "{0} is not happy.",
            "I wont be telling my friend, {0}, about this place!"
        };
        
        private static List<string> _shopperSpecificHappyPrompts = new List<string>
        {
            "I'm one happy {0}!",
            "I'll be telling my friend, {0}, about this place!"
        };
        
        public string ShopperName { get; private set; }
        public string ReviewSpriteFilePath => $"ReviewIcons/{ShopperName}";
        
        public ReviewType Type { get; set; }
        
        public BaseStock RequestedStock { get; set; }
        
        public string ReviewText { get; private set; }

        public ShopperReview(string shopperName)
        {
            ShopperName = shopperName;
        }

        public ShopperReview(string shopperName, ReviewType type, BaseStock stockType)
        {
            ShopperName = shopperName;
            Type = type;
            RequestedStock = stockType;
        }

        public void GenerateReviewText()
        {
            var isShopperSpecific = Random.Range(0, 100) <= 10;
            
            if (Type == ReviewType.Happy)
            {
                var randomPrompts = isShopperSpecific ? _shopperSpecificHappyPrompts : _happyPrompts;
                ReviewText = randomPrompts[Random.Range(0, randomPrompts.Count)];
                if (ReviewText.Contains("{0}"))
                {
                    ReviewText = string.Format(ReviewText, isShopperSpecific ? ShopperName : RequestedStock.Name);
                }

                return;
            }

            var prompts = RequestedStock == null ? _unhappyImpatientPrompts : _unhappyNoStockPrompts;
            prompts = isShopperSpecific ? _shopperSpecificUnhappyPrompts : prompts;
            
            ReviewText = prompts[Random.Range(0, prompts.Count)];
            if (ReviewText.Contains("{0}"))
            {
                ReviewText = string.Format(ReviewText, isShopperSpecific ? ShopperName.ToTMProColor(Color.yellow) : RequestedStock!.Name.ToTMProColor(Color.yellow));
            }
        }
    }
}
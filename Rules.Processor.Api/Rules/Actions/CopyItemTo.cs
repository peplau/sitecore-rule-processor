using Sitecore.Data;
using Sitecore.Rules;
using Sitecore.Rules.Actions;
using Sitecore.SecurityModel;

namespace Rules.Processor.Api.Rules.Actions
{
    class CopyItemTo<T> : RuleAction<T> where T : RuleContext
    {
        public string Copy { get; set; }
        public string ItemId { get; set; }

        public override void Apply(T ruleContext)
        {
            // Scape in case of empty params
            if (string.IsNullOrEmpty(ItemId) || string.IsNullOrEmpty(Copy))
                return;

            // Scape if target item does not exists
            var item = ruleContext.Item;
            var targetItem = item.Database.GetItem(new ID(ItemId));
            if (targetItem == null)
                return;

            // Copy item to target
            using (new SecurityDisabler())
            {
                switch (Copy.ToLower())
                {
                    case "clone":
                        item.CloneTo(targetItem);
                        break;
                    case "copy":
                        item.CopyTo(targetItem, item.Name);
                        break;
                    case "move":
                        item.MoveTo(targetItem);
                        break;
                }
            }
        }
    }
}

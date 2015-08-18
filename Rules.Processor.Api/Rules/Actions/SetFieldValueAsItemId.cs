using Sitecore.Rules;
using Sitecore.Rules.Actions;
using Sitecore.SecurityModel;

namespace Rules.Processor.Api.Rules.Actions
{
    public class SetFieldValueAsItemId<T> : RuleAction<T> where T : RuleContext
    {
        public string Name { get; set; }
        public string ItemId { get; set; }

        public override void Apply(T ruleContext)
        {
            // Scape in case of empty params
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(ItemId))
                return;

            // Scape if field does not exists
            var item = ruleContext.Item;
            var field = item.Fields[Name];
            if (field == null)
                return;

            // Scape if value is the same
            if (field.Value == ItemId)
                return;

            // Save updated val
            using (new SecurityDisabler())
            {
                item.Editing.BeginEdit();
                item.Fields[Name].SetValue(ItemId, false);
                item.Editing.EndEdit();
            }
        }
    }
}

using System;
using Sitecore.Data;
using Sitecore.Rules;
using Sitecore.Rules.Actions;
using Sitecore.SecurityModel;

namespace Rules.Processor.Api.Rules.Actions
{
    class SetFieldValueAsItemFieldValue<T> : RuleAction<T> where T : RuleContext
    {
        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public string ItemId { get; set; }

        public override void Apply(T ruleContext)
        {
            // Scape in case of empty params
            if (string.IsNullOrEmpty(Name1) || string.IsNullOrEmpty(Name2) || string.IsNullOrEmpty(ItemId))
                return;

            // Scape if field does not exists
            var item = ruleContext.Item;
            var fieldTarget = item.Fields[Name1];
            if (fieldTarget == null)
                return;

            // Get source item
            var sourceItem = item.Database.GetItem(new ID(ItemId));
            if (sourceItem == null)
                return;

            // Get source value
            var fieldSource = sourceItem.Fields[Name2];
            if (fieldSource == null)
                return;
            var sourceVal = fieldSource.Value;
            var targetVal = fieldTarget.Value;

            // Scape if value is the same
            if (sourceVal == targetVal)
                return;

            // Save updated val
            using (new SecurityDisabler())
            {
                item.Editing.BeginEdit();
                item.Fields[Name1].SetValue(sourceVal, false);
                item.Editing.EndEdit();
            }
        }
    }
}

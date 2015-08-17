using Sitecore.Rules;
using Sitecore.Rules.Actions;
using Sitecore.SecurityModel;

namespace Rules.Processor.Api.Rules.Actions
{
    class SetFieldValue<T> : RuleAction<T> where T : RuleContext
    {
        public string Text { get; set; }
        public string Name { get; set; }

        public override void Apply(T ruleContext)
        {
            // Scape in case of empty params
            if (string.IsNullOrEmpty(Text) || string.IsNullOrEmpty(Name))
                return;

            // Scape if field does not exists or has no val
            var item = ruleContext.Item;
            var field = item.Fields[Name];
            if (field == null || !field.HasValue)
                return;

            // Scape if value is the same
            var fieldVal = field.Value;
            if (fieldVal == Text)
                return;

            // Save updated val
            using (new SecurityDisabler())
            {
                item.Editing.BeginEdit();
                item.Fields[Name].SetValue(Text, false);
                item.Editing.EndEdit();
            }
        }
    }
}

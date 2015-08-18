using Rules.Processor.Api.Extensions;
using Sitecore.Rules;
using Sitecore.Rules.Actions;
using Sitecore.SecurityModel;

namespace Rules.Processor.Api.Rules.Actions
{
    class ReplaceStringInItemName<T> : RuleAction<T> where T : RuleContext
    {
        public string String1 { get; set; }
        public string String2 { get; set; }

        public override void Apply(T ruleContext)
        {
            // Scape in case of empty params
            if (string.IsNullOrEmpty(String1) || string.IsNullOrEmpty(String2))
                return;

            String1 = String1.ReplaceQuotesForSpecialChars();
            String2 = String2.ReplaceQuotesForSpecialChars();

            var item = ruleContext.Item;

            // Replace on memory
            var currentName = item.Name;
            var targetName = currentName.Replace(String1, String2);

            // Scape if name does not changed
            if (currentName == targetName)
                return;

            // Save updated val
            using (new SecurityDisabler())
            {
                item.Editing.BeginEdit();
                item.Name = targetName;
                item.Editing.EndEdit();
            }
        }
    }
}

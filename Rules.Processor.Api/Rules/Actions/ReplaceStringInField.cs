﻿using Rules.Processor.Api.Extensions;
using Sitecore.Rules;
using Sitecore.Rules.Actions;
using Sitecore.SecurityModel;

namespace Rules.Processor.Api.Rules.Actions
{
    public class ReplaceStringInField<T> : RuleAction<T> where T : RuleContext
    {
        public string String1 { get; set; }
        public string String2 { get; set; }
        public string Name { get; set; }

        public override void Apply(T ruleContext)
        {
            // Scape in case of empty params
            if (string.IsNullOrEmpty(String1) || string.IsNullOrEmpty(String2) || string.IsNullOrEmpty(Name))
                return;

            String1 = String1.ReplaceQuotesForSpecialChars();
            String2 = String2.ReplaceQuotesForSpecialChars();

            // Scape if field does not exists or has no val
            var item = ruleContext.Item;
            var field = item.Fields[Name];
            if (field == null || !field.HasValue)
                return;

            // Do replacement on memory
            var fieldVal = field.Value;
            var replaced = fieldVal.Replace(String1, String2);

            // Scape if value is the same
            if (fieldVal == replaced)
                return;

            // Save updated val
            using (new SecurityDisabler())
            {
                item.Editing.BeginEdit();
                item.Fields[Name].SetValue(replaced, false);
                item.Editing.EndEdit();
            }
        }
    }
}

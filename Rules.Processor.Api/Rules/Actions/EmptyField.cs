﻿using Sitecore.Rules;
using Sitecore.Rules.Actions;
using Sitecore.SecurityModel;

namespace Rules.Processor.Api.Rules.Actions
{
    class EmptyField<T> : RuleAction<T> where T : RuleContext
    {
        public string Name { get; set; }

        public override void Apply(T ruleContext)
        {
            // Scape in case of empty params
            if (string.IsNullOrEmpty(Name))
                return;

            // Scape if field does not exists or has no val
            var item = ruleContext.Item;
            var field = item.Fields[Name];
            if (field == null || !field.HasValue)
                return;

            // Save val
            using (new SecurityDisabler())
            {
                item.Editing.BeginEdit();
                item.Fields[Name].SetValue(null, false);
                item.Editing.EndEdit();
            }
        }
    }
}

using System;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Rules;

namespace Rules.Processor.Api.Rules
{
    public class RuleProcessor
    {
        public RuleList<RuleContext> Rules { get; private set; }
        public Item RuleItem { get; private set; }
        public RuleProcessor(RuleList<RuleContext> rules, Item ruleItem)
        {
            Rules = rules;
            RuleItem = ruleItem;
        }

        public bool ValidateRule(Item checkItem)
        {
            return ValidateRule(Rules, RuleItem, checkItem);
        }

        public static bool ValidateRule(RuleList<RuleContext> rules, Item ruleItem, Item checkItem)
        {
            var ruleContext = new RuleContext { Item = checkItem };
            var result = false;
            using (
                new LongRunningOperationWatcher(Settings.Profiling.RenderFieldThreshold, "Long running rule set: {0}",
                    new string[1]
                    {
                        ruleItem.Name ?? string.Empty
                    }))
            {
                foreach (var rule in rules.Rules)
                {
                    if (rule.Condition == null)
                        continue;

                    var stack = new RuleStack();

                    try
                    {
                        rule.Condition.Evaluate(ruleContext, stack);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(
                            string.Format("Evaluation of condition failed. Rule item ID: {0}, condition item ID: {1}",
                                rule.UniqueId != (ID)null ? rule.UniqueId.ToString() : "Unknown",
                                rule.Condition.UniqueId != ID.Null.ToString()
                                    ? rule.Condition.UniqueId
                                    : "Unknown"), ex, typeof(RuleProcessor));
                        ruleContext.Abort();
                    }

                    if (ruleContext.IsAborted)
                        break;

                    if (stack.Count != 0 && (bool)stack.Pop())
                        result = true;
                    else
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }

    }
}

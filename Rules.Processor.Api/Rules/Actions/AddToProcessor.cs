using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.Rules.Actions;

namespace Rules.Processor.Api.Rules.Actions
{
    public class AddToProcessor<T> : RuleAction<T> where T : RuleContext
    {
        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull((object)ruleContext, "ruleContext");
            Item obj1 = ruleContext.Item;
            if (obj1 == null)
                return;
            //Item obj2 = obj1.Database.GetItem(this.ScriptId);
            //if (obj2 == null)
            //    return;
            //ItemScripts.InvokeDefaultMethod<T>(obj2, new object[0]);
        }
    }
}

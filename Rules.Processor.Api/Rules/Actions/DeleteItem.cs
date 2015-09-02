using Sitecore.Rules;
using Sitecore.Rules.Actions;
using Sitecore.SecurityModel;

namespace Rules.Processor.Api.Rules.Actions
{
    class DeleteItem<T> : RuleAction<T> where T : RuleContext
    {
        public override void Apply(T ruleContext)
        {
            var item = ruleContext.Item;
            using (new SecurityDisabler())
            {
                item.DeleteChildren();
                item.Delete();
            }
        }
    }
}

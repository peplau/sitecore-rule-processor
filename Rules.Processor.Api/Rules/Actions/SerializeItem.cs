using Sitecore.Data.Serialization;
using Sitecore.Rules;
using Sitecore.Rules.Actions;
namespace Rules.Processor.Api.Rules.Actions
{
    public class SerializeItem<T> : RuleAction<T> where T : RuleContext
    {
        public override void Apply(T ruleContext)
        {
            var item = ruleContext.Item;
            Manager.DumpItem(item);
        }
    }
}

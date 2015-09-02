using Sitecore.Data;
using Sitecore.Rules;
using Sitecore.Rules.Actions;
using Sitecore.SecurityModel;

namespace Rules.Processor.Api.Rules.Actions
{
    class ChangeTemplate<T> : RuleAction<T> where T : RuleContext
    {
        public string Template { get; set; }

        public override void Apply(T ruleContext)
        {
            // Scape in case of empty params
            if (string.IsNullOrEmpty(Template))
                return;

            // Scape if template does not exists
            var item = ruleContext.Item;
            var targetTemplate = item.Database.GetItem(new ID(Template));
            if (targetTemplate == null)
                return;

            // Change template
            using (new SecurityDisabler())
            {
                item.ChangeTemplate(targetTemplate);
            }
        }
    }
}

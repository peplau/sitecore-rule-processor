using Sitecore.Data.Managers;
using Sitecore.Rules;
using Sitecore.Rules.Actions;
using Sitecore.SecurityModel;

namespace Rules.Processor.Api.Rules.Actions
{
    class AddVersion<T> : RuleAction<T> where T : RuleContext
    {
        public string Language { get; set; }

        public override void Apply(T ruleContext)
        {
            // Scape in case of empty params
            if (string.IsNullOrEmpty(Language))
                return;

            // Scape if language does not exists
            var item = ruleContext.Item;

            // Add Version
            var language = LanguageManager.GetLanguage(Language);
            var localizedItem = item.Database.GetItem(item.ID, language);
            using (new SecurityDisabler())
            {
                localizedItem.Editing.BeginEdit();
                localizedItem.Versions.AddVersion();
                localizedItem.Editing.EndEdit();
            }
        }
    }
}
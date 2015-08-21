using System;
using Sitecore.Diagnostics;
using Sitecore.Publishing;
using Sitecore.Rules;
using Sitecore.Rules.Actions;

namespace Rules.Processor.Api.Rules.Actions
{
    class PublishItem<T> : RuleAction<T> where T : RuleContext
    {
        public string Method { get; set; }
        public string TargetDb { get; set; }

        public override void Apply(T ruleContext)
        {
            // Scape in case of empty params
            if (string.IsNullOrEmpty(Method) || string.IsNullOrEmpty(TargetDb))
                return;

            // Item to Publish
            var item = ruleContext.Item;

            // Publish Mode
            var publishMode = PublishMode.Smart;
            switch (Method)
            {
                case "Incremental":
                    publishMode = PublishMode.Incremental;
                    break;
                case "Smart":
                    publishMode = PublishMode.Smart;
                    break;
                case "SingleItem":
                    publishMode = PublishMode.SingleItem;
                    break;
                case "Republish":
                    publishMode = PublishMode.Full;
                    break;
            }

            // Publish Item
            using (new Sitecore.SecurityModel.SecurityDisabler())
            {
                // Get Source DB
                var sourceDb = item.Database;
                // Get Target DB
                var targetDb = Sitecore.Configuration.Factory.GetDatabase(TargetDb);
                if (targetDb == null)
                {
                    Log.Error("PublishItem Action: Cannot find target database: " + TargetDb, this);
                    return;
                }

                // Publish
                try
                {
                    foreach (var language in sourceDb.Languages)
                    {
                        //loops on the languages and do a full republish on the whole sitecore content tree
                        var options = new PublishOptions(sourceDb, targetDb, publishMode, language, DateTime.Now)
                        {
                            RootItem = item,
                            RepublishAll = true,
                            Deep = true
                        };

                        var message =
                            String.Format("PublishItem Action: Starting publish. ID={0}, Mode={1}, Language={2}",
                                item.ID, publishMode, language);
                        Log.Info(message, this);

                        var myPublisher = new Publisher(options);
                        myPublisher.PublishAsync();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(string.Format("PublishItem Action: Could not publish item {0} to the {1} database", item.ID, TargetDb), ex);
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.Rules.Actions;

namespace Rules.Processor.Api.Rules.Actions
{
    public class LogAction<T> : RuleAction<T> where T : RuleContext
    {
        public string Level { get; set; }
        public string Text { get; set; }
        public string Fields { get; set; }

        /// <summary>
        /// The apply
        /// </summary>
        /// <param name="ruleContext">The rule context.
        /// </param>
        public override void Apply(T ruleContext)
        {
            switch (Level)
            {
                case "Error":
                    Log.Error(GetParsedText(ruleContext.Item, Text), this);
                    break;
                case "Info":
                    Log.Info(GetParsedText(ruleContext.Item, Text), this);
                    break;
                case "Warn":
                    Log.Warn(GetParsedText(ruleContext.Item, Text), this);
                    break;
                default:
                    Log.Error("LogAction : cannot determine Log Level: " + Level, this);
                    break;
            }
        }

        private string GetParsedText(Item item, string text)
        {
            var fieldNames = Fields.Split('|').Where(p=>!string.IsNullOrEmpty(p)).ToArray();
            var replacements = fieldNames.Select(fieldName => (object) GetFieldValue(item, fieldName)).ToArray();
            return string.Format(text, replacements);
        }

        private string GetFieldValue(Item item, string fieldName)
        {
            var ret = "";
            switch (fieldName.ToLower())
            {
                case "id":
                    ret = item.ID.ToString();
                    break;
                case "name":
                    ret = item.Name;
                    break;
                case "displayname":
                    ret = item.DisplayName;
                    break;
                case "templateid":
                    ret = item.TemplateID.ToString();
                    break;
                case "templatename":
                    ret = item.TemplateName;
                    break;
                default:
                    var field = item.Fields[fieldName];
                    if (field != null)
                        ret = field.Value;
                    break;
            }
            return ret;
        }
    }
}

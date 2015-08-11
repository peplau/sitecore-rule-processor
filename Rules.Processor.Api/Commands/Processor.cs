using System;
using System.Collections.Specialized;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Text;
using Sitecore.Web.UI.Sheer;

namespace Rules.Processor.Api.Commands
{
    [Serializable]
    public class Processor : Command
    {
        /// <summary>
        /// Executes the command in the specified context.
        /// </summary>
        /// <param name="context">The context.</param><contract><requires name="context" condition="not null"/></contract>
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull((object)context, "context");
            Context.ClientPage.Start(this, "Run",
                new ClientPipelineArgs(new NameValueCollection()
                {
                    {Constants.QueryStringParams.RuleId, context.Items[0].ID.ToString()}
                }));
        }

        protected void Run(ClientPipelineArgs args)
        {
            if (args.IsPostBack)
            {
                if (!args.HasResult)
                    ;
            }
            else
            {
                var urlString =
                    new UrlString(UIUtil.GetUri("control:RuleProcessor",
                        Constants.QueryStringParams.RuleId + "=" + args.Parameters[Constants.QueryStringParams.RuleId]));
                SheerResponse.ShowModalDialog(urlString.ToString(), "600px", "600px", "", true);
                args.WaitForPostBack();
            }
        }

        /// <summary>
        /// Queries the state of the command.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The state of the command.
        /// </returns>
        /// <contract><requires name="context" condition="not null"/></contract>
        public override CommandState QueryState(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            if (context.Items.Length != 1)
                return CommandState.Hidden;
            if (context.Items[0].TemplateID!=Rule.TemplateID)
                return CommandState.Hidden;
            return base.QueryState(context);
        }
    }
}
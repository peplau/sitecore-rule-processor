using System;
using System.Collections.Generic;
using System.Linq;
using Rules.Processor.Api.Extensions;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Query;
using Sitecore.Rules;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Literal = Sitecore.Web.UI.HtmlControls.Literal;

namespace Rules.Processor.Api.CodeBeside
{
    public class RuleProcessor : BaseForm
    {
        #region Registered controls from XAML
        protected Listview ResultItems;
        protected TreePicker RootItemSelected;
        protected DataContext RootItemDataContext;
        protected Button RunButton;
        protected Literal litMessages;
        protected Toolbutton btnExecuteAll;
        protected Toolbutton btnExecuteSelected;
        protected Toolbutton btnSelectAll;
        #endregion

        #region Private Properties
        private static ID RuleId
        {
            get
            {
                return new ID(WebUtil.GetQueryString(Constants.QueryStringParams.RuleId));
            }
        }
        private Rule _ruleItem;
        private Rule RuleItem
        {
            get
            {
                if (_ruleItem != null)
                    return _ruleItem;
                _ruleItem = new Rule(Client.ContentDatabase.GetItem(RuleId));
                return _ruleItem;
            }
        }

        private RuleList<RuleContext> _rules;
        private RuleList<RuleContext> Rules
        {
            get
            {
                if (_rules != null)
                    return _rules;

                _rules = new RuleList<RuleContext>();
                var parsed = RuleFactory.ParseRules<RuleContext>(Client.ContentDatabase, RuleItem.Rules);
                _rules.AddRange(parsed.Rules);
                if (_rules.Count < 1)
                    return null;
                _rules.Evaluating += RulesOnEvaluating;
                _rules.Evaluated += RulesOnEvaluated;
                return _rules;
            }
        }
        #endregion

        protected override void OnLoad(EventArgs args)
        {
            if (!Context.ClientPage.IsEvent)
            {
                InitializeControls();
            }
            else
            {
                //RunButton.OnClick += RunButtonClick;
            }
            base.OnLoad(args);
        }

        private void InitializeControls()
        {
            RootItemDataContext.GetFromQueryString();
            RootItemDataContext.Root = Client.ContentDatabase.GetRootItem().ID.ToString();
            RootItemDataContext.Folder = Client.ContentDatabase.GetRootItem().ID.ToString();
        }

        [HandleMessage("local:selectAll")]
        protected void SelectAllItems(Message message)
        {
            foreach (var listviewItem in ResultItems.Items)
                listviewItem.Selected = true;
            ResultItems.Refresh();
        }

        [HandleMessage("local:unselectAll")]
        protected void UnselectAllItems(Message message)
        {
            foreach (var listviewItem in ResultItems.Items)
                listviewItem.Selected = false;
            ResultItems.Refresh();
        }

        [HandleMessage("local:executeSelectedActions")]
        protected void ExecuteSelectedActions(Message message)
        {
            ExecuteActions(ResultItems.SelectedItems);
        }

        [HandleMessage("local:executeAllActions")]
        protected void ExecuteAllActions(Message message)
        {
            ExecuteActions(ResultItems.Items);
        }

        private void ExecuteActions(IEnumerable<ListviewItem> items)
        {
            // Show checkboxes and set everything as unckecked
            ResultItems.ShowCheckboxes = true;
            foreach (var listviewItem in ResultItems.Items)
                listviewItem.Checked = false;

            var itemsExecuted = 0;
            var actionsExecuted = 0;

            foreach (var listviewItem in items)
            {
                var id = new ID(listviewItem.Value);
                var item = Client.ContentDatabase.GetItem(id);
                if (item == null)
                    continue;

                itemsExecuted++;
                var qtdActions = 0;
                Rules.Run(new RuleContext {Item = item}, out qtdActions);
                actionsExecuted = actionsExecuted + qtdActions;
            }

            litMessages.Text = String.Format(" - {0} actions has been executed on {1} items", actionsExecuted, itemsExecuted);
        }

        [HandleMessage("local:bringResults")]
        protected void BringResults(Message message)
        {
            var rootItem = RootItemDataContext.GetFolder();

            // Activate buttons
            btnExecuteAll.Disabled =
                btnExecuteSelected.Disabled = btnSelectAll.Disabled = false;

            // Switch Context DB for the query
            var currentDb = Context.Database;
            Context.Database = rootItem.Database;

            // Execute query
            var query = rootItem.GetItemXPath() + "//*";
            var scQuery = new Query(query) { Max = 100000 };
            var queryResult = scQuery.Execute();

            // Get Items from Query
            var allItems = new List<Item>();
            if (queryResult.GetType() == typeof(QueryContext))
                allItems.Add(((QueryContext)queryResult).GetQueryContextItem());
            else
                allItems = ((QueryContext[])queryResult).Select(p => p.GetQueryContextItem()).ToList();
            allItems.Insert(0, rootItem);

            // Switch Context DB back
            Context.Database = currentDb;

            // Render results
            ResultItems.Controls.Clear();
            var processor = new Rules.RuleProcessor(Rules, RuleItem);
            foreach (var item in allItems)
            {
                if (!processor.ValidateRule(item))
                    continue;

                // Create and add the new ListviewItem control to the Listview. 
                // We have to assign an unique ID to each control on the page. 
                var listItem = new ListviewItem();
                Context.ClientPage.AddControl(ResultItems, listItem);
                listItem.ID = Control.GetUniqueID("I");

                // Populate the list item with data. 
                listItem.Header = item.Name;
                listItem.Icon = item.Appearance.Icon;
                listItem.Value = item.ID.ToString();
                listItem.ColumnValues["path"] = item.Paths.Path;
                listItem.ColumnValues["language"] = item.Language.Name;
                listItem.ColumnValues["version"] = item.Version.ToString();
            }

            // We need to replace the html in order to avoid duplicate ID's 
            Context.ClientPage.ClientResponse.SetOuterHtml("ItemList", ResultItems);
            ResultItems.Refresh();
        }

        private void RulesOnEvaluating(RuleList<RuleContext> ruleList, RuleContext ruleContext, Rule<RuleContext> rule)
        {
            //var listviewItem = ResultItems.Items.First(p => p.Value == ruleContext.Item.ID.ToString());
            //listviewItem.Style["background"] = "yellow";
            //ResultItems.Refresh();
        }

        private void RulesOnEvaluated(RuleList<RuleContext> ruleList, RuleContext ruleContext, Rule<RuleContext> rule)
        {
            var listviewItem = ResultItems.Items.First(p => p.Value == ruleContext.Item.ID.ToString());
            listviewItem.Checked = true;
            ResultItems.Refresh();
        }
    }
}
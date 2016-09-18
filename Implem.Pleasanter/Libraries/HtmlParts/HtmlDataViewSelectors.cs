using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlDataViewSelectors
    {
        public static HtmlBuilder DataViewSelector(
            this HtmlBuilder hb, string referenceType, string dataViewName)
        {
            var optionCollection = new Dictionary<string, ControlData>
            {
                { "index", new ControlData(Displays.Grid()) }
            };
            Def.DataViewDefinitionCollection
                .Where(o => o.ReferenceType == referenceType)
                .Select(o => o.Name)
                .ForEach(name =>
                    optionCollection.Add(
                        name.ToLower(),
                        new ControlData(Displays.Get(name))));
            return optionCollection.Count > 1
                ? hb.Div(css: "dataview-selector", action: () => hb
                    .FieldDropDown(
                        controlId: "DataViewSelector",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.DataViewSelector(),
                        optionCollection: optionCollection,
                        selectedValue: dataViewName,
                        onChange: "$p.dataView($(this));")
                    .Button(
                        attributes: new HtmlAttributes()
                            .Id("PreviousDataView")
                            .Class("button button-icon")
                            .AccessKey("b")
                            .OnClick("$(this).trigger('go');")
                            .DataIcon("ui-icon-circle-triangle-w")
                            .DataSelector("#DataViewSelector"),
                        action: () => hb
                            .Displays_Previous())
                    .Button(
                        attributes: new HtmlAttributes()
                            .Id("NextDataView")
                            .Class("button button-icon")
                            .AccessKey("n")
                            .OnClick("$(this).trigger('go');")
                            .DataIcon("ui-icon-circle-triangle-e")
                            .DataSelector("#DataViewSelector"),
                        action: () => hb
                            .Displays_Next()))
                : hb;
        }
    }
}
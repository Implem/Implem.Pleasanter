using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Responses;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.ViewParts
{
    public static class HtmlDataViewSelectors
    {
        public static HtmlBuilder DataViewSelector(
            this HtmlBuilder hb, string referenceType, string dataViewName)
        {
            var optionCollection = new Dictionary<string, ControlData>
            {
                { "Grid", new ControlData(Displays.Grid()) }
            };
            Def.DataViewDefinitionCollection
                .Where(o => o.ReferenceType == referenceType)
                .Select(o => o.Name)
                .ForEach(name =>
                    optionCollection.Add(name, new ControlData(Displays.Get(name))));
            return optionCollection.Count > 1
                ? hb.Div(css: "dataview-selector", action: () => hb
                    .FieldDropDown(
                        controlId: "DataViewSelector",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.DataViewSelector(),
                        optionCollection: optionCollection,
                        selectedValue: dataViewName,
                        action: "DataView",
                        method: "post")
                    .Button(
                        attributes: Html.Attributes()
                            .Class("button button-to-left")
                            .OnClick(Def.JavaScript.Go)
                            .AccessKey("b")
                            .DataSelector("#DataViewSelector"),
                        action: () => hb
                            .Displays_Previous())
                    .Button(
                        attributes: Html.Attributes()
                            .Class("button button-to-right")
                            .OnClick(Def.JavaScript.Go)
                            .AccessKey("n")
                            .DataSelector("#DataViewSelector"),
                        action: () => hb
                            .Displays_Next()))
                : hb;
        }
    }
}
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    public class PublishesController
    {
        public string Index(Context context, long id = 0)
        {
            if (!context.Ajax)
            {
                var log = new SysLogModel(context: context);
                var html = new ItemModel(
                    context: context,
                    referenceId: id)
                        .Index(context: context);
                log.Finish(
                    context: context,
                    responseSize: html.Length);
                return html;
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = new ItemModel(
                    context: context,
                    referenceId: id)
                        .IndexJson(context: context);
                log.Finish(
                    context: context,
                    responseSize: json.Length);
                return json;
            }
        }

        public string SearchDropDown(Context context, long id = 0)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).SearchDropDown(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string SelectSearchDropDown(Context context, long id = 0)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).SelectSearchDropDown(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string GridRows(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new ItemModel(context: context, referenceId: id).GridRows(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string Edit(Context context, long id)
        {
            if (!context.Ajax)
            {
                var log = new SysLogModel(context: context);
                var html = new ItemModel(context: context, referenceId: id).Editor(context: context);
                log.Finish(context: context, responseSize: html.Length);
                return html;
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = new ItemModel(context: context, referenceId: id).EditorJson(context: context);
                log.Finish(context: context, responseSize: json.Length);
                return json;
            }
        }
    }
}
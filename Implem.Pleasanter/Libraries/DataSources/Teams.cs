using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace Implem.Pleasanter.Libraries.DataSources
{
    public class Teams
    {
        public Context context;
        public string _text;
        public string _url;
        public string _title;
        public string _prefix;

        public Teams(Context _context, string _text)
        {

        }

        public Teams(Context _context, string _prefix, string _title, string _url, string _body)
        {
            _text = WebUtility.HtmlEncode(_body);
            _text = _text
                .Replace("\r\n", "<br>")
                .Replace("\n", "<br>");
            this._title = _title;
            this._url = _url;
            this._prefix = _prefix;
            context = _context;
        }

        public void Send(string address)
        {
            foreach (var url in address.Split(',').Select(x => x.Trim()))
            {
                Task.Run(() =>
                {
                    try
                    {
                        using (var client = new WebClient())
                        {
                            var param = new TeamsModel()
                            {
                                Summary = $"{context.RecordTitle} {context.Action}",
                                Sections = new List<TeamsSectionModel>()
                                {
                                    new TeamsSectionModel(){
                                        ActivityTitle = _title,
                                        ActivityText = _text,
                                        Facts = new List<TeamsFactModel>()
                                        {
                                            new TeamsFactModel()
                                            {
                                                Name = "サイト",
                                                Value = context.SiteTitle
                                            },
                                            new TeamsFactModel()
                                            {
                                                Name = "レコード",
                                                Value = context.RecordTitle
                                            },
                                            new TeamsFactModel()
                                            {
                                                Name = "更新者",
                                                Value = context.User.Name
                                            }
                                        }
                                    }
                                },
                                PotentialAction = new List<TeamsPotentialActionModel>()
                                {
                                    new TeamsPotentialActionModel(){
                                        Type = "OpenUri",
                                        Name = "ブラウザで開く",
                                        Targets = new List<TeamsTargetModel>(){
                                            new TeamsTargetModel(){
                                                OS = "default",
                                                Uri = _url
                                            }
                                        }
                                    }
                                }
                            };

                            string json = JsonConvert.SerializeObject(param);

                            client.Headers[HttpRequestHeader.ContentType] = "application/json;charset=UTF-8";
                            client.Headers[HttpRequestHeader.Accept] = "application/json";
                            client.Encoding = Encoding.UTF8;
                            client.UploadString(url, "POST", json);
                        }
                    }
                    catch (Exception e)
                    {
                        new SysLogModel(context, e);
                    }
                });
            }
        }
    }

    public class TeamsModel
    {
        [JsonProperty("@type")]
        public string Type => "MessageCard";
        [JsonProperty("@context")]
        public string Context => "http://schema.org/extensions";
        [JsonProperty("summary")]
        public string Summary { get; set; }
        [JsonProperty("sections")]
        public List<TeamsSectionModel> Sections { get; set; }
        [JsonProperty("potentialAction")]
        public List<TeamsPotentialActionModel> PotentialAction { get; set; }
    }

    public class TeamsSectionModel
    {
        [JsonProperty("activityTitle")]
        public string ActivityTitle { get; set; }
        [JsonProperty("activityText")]
        public string ActivityText { get; set; }
        [JsonProperty("facts")]
        public List<TeamsFactModel> Facts { get; set; }
    }

    public class TeamsFactModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }
    public class TeamsPotentialActionModel
    {
        [JsonProperty("@type")]
        public string Type { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("targets")]
        public List<TeamsTargetModel> Targets { get; set; }
    }
    public class TeamsTargetModel
    {
        [JsonProperty("os")]
        public string OS { get; set; }
        [JsonProperty("uri")]
        public string Uri { get; set; }
    }
}
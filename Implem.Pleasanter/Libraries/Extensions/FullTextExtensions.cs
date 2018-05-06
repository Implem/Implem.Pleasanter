using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Extensions
{
    public static class FullTextExtensions
    {
        public static void FullText(this int self, List<string> fullText)
        {
            fullText.Add(self.ToString());
        }

        public static void FullText(this long self, List<string> fullText)
        {
            fullText.Add(self.ToString());
        }

        public static void FullText(this decimal self, List<string> fullText)
        {
            fullText.Add(self.ToString());
        }

        public static void FullText(this DateTime self, List<string> fullText)
        {
            var value = self.ToLocal();
            if (value.InRange())
            {
                fullText.Add(value.ToString());
            }
        }

        public static void FullText(this string self, List<string> fullText)
        {
            if (self != null)
            {
                fullText.Add(self);
            }
        }

        public static void FullText(this string self, Column column, List<string> fullText)
        {
            if (self != null)
            {
                fullText.Add(column?.HasChoices() == true
                    ? column.Choice(self).SearchText()
                    : self);
            }
        }

        public static void FullText(this IEnumerable<SiteMenuElement> self, List<string> fullText)
        {
            if (self != null)
            {
                fullText.Add(self.Select(o => o.Title).Join(" "));
            }
        }

        public static void FullText(this ProgressRate self, List<string> fullText)
        {
            if (self != null)
            {
                fullText.Add(self.Value.ToString());
            }
        }

        public static void FullText(this Status self, Column column, List<string> fullText)
        {
            if (self != null)
            {
                fullText.Add(column?.HasChoices() == true
                    ? column.Choice(self.Value.ToString()).SearchText()
                    : self.Value.ToString());
            }
        }

        public static void FullText(this CompletionTime self, List<string> fullText)
        {
            var value = self?.Value.ToLocal().AddDays(-1);
            if (value?.InRange() == true)
            {
                fullText.Add(value.ToString());
            }
        }

        public static void FullText(this Time self, List<string> fullText)
        {
            var value = self?.Value.ToLocal();
            if (value?.InRange() == true)
            {
                fullText.Add(value.ToString());
            }
        }

        public static void FullText(this Title self, List<string> fullText)
        {
            if (self != null)
            {
                fullText.Add(self.Value);
            }
        }

        public static void FullText(this User self, List<string> fullText)
        {
            if (self != null)
            {
                fullText.Add(self.Name);
            }
        }

        public static void FullText(this Comments self, List<string> fullText)
        {
            if (self != null)
            {
                fullText.Add(self.Select(o =>
                    SiteInfo.UserName(o.Creator) + " " + o.Body)
                        .Join(" "));
            }
        }

        public static void FullText(this WorkValue self, List<string> fullText)
        {
            if (self != null)
            {
                fullText.Add(self.Value.ToString());
            }
        }

        public static void FullText(this Attachments self, List<string> fullText)
        {
            self?.ForEach(attachment => fullText.Add(attachment.Name));
        }

        public static void OutgoingMailsFullText(
            List<string> fullText, string referenceType, long referenceId)
        {
            new OutgoingMailCollection(where: Rds.OutgoingMailsWhere()
                .ReferenceType(referenceType)
                .ReferenceId(referenceId))
                    .ForEach(o =>
                    {
                        fullText.Add(o.From.ToString());
                        fullText.Add(o.To);
                        fullText.Add(o.Cc);
                        fullText.Add(o.Bcc);
                        fullText.Add(o.Title.Value);
                        fullText.Add(o.Body);
                    });
        }
    }
}
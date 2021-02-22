using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Text;
namespace Implem.Pleasanter.Libraries.Extensions
{
    public static class FullTextExtensions
    {
        public static void FullText(
            this int self,
            Context context,
            Column column,
            StringBuilder fullText)
        {
            switch (column?.FullTextType)
            {
                case Column.FullTextTypes.DisplayName:
                case Column.FullTextTypes.Value:
                case Column.FullTextTypes.ValueAndDisplayName:
                    fullText
                        .Append(" ")
                        .Append(self.ToString());
                    break;
            }
        }

        public static void FullText(
            this long self,
            Context context,
            Column column,
            StringBuilder fullText)
        {
            switch (column?.FullTextType)
            {
                case Column.FullTextTypes.DisplayName:
                case Column.FullTextTypes.Value:
                case Column.FullTextTypes.ValueAndDisplayName:
                    fullText
                        .Append(" ")
                        .Append(self.ToString());
                    break;
            }
        }

        public static void FullText(
            this DateTime self,
            Context context,
            Column column,
            StringBuilder fullText)
        {
            var value = self.ToLocal(context: context);
            if (value.InRange())
            {
                switch (column?.FullTextType)
                {
                    case Column.FullTextTypes.DisplayName:
                    case Column.FullTextTypes.Value:
                    case Column.FullTextTypes.ValueAndDisplayName:
                        fullText
                            .Append(" ")
                            .Append(value.ToString());
                        break;
                }
            }
        }

        public static void FullText(
            this string self,
            Context context,
            Column column,
            StringBuilder fullText)
        {
            if (self != null && column != null)
            {
                if (column.HasChoices() == true)
                {
                    switch (column.Type)
                    {
                        case Column.Types.User:
                            var user = SiteInfo.User(
                                context: context,
                                userId: self.ToInt());
                            switch (column.FullTextType)
                            {
                                case Column.FullTextTypes.DisplayName:
                                    fullText
                                        .Append(" ")
                                        .Append(user.Anonymous()
                                            ? string.Empty
                                            : user.Name);
                                    break;
                                case Column.FullTextTypes.Value:
                                    fullText
                                        .Append(" ")
                                        .Append(user.Anonymous()
                                            ? string.Empty
                                            : user.Id.ToString());
                                    break;
                                case Column.FullTextTypes.ValueAndDisplayName:
                                    fullText
                                        .Append(" ")
                                        .Append(user.Anonymous()
                                            ? string.Empty
                                            : user.Id.ToString());
                                    fullText
                                        .Append(" ")
                                        .Append(user.Anonymous()
                                            ? string.Empty
                                            : user.Name);
                                    break;
                            }
                            break;
                        default:
                            switch (column.FullTextType)
                            {
                                case Column.FullTextTypes.DisplayName:
                                    fullText
                                        .Append(" ")
                                        .Append(column.Choice(self).Text ?? string.Empty);
                                    break;
                                case Column.FullTextTypes.Value:
                                    fullText
                                        .Append(" ")
                                        .Append(column.Choice(self).Value ?? string.Empty);
                                    break;
                                case Column.FullTextTypes.ValueAndDisplayName:
                                    fullText
                                        .Append(" ")
                                        .Append(column.Choice(self).Value ?? string.Empty);
                                    fullText
                                        .Append(" ")
                                        .Append(column.Choice(self).Text ?? string.Empty);
                                    break;
                            }
                            break;
                    }
                }
                else
                {
                    switch (column.FullTextType)
                    {
                        case Column.FullTextTypes.DisplayName:
                        case Column.FullTextTypes.Value:
                        case Column.FullTextTypes.ValueAndDisplayName:
                            fullText
                                .Append(" ")
                                .Append(self);
                            break;
                    }
                }
            }
            else if (self != null)
            {
                fullText
                    .Append(" ")
                    .Append(self);
            }
        }

        public static void FullText(
            this IEnumerable<SiteMenuElement> self,
            Context context,
            StringBuilder fullText)
        {
            self?.ForEach(o =>
                fullText
                    .Append(" ")
                    .Append(o.Title));
        }

        public static void FullText(
            this ProgressRate self,
            Context context,
            Column column,
            StringBuilder fullText)
        {
            if (self != null)
            {
                switch (column?.FullTextType)
                {
                    case Column.FullTextTypes.DisplayName:
                    case Column.FullTextTypes.Value:
                    case Column.FullTextTypes.ValueAndDisplayName:
                        fullText
                            .Append(" ")
                            .Append(self.Value.ToString());
                        break;
                }
            }
        }

        public static void FullText(
            this Status self,
            Context context,
            Column column,
            StringBuilder fullText)
        {
            if (self != null && column != null)
            {
                if (column.HasChoices() == true)
                {
                    switch (column.FullTextType)
                    {
                        case Column.FullTextTypes.DisplayName:
                            fullText
                                .Append(" ")
                                .Append(column.Choice(self.Value.ToString()).Text ?? string.Empty);
                            break;
                        case Column.FullTextTypes.Value:
                            fullText
                                .Append(" ")
                                .Append(column.Choice(self.Value.ToString()).Value ?? string.Empty);
                            break;
                        case Column.FullTextTypes.ValueAndDisplayName:
                            fullText
                                .Append(" ")
                                .Append(column.Choice(self.Value.ToString()).Value ?? string.Empty);
                            fullText
                                .Append(" ")
                                .Append(column.Choice(self.Value.ToString()).Text ?? string.Empty);
                            break;
                    }
                }
                else
                {
                    switch (column.FullTextType)
                    {
                        case Column.FullTextTypes.DisplayName:
                        case Column.FullTextTypes.Value:
                        case Column.FullTextTypes.ValueAndDisplayName:
                            fullText
                                .Append(" ")
                                .Append(self.Value.ToString());
                            break;
                    }
                }
            }
        }

        public static void FullText(
            this CompletionTime self,
            Context context,
            Column column,
            StringBuilder fullText)
        {
            var value = self?.Value.ToLocal(context: context).AddDays(-1);
            if (value?.InRange() == true)
            {
                switch (column?.FullTextType)
                {
                    case Column.FullTextTypes.DisplayName:
                    case Column.FullTextTypes.Value:
                    case Column.FullTextTypes.ValueAndDisplayName:
                        fullText
                            .Append(" ")
                            .Append(value.ToString());
                        break;
                }
            }
        }

        public static void FullText(
            this Time self,
            Context context,
            Column column,
            StringBuilder fullText)
        {
            var value = self?.Value.ToLocal(context: context);
            if (value?.InRange() == true)
            {
                switch (column?.FullTextType)
                {
                    case Column.FullTextTypes.DisplayName:
                    case Column.FullTextTypes.Value:
                    case Column.FullTextTypes.ValueAndDisplayName:
                        fullText
                            .Append(" ")
                            .Append(value.ToString());
                        break;
                }
            }
        }

        public static void FullText(
            this Title self,
            Context context,
            Column column,
            StringBuilder fullText)
        {
            if (self != null)
            {
                switch (column?.FullTextType)
                {
                    case Column.FullTextTypes.DisplayName:
                        fullText
                            .Append(" ")
                            .Append(self.DisplayValue);
                        break;
                    case Column.FullTextTypes.Value:
                        fullText
                            .Append(" ")
                            .Append(self.Value);
                        break;
                    case Column.FullTextTypes.ValueAndDisplayName:
                        fullText
                            .Append(" ")
                            .Append(self.Value)
                            .Append(" ")
                            .Append(self.DisplayValue);
                        break;
                }
            }
        }

        public static void FullText(
            this User self,
            Context context,
            Column column,
            StringBuilder fullText)
        {
            if (self != null)
            {
                switch (column?.FullTextType)
                {
                    case Column.FullTextTypes.DisplayName:
                        fullText
                            .Append(" ")
                            .Append(self.Name);
                        break;
                    case Column.FullTextTypes.Value:
                        fullText
                            .Append(" ")
                            .Append(self.Id);
                        break;
                    case Column.FullTextTypes.ValueAndDisplayName:
                        fullText
                            .Append(" ")
                            .Append(self.Id);
                        fullText
                            .Append(" ")
                            .Append(self.Name);
                        break;
                }
            }
        }

        public static void FullText(
            this Comments self,
            Context context,
            Column column,
            StringBuilder fullText)
        {
            if (self != null)
            {
                switch (column?.FullTextType)
                {
                    case Column.FullTextTypes.DisplayName:
                    case Column.FullTextTypes.Value:
                    case Column.FullTextTypes.ValueAndDisplayName:
                        self.ForEach(o =>
                            fullText
                                .Append(" ")
                                .Append(SiteInfo.UserName(
                                    context: context,
                                    userId: o.Creator))
                                .Append(" ")
                                .Append(o.Body));
                        break;
                }
            }
        }

        public static void FullText(
            this WorkValue self,
            Context context,
            Column column,
            StringBuilder fullText)
        {
            if (self != null)
            {
                switch (column?.FullTextType)
                {
                    case Column.FullTextTypes.DisplayName:
                    case Column.FullTextTypes.Value:
                    case Column.FullTextTypes.ValueAndDisplayName:
                        fullText
                            .Append(" ")
                            .Append(self.Value.ToString());
                        break;
                }
            }
        }

        public static void FullText(
            this Attachments self,
            Context context,
            Column column,
            StringBuilder fullText)
        {
            if (self != null)
            {
                switch (column?.FullTextType)
                {
                    case Column.FullTextTypes.DisplayName:
                        self?.ForEach(o =>
                            fullText
                                .Append(" ")
                                .Append(o.Name));
                        break;
                    case Column.FullTextTypes.Value:
                        self?.ForEach(o =>
                            fullText
                                .Append(" ")
                                .Append(o.Guid));
                        break;
                    case Column.FullTextTypes.ValueAndDisplayName:
                        self?.ForEach(o =>
                            fullText
                                .Append(" ")
                                .Append(o.Guid)
                                .Append(" ")
                                .Append(o.Name));
                        break;
                }
            }
        }

        public static void OutgoingMailsFullText(
            Context context,
            StringBuilder fullText,
            string referenceType,
            long referenceId)
        {
            new OutgoingMailCollection(
                context: context,
                where: Rds.OutgoingMailsWhere()
                    .ReferenceType(referenceType)
                    .ReferenceId(referenceId))
                        .ForEach(o =>
                        {
                            fullText
                                .Append(" ")
                                .Append(o.From.ToString());
                            fullText
                                .Append(" ")
                                .Append(o.To);
                            fullText
                                .Append(" ")
                                .Append(o.Cc);
                            fullText
                                .Append(" ")
                                .Append(o.Bcc);
                            fullText
                                .Append(" ")
                                .Append(o.Title.Value);
                            fullText
                                .Append(" ")
                                .Append(o.Body);
                        });
        }
    }
}
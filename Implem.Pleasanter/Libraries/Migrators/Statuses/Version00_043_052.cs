﻿using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
namespace Implem.Pleasanter.Libraries.Migrators.Statuses
{
    public static class Version00_043_052
    {
        public static void Migrate(Context context)
        {
            var sub = Rds.SelectItems(
                column: Rds.ItemsColumn().ReferenceId());
            Rds.ExecuteNonQuery(
                context: context,
                statements: Rds.PhysicalDeleteLinks(
                    where: Rds.LinksWhere()
                        .Or(or: Rds.LinksWhere()
                            .DestinationId_In(sub: sub, negative: true)
                            .SourceId_In(sub: sub, negative: true))));
        }
    }
}
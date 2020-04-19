﻿namespace Implem.IRds
{
    public interface ISqls
    {
        string TrueString { get; }

        string FalseString { get; }

        string IsNotTrue { get; }

        string CurrentDateTime { get; }

        string WhereLikeTemplateForward { get; }

        string WhereLikeTemplate { get; }

        string GenerateIdentity { get; }

        object DateTimeValue(object value);

        string BooleanString(string value);

        string DateGroupYearly { get; }

        string DateGroupMonthly { get; }

        string DateGroupWeeklyPart { get; }

        string DateGroupWeekly { get; }

        string DateGroupDaily { get; }
    }
}
namespace Implem.IRds
{
    public interface ISqls
    {
        string TrueString { get; }

        string FalseString { get; }

        object TrueValue { get; }

        object FalseValue { get; }

        string IsNotTrue { get; }

        string CurrentDateTime { get; }

        string WhereLikeTemplateForward { get; }

        string WhereLikeTemplate { get; }

        string GenerateIdentity { get; }

        object DateTimeValue(object value);

        string BooleanString(string value);
    }
}

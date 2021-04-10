namespace Implem.ParameterAccessor.Parts
{
    public class User
    {
        public bool DisableTopSiteCreation;
        public bool DisableGroupAdmin;
        public SelectorToolTipKind? SelectorToolTip;

        public enum SelectorToolTipKind
        {
            LoginId,
            MailAddress
        }

        public bool IsMailAddressSelectorToolTip()
        {
            return SelectorToolTip == SelectorToolTipKind.MailAddress;
        }
    }
}
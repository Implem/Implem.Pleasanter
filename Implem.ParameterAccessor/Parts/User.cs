namespace Implem.ParameterAccessor.Parts
{
    public class User
    {
        public bool DisableTopSiteCreation;
        public bool DisableGroupAdmin;
        public bool DisableGroupCreation;
        public bool DisableMovingFromTopSite;
        public bool DisableApi;
        public SelectorToolTipKind? SelectorToolTip;
        public string Theme;

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
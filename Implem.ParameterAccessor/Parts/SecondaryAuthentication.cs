namespace Implem.ParameterAccessor.Parts
{
    public class SecondaryAuthentication
    {
        public enum SecondaryAuthenticationMode
        {
            None,
            DefaultEnable,
            DefaultDisable
        }

        public SecondaryAuthenticationMode Mode;
        public string NotificationType;
        public bool NotificationMailBcc;
        public string AuthenticationCodeCharacterType;
        public int? AuthenticationCodeLength;
        public int? AuthenticationCodeExpirationPeriod;
    }
}

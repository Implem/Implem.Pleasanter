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
        public enum SecondaryAuthenticationModeNotificationType
        {
            Mail,
            GoogleAuthenticator
        }

        public SecondaryAuthenticationMode Mode;
        public SecondaryAuthenticationModeNotificationType NotificationType;
        public bool NotificationMailBcc;
        public string AuthenticationCodeCharacterType;
        public int? AuthenticationCodeLength;
        public int? AuthenticationCodeExpirationPeriod;
    }
}

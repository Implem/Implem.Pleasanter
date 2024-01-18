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
        public enum SecondaryAuthenticationModeNotificationTypes
        {
            Mail,
            Totp
        }

        public SecondaryAuthenticationMode Mode;
        public SecondaryAuthenticationModeNotificationTypes NotificationType;
        public double? CountTolerances;
        public bool NotificationMailBcc;
        public string AuthenticationCodeCharacterType;
        public int? AuthenticationCodeLength;
        public int? AuthenticationCodeExpirationPeriod;
    }
}

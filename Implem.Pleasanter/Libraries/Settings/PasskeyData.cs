using Fido2NetLib.Objects;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using System;

namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable]
    public class PasskeyData
    {
        public byte[] PublicKey { get; set; }

        public uint SignCount { get; set; }

        public AuthenticatorTransport[] Transports { get; set; }

        public bool IsBackupEligible { get; set; }

        public bool IsBackedUp { get; set; }

        public byte[] AttestationObject { get; set; }

        public byte[] AttestationClientDataJson { get; set; }

        public string AttestationFormat { get; set; }

        public Guid AaGuid { get; set; }


        public PasskeyData()
        {
            AaGuid = Guid.Empty;
            Transports = [];
            SignCount = 0;
            IsBackedUp = false;
            IsBackupEligible = false;
            PublicKey = [];
            AttestationObject = [];
            AttestationClientDataJson = [];
            AttestationFormat = string.Empty;

        }

        public bool InitialValue(Context context)
        {
            return this.ToJson() == "{}";
        }
    }
}

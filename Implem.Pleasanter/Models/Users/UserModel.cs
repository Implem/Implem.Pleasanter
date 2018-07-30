using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class UserModel : BaseModel, Interfaces.IConvertable
    {
        public int TenantId = Sessions.TenantId();
        public int UserId = 0;
        public string LoginId = string.Empty;
        public string GlobalId = string.Empty;
        public string Name = string.Empty;
        public string UserCode = string.Empty;
        public string Password = string.Empty;
        public string PasswordValidate = string.Empty;
        public string PasswordDummy = string.Empty;
        public bool RememberMe = false;
        public string LastName = string.Empty;
        public string FirstName = string.Empty;
        public Time Birthday = new Time();
        public string Gender = string.Empty;
        public string Language = "ja";
        public string TimeZone = "Tokyo Standard Time";
        public string DeptCode = string.Empty;
        public int DeptId = 0;
        public Names.FirstAndLastNameOrders FirstAndLastNameOrder = (Names.FirstAndLastNameOrders)2;
        public string Body = string.Empty;
        public Time LastLoginTime = new Time();
        public Time PasswordExpirationTime = new Time();
        public Time PasswordChangeTime = new Time();
        public int NumberOfLogins = 0;
        public int NumberOfDenial = 0;
        public bool TenantManager = false;
        public bool ServiceManager = false;
        public bool Disabled = false;
        public bool Developer = false;
        public UserSettings UserSettings = new UserSettings();
        public string ApiKey = string.Empty;
        public string OldPassword = string.Empty;
        public string ChangedPassword = string.Empty;
        public string ChangedPasswordValidator = string.Empty;
        public string AfterResetPassword = string.Empty;
        public string AfterResetPasswordValidator = string.Empty;
        public List<string> MailAddresses = new List<string>();
        public string DemoMailAddress = string.Empty;
        public string SessionGuid = string.Empty;
        public string ClassA = string.Empty;
        public string ClassB = string.Empty;
        public string ClassC = string.Empty;
        public string ClassD = string.Empty;
        public string ClassE = string.Empty;
        public string ClassF = string.Empty;
        public string ClassG = string.Empty;
        public string ClassH = string.Empty;
        public string ClassI = string.Empty;
        public string ClassJ = string.Empty;
        public string ClassK = string.Empty;
        public string ClassL = string.Empty;
        public string ClassM = string.Empty;
        public string ClassN = string.Empty;
        public string ClassO = string.Empty;
        public string ClassP = string.Empty;
        public string ClassQ = string.Empty;
        public string ClassR = string.Empty;
        public string ClassS = string.Empty;
        public string ClassT = string.Empty;
        public string ClassU = string.Empty;
        public string ClassV = string.Empty;
        public string ClassW = string.Empty;
        public string ClassX = string.Empty;
        public string ClassY = string.Empty;
        public string ClassZ = string.Empty;
        public decimal NumA = 0;
        public decimal NumB = 0;
        public decimal NumC = 0;
        public decimal NumD = 0;
        public decimal NumE = 0;
        public decimal NumF = 0;
        public decimal NumG = 0;
        public decimal NumH = 0;
        public decimal NumI = 0;
        public decimal NumJ = 0;
        public decimal NumK = 0;
        public decimal NumL = 0;
        public decimal NumM = 0;
        public decimal NumN = 0;
        public decimal NumO = 0;
        public decimal NumP = 0;
        public decimal NumQ = 0;
        public decimal NumR = 0;
        public decimal NumS = 0;
        public decimal NumT = 0;
        public decimal NumU = 0;
        public decimal NumV = 0;
        public decimal NumW = 0;
        public decimal NumX = 0;
        public decimal NumY = 0;
        public decimal NumZ = 0;
        public DateTime DateA = 0.ToDateTime();
        public DateTime DateB = 0.ToDateTime();
        public DateTime DateC = 0.ToDateTime();
        public DateTime DateD = 0.ToDateTime();
        public DateTime DateE = 0.ToDateTime();
        public DateTime DateF = 0.ToDateTime();
        public DateTime DateG = 0.ToDateTime();
        public DateTime DateH = 0.ToDateTime();
        public DateTime DateI = 0.ToDateTime();
        public DateTime DateJ = 0.ToDateTime();
        public DateTime DateK = 0.ToDateTime();
        public DateTime DateL = 0.ToDateTime();
        public DateTime DateM = 0.ToDateTime();
        public DateTime DateN = 0.ToDateTime();
        public DateTime DateO = 0.ToDateTime();
        public DateTime DateP = 0.ToDateTime();
        public DateTime DateQ = 0.ToDateTime();
        public DateTime DateR = 0.ToDateTime();
        public DateTime DateS = 0.ToDateTime();
        public DateTime DateT = 0.ToDateTime();
        public DateTime DateU = 0.ToDateTime();
        public DateTime DateV = 0.ToDateTime();
        public DateTime DateW = 0.ToDateTime();
        public DateTime DateX = 0.ToDateTime();
        public DateTime DateY = 0.ToDateTime();
        public DateTime DateZ = 0.ToDateTime();
        public string DescriptionA = string.Empty;
        public string DescriptionB = string.Empty;
        public string DescriptionC = string.Empty;
        public string DescriptionD = string.Empty;
        public string DescriptionE = string.Empty;
        public string DescriptionF = string.Empty;
        public string DescriptionG = string.Empty;
        public string DescriptionH = string.Empty;
        public string DescriptionI = string.Empty;
        public string DescriptionJ = string.Empty;
        public string DescriptionK = string.Empty;
        public string DescriptionL = string.Empty;
        public string DescriptionM = string.Empty;
        public string DescriptionN = string.Empty;
        public string DescriptionO = string.Empty;
        public string DescriptionP = string.Empty;
        public string DescriptionQ = string.Empty;
        public string DescriptionR = string.Empty;
        public string DescriptionS = string.Empty;
        public string DescriptionT = string.Empty;
        public string DescriptionU = string.Empty;
        public string DescriptionV = string.Empty;
        public string DescriptionW = string.Empty;
        public string DescriptionX = string.Empty;
        public string DescriptionY = string.Empty;
        public string DescriptionZ = string.Empty;
        public bool CheckA = false;
        public bool CheckB = false;
        public bool CheckC = false;
        public bool CheckD = false;
        public bool CheckE = false;
        public bool CheckF = false;
        public bool CheckG = false;
        public bool CheckH = false;
        public bool CheckI = false;
        public bool CheckJ = false;
        public bool CheckK = false;
        public bool CheckL = false;
        public bool CheckM = false;
        public bool CheckN = false;
        public bool CheckO = false;
        public bool CheckP = false;
        public bool CheckQ = false;
        public bool CheckR = false;
        public bool CheckS = false;
        public bool CheckT = false;
        public bool CheckU = false;
        public bool CheckV = false;
        public bool CheckW = false;
        public bool CheckX = false;
        public bool CheckY = false;
        public bool CheckZ = false;
        public string LdapSearchRoot = string.Empty;
        public DateTime SynchronizedTime = 0.ToDateTime();

        public TimeZoneInfo TimeZoneInfo
        {
            get
            {
                return TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(o => o.Id == TimeZone);
            }
        }

        public Dept Dept
        {
            get
            {
                return SiteInfo.Dept(DeptId);
            }
        }

        public Title Title
        {
            get
            {
                return new Title(UserId, Name);
            }
        }

        [NonSerialized] public int SavedTenantId = Sessions.TenantId();
        [NonSerialized] public int SavedUserId = 0;
        [NonSerialized] public string SavedLoginId = string.Empty;
        [NonSerialized] public string SavedGlobalId = string.Empty;
        [NonSerialized] public string SavedName = string.Empty;
        [NonSerialized] public string SavedUserCode = string.Empty;
        [NonSerialized] public string SavedPassword = string.Empty;
        [NonSerialized] public string SavedPasswordValidate = string.Empty;
        [NonSerialized] public string SavedPasswordDummy = string.Empty;
        [NonSerialized] public bool SavedRememberMe = false;
        [NonSerialized] public string SavedLastName = string.Empty;
        [NonSerialized] public string SavedFirstName = string.Empty;
        [NonSerialized] public DateTime SavedBirthday = 0.ToDateTime();
        [NonSerialized] public string SavedGender = string.Empty;
        [NonSerialized] public string SavedLanguage = "ja";
        [NonSerialized] public string SavedTimeZone = "Tokyo Standard Time";
        [NonSerialized] public string SavedDeptCode = string.Empty;
        [NonSerialized] public int SavedDeptId = 0;
        [NonSerialized] public int SavedFirstAndLastNameOrder = 2;
        [NonSerialized] public string SavedBody = string.Empty;
        [NonSerialized] public DateTime SavedLastLoginTime = 0.ToDateTime();
        [NonSerialized] public DateTime SavedPasswordExpirationTime = 0.ToDateTime();
        [NonSerialized] public DateTime SavedPasswordChangeTime = 0.ToDateTime();
        [NonSerialized] public int SavedNumberOfLogins = 0;
        [NonSerialized] public int SavedNumberOfDenial = 0;
        [NonSerialized] public bool SavedTenantManager = false;
        [NonSerialized] public bool SavedServiceManager = false;
        [NonSerialized] public bool SavedDisabled = false;
        [NonSerialized] public bool SavedDeveloper = false;
        [NonSerialized] public string SavedUserSettings = "[]";
        [NonSerialized] public string SavedApiKey = string.Empty;
        [NonSerialized] public string SavedOldPassword = string.Empty;
        [NonSerialized] public string SavedChangedPassword = string.Empty;
        [NonSerialized] public string SavedChangedPasswordValidator = string.Empty;
        [NonSerialized] public string SavedAfterResetPassword = string.Empty;
        [NonSerialized] public string SavedAfterResetPasswordValidator = string.Empty;
        [NonSerialized] public string SavedMailAddresses = string.Empty;
        [NonSerialized] public string SavedDemoMailAddress = string.Empty;
        [NonSerialized] public string SavedSessionGuid = string.Empty;
        [NonSerialized] public string SavedClassA = string.Empty;
        [NonSerialized] public string SavedClassB = string.Empty;
        [NonSerialized] public string SavedClassC = string.Empty;
        [NonSerialized] public string SavedClassD = string.Empty;
        [NonSerialized] public string SavedClassE = string.Empty;
        [NonSerialized] public string SavedClassF = string.Empty;
        [NonSerialized] public string SavedClassG = string.Empty;
        [NonSerialized] public string SavedClassH = string.Empty;
        [NonSerialized] public string SavedClassI = string.Empty;
        [NonSerialized] public string SavedClassJ = string.Empty;
        [NonSerialized] public string SavedClassK = string.Empty;
        [NonSerialized] public string SavedClassL = string.Empty;
        [NonSerialized] public string SavedClassM = string.Empty;
        [NonSerialized] public string SavedClassN = string.Empty;
        [NonSerialized] public string SavedClassO = string.Empty;
        [NonSerialized] public string SavedClassP = string.Empty;
        [NonSerialized] public string SavedClassQ = string.Empty;
        [NonSerialized] public string SavedClassR = string.Empty;
        [NonSerialized] public string SavedClassS = string.Empty;
        [NonSerialized] public string SavedClassT = string.Empty;
        [NonSerialized] public string SavedClassU = string.Empty;
        [NonSerialized] public string SavedClassV = string.Empty;
        [NonSerialized] public string SavedClassW = string.Empty;
        [NonSerialized] public string SavedClassX = string.Empty;
        [NonSerialized] public string SavedClassY = string.Empty;
        [NonSerialized] public string SavedClassZ = string.Empty;
        [NonSerialized] public decimal SavedNumA = 0;
        [NonSerialized] public decimal SavedNumB = 0;
        [NonSerialized] public decimal SavedNumC = 0;
        [NonSerialized] public decimal SavedNumD = 0;
        [NonSerialized] public decimal SavedNumE = 0;
        [NonSerialized] public decimal SavedNumF = 0;
        [NonSerialized] public decimal SavedNumG = 0;
        [NonSerialized] public decimal SavedNumH = 0;
        [NonSerialized] public decimal SavedNumI = 0;
        [NonSerialized] public decimal SavedNumJ = 0;
        [NonSerialized] public decimal SavedNumK = 0;
        [NonSerialized] public decimal SavedNumL = 0;
        [NonSerialized] public decimal SavedNumM = 0;
        [NonSerialized] public decimal SavedNumN = 0;
        [NonSerialized] public decimal SavedNumO = 0;
        [NonSerialized] public decimal SavedNumP = 0;
        [NonSerialized] public decimal SavedNumQ = 0;
        [NonSerialized] public decimal SavedNumR = 0;
        [NonSerialized] public decimal SavedNumS = 0;
        [NonSerialized] public decimal SavedNumT = 0;
        [NonSerialized] public decimal SavedNumU = 0;
        [NonSerialized] public decimal SavedNumV = 0;
        [NonSerialized] public decimal SavedNumW = 0;
        [NonSerialized] public decimal SavedNumX = 0;
        [NonSerialized] public decimal SavedNumY = 0;
        [NonSerialized] public decimal SavedNumZ = 0;
        [NonSerialized] public DateTime SavedDateA = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateB = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateC = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateD = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateE = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateF = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateG = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateH = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateI = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateJ = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateK = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateL = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateM = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateN = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateO = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateP = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateQ = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateR = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateS = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateT = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateU = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateV = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateW = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateX = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateY = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateZ = 0.ToDateTime();
        [NonSerialized] public string SavedDescriptionA = string.Empty;
        [NonSerialized] public string SavedDescriptionB = string.Empty;
        [NonSerialized] public string SavedDescriptionC = string.Empty;
        [NonSerialized] public string SavedDescriptionD = string.Empty;
        [NonSerialized] public string SavedDescriptionE = string.Empty;
        [NonSerialized] public string SavedDescriptionF = string.Empty;
        [NonSerialized] public string SavedDescriptionG = string.Empty;
        [NonSerialized] public string SavedDescriptionH = string.Empty;
        [NonSerialized] public string SavedDescriptionI = string.Empty;
        [NonSerialized] public string SavedDescriptionJ = string.Empty;
        [NonSerialized] public string SavedDescriptionK = string.Empty;
        [NonSerialized] public string SavedDescriptionL = string.Empty;
        [NonSerialized] public string SavedDescriptionM = string.Empty;
        [NonSerialized] public string SavedDescriptionN = string.Empty;
        [NonSerialized] public string SavedDescriptionO = string.Empty;
        [NonSerialized] public string SavedDescriptionP = string.Empty;
        [NonSerialized] public string SavedDescriptionQ = string.Empty;
        [NonSerialized] public string SavedDescriptionR = string.Empty;
        [NonSerialized] public string SavedDescriptionS = string.Empty;
        [NonSerialized] public string SavedDescriptionT = string.Empty;
        [NonSerialized] public string SavedDescriptionU = string.Empty;
        [NonSerialized] public string SavedDescriptionV = string.Empty;
        [NonSerialized] public string SavedDescriptionW = string.Empty;
        [NonSerialized] public string SavedDescriptionX = string.Empty;
        [NonSerialized] public string SavedDescriptionY = string.Empty;
        [NonSerialized] public string SavedDescriptionZ = string.Empty;
        [NonSerialized] public bool SavedCheckA = false;
        [NonSerialized] public bool SavedCheckB = false;
        [NonSerialized] public bool SavedCheckC = false;
        [NonSerialized] public bool SavedCheckD = false;
        [NonSerialized] public bool SavedCheckE = false;
        [NonSerialized] public bool SavedCheckF = false;
        [NonSerialized] public bool SavedCheckG = false;
        [NonSerialized] public bool SavedCheckH = false;
        [NonSerialized] public bool SavedCheckI = false;
        [NonSerialized] public bool SavedCheckJ = false;
        [NonSerialized] public bool SavedCheckK = false;
        [NonSerialized] public bool SavedCheckL = false;
        [NonSerialized] public bool SavedCheckM = false;
        [NonSerialized] public bool SavedCheckN = false;
        [NonSerialized] public bool SavedCheckO = false;
        [NonSerialized] public bool SavedCheckP = false;
        [NonSerialized] public bool SavedCheckQ = false;
        [NonSerialized] public bool SavedCheckR = false;
        [NonSerialized] public bool SavedCheckS = false;
        [NonSerialized] public bool SavedCheckT = false;
        [NonSerialized] public bool SavedCheckU = false;
        [NonSerialized] public bool SavedCheckV = false;
        [NonSerialized] public bool SavedCheckW = false;
        [NonSerialized] public bool SavedCheckX = false;
        [NonSerialized] public bool SavedCheckY = false;
        [NonSerialized] public bool SavedCheckZ = false;
        [NonSerialized] public string SavedLdapSearchRoot = string.Empty;
        [NonSerialized] public DateTime SavedSynchronizedTime = 0.ToDateTime();

        public bool TenantId_Updated(Column column = null)
        {
            return TenantId != SavedTenantId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != TenantId);
        }

        public bool UserId_Updated(Column column = null)
        {
            return UserId != SavedUserId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != UserId);
        }

        public bool LoginId_Updated(Column column = null)
        {
            return LoginId != SavedLoginId && LoginId != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != LoginId);
        }

        public bool GlobalId_Updated(Column column = null)
        {
            return GlobalId != SavedGlobalId && GlobalId != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != GlobalId);
        }

        public bool Name_Updated(Column column = null)
        {
            return Name != SavedName && Name != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != Name);
        }

        public bool UserCode_Updated(Column column = null)
        {
            return UserCode != SavedUserCode && UserCode != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != UserCode);
        }

        public bool Password_Updated(Column column = null)
        {
            return Password != SavedPassword && Password != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != Password);
        }

        public bool LastName_Updated(Column column = null)
        {
            return LastName != SavedLastName && LastName != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != LastName);
        }

        public bool FirstName_Updated(Column column = null)
        {
            return FirstName != SavedFirstName && FirstName != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != FirstName);
        }

        public bool Gender_Updated(Column column = null)
        {
            return Gender != SavedGender && Gender != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != Gender);
        }

        public bool Language_Updated(Column column = null)
        {
            return Language != SavedLanguage && Language != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != Language);
        }

        public bool TimeZone_Updated(Column column = null)
        {
            return TimeZone != SavedTimeZone && TimeZone != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != TimeZone);
        }

        public bool DeptId_Updated(Column column = null)
        {
            return DeptId != SavedDeptId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != DeptId);
        }

        public bool FirstAndLastNameOrder_Updated(Column column = null)
        {
            return FirstAndLastNameOrder.ToInt() != SavedFirstAndLastNameOrder &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != FirstAndLastNameOrder.ToInt());
        }

        public bool Body_Updated(Column column = null)
        {
            return Body != SavedBody && Body != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != Body);
        }

        public bool NumberOfLogins_Updated(Column column = null)
        {
            return NumberOfLogins != SavedNumberOfLogins &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != NumberOfLogins);
        }

        public bool NumberOfDenial_Updated(Column column = null)
        {
            return NumberOfDenial != SavedNumberOfDenial &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != NumberOfDenial);
        }

        public bool TenantManager_Updated(Column column = null)
        {
            return TenantManager != SavedTenantManager &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != TenantManager);
        }

        public bool ServiceManager_Updated(Column column = null)
        {
            return ServiceManager != SavedServiceManager &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != ServiceManager);
        }

        public bool Disabled_Updated(Column column = null)
        {
            return Disabled != SavedDisabled &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != Disabled);
        }

        public bool Developer_Updated(Column column = null)
        {
            return Developer != SavedDeveloper &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != Developer);
        }

        public bool UserSettings_Updated(Column column = null)
        {
            return UserSettings.RecordingJson() != SavedUserSettings && UserSettings.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != UserSettings.RecordingJson());
        }

        public bool ApiKey_Updated(Column column = null)
        {
            return ApiKey != SavedApiKey && ApiKey != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ApiKey);
        }

        public bool ClassA_Updated(Column column = null)
        {
            return ClassA != SavedClassA && ClassA != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ClassA);
        }

        public bool ClassB_Updated(Column column = null)
        {
            return ClassB != SavedClassB && ClassB != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ClassB);
        }

        public bool ClassC_Updated(Column column = null)
        {
            return ClassC != SavedClassC && ClassC != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ClassC);
        }

        public bool ClassD_Updated(Column column = null)
        {
            return ClassD != SavedClassD && ClassD != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ClassD);
        }

        public bool ClassE_Updated(Column column = null)
        {
            return ClassE != SavedClassE && ClassE != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ClassE);
        }

        public bool ClassF_Updated(Column column = null)
        {
            return ClassF != SavedClassF && ClassF != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ClassF);
        }

        public bool ClassG_Updated(Column column = null)
        {
            return ClassG != SavedClassG && ClassG != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ClassG);
        }

        public bool ClassH_Updated(Column column = null)
        {
            return ClassH != SavedClassH && ClassH != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ClassH);
        }

        public bool ClassI_Updated(Column column = null)
        {
            return ClassI != SavedClassI && ClassI != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ClassI);
        }

        public bool ClassJ_Updated(Column column = null)
        {
            return ClassJ != SavedClassJ && ClassJ != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ClassJ);
        }

        public bool ClassK_Updated(Column column = null)
        {
            return ClassK != SavedClassK && ClassK != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ClassK);
        }

        public bool ClassL_Updated(Column column = null)
        {
            return ClassL != SavedClassL && ClassL != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ClassL);
        }

        public bool ClassM_Updated(Column column = null)
        {
            return ClassM != SavedClassM && ClassM != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ClassM);
        }

        public bool ClassN_Updated(Column column = null)
        {
            return ClassN != SavedClassN && ClassN != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ClassN);
        }

        public bool ClassO_Updated(Column column = null)
        {
            return ClassO != SavedClassO && ClassO != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ClassO);
        }

        public bool ClassP_Updated(Column column = null)
        {
            return ClassP != SavedClassP && ClassP != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ClassP);
        }

        public bool ClassQ_Updated(Column column = null)
        {
            return ClassQ != SavedClassQ && ClassQ != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ClassQ);
        }

        public bool ClassR_Updated(Column column = null)
        {
            return ClassR != SavedClassR && ClassR != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ClassR);
        }

        public bool ClassS_Updated(Column column = null)
        {
            return ClassS != SavedClassS && ClassS != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ClassS);
        }

        public bool ClassT_Updated(Column column = null)
        {
            return ClassT != SavedClassT && ClassT != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ClassT);
        }

        public bool ClassU_Updated(Column column = null)
        {
            return ClassU != SavedClassU && ClassU != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ClassU);
        }

        public bool ClassV_Updated(Column column = null)
        {
            return ClassV != SavedClassV && ClassV != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ClassV);
        }

        public bool ClassW_Updated(Column column = null)
        {
            return ClassW != SavedClassW && ClassW != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ClassW);
        }

        public bool ClassX_Updated(Column column = null)
        {
            return ClassX != SavedClassX && ClassX != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ClassX);
        }

        public bool ClassY_Updated(Column column = null)
        {
            return ClassY != SavedClassY && ClassY != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ClassY);
        }

        public bool ClassZ_Updated(Column column = null)
        {
            return ClassZ != SavedClassZ && ClassZ != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ClassZ);
        }

        public bool NumA_Updated(Column column = null)
        {
            return NumA != SavedNumA &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToDecimal() != NumA);
        }

        public bool NumB_Updated(Column column = null)
        {
            return NumB != SavedNumB &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToDecimal() != NumB);
        }

        public bool NumC_Updated(Column column = null)
        {
            return NumC != SavedNumC &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToDecimal() != NumC);
        }

        public bool NumD_Updated(Column column = null)
        {
            return NumD != SavedNumD &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToDecimal() != NumD);
        }

        public bool NumE_Updated(Column column = null)
        {
            return NumE != SavedNumE &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToDecimal() != NumE);
        }

        public bool NumF_Updated(Column column = null)
        {
            return NumF != SavedNumF &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToDecimal() != NumF);
        }

        public bool NumG_Updated(Column column = null)
        {
            return NumG != SavedNumG &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToDecimal() != NumG);
        }

        public bool NumH_Updated(Column column = null)
        {
            return NumH != SavedNumH &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToDecimal() != NumH);
        }

        public bool NumI_Updated(Column column = null)
        {
            return NumI != SavedNumI &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToDecimal() != NumI);
        }

        public bool NumJ_Updated(Column column = null)
        {
            return NumJ != SavedNumJ &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToDecimal() != NumJ);
        }

        public bool NumK_Updated(Column column = null)
        {
            return NumK != SavedNumK &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToDecimal() != NumK);
        }

        public bool NumL_Updated(Column column = null)
        {
            return NumL != SavedNumL &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToDecimal() != NumL);
        }

        public bool NumM_Updated(Column column = null)
        {
            return NumM != SavedNumM &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToDecimal() != NumM);
        }

        public bool NumN_Updated(Column column = null)
        {
            return NumN != SavedNumN &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToDecimal() != NumN);
        }

        public bool NumO_Updated(Column column = null)
        {
            return NumO != SavedNumO &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToDecimal() != NumO);
        }

        public bool NumP_Updated(Column column = null)
        {
            return NumP != SavedNumP &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToDecimal() != NumP);
        }

        public bool NumQ_Updated(Column column = null)
        {
            return NumQ != SavedNumQ &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToDecimal() != NumQ);
        }

        public bool NumR_Updated(Column column = null)
        {
            return NumR != SavedNumR &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToDecimal() != NumR);
        }

        public bool NumS_Updated(Column column = null)
        {
            return NumS != SavedNumS &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToDecimal() != NumS);
        }

        public bool NumT_Updated(Column column = null)
        {
            return NumT != SavedNumT &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToDecimal() != NumT);
        }

        public bool NumU_Updated(Column column = null)
        {
            return NumU != SavedNumU &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToDecimal() != NumU);
        }

        public bool NumV_Updated(Column column = null)
        {
            return NumV != SavedNumV &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToDecimal() != NumV);
        }

        public bool NumW_Updated(Column column = null)
        {
            return NumW != SavedNumW &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToDecimal() != NumW);
        }

        public bool NumX_Updated(Column column = null)
        {
            return NumX != SavedNumX &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToDecimal() != NumX);
        }

        public bool NumY_Updated(Column column = null)
        {
            return NumY != SavedNumY &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToDecimal() != NumY);
        }

        public bool NumZ_Updated(Column column = null)
        {
            return NumZ != SavedNumZ &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToDecimal() != NumZ);
        }

        public bool DescriptionA_Updated(Column column = null)
        {
            return DescriptionA != SavedDescriptionA && DescriptionA != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != DescriptionA);
        }

        public bool DescriptionB_Updated(Column column = null)
        {
            return DescriptionB != SavedDescriptionB && DescriptionB != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != DescriptionB);
        }

        public bool DescriptionC_Updated(Column column = null)
        {
            return DescriptionC != SavedDescriptionC && DescriptionC != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != DescriptionC);
        }

        public bool DescriptionD_Updated(Column column = null)
        {
            return DescriptionD != SavedDescriptionD && DescriptionD != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != DescriptionD);
        }

        public bool DescriptionE_Updated(Column column = null)
        {
            return DescriptionE != SavedDescriptionE && DescriptionE != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != DescriptionE);
        }

        public bool DescriptionF_Updated(Column column = null)
        {
            return DescriptionF != SavedDescriptionF && DescriptionF != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != DescriptionF);
        }

        public bool DescriptionG_Updated(Column column = null)
        {
            return DescriptionG != SavedDescriptionG && DescriptionG != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != DescriptionG);
        }

        public bool DescriptionH_Updated(Column column = null)
        {
            return DescriptionH != SavedDescriptionH && DescriptionH != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != DescriptionH);
        }

        public bool DescriptionI_Updated(Column column = null)
        {
            return DescriptionI != SavedDescriptionI && DescriptionI != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != DescriptionI);
        }

        public bool DescriptionJ_Updated(Column column = null)
        {
            return DescriptionJ != SavedDescriptionJ && DescriptionJ != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != DescriptionJ);
        }

        public bool DescriptionK_Updated(Column column = null)
        {
            return DescriptionK != SavedDescriptionK && DescriptionK != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != DescriptionK);
        }

        public bool DescriptionL_Updated(Column column = null)
        {
            return DescriptionL != SavedDescriptionL && DescriptionL != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != DescriptionL);
        }

        public bool DescriptionM_Updated(Column column = null)
        {
            return DescriptionM != SavedDescriptionM && DescriptionM != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != DescriptionM);
        }

        public bool DescriptionN_Updated(Column column = null)
        {
            return DescriptionN != SavedDescriptionN && DescriptionN != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != DescriptionN);
        }

        public bool DescriptionO_Updated(Column column = null)
        {
            return DescriptionO != SavedDescriptionO && DescriptionO != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != DescriptionO);
        }

        public bool DescriptionP_Updated(Column column = null)
        {
            return DescriptionP != SavedDescriptionP && DescriptionP != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != DescriptionP);
        }

        public bool DescriptionQ_Updated(Column column = null)
        {
            return DescriptionQ != SavedDescriptionQ && DescriptionQ != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != DescriptionQ);
        }

        public bool DescriptionR_Updated(Column column = null)
        {
            return DescriptionR != SavedDescriptionR && DescriptionR != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != DescriptionR);
        }

        public bool DescriptionS_Updated(Column column = null)
        {
            return DescriptionS != SavedDescriptionS && DescriptionS != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != DescriptionS);
        }

        public bool DescriptionT_Updated(Column column = null)
        {
            return DescriptionT != SavedDescriptionT && DescriptionT != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != DescriptionT);
        }

        public bool DescriptionU_Updated(Column column = null)
        {
            return DescriptionU != SavedDescriptionU && DescriptionU != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != DescriptionU);
        }

        public bool DescriptionV_Updated(Column column = null)
        {
            return DescriptionV != SavedDescriptionV && DescriptionV != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != DescriptionV);
        }

        public bool DescriptionW_Updated(Column column = null)
        {
            return DescriptionW != SavedDescriptionW && DescriptionW != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != DescriptionW);
        }

        public bool DescriptionX_Updated(Column column = null)
        {
            return DescriptionX != SavedDescriptionX && DescriptionX != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != DescriptionX);
        }

        public bool DescriptionY_Updated(Column column = null)
        {
            return DescriptionY != SavedDescriptionY && DescriptionY != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != DescriptionY);
        }

        public bool DescriptionZ_Updated(Column column = null)
        {
            return DescriptionZ != SavedDescriptionZ && DescriptionZ != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != DescriptionZ);
        }

        public bool CheckA_Updated(Column column = null)
        {
            return CheckA != SavedCheckA &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != CheckA);
        }

        public bool CheckB_Updated(Column column = null)
        {
            return CheckB != SavedCheckB &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != CheckB);
        }

        public bool CheckC_Updated(Column column = null)
        {
            return CheckC != SavedCheckC &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != CheckC);
        }

        public bool CheckD_Updated(Column column = null)
        {
            return CheckD != SavedCheckD &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != CheckD);
        }

        public bool CheckE_Updated(Column column = null)
        {
            return CheckE != SavedCheckE &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != CheckE);
        }

        public bool CheckF_Updated(Column column = null)
        {
            return CheckF != SavedCheckF &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != CheckF);
        }

        public bool CheckG_Updated(Column column = null)
        {
            return CheckG != SavedCheckG &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != CheckG);
        }

        public bool CheckH_Updated(Column column = null)
        {
            return CheckH != SavedCheckH &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != CheckH);
        }

        public bool CheckI_Updated(Column column = null)
        {
            return CheckI != SavedCheckI &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != CheckI);
        }

        public bool CheckJ_Updated(Column column = null)
        {
            return CheckJ != SavedCheckJ &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != CheckJ);
        }

        public bool CheckK_Updated(Column column = null)
        {
            return CheckK != SavedCheckK &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != CheckK);
        }

        public bool CheckL_Updated(Column column = null)
        {
            return CheckL != SavedCheckL &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != CheckL);
        }

        public bool CheckM_Updated(Column column = null)
        {
            return CheckM != SavedCheckM &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != CheckM);
        }

        public bool CheckN_Updated(Column column = null)
        {
            return CheckN != SavedCheckN &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != CheckN);
        }

        public bool CheckO_Updated(Column column = null)
        {
            return CheckO != SavedCheckO &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != CheckO);
        }

        public bool CheckP_Updated(Column column = null)
        {
            return CheckP != SavedCheckP &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != CheckP);
        }

        public bool CheckQ_Updated(Column column = null)
        {
            return CheckQ != SavedCheckQ &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != CheckQ);
        }

        public bool CheckR_Updated(Column column = null)
        {
            return CheckR != SavedCheckR &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != CheckR);
        }

        public bool CheckS_Updated(Column column = null)
        {
            return CheckS != SavedCheckS &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != CheckS);
        }

        public bool CheckT_Updated(Column column = null)
        {
            return CheckT != SavedCheckT &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != CheckT);
        }

        public bool CheckU_Updated(Column column = null)
        {
            return CheckU != SavedCheckU &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != CheckU);
        }

        public bool CheckV_Updated(Column column = null)
        {
            return CheckV != SavedCheckV &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != CheckV);
        }

        public bool CheckW_Updated(Column column = null)
        {
            return CheckW != SavedCheckW &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != CheckW);
        }

        public bool CheckX_Updated(Column column = null)
        {
            return CheckX != SavedCheckX &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != CheckX);
        }

        public bool CheckY_Updated(Column column = null)
        {
            return CheckY != SavedCheckY &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != CheckY);
        }

        public bool CheckZ_Updated(Column column = null)
        {
            return CheckZ != SavedCheckZ &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != CheckZ);
        }

        public bool LdapSearchRoot_Updated(Column column = null)
        {
            return LdapSearchRoot != SavedLdapSearchRoot && LdapSearchRoot != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != LdapSearchRoot);
        }

        public bool Birthday_Updated(Column column = null)
        {
            return Birthday.Value != SavedBirthday &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != Birthday.Value.Date);
        }

        public bool LastLoginTime_Updated(Column column = null)
        {
            return LastLoginTime.Value != SavedLastLoginTime &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != LastLoginTime.Value.Date);
        }

        public bool PasswordExpirationTime_Updated(Column column = null)
        {
            return PasswordExpirationTime.Value != SavedPasswordExpirationTime &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != PasswordExpirationTime.Value.Date);
        }

        public bool PasswordChangeTime_Updated(Column column = null)
        {
            return PasswordChangeTime.Value != SavedPasswordChangeTime &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != PasswordChangeTime.Value.Date);
        }

        public bool DateA_Updated(Column column = null)
        {
            return DateA != SavedDateA &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateA.Date);
        }

        public bool DateB_Updated(Column column = null)
        {
            return DateB != SavedDateB &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateB.Date);
        }

        public bool DateC_Updated(Column column = null)
        {
            return DateC != SavedDateC &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateC.Date);
        }

        public bool DateD_Updated(Column column = null)
        {
            return DateD != SavedDateD &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateD.Date);
        }

        public bool DateE_Updated(Column column = null)
        {
            return DateE != SavedDateE &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateE.Date);
        }

        public bool DateF_Updated(Column column = null)
        {
            return DateF != SavedDateF &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateF.Date);
        }

        public bool DateG_Updated(Column column = null)
        {
            return DateG != SavedDateG &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateG.Date);
        }

        public bool DateH_Updated(Column column = null)
        {
            return DateH != SavedDateH &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateH.Date);
        }

        public bool DateI_Updated(Column column = null)
        {
            return DateI != SavedDateI &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateI.Date);
        }

        public bool DateJ_Updated(Column column = null)
        {
            return DateJ != SavedDateJ &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateJ.Date);
        }

        public bool DateK_Updated(Column column = null)
        {
            return DateK != SavedDateK &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateK.Date);
        }

        public bool DateL_Updated(Column column = null)
        {
            return DateL != SavedDateL &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateL.Date);
        }

        public bool DateM_Updated(Column column = null)
        {
            return DateM != SavedDateM &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateM.Date);
        }

        public bool DateN_Updated(Column column = null)
        {
            return DateN != SavedDateN &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateN.Date);
        }

        public bool DateO_Updated(Column column = null)
        {
            return DateO != SavedDateO &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateO.Date);
        }

        public bool DateP_Updated(Column column = null)
        {
            return DateP != SavedDateP &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateP.Date);
        }

        public bool DateQ_Updated(Column column = null)
        {
            return DateQ != SavedDateQ &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateQ.Date);
        }

        public bool DateR_Updated(Column column = null)
        {
            return DateR != SavedDateR &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateR.Date);
        }

        public bool DateS_Updated(Column column = null)
        {
            return DateS != SavedDateS &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateS.Date);
        }

        public bool DateT_Updated(Column column = null)
        {
            return DateT != SavedDateT &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateT.Date);
        }

        public bool DateU_Updated(Column column = null)
        {
            return DateU != SavedDateU &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateU.Date);
        }

        public bool DateV_Updated(Column column = null)
        {
            return DateV != SavedDateV &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateV.Date);
        }

        public bool DateW_Updated(Column column = null)
        {
            return DateW != SavedDateW &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateW.Date);
        }

        public bool DateX_Updated(Column column = null)
        {
            return DateX != SavedDateX &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateX.Date);
        }

        public bool DateY_Updated(Column column = null)
        {
            return DateY != SavedDateY &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateY.Date);
        }

        public bool DateZ_Updated(Column column = null)
        {
            return DateZ != SavedDateZ &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateZ.Date);
        }

        public bool SynchronizedTime_Updated(Column column = null)
        {
            return SynchronizedTime != SavedSynchronizedTime &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != SynchronizedTime.Date);
        }

        public UserSettings Session_UserSettings()
        {
            return this.PageSession("UserSettings") != null
                ? this.PageSession("UserSettings")?.ToString().Deserialize<UserSettings>() ?? new UserSettings()
                : UserSettings;
        }

        public void Session_UserSettings(object value)
        {
            this.PageSession("UserSettings", value);
        }

        public List<string> Session_MailAddresses()
        {
            return this.PageSession("MailAddresses") != null
                ? this.PageSession("MailAddresses") as List<string>
                : MailAddresses;
        }

        public void Session_MailAddresses(object value)
        {
            this.PageSession("MailAddresses", value);
        }

        public List<int> SwitchTargets;

        public UserModel()
        {
        }

        public UserModel(
            SiteSettings ss,
            bool setByForm = false,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            if (setByForm) SetByForm(ss);
            if (setByApi) SetByApi(ss);
            MethodType = methodType;
            OnConstructed();
        }

        public UserModel(
            SiteSettings ss,
            int userId,
            bool clearSessions = false,
            bool setByForm = false,
            bool setByApi = false,
            List<int> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            UserId = userId;
            Get(ss);
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm(ss);
            if (setByApi) SetByApi(ss);
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed();
        }

        public UserModel(SiteSettings ss, DataRow dataRow, string tableAlias = null)
        {
            OnConstructing();
            Set(ss, dataRow, tableAlias);
            OnConstructed();
        }

        private void OnConstructing()
        {
        }

        private void OnConstructed()
        {
        }

        public void ClearSessions()
        {
            Session_UserSettings(null);
            Session_MailAddresses(null);
        }

        public UserModel Get(
            SiteSettings ss,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(ss, Rds.ExecuteTable(statements: Rds.SelectUsers(
                tableType: tableType,
                column: column ?? Rds.UsersDefaultColumns(),
                join: join ??  Rds.UsersJoinDefault(),
                where: where ?? Rds.UsersWhereDefault(this),
                orderBy: orderBy,
                param: param,
                distinct: distinct,
                top: top)));
            return this;
        }

        public UserApiModel GetByApi(SiteSettings ss)
        {
            var data = new UserApiModel();
            ss.ReadableColumns(noJoined: true).ForEach(column =>
            {
                switch (column.ColumnName)
                {
                    case "TenantId": data.TenantId = TenantId; break;
                    case "UserId": data.UserId = UserId; break;
                    case "Ver": data.Ver = Ver; break;
                    case "LoginId": data.LoginId = LoginId; break;
                    case "GlobalId": data.GlobalId = GlobalId; break;
                    case "Name": data.Name = Name; break;
                    case "UserCode": data.UserCode = UserCode; break;
                    case "Password": data.Password = Password; break;
                    case "LastName": data.LastName = LastName; break;
                    case "FirstName": data.FirstName = FirstName; break;
                    case "Birthday": data.Birthday = Birthday.Value.ToLocal(); break;
                    case "Gender": data.Gender = Gender; break;
                    case "Language": data.Language = Language; break;
                    case "TimeZone": data.TimeZone = TimeZone; break;
                    case "DeptCode": data.DeptCode = DeptCode; break;
                    case "DeptId": data.DeptId = DeptId; break;
                    case "FirstAndLastNameOrder": data.FirstAndLastNameOrder = FirstAndLastNameOrder.ToInt(); break;
                    case "Body": data.Body = Body; break;
                    case "LastLoginTime": data.LastLoginTime = LastLoginTime.Value.ToLocal(); break;
                    case "PasswordExpirationTime": data.PasswordExpirationTime = PasswordExpirationTime.Value.ToLocal(); break;
                    case "PasswordChangeTime": data.PasswordChangeTime = PasswordChangeTime.Value.ToLocal(); break;
                    case "NumberOfLogins": data.NumberOfLogins = NumberOfLogins; break;
                    case "NumberOfDenial": data.NumberOfDenial = NumberOfDenial; break;
                    case "TenantManager": data.TenantManager = TenantManager; break;
                    case "ServiceManager": data.ServiceManager = ServiceManager; break;
                    case "Disabled": data.Disabled = Disabled; break;
                    case "Developer": data.Developer = Developer; break;
                    case "UserSettings": data.UserSettings = UserSettings.RecordingJson(); break;
                    case "ApiKey": data.ApiKey = ApiKey; break;
                    case "ClassA": data.ClassA = ClassA; break;
                    case "ClassB": data.ClassB = ClassB; break;
                    case "ClassC": data.ClassC = ClassC; break;
                    case "ClassD": data.ClassD = ClassD; break;
                    case "ClassE": data.ClassE = ClassE; break;
                    case "ClassF": data.ClassF = ClassF; break;
                    case "ClassG": data.ClassG = ClassG; break;
                    case "ClassH": data.ClassH = ClassH; break;
                    case "ClassI": data.ClassI = ClassI; break;
                    case "ClassJ": data.ClassJ = ClassJ; break;
                    case "ClassK": data.ClassK = ClassK; break;
                    case "ClassL": data.ClassL = ClassL; break;
                    case "ClassM": data.ClassM = ClassM; break;
                    case "ClassN": data.ClassN = ClassN; break;
                    case "ClassO": data.ClassO = ClassO; break;
                    case "ClassP": data.ClassP = ClassP; break;
                    case "ClassQ": data.ClassQ = ClassQ; break;
                    case "ClassR": data.ClassR = ClassR; break;
                    case "ClassS": data.ClassS = ClassS; break;
                    case "ClassT": data.ClassT = ClassT; break;
                    case "ClassU": data.ClassU = ClassU; break;
                    case "ClassV": data.ClassV = ClassV; break;
                    case "ClassW": data.ClassW = ClassW; break;
                    case "ClassX": data.ClassX = ClassX; break;
                    case "ClassY": data.ClassY = ClassY; break;
                    case "ClassZ": data.ClassZ = ClassZ; break;
                    case "NumA": data.NumA = NumA; break;
                    case "NumB": data.NumB = NumB; break;
                    case "NumC": data.NumC = NumC; break;
                    case "NumD": data.NumD = NumD; break;
                    case "NumE": data.NumE = NumE; break;
                    case "NumF": data.NumF = NumF; break;
                    case "NumG": data.NumG = NumG; break;
                    case "NumH": data.NumH = NumH; break;
                    case "NumI": data.NumI = NumI; break;
                    case "NumJ": data.NumJ = NumJ; break;
                    case "NumK": data.NumK = NumK; break;
                    case "NumL": data.NumL = NumL; break;
                    case "NumM": data.NumM = NumM; break;
                    case "NumN": data.NumN = NumN; break;
                    case "NumO": data.NumO = NumO; break;
                    case "NumP": data.NumP = NumP; break;
                    case "NumQ": data.NumQ = NumQ; break;
                    case "NumR": data.NumR = NumR; break;
                    case "NumS": data.NumS = NumS; break;
                    case "NumT": data.NumT = NumT; break;
                    case "NumU": data.NumU = NumU; break;
                    case "NumV": data.NumV = NumV; break;
                    case "NumW": data.NumW = NumW; break;
                    case "NumX": data.NumX = NumX; break;
                    case "NumY": data.NumY = NumY; break;
                    case "NumZ": data.NumZ = NumZ; break;
                    case "DateA": data.DateA = DateA.ToLocal(); break;
                    case "DateB": data.DateB = DateB.ToLocal(); break;
                    case "DateC": data.DateC = DateC.ToLocal(); break;
                    case "DateD": data.DateD = DateD.ToLocal(); break;
                    case "DateE": data.DateE = DateE.ToLocal(); break;
                    case "DateF": data.DateF = DateF.ToLocal(); break;
                    case "DateG": data.DateG = DateG.ToLocal(); break;
                    case "DateH": data.DateH = DateH.ToLocal(); break;
                    case "DateI": data.DateI = DateI.ToLocal(); break;
                    case "DateJ": data.DateJ = DateJ.ToLocal(); break;
                    case "DateK": data.DateK = DateK.ToLocal(); break;
                    case "DateL": data.DateL = DateL.ToLocal(); break;
                    case "DateM": data.DateM = DateM.ToLocal(); break;
                    case "DateN": data.DateN = DateN.ToLocal(); break;
                    case "DateO": data.DateO = DateO.ToLocal(); break;
                    case "DateP": data.DateP = DateP.ToLocal(); break;
                    case "DateQ": data.DateQ = DateQ.ToLocal(); break;
                    case "DateR": data.DateR = DateR.ToLocal(); break;
                    case "DateS": data.DateS = DateS.ToLocal(); break;
                    case "DateT": data.DateT = DateT.ToLocal(); break;
                    case "DateU": data.DateU = DateU.ToLocal(); break;
                    case "DateV": data.DateV = DateV.ToLocal(); break;
                    case "DateW": data.DateW = DateW.ToLocal(); break;
                    case "DateX": data.DateX = DateX.ToLocal(); break;
                    case "DateY": data.DateY = DateY.ToLocal(); break;
                    case "DateZ": data.DateZ = DateZ.ToLocal(); break;
                    case "DescriptionA": data.DescriptionA = DescriptionA; break;
                    case "DescriptionB": data.DescriptionB = DescriptionB; break;
                    case "DescriptionC": data.DescriptionC = DescriptionC; break;
                    case "DescriptionD": data.DescriptionD = DescriptionD; break;
                    case "DescriptionE": data.DescriptionE = DescriptionE; break;
                    case "DescriptionF": data.DescriptionF = DescriptionF; break;
                    case "DescriptionG": data.DescriptionG = DescriptionG; break;
                    case "DescriptionH": data.DescriptionH = DescriptionH; break;
                    case "DescriptionI": data.DescriptionI = DescriptionI; break;
                    case "DescriptionJ": data.DescriptionJ = DescriptionJ; break;
                    case "DescriptionK": data.DescriptionK = DescriptionK; break;
                    case "DescriptionL": data.DescriptionL = DescriptionL; break;
                    case "DescriptionM": data.DescriptionM = DescriptionM; break;
                    case "DescriptionN": data.DescriptionN = DescriptionN; break;
                    case "DescriptionO": data.DescriptionO = DescriptionO; break;
                    case "DescriptionP": data.DescriptionP = DescriptionP; break;
                    case "DescriptionQ": data.DescriptionQ = DescriptionQ; break;
                    case "DescriptionR": data.DescriptionR = DescriptionR; break;
                    case "DescriptionS": data.DescriptionS = DescriptionS; break;
                    case "DescriptionT": data.DescriptionT = DescriptionT; break;
                    case "DescriptionU": data.DescriptionU = DescriptionU; break;
                    case "DescriptionV": data.DescriptionV = DescriptionV; break;
                    case "DescriptionW": data.DescriptionW = DescriptionW; break;
                    case "DescriptionX": data.DescriptionX = DescriptionX; break;
                    case "DescriptionY": data.DescriptionY = DescriptionY; break;
                    case "DescriptionZ": data.DescriptionZ = DescriptionZ; break;
                    case "CheckA": data.CheckA = CheckA; break;
                    case "CheckB": data.CheckB = CheckB; break;
                    case "CheckC": data.CheckC = CheckC; break;
                    case "CheckD": data.CheckD = CheckD; break;
                    case "CheckE": data.CheckE = CheckE; break;
                    case "CheckF": data.CheckF = CheckF; break;
                    case "CheckG": data.CheckG = CheckG; break;
                    case "CheckH": data.CheckH = CheckH; break;
                    case "CheckI": data.CheckI = CheckI; break;
                    case "CheckJ": data.CheckJ = CheckJ; break;
                    case "CheckK": data.CheckK = CheckK; break;
                    case "CheckL": data.CheckL = CheckL; break;
                    case "CheckM": data.CheckM = CheckM; break;
                    case "CheckN": data.CheckN = CheckN; break;
                    case "CheckO": data.CheckO = CheckO; break;
                    case "CheckP": data.CheckP = CheckP; break;
                    case "CheckQ": data.CheckQ = CheckQ; break;
                    case "CheckR": data.CheckR = CheckR; break;
                    case "CheckS": data.CheckS = CheckS; break;
                    case "CheckT": data.CheckT = CheckT; break;
                    case "CheckU": data.CheckU = CheckU; break;
                    case "CheckV": data.CheckV = CheckV; break;
                    case "CheckW": data.CheckW = CheckW; break;
                    case "CheckX": data.CheckX = CheckX; break;
                    case "CheckY": data.CheckY = CheckY; break;
                    case "CheckZ": data.CheckZ = CheckZ; break;
                    case "LdapSearchRoot": data.LdapSearchRoot = LdapSearchRoot; break;
                    case "SynchronizedTime": data.SynchronizedTime = SynchronizedTime.ToLocal(); break;
                    case "Creator": data.Creator = Creator.Id; break;
                    case "Updator": data.Updator = Updator.Id; break;
                    case "CreatedTime": data.CreatedTime = CreatedTime.Value.ToLocal(); break;
                    case "UpdatedTime": data.UpdatedTime = UpdatedTime.Value.ToLocal(); break;
                    case "Comments": data.Comments = Comments.ToLocal().ToJson(); break;
                }
            });
            return data;
        }

        public Error.Types Create(
            SiteSettings ss,
            RdsUser rdsUser = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool otherInitValue = false,
            bool get = true)
        {
            PasswordExpirationPeriod();
            var statements = new List<SqlStatement>();
            CreateStatements(ss, statements, tableType, param, otherInitValue);
            try
            {
                var response = Rds.ExecuteScalar_response(
                    rdsUser: rdsUser,
                    transactional: true,
                    selectIdentity: true,
                    statements: statements.ToArray());
                UserId = response.Identity.ToInt();
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                if (e.Number == 2601)
                {
                    return Error.Types.LoginIdAlreadyUse;
                }
                else
                {
                    throw;
                }
            }
            if (get) Get(ss);
            return Error.Types.None;
        }

        public List<SqlStatement> CreateStatements(
            SiteSettings ss,
            List<SqlStatement> statements,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool otherInitValue = false)
        {
            statements.AddRange(new List<SqlStatement>
            {
                Rds.InsertUsers(
                    tableType: tableType,
                    setIdentity: true,
                    param: param ?? Rds.UsersParamDefault(
                        this, setDefault: true, otherInitValue: otherInitValue)),
                StatusUtilities.UpdateStatus(StatusUtilities.Types.UsersUpdated),
            });
            return statements;
        }

        public Error.Types Update(
            SiteSettings ss,
            IEnumerable<string> permissions = null,
            bool permissionChanged = false,
            RdsUser rdsUser = null,
            SqlParamCollection param = null,
            List<SqlStatement> additionalStatements = null,
            bool otherInitValue = false,
            bool setBySession = true,
            bool get = true)
        {
            if (setBySession) SetBySession();
            var timestamp = Timestamp.ToDateTime();
            var statements = new List<SqlStatement>();
            UpdateStatements(statements, timestamp, param, otherInitValue, additionalStatements);
            try
            {
                var response = Rds.ExecuteScalar_response(
                    rdsUser: rdsUser,
                    transactional: true,
                    statements: statements.ToArray());
                if (response.Count == 0) return Error.Types.UpdateConflicts;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                if (e.Number == 2601)
                {
                    return Error.Types.LoginIdAlreadyUse;
                }
                else
                {
                    throw;
                }
            }
            if (get) Get(ss);
            UpdateMailAddresses();
            SetSiteInfo();
            return Error.Types.None;
        }

        private List<SqlStatement> UpdateStatements(
            List<SqlStatement> statements,
            DateTime timestamp,
            SqlParamCollection param,
            bool otherInitValue = false,
            List<SqlStatement> additionalStatements = null)
        {
            var where = Rds.UsersWhereDefault(this)
                .UpdatedTime(timestamp, _using: timestamp.InRange());
            if (VerUp)
            {
                statements.Add(CopyToStatement(where, Sqls.TableTypes.History));
                Ver++;
            }
            statements.AddRange(new List<SqlStatement>
            {
                Rds.UpdateUsers(
                    where: where,
                    param: param ?? Rds.UsersParamDefault(this, otherInitValue: otherInitValue),
                    countRecord: true),
                StatusUtilities.UpdateStatus(StatusUtilities.Types.UsersUpdated),
            });
            if (additionalStatements?.Any() == true)
            {
                statements.AddRange(additionalStatements);
            }
            return statements;
        }

        private SqlStatement CopyToStatement(SqlWhereCollection where, Sqls.TableTypes tableType)
        {
            var column = new Rds.UsersColumnCollection();
            var param = new Rds.UsersParamCollection();
            column.TenantId(function: Sqls.Functions.SingleColumn); param.TenantId();
            column.UserId(function: Sqls.Functions.SingleColumn); param.UserId();
            column.Ver(function: Sqls.Functions.SingleColumn); param.Ver();
            column.LoginId(function: Sqls.Functions.SingleColumn); param.LoginId();
            column.GlobalId(function: Sqls.Functions.SingleColumn); param.GlobalId();
            column.Name(function: Sqls.Functions.SingleColumn); param.Name();
            column.UserCode(function: Sqls.Functions.SingleColumn); param.UserCode();
            column.Password(function: Sqls.Functions.SingleColumn); param.Password();
            column.LastName(function: Sqls.Functions.SingleColumn); param.LastName();
            column.FirstName(function: Sqls.Functions.SingleColumn); param.FirstName();
            column.Birthday(function: Sqls.Functions.SingleColumn); param.Birthday();
            column.Gender(function: Sqls.Functions.SingleColumn); param.Gender();
            column.Language(function: Sqls.Functions.SingleColumn); param.Language();
            column.TimeZone(function: Sqls.Functions.SingleColumn); param.TimeZone();
            column.DeptId(function: Sqls.Functions.SingleColumn); param.DeptId();
            column.FirstAndLastNameOrder(function: Sqls.Functions.SingleColumn); param.FirstAndLastNameOrder();
            column.Body(function: Sqls.Functions.SingleColumn); param.Body();
            column.LastLoginTime(function: Sqls.Functions.SingleColumn); param.LastLoginTime();
            column.PasswordExpirationTime(function: Sqls.Functions.SingleColumn); param.PasswordExpirationTime();
            column.PasswordChangeTime(function: Sqls.Functions.SingleColumn); param.PasswordChangeTime();
            column.NumberOfLogins(function: Sqls.Functions.SingleColumn); param.NumberOfLogins();
            column.NumberOfDenial(function: Sqls.Functions.SingleColumn); param.NumberOfDenial();
            column.TenantManager(function: Sqls.Functions.SingleColumn); param.TenantManager();
            column.ServiceManager(function: Sqls.Functions.SingleColumn); param.ServiceManager();
            column.Disabled(function: Sqls.Functions.SingleColumn); param.Disabled();
            column.Developer(function: Sqls.Functions.SingleColumn); param.Developer();
            column.UserSettings(function: Sqls.Functions.SingleColumn); param.UserSettings();
            column.ApiKey(function: Sqls.Functions.SingleColumn); param.ApiKey();
            column.ClassA(function: Sqls.Functions.SingleColumn); param.ClassA();
            column.ClassB(function: Sqls.Functions.SingleColumn); param.ClassB();
            column.ClassC(function: Sqls.Functions.SingleColumn); param.ClassC();
            column.ClassD(function: Sqls.Functions.SingleColumn); param.ClassD();
            column.ClassE(function: Sqls.Functions.SingleColumn); param.ClassE();
            column.ClassF(function: Sqls.Functions.SingleColumn); param.ClassF();
            column.ClassG(function: Sqls.Functions.SingleColumn); param.ClassG();
            column.ClassH(function: Sqls.Functions.SingleColumn); param.ClassH();
            column.ClassI(function: Sqls.Functions.SingleColumn); param.ClassI();
            column.ClassJ(function: Sqls.Functions.SingleColumn); param.ClassJ();
            column.ClassK(function: Sqls.Functions.SingleColumn); param.ClassK();
            column.ClassL(function: Sqls.Functions.SingleColumn); param.ClassL();
            column.ClassM(function: Sqls.Functions.SingleColumn); param.ClassM();
            column.ClassN(function: Sqls.Functions.SingleColumn); param.ClassN();
            column.ClassO(function: Sqls.Functions.SingleColumn); param.ClassO();
            column.ClassP(function: Sqls.Functions.SingleColumn); param.ClassP();
            column.ClassQ(function: Sqls.Functions.SingleColumn); param.ClassQ();
            column.ClassR(function: Sqls.Functions.SingleColumn); param.ClassR();
            column.ClassS(function: Sqls.Functions.SingleColumn); param.ClassS();
            column.ClassT(function: Sqls.Functions.SingleColumn); param.ClassT();
            column.ClassU(function: Sqls.Functions.SingleColumn); param.ClassU();
            column.ClassV(function: Sqls.Functions.SingleColumn); param.ClassV();
            column.ClassW(function: Sqls.Functions.SingleColumn); param.ClassW();
            column.ClassX(function: Sqls.Functions.SingleColumn); param.ClassX();
            column.ClassY(function: Sqls.Functions.SingleColumn); param.ClassY();
            column.ClassZ(function: Sqls.Functions.SingleColumn); param.ClassZ();
            column.NumA(function: Sqls.Functions.SingleColumn); param.NumA();
            column.NumB(function: Sqls.Functions.SingleColumn); param.NumB();
            column.NumC(function: Sqls.Functions.SingleColumn); param.NumC();
            column.NumD(function: Sqls.Functions.SingleColumn); param.NumD();
            column.NumE(function: Sqls.Functions.SingleColumn); param.NumE();
            column.NumF(function: Sqls.Functions.SingleColumn); param.NumF();
            column.NumG(function: Sqls.Functions.SingleColumn); param.NumG();
            column.NumH(function: Sqls.Functions.SingleColumn); param.NumH();
            column.NumI(function: Sqls.Functions.SingleColumn); param.NumI();
            column.NumJ(function: Sqls.Functions.SingleColumn); param.NumJ();
            column.NumK(function: Sqls.Functions.SingleColumn); param.NumK();
            column.NumL(function: Sqls.Functions.SingleColumn); param.NumL();
            column.NumM(function: Sqls.Functions.SingleColumn); param.NumM();
            column.NumN(function: Sqls.Functions.SingleColumn); param.NumN();
            column.NumO(function: Sqls.Functions.SingleColumn); param.NumO();
            column.NumP(function: Sqls.Functions.SingleColumn); param.NumP();
            column.NumQ(function: Sqls.Functions.SingleColumn); param.NumQ();
            column.NumR(function: Sqls.Functions.SingleColumn); param.NumR();
            column.NumS(function: Sqls.Functions.SingleColumn); param.NumS();
            column.NumT(function: Sqls.Functions.SingleColumn); param.NumT();
            column.NumU(function: Sqls.Functions.SingleColumn); param.NumU();
            column.NumV(function: Sqls.Functions.SingleColumn); param.NumV();
            column.NumW(function: Sqls.Functions.SingleColumn); param.NumW();
            column.NumX(function: Sqls.Functions.SingleColumn); param.NumX();
            column.NumY(function: Sqls.Functions.SingleColumn); param.NumY();
            column.NumZ(function: Sqls.Functions.SingleColumn); param.NumZ();
            column.DateA(function: Sqls.Functions.SingleColumn); param.DateA();
            column.DateB(function: Sqls.Functions.SingleColumn); param.DateB();
            column.DateC(function: Sqls.Functions.SingleColumn); param.DateC();
            column.DateD(function: Sqls.Functions.SingleColumn); param.DateD();
            column.DateE(function: Sqls.Functions.SingleColumn); param.DateE();
            column.DateF(function: Sqls.Functions.SingleColumn); param.DateF();
            column.DateG(function: Sqls.Functions.SingleColumn); param.DateG();
            column.DateH(function: Sqls.Functions.SingleColumn); param.DateH();
            column.DateI(function: Sqls.Functions.SingleColumn); param.DateI();
            column.DateJ(function: Sqls.Functions.SingleColumn); param.DateJ();
            column.DateK(function: Sqls.Functions.SingleColumn); param.DateK();
            column.DateL(function: Sqls.Functions.SingleColumn); param.DateL();
            column.DateM(function: Sqls.Functions.SingleColumn); param.DateM();
            column.DateN(function: Sqls.Functions.SingleColumn); param.DateN();
            column.DateO(function: Sqls.Functions.SingleColumn); param.DateO();
            column.DateP(function: Sqls.Functions.SingleColumn); param.DateP();
            column.DateQ(function: Sqls.Functions.SingleColumn); param.DateQ();
            column.DateR(function: Sqls.Functions.SingleColumn); param.DateR();
            column.DateS(function: Sqls.Functions.SingleColumn); param.DateS();
            column.DateT(function: Sqls.Functions.SingleColumn); param.DateT();
            column.DateU(function: Sqls.Functions.SingleColumn); param.DateU();
            column.DateV(function: Sqls.Functions.SingleColumn); param.DateV();
            column.DateW(function: Sqls.Functions.SingleColumn); param.DateW();
            column.DateX(function: Sqls.Functions.SingleColumn); param.DateX();
            column.DateY(function: Sqls.Functions.SingleColumn); param.DateY();
            column.DateZ(function: Sqls.Functions.SingleColumn); param.DateZ();
            column.DescriptionA(function: Sqls.Functions.SingleColumn); param.DescriptionA();
            column.DescriptionB(function: Sqls.Functions.SingleColumn); param.DescriptionB();
            column.DescriptionC(function: Sqls.Functions.SingleColumn); param.DescriptionC();
            column.DescriptionD(function: Sqls.Functions.SingleColumn); param.DescriptionD();
            column.DescriptionE(function: Sqls.Functions.SingleColumn); param.DescriptionE();
            column.DescriptionF(function: Sqls.Functions.SingleColumn); param.DescriptionF();
            column.DescriptionG(function: Sqls.Functions.SingleColumn); param.DescriptionG();
            column.DescriptionH(function: Sqls.Functions.SingleColumn); param.DescriptionH();
            column.DescriptionI(function: Sqls.Functions.SingleColumn); param.DescriptionI();
            column.DescriptionJ(function: Sqls.Functions.SingleColumn); param.DescriptionJ();
            column.DescriptionK(function: Sqls.Functions.SingleColumn); param.DescriptionK();
            column.DescriptionL(function: Sqls.Functions.SingleColumn); param.DescriptionL();
            column.DescriptionM(function: Sqls.Functions.SingleColumn); param.DescriptionM();
            column.DescriptionN(function: Sqls.Functions.SingleColumn); param.DescriptionN();
            column.DescriptionO(function: Sqls.Functions.SingleColumn); param.DescriptionO();
            column.DescriptionP(function: Sqls.Functions.SingleColumn); param.DescriptionP();
            column.DescriptionQ(function: Sqls.Functions.SingleColumn); param.DescriptionQ();
            column.DescriptionR(function: Sqls.Functions.SingleColumn); param.DescriptionR();
            column.DescriptionS(function: Sqls.Functions.SingleColumn); param.DescriptionS();
            column.DescriptionT(function: Sqls.Functions.SingleColumn); param.DescriptionT();
            column.DescriptionU(function: Sqls.Functions.SingleColumn); param.DescriptionU();
            column.DescriptionV(function: Sqls.Functions.SingleColumn); param.DescriptionV();
            column.DescriptionW(function: Sqls.Functions.SingleColumn); param.DescriptionW();
            column.DescriptionX(function: Sqls.Functions.SingleColumn); param.DescriptionX();
            column.DescriptionY(function: Sqls.Functions.SingleColumn); param.DescriptionY();
            column.DescriptionZ(function: Sqls.Functions.SingleColumn); param.DescriptionZ();
            column.CheckA(function: Sqls.Functions.SingleColumn); param.CheckA();
            column.CheckB(function: Sqls.Functions.SingleColumn); param.CheckB();
            column.CheckC(function: Sqls.Functions.SingleColumn); param.CheckC();
            column.CheckD(function: Sqls.Functions.SingleColumn); param.CheckD();
            column.CheckE(function: Sqls.Functions.SingleColumn); param.CheckE();
            column.CheckF(function: Sqls.Functions.SingleColumn); param.CheckF();
            column.CheckG(function: Sqls.Functions.SingleColumn); param.CheckG();
            column.CheckH(function: Sqls.Functions.SingleColumn); param.CheckH();
            column.CheckI(function: Sqls.Functions.SingleColumn); param.CheckI();
            column.CheckJ(function: Sqls.Functions.SingleColumn); param.CheckJ();
            column.CheckK(function: Sqls.Functions.SingleColumn); param.CheckK();
            column.CheckL(function: Sqls.Functions.SingleColumn); param.CheckL();
            column.CheckM(function: Sqls.Functions.SingleColumn); param.CheckM();
            column.CheckN(function: Sqls.Functions.SingleColumn); param.CheckN();
            column.CheckO(function: Sqls.Functions.SingleColumn); param.CheckO();
            column.CheckP(function: Sqls.Functions.SingleColumn); param.CheckP();
            column.CheckQ(function: Sqls.Functions.SingleColumn); param.CheckQ();
            column.CheckR(function: Sqls.Functions.SingleColumn); param.CheckR();
            column.CheckS(function: Sqls.Functions.SingleColumn); param.CheckS();
            column.CheckT(function: Sqls.Functions.SingleColumn); param.CheckT();
            column.CheckU(function: Sqls.Functions.SingleColumn); param.CheckU();
            column.CheckV(function: Sqls.Functions.SingleColumn); param.CheckV();
            column.CheckW(function: Sqls.Functions.SingleColumn); param.CheckW();
            column.CheckX(function: Sqls.Functions.SingleColumn); param.CheckX();
            column.CheckY(function: Sqls.Functions.SingleColumn); param.CheckY();
            column.CheckZ(function: Sqls.Functions.SingleColumn); param.CheckZ();
            column.LdapSearchRoot(function: Sqls.Functions.SingleColumn); param.LdapSearchRoot();
            column.SynchronizedTime(function: Sqls.Functions.SingleColumn); param.SynchronizedTime();
            column.Comments(function: Sqls.Functions.SingleColumn); param.Comments();
            column.Creator(function: Sqls.Functions.SingleColumn); param.Creator();
            column.Updator(function: Sqls.Functions.SingleColumn); param.Updator();
            column.CreatedTime(function: Sqls.Functions.SingleColumn); param.CreatedTime();
            column.UpdatedTime(function: Sqls.Functions.SingleColumn); param.UpdatedTime();
            return Rds.InsertUsers(
                tableType: tableType,
                param: param,
                select: Rds.SelectUsers(column: column, where: where),
                addUpdatorParam: false);
        }

        public Error.Types Delete(SiteSettings ss, bool notice = false)
        {
            var statements = new List<SqlStatement>();
            var where = Rds.UsersWhere().UserId(UserId);
            statements.AddRange(new List<SqlStatement>
            {
                Rds.DeleteUsers(where: where),
                StatusUtilities.UpdateStatus(StatusUtilities.Types.UsersUpdated),
            });
            var response = Rds.ExecuteScalar_response(
                transactional: true,
                statements: statements.ToArray());
            var userHash = SiteInfo.TenantCaches[Sessions.TenantId()].UserHash;
            if (userHash.Keys.Contains(UserId))
            {
                userHash.Remove(UserId);
            }
            return Error.Types.None;
        }

        public Error.Types Restore(SiteSettings ss,int userId)
        {
            UserId = userId;
            Rds.ExecuteNonQuery(
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreUsers(
                        where: Rds.UsersWhere().UserId(UserId)),
                StatusUtilities.UpdateStatus(StatusUtilities.Types.UsersUpdated),
                });
            return Error.Types.None;
        }

        public Error.Types PhysicalDelete(
            SiteSettings ss,Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: Rds.PhysicalDeleteUsers(
                    tableType: tableType,
                    param: Rds.UsersParam().UserId(UserId)));
            return Error.Types.None;
        }

        public void SetByForm(SiteSettings ss)
        {
            Forms.Keys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "Users_LoginId": LoginId = Forms.Data(controlId).ToString(); break;
                    case "Users_GlobalId": GlobalId = Forms.Data(controlId).ToString(); break;
                    case "Users_Name": Name = Forms.Data(controlId).ToString(); break;
                    case "Users_UserCode": UserCode = Forms.Data(controlId).ToString(); break;
                    case "Users_Password": Password = Forms.Data(controlId).ToString().Sha512Cng(); break;
                    case "Users_PasswordValidate": PasswordValidate = Forms.Data(controlId).ToString().Sha512Cng(); break;
                    case "Users_PasswordDummy": PasswordDummy = Forms.Data(controlId).ToString().Sha512Cng(); break;
                    case "Users_RememberMe": RememberMe = Forms.Data(controlId).ToBool(); break;
                    case "Users_LastName": LastName = Forms.Data(controlId).ToString(); break;
                    case "Users_FirstName": FirstName = Forms.Data(controlId).ToString(); break;
                    case "Users_Birthday": Birthday = new Time(Forms.Data(controlId).ToDateTime(), byForm: true); break;
                    case "Users_Gender": Gender = Forms.Data(controlId).ToString(); break;
                    case "Users_Language": Language = Forms.Data(controlId).ToString(); break;
                    case "Users_TimeZone": TimeZone = Forms.Data(controlId).ToString(); break;
                    case "Users_DeptCode": DeptCode = Forms.Data(controlId).ToString(); break;
                    case "Users_DeptId": DeptId = Forms.Data(controlId).ToInt(); break;
                    case "Users_FirstAndLastNameOrder": FirstAndLastNameOrder = (Names.FirstAndLastNameOrders)Forms.Data(controlId).ToInt(); break;
                    case "Users_Body": Body = Forms.Data(controlId).ToString(); break;
                    case "Users_LastLoginTime": LastLoginTime = new Time(Forms.Data(controlId).ToDateTime(), byForm: true); break;
                    case "Users_PasswordExpirationTime": PasswordExpirationTime = new Time(Forms.Data(controlId).ToDateTime(), byForm: true); break;
                    case "Users_PasswordChangeTime": PasswordChangeTime = new Time(Forms.Data(controlId).ToDateTime(), byForm: true); break;
                    case "Users_NumberOfLogins": NumberOfLogins = Forms.Data(controlId).ToInt(); break;
                    case "Users_NumberOfDenial": NumberOfDenial = Forms.Data(controlId).ToInt(); break;
                    case "Users_TenantManager": TenantManager = Forms.Data(controlId).ToBool(); break;
                    case "Users_Disabled": Disabled = Forms.Data(controlId).ToBool(); break;
                    case "Users_ApiKey": ApiKey = Forms.Data(controlId).ToString(); break;
                    case "Users_OldPassword": OldPassword = Forms.Data(controlId).ToString().Sha512Cng(); break;
                    case "Users_ChangedPassword": ChangedPassword = Forms.Data(controlId).ToString().Sha512Cng(); break;
                    case "Users_ChangedPasswordValidator": ChangedPasswordValidator = Forms.Data(controlId).ToString().Sha512Cng(); break;
                    case "Users_AfterResetPassword": AfterResetPassword = Forms.Data(controlId).ToString().Sha512Cng(); break;
                    case "Users_AfterResetPasswordValidator": AfterResetPasswordValidator = Forms.Data(controlId).ToString().Sha512Cng(); break;
                    case "Users_DemoMailAddress": DemoMailAddress = Forms.Data(controlId).ToString(); break;
                    case "Users_SessionGuid": SessionGuid = Forms.Data(controlId).ToString(); break;
                    case "Users_ClassA": ClassA = Forms.Data(controlId).ToString(); break;
                    case "Users_ClassB": ClassB = Forms.Data(controlId).ToString(); break;
                    case "Users_ClassC": ClassC = Forms.Data(controlId).ToString(); break;
                    case "Users_ClassD": ClassD = Forms.Data(controlId).ToString(); break;
                    case "Users_ClassE": ClassE = Forms.Data(controlId).ToString(); break;
                    case "Users_ClassF": ClassF = Forms.Data(controlId).ToString(); break;
                    case "Users_ClassG": ClassG = Forms.Data(controlId).ToString(); break;
                    case "Users_ClassH": ClassH = Forms.Data(controlId).ToString(); break;
                    case "Users_ClassI": ClassI = Forms.Data(controlId).ToString(); break;
                    case "Users_ClassJ": ClassJ = Forms.Data(controlId).ToString(); break;
                    case "Users_ClassK": ClassK = Forms.Data(controlId).ToString(); break;
                    case "Users_ClassL": ClassL = Forms.Data(controlId).ToString(); break;
                    case "Users_ClassM": ClassM = Forms.Data(controlId).ToString(); break;
                    case "Users_ClassN": ClassN = Forms.Data(controlId).ToString(); break;
                    case "Users_ClassO": ClassO = Forms.Data(controlId).ToString(); break;
                    case "Users_ClassP": ClassP = Forms.Data(controlId).ToString(); break;
                    case "Users_ClassQ": ClassQ = Forms.Data(controlId).ToString(); break;
                    case "Users_ClassR": ClassR = Forms.Data(controlId).ToString(); break;
                    case "Users_ClassS": ClassS = Forms.Data(controlId).ToString(); break;
                    case "Users_ClassT": ClassT = Forms.Data(controlId).ToString(); break;
                    case "Users_ClassU": ClassU = Forms.Data(controlId).ToString(); break;
                    case "Users_ClassV": ClassV = Forms.Data(controlId).ToString(); break;
                    case "Users_ClassW": ClassW = Forms.Data(controlId).ToString(); break;
                    case "Users_ClassX": ClassX = Forms.Data(controlId).ToString(); break;
                    case "Users_ClassY": ClassY = Forms.Data(controlId).ToString(); break;
                    case "Users_ClassZ": ClassZ = Forms.Data(controlId).ToString(); break;
                    case "Users_NumA": NumA = ss.GetColumn("NumA").Round(Forms.Decimal(controlId)); break;
                    case "Users_NumB": NumB = ss.GetColumn("NumB").Round(Forms.Decimal(controlId)); break;
                    case "Users_NumC": NumC = ss.GetColumn("NumC").Round(Forms.Decimal(controlId)); break;
                    case "Users_NumD": NumD = ss.GetColumn("NumD").Round(Forms.Decimal(controlId)); break;
                    case "Users_NumE": NumE = ss.GetColumn("NumE").Round(Forms.Decimal(controlId)); break;
                    case "Users_NumF": NumF = ss.GetColumn("NumF").Round(Forms.Decimal(controlId)); break;
                    case "Users_NumG": NumG = ss.GetColumn("NumG").Round(Forms.Decimal(controlId)); break;
                    case "Users_NumH": NumH = ss.GetColumn("NumH").Round(Forms.Decimal(controlId)); break;
                    case "Users_NumI": NumI = ss.GetColumn("NumI").Round(Forms.Decimal(controlId)); break;
                    case "Users_NumJ": NumJ = ss.GetColumn("NumJ").Round(Forms.Decimal(controlId)); break;
                    case "Users_NumK": NumK = ss.GetColumn("NumK").Round(Forms.Decimal(controlId)); break;
                    case "Users_NumL": NumL = ss.GetColumn("NumL").Round(Forms.Decimal(controlId)); break;
                    case "Users_NumM": NumM = ss.GetColumn("NumM").Round(Forms.Decimal(controlId)); break;
                    case "Users_NumN": NumN = ss.GetColumn("NumN").Round(Forms.Decimal(controlId)); break;
                    case "Users_NumO": NumO = ss.GetColumn("NumO").Round(Forms.Decimal(controlId)); break;
                    case "Users_NumP": NumP = ss.GetColumn("NumP").Round(Forms.Decimal(controlId)); break;
                    case "Users_NumQ": NumQ = ss.GetColumn("NumQ").Round(Forms.Decimal(controlId)); break;
                    case "Users_NumR": NumR = ss.GetColumn("NumR").Round(Forms.Decimal(controlId)); break;
                    case "Users_NumS": NumS = ss.GetColumn("NumS").Round(Forms.Decimal(controlId)); break;
                    case "Users_NumT": NumT = ss.GetColumn("NumT").Round(Forms.Decimal(controlId)); break;
                    case "Users_NumU": NumU = ss.GetColumn("NumU").Round(Forms.Decimal(controlId)); break;
                    case "Users_NumV": NumV = ss.GetColumn("NumV").Round(Forms.Decimal(controlId)); break;
                    case "Users_NumW": NumW = ss.GetColumn("NumW").Round(Forms.Decimal(controlId)); break;
                    case "Users_NumX": NumX = ss.GetColumn("NumX").Round(Forms.Decimal(controlId)); break;
                    case "Users_NumY": NumY = ss.GetColumn("NumY").Round(Forms.Decimal(controlId)); break;
                    case "Users_NumZ": NumZ = ss.GetColumn("NumZ").Round(Forms.Decimal(controlId)); break;
                    case "Users_DateA": DateA = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Users_DateB": DateB = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Users_DateC": DateC = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Users_DateD": DateD = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Users_DateE": DateE = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Users_DateF": DateF = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Users_DateG": DateG = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Users_DateH": DateH = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Users_DateI": DateI = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Users_DateJ": DateJ = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Users_DateK": DateK = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Users_DateL": DateL = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Users_DateM": DateM = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Users_DateN": DateN = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Users_DateO": DateO = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Users_DateP": DateP = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Users_DateQ": DateQ = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Users_DateR": DateR = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Users_DateS": DateS = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Users_DateT": DateT = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Users_DateU": DateU = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Users_DateV": DateV = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Users_DateW": DateW = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Users_DateX": DateX = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Users_DateY": DateY = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Users_DateZ": DateZ = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Users_DescriptionA": DescriptionA = Forms.Data(controlId).ToString(); break;
                    case "Users_DescriptionB": DescriptionB = Forms.Data(controlId).ToString(); break;
                    case "Users_DescriptionC": DescriptionC = Forms.Data(controlId).ToString(); break;
                    case "Users_DescriptionD": DescriptionD = Forms.Data(controlId).ToString(); break;
                    case "Users_DescriptionE": DescriptionE = Forms.Data(controlId).ToString(); break;
                    case "Users_DescriptionF": DescriptionF = Forms.Data(controlId).ToString(); break;
                    case "Users_DescriptionG": DescriptionG = Forms.Data(controlId).ToString(); break;
                    case "Users_DescriptionH": DescriptionH = Forms.Data(controlId).ToString(); break;
                    case "Users_DescriptionI": DescriptionI = Forms.Data(controlId).ToString(); break;
                    case "Users_DescriptionJ": DescriptionJ = Forms.Data(controlId).ToString(); break;
                    case "Users_DescriptionK": DescriptionK = Forms.Data(controlId).ToString(); break;
                    case "Users_DescriptionL": DescriptionL = Forms.Data(controlId).ToString(); break;
                    case "Users_DescriptionM": DescriptionM = Forms.Data(controlId).ToString(); break;
                    case "Users_DescriptionN": DescriptionN = Forms.Data(controlId).ToString(); break;
                    case "Users_DescriptionO": DescriptionO = Forms.Data(controlId).ToString(); break;
                    case "Users_DescriptionP": DescriptionP = Forms.Data(controlId).ToString(); break;
                    case "Users_DescriptionQ": DescriptionQ = Forms.Data(controlId).ToString(); break;
                    case "Users_DescriptionR": DescriptionR = Forms.Data(controlId).ToString(); break;
                    case "Users_DescriptionS": DescriptionS = Forms.Data(controlId).ToString(); break;
                    case "Users_DescriptionT": DescriptionT = Forms.Data(controlId).ToString(); break;
                    case "Users_DescriptionU": DescriptionU = Forms.Data(controlId).ToString(); break;
                    case "Users_DescriptionV": DescriptionV = Forms.Data(controlId).ToString(); break;
                    case "Users_DescriptionW": DescriptionW = Forms.Data(controlId).ToString(); break;
                    case "Users_DescriptionX": DescriptionX = Forms.Data(controlId).ToString(); break;
                    case "Users_DescriptionY": DescriptionY = Forms.Data(controlId).ToString(); break;
                    case "Users_DescriptionZ": DescriptionZ = Forms.Data(controlId).ToString(); break;
                    case "Users_CheckA": CheckA = Forms.Data(controlId).ToBool(); break;
                    case "Users_CheckB": CheckB = Forms.Data(controlId).ToBool(); break;
                    case "Users_CheckC": CheckC = Forms.Data(controlId).ToBool(); break;
                    case "Users_CheckD": CheckD = Forms.Data(controlId).ToBool(); break;
                    case "Users_CheckE": CheckE = Forms.Data(controlId).ToBool(); break;
                    case "Users_CheckF": CheckF = Forms.Data(controlId).ToBool(); break;
                    case "Users_CheckG": CheckG = Forms.Data(controlId).ToBool(); break;
                    case "Users_CheckH": CheckH = Forms.Data(controlId).ToBool(); break;
                    case "Users_CheckI": CheckI = Forms.Data(controlId).ToBool(); break;
                    case "Users_CheckJ": CheckJ = Forms.Data(controlId).ToBool(); break;
                    case "Users_CheckK": CheckK = Forms.Data(controlId).ToBool(); break;
                    case "Users_CheckL": CheckL = Forms.Data(controlId).ToBool(); break;
                    case "Users_CheckM": CheckM = Forms.Data(controlId).ToBool(); break;
                    case "Users_CheckN": CheckN = Forms.Data(controlId).ToBool(); break;
                    case "Users_CheckO": CheckO = Forms.Data(controlId).ToBool(); break;
                    case "Users_CheckP": CheckP = Forms.Data(controlId).ToBool(); break;
                    case "Users_CheckQ": CheckQ = Forms.Data(controlId).ToBool(); break;
                    case "Users_CheckR": CheckR = Forms.Data(controlId).ToBool(); break;
                    case "Users_CheckS": CheckS = Forms.Data(controlId).ToBool(); break;
                    case "Users_CheckT": CheckT = Forms.Data(controlId).ToBool(); break;
                    case "Users_CheckU": CheckU = Forms.Data(controlId).ToBool(); break;
                    case "Users_CheckV": CheckV = Forms.Data(controlId).ToBool(); break;
                    case "Users_CheckW": CheckW = Forms.Data(controlId).ToBool(); break;
                    case "Users_CheckX": CheckX = Forms.Data(controlId).ToBool(); break;
                    case "Users_CheckY": CheckY = Forms.Data(controlId).ToBool(); break;
                    case "Users_CheckZ": CheckZ = Forms.Data(controlId).ToBool(); break;
                    case "Users_LdapSearchRoot": LdapSearchRoot = Forms.Data(controlId).ToString(); break;
                    case "Users_SynchronizedTime": SynchronizedTime = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Users_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
                    case "Comments": Comments.Prepend(Forms.Data("Comments")); break;
                    case "VerUp": VerUp = Forms.Data(controlId).ToBool(); break;
                    default:
                        if (controlId.RegexExists("Comment[0-9]+"))
                        {
                            Comments.Update(
                                controlId.Substring("Comment".Length).ToInt(),
                                Forms.Data(controlId));
                        }
                        break;
                }
            });
            if (Routes.Action() == "deletecomment")
            {
                DeleteCommentId = Forms.ControlId().Split(',')._2nd().ToInt();
                Comments.RemoveAll(o => o.CommentId == DeleteCommentId);
            }
            Forms.FileKeys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    default: break;
                }
            });
        }

        public void SetByModel(UserModel userModel)
        {
            TenantId = userModel.TenantId;
            LoginId = userModel.LoginId;
            GlobalId = userModel.GlobalId;
            Name = userModel.Name;
            UserCode = userModel.UserCode;
            Password = userModel.Password;
            PasswordValidate = userModel.PasswordValidate;
            PasswordDummy = userModel.PasswordDummy;
            RememberMe = userModel.RememberMe;
            LastName = userModel.LastName;
            FirstName = userModel.FirstName;
            Birthday = userModel.Birthday;
            Gender = userModel.Gender;
            Language = userModel.Language;
            TimeZone = userModel.TimeZone;
            DeptCode = userModel.DeptCode;
            DeptId = userModel.DeptId;
            FirstAndLastNameOrder = userModel.FirstAndLastNameOrder;
            Body = userModel.Body;
            LastLoginTime = userModel.LastLoginTime;
            PasswordExpirationTime = userModel.PasswordExpirationTime;
            PasswordChangeTime = userModel.PasswordChangeTime;
            NumberOfLogins = userModel.NumberOfLogins;
            NumberOfDenial = userModel.NumberOfDenial;
            TenantManager = userModel.TenantManager;
            ServiceManager = userModel.ServiceManager;
            Disabled = userModel.Disabled;
            Developer = userModel.Developer;
            UserSettings = userModel.UserSettings;
            ApiKey = userModel.ApiKey;
            OldPassword = userModel.OldPassword;
            ChangedPassword = userModel.ChangedPassword;
            ChangedPasswordValidator = userModel.ChangedPasswordValidator;
            AfterResetPassword = userModel.AfterResetPassword;
            AfterResetPasswordValidator = userModel.AfterResetPasswordValidator;
            MailAddresses = userModel.MailAddresses;
            DemoMailAddress = userModel.DemoMailAddress;
            SessionGuid = userModel.SessionGuid;
            ClassA = userModel.ClassA;
            ClassB = userModel.ClassB;
            ClassC = userModel.ClassC;
            ClassD = userModel.ClassD;
            ClassE = userModel.ClassE;
            ClassF = userModel.ClassF;
            ClassG = userModel.ClassG;
            ClassH = userModel.ClassH;
            ClassI = userModel.ClassI;
            ClassJ = userModel.ClassJ;
            ClassK = userModel.ClassK;
            ClassL = userModel.ClassL;
            ClassM = userModel.ClassM;
            ClassN = userModel.ClassN;
            ClassO = userModel.ClassO;
            ClassP = userModel.ClassP;
            ClassQ = userModel.ClassQ;
            ClassR = userModel.ClassR;
            ClassS = userModel.ClassS;
            ClassT = userModel.ClassT;
            ClassU = userModel.ClassU;
            ClassV = userModel.ClassV;
            ClassW = userModel.ClassW;
            ClassX = userModel.ClassX;
            ClassY = userModel.ClassY;
            ClassZ = userModel.ClassZ;
            NumA = userModel.NumA;
            NumB = userModel.NumB;
            NumC = userModel.NumC;
            NumD = userModel.NumD;
            NumE = userModel.NumE;
            NumF = userModel.NumF;
            NumG = userModel.NumG;
            NumH = userModel.NumH;
            NumI = userModel.NumI;
            NumJ = userModel.NumJ;
            NumK = userModel.NumK;
            NumL = userModel.NumL;
            NumM = userModel.NumM;
            NumN = userModel.NumN;
            NumO = userModel.NumO;
            NumP = userModel.NumP;
            NumQ = userModel.NumQ;
            NumR = userModel.NumR;
            NumS = userModel.NumS;
            NumT = userModel.NumT;
            NumU = userModel.NumU;
            NumV = userModel.NumV;
            NumW = userModel.NumW;
            NumX = userModel.NumX;
            NumY = userModel.NumY;
            NumZ = userModel.NumZ;
            DateA = userModel.DateA;
            DateB = userModel.DateB;
            DateC = userModel.DateC;
            DateD = userModel.DateD;
            DateE = userModel.DateE;
            DateF = userModel.DateF;
            DateG = userModel.DateG;
            DateH = userModel.DateH;
            DateI = userModel.DateI;
            DateJ = userModel.DateJ;
            DateK = userModel.DateK;
            DateL = userModel.DateL;
            DateM = userModel.DateM;
            DateN = userModel.DateN;
            DateO = userModel.DateO;
            DateP = userModel.DateP;
            DateQ = userModel.DateQ;
            DateR = userModel.DateR;
            DateS = userModel.DateS;
            DateT = userModel.DateT;
            DateU = userModel.DateU;
            DateV = userModel.DateV;
            DateW = userModel.DateW;
            DateX = userModel.DateX;
            DateY = userModel.DateY;
            DateZ = userModel.DateZ;
            DescriptionA = userModel.DescriptionA;
            DescriptionB = userModel.DescriptionB;
            DescriptionC = userModel.DescriptionC;
            DescriptionD = userModel.DescriptionD;
            DescriptionE = userModel.DescriptionE;
            DescriptionF = userModel.DescriptionF;
            DescriptionG = userModel.DescriptionG;
            DescriptionH = userModel.DescriptionH;
            DescriptionI = userModel.DescriptionI;
            DescriptionJ = userModel.DescriptionJ;
            DescriptionK = userModel.DescriptionK;
            DescriptionL = userModel.DescriptionL;
            DescriptionM = userModel.DescriptionM;
            DescriptionN = userModel.DescriptionN;
            DescriptionO = userModel.DescriptionO;
            DescriptionP = userModel.DescriptionP;
            DescriptionQ = userModel.DescriptionQ;
            DescriptionR = userModel.DescriptionR;
            DescriptionS = userModel.DescriptionS;
            DescriptionT = userModel.DescriptionT;
            DescriptionU = userModel.DescriptionU;
            DescriptionV = userModel.DescriptionV;
            DescriptionW = userModel.DescriptionW;
            DescriptionX = userModel.DescriptionX;
            DescriptionY = userModel.DescriptionY;
            DescriptionZ = userModel.DescriptionZ;
            CheckA = userModel.CheckA;
            CheckB = userModel.CheckB;
            CheckC = userModel.CheckC;
            CheckD = userModel.CheckD;
            CheckE = userModel.CheckE;
            CheckF = userModel.CheckF;
            CheckG = userModel.CheckG;
            CheckH = userModel.CheckH;
            CheckI = userModel.CheckI;
            CheckJ = userModel.CheckJ;
            CheckK = userModel.CheckK;
            CheckL = userModel.CheckL;
            CheckM = userModel.CheckM;
            CheckN = userModel.CheckN;
            CheckO = userModel.CheckO;
            CheckP = userModel.CheckP;
            CheckQ = userModel.CheckQ;
            CheckR = userModel.CheckR;
            CheckS = userModel.CheckS;
            CheckT = userModel.CheckT;
            CheckU = userModel.CheckU;
            CheckV = userModel.CheckV;
            CheckW = userModel.CheckW;
            CheckX = userModel.CheckX;
            CheckY = userModel.CheckY;
            CheckZ = userModel.CheckZ;
            LdapSearchRoot = userModel.LdapSearchRoot;
            SynchronizedTime = userModel.SynchronizedTime;
            Comments = userModel.Comments;
            Creator = userModel.Creator;
            Updator = userModel.Updator;
            CreatedTime = userModel.CreatedTime;
            UpdatedTime = userModel.UpdatedTime;
            VerUp = userModel.VerUp;
            Comments = userModel.Comments;
        }

        public void SetByApi(SiteSettings ss)
        {
            var data = Forms.String().Deserialize<UserApiModel>();
            if (data == null)
            {
                return;
            }
            if (data.LoginId != null) LoginId = data.LoginId.ToString().ToString();
            if (data.GlobalId != null) GlobalId = data.GlobalId.ToString().ToString();
            if (data.Name != null) Name = data.Name.ToString().ToString();
            if (data.UserCode != null) UserCode = data.UserCode.ToString().ToString();
            if (data.Password != null) Password = data.Password.ToString().ToString().Sha512Cng();
            if (data.LastName != null) LastName = data.LastName.ToString().ToString();
            if (data.FirstName != null) FirstName = data.FirstName.ToString().ToString();
            if (data.Birthday != null) Birthday = new Time(data.Birthday.ToDateTime(), byForm: true);
            if (data.Gender != null) Gender = data.Gender.ToString().ToString();
            if (data.Language != null) Language = data.Language.ToString().ToString();
            if (data.TimeZone != null) TimeZone = data.TimeZone.ToString().ToString();
            if (data.DeptCode != null) DeptCode = data.DeptCode.ToString().ToString();
            if (data.DeptId != null) DeptId = data.DeptId.ToInt().ToInt();
            if (data.FirstAndLastNameOrder != null) FirstAndLastNameOrder = (Names.FirstAndLastNameOrders)data.FirstAndLastNameOrder.ToInt().ToInt();
            if (data.Body != null) Body = data.Body.ToString().ToString();
            if (data.LastLoginTime != null) LastLoginTime = new Time(data.LastLoginTime.ToDateTime(), byForm: true);
            if (data.PasswordExpirationTime != null) PasswordExpirationTime = new Time(data.PasswordExpirationTime.ToDateTime(), byForm: true);
            if (data.PasswordChangeTime != null) PasswordChangeTime = new Time(data.PasswordChangeTime.ToDateTime(), byForm: true);
            if (data.NumberOfLogins != null) NumberOfLogins = data.NumberOfLogins.ToInt().ToInt();
            if (data.NumberOfDenial != null) NumberOfDenial = data.NumberOfDenial.ToInt().ToInt();
            if (data.TenantManager != null) TenantManager = data.TenantManager.ToBool().ToBool();
            if (data.Disabled != null) Disabled = data.Disabled.ToBool().ToBool();
            if (data.ApiKey != null) ApiKey = data.ApiKey.ToString().ToString();
            if (data.ClassA != null) ClassA = data.ClassA.ToString().ToString();
            if (data.ClassB != null) ClassB = data.ClassB.ToString().ToString();
            if (data.ClassC != null) ClassC = data.ClassC.ToString().ToString();
            if (data.ClassD != null) ClassD = data.ClassD.ToString().ToString();
            if (data.ClassE != null) ClassE = data.ClassE.ToString().ToString();
            if (data.ClassF != null) ClassF = data.ClassF.ToString().ToString();
            if (data.ClassG != null) ClassG = data.ClassG.ToString().ToString();
            if (data.ClassH != null) ClassH = data.ClassH.ToString().ToString();
            if (data.ClassI != null) ClassI = data.ClassI.ToString().ToString();
            if (data.ClassJ != null) ClassJ = data.ClassJ.ToString().ToString();
            if (data.ClassK != null) ClassK = data.ClassK.ToString().ToString();
            if (data.ClassL != null) ClassL = data.ClassL.ToString().ToString();
            if (data.ClassM != null) ClassM = data.ClassM.ToString().ToString();
            if (data.ClassN != null) ClassN = data.ClassN.ToString().ToString();
            if (data.ClassO != null) ClassO = data.ClassO.ToString().ToString();
            if (data.ClassP != null) ClassP = data.ClassP.ToString().ToString();
            if (data.ClassQ != null) ClassQ = data.ClassQ.ToString().ToString();
            if (data.ClassR != null) ClassR = data.ClassR.ToString().ToString();
            if (data.ClassS != null) ClassS = data.ClassS.ToString().ToString();
            if (data.ClassT != null) ClassT = data.ClassT.ToString().ToString();
            if (data.ClassU != null) ClassU = data.ClassU.ToString().ToString();
            if (data.ClassV != null) ClassV = data.ClassV.ToString().ToString();
            if (data.ClassW != null) ClassW = data.ClassW.ToString().ToString();
            if (data.ClassX != null) ClassX = data.ClassX.ToString().ToString();
            if (data.ClassY != null) ClassY = data.ClassY.ToString().ToString();
            if (data.ClassZ != null) ClassZ = data.ClassZ.ToString().ToString();
            if (data.NumA != null) NumA = ss.GetColumn("NumA").Round(data.NumA.ToDecimal());
            if (data.NumB != null) NumB = ss.GetColumn("NumB").Round(data.NumB.ToDecimal());
            if (data.NumC != null) NumC = ss.GetColumn("NumC").Round(data.NumC.ToDecimal());
            if (data.NumD != null) NumD = ss.GetColumn("NumD").Round(data.NumD.ToDecimal());
            if (data.NumE != null) NumE = ss.GetColumn("NumE").Round(data.NumE.ToDecimal());
            if (data.NumF != null) NumF = ss.GetColumn("NumF").Round(data.NumF.ToDecimal());
            if (data.NumG != null) NumG = ss.GetColumn("NumG").Round(data.NumG.ToDecimal());
            if (data.NumH != null) NumH = ss.GetColumn("NumH").Round(data.NumH.ToDecimal());
            if (data.NumI != null) NumI = ss.GetColumn("NumI").Round(data.NumI.ToDecimal());
            if (data.NumJ != null) NumJ = ss.GetColumn("NumJ").Round(data.NumJ.ToDecimal());
            if (data.NumK != null) NumK = ss.GetColumn("NumK").Round(data.NumK.ToDecimal());
            if (data.NumL != null) NumL = ss.GetColumn("NumL").Round(data.NumL.ToDecimal());
            if (data.NumM != null) NumM = ss.GetColumn("NumM").Round(data.NumM.ToDecimal());
            if (data.NumN != null) NumN = ss.GetColumn("NumN").Round(data.NumN.ToDecimal());
            if (data.NumO != null) NumO = ss.GetColumn("NumO").Round(data.NumO.ToDecimal());
            if (data.NumP != null) NumP = ss.GetColumn("NumP").Round(data.NumP.ToDecimal());
            if (data.NumQ != null) NumQ = ss.GetColumn("NumQ").Round(data.NumQ.ToDecimal());
            if (data.NumR != null) NumR = ss.GetColumn("NumR").Round(data.NumR.ToDecimal());
            if (data.NumS != null) NumS = ss.GetColumn("NumS").Round(data.NumS.ToDecimal());
            if (data.NumT != null) NumT = ss.GetColumn("NumT").Round(data.NumT.ToDecimal());
            if (data.NumU != null) NumU = ss.GetColumn("NumU").Round(data.NumU.ToDecimal());
            if (data.NumV != null) NumV = ss.GetColumn("NumV").Round(data.NumV.ToDecimal());
            if (data.NumW != null) NumW = ss.GetColumn("NumW").Round(data.NumW.ToDecimal());
            if (data.NumX != null) NumX = ss.GetColumn("NumX").Round(data.NumX.ToDecimal());
            if (data.NumY != null) NumY = ss.GetColumn("NumY").Round(data.NumY.ToDecimal());
            if (data.NumZ != null) NumZ = ss.GetColumn("NumZ").Round(data.NumZ.ToDecimal());
            if (data.DateA != null) DateA = data.DateA.ToDateTime().ToDateTime().ToUniversal();
            if (data.DateB != null) DateB = data.DateB.ToDateTime().ToDateTime().ToUniversal();
            if (data.DateC != null) DateC = data.DateC.ToDateTime().ToDateTime().ToUniversal();
            if (data.DateD != null) DateD = data.DateD.ToDateTime().ToDateTime().ToUniversal();
            if (data.DateE != null) DateE = data.DateE.ToDateTime().ToDateTime().ToUniversal();
            if (data.DateF != null) DateF = data.DateF.ToDateTime().ToDateTime().ToUniversal();
            if (data.DateG != null) DateG = data.DateG.ToDateTime().ToDateTime().ToUniversal();
            if (data.DateH != null) DateH = data.DateH.ToDateTime().ToDateTime().ToUniversal();
            if (data.DateI != null) DateI = data.DateI.ToDateTime().ToDateTime().ToUniversal();
            if (data.DateJ != null) DateJ = data.DateJ.ToDateTime().ToDateTime().ToUniversal();
            if (data.DateK != null) DateK = data.DateK.ToDateTime().ToDateTime().ToUniversal();
            if (data.DateL != null) DateL = data.DateL.ToDateTime().ToDateTime().ToUniversal();
            if (data.DateM != null) DateM = data.DateM.ToDateTime().ToDateTime().ToUniversal();
            if (data.DateN != null) DateN = data.DateN.ToDateTime().ToDateTime().ToUniversal();
            if (data.DateO != null) DateO = data.DateO.ToDateTime().ToDateTime().ToUniversal();
            if (data.DateP != null) DateP = data.DateP.ToDateTime().ToDateTime().ToUniversal();
            if (data.DateQ != null) DateQ = data.DateQ.ToDateTime().ToDateTime().ToUniversal();
            if (data.DateR != null) DateR = data.DateR.ToDateTime().ToDateTime().ToUniversal();
            if (data.DateS != null) DateS = data.DateS.ToDateTime().ToDateTime().ToUniversal();
            if (data.DateT != null) DateT = data.DateT.ToDateTime().ToDateTime().ToUniversal();
            if (data.DateU != null) DateU = data.DateU.ToDateTime().ToDateTime().ToUniversal();
            if (data.DateV != null) DateV = data.DateV.ToDateTime().ToDateTime().ToUniversal();
            if (data.DateW != null) DateW = data.DateW.ToDateTime().ToDateTime().ToUniversal();
            if (data.DateX != null) DateX = data.DateX.ToDateTime().ToDateTime().ToUniversal();
            if (data.DateY != null) DateY = data.DateY.ToDateTime().ToDateTime().ToUniversal();
            if (data.DateZ != null) DateZ = data.DateZ.ToDateTime().ToDateTime().ToUniversal();
            if (data.DescriptionA != null) DescriptionA = data.DescriptionA.ToString().ToString();
            if (data.DescriptionB != null) DescriptionB = data.DescriptionB.ToString().ToString();
            if (data.DescriptionC != null) DescriptionC = data.DescriptionC.ToString().ToString();
            if (data.DescriptionD != null) DescriptionD = data.DescriptionD.ToString().ToString();
            if (data.DescriptionE != null) DescriptionE = data.DescriptionE.ToString().ToString();
            if (data.DescriptionF != null) DescriptionF = data.DescriptionF.ToString().ToString();
            if (data.DescriptionG != null) DescriptionG = data.DescriptionG.ToString().ToString();
            if (data.DescriptionH != null) DescriptionH = data.DescriptionH.ToString().ToString();
            if (data.DescriptionI != null) DescriptionI = data.DescriptionI.ToString().ToString();
            if (data.DescriptionJ != null) DescriptionJ = data.DescriptionJ.ToString().ToString();
            if (data.DescriptionK != null) DescriptionK = data.DescriptionK.ToString().ToString();
            if (data.DescriptionL != null) DescriptionL = data.DescriptionL.ToString().ToString();
            if (data.DescriptionM != null) DescriptionM = data.DescriptionM.ToString().ToString();
            if (data.DescriptionN != null) DescriptionN = data.DescriptionN.ToString().ToString();
            if (data.DescriptionO != null) DescriptionO = data.DescriptionO.ToString().ToString();
            if (data.DescriptionP != null) DescriptionP = data.DescriptionP.ToString().ToString();
            if (data.DescriptionQ != null) DescriptionQ = data.DescriptionQ.ToString().ToString();
            if (data.DescriptionR != null) DescriptionR = data.DescriptionR.ToString().ToString();
            if (data.DescriptionS != null) DescriptionS = data.DescriptionS.ToString().ToString();
            if (data.DescriptionT != null) DescriptionT = data.DescriptionT.ToString().ToString();
            if (data.DescriptionU != null) DescriptionU = data.DescriptionU.ToString().ToString();
            if (data.DescriptionV != null) DescriptionV = data.DescriptionV.ToString().ToString();
            if (data.DescriptionW != null) DescriptionW = data.DescriptionW.ToString().ToString();
            if (data.DescriptionX != null) DescriptionX = data.DescriptionX.ToString().ToString();
            if (data.DescriptionY != null) DescriptionY = data.DescriptionY.ToString().ToString();
            if (data.DescriptionZ != null) DescriptionZ = data.DescriptionZ.ToString().ToString();
            if (data.CheckA != null) CheckA = data.CheckA.ToBool().ToBool();
            if (data.CheckB != null) CheckB = data.CheckB.ToBool().ToBool();
            if (data.CheckC != null) CheckC = data.CheckC.ToBool().ToBool();
            if (data.CheckD != null) CheckD = data.CheckD.ToBool().ToBool();
            if (data.CheckE != null) CheckE = data.CheckE.ToBool().ToBool();
            if (data.CheckF != null) CheckF = data.CheckF.ToBool().ToBool();
            if (data.CheckG != null) CheckG = data.CheckG.ToBool().ToBool();
            if (data.CheckH != null) CheckH = data.CheckH.ToBool().ToBool();
            if (data.CheckI != null) CheckI = data.CheckI.ToBool().ToBool();
            if (data.CheckJ != null) CheckJ = data.CheckJ.ToBool().ToBool();
            if (data.CheckK != null) CheckK = data.CheckK.ToBool().ToBool();
            if (data.CheckL != null) CheckL = data.CheckL.ToBool().ToBool();
            if (data.CheckM != null) CheckM = data.CheckM.ToBool().ToBool();
            if (data.CheckN != null) CheckN = data.CheckN.ToBool().ToBool();
            if (data.CheckO != null) CheckO = data.CheckO.ToBool().ToBool();
            if (data.CheckP != null) CheckP = data.CheckP.ToBool().ToBool();
            if (data.CheckQ != null) CheckQ = data.CheckQ.ToBool().ToBool();
            if (data.CheckR != null) CheckR = data.CheckR.ToBool().ToBool();
            if (data.CheckS != null) CheckS = data.CheckS.ToBool().ToBool();
            if (data.CheckT != null) CheckT = data.CheckT.ToBool().ToBool();
            if (data.CheckU != null) CheckU = data.CheckU.ToBool().ToBool();
            if (data.CheckV != null) CheckV = data.CheckV.ToBool().ToBool();
            if (data.CheckW != null) CheckW = data.CheckW.ToBool().ToBool();
            if (data.CheckX != null) CheckX = data.CheckX.ToBool().ToBool();
            if (data.CheckY != null) CheckY = data.CheckY.ToBool().ToBool();
            if (data.CheckZ != null) CheckZ = data.CheckZ.ToBool().ToBool();
            if (data.LdapSearchRoot != null) LdapSearchRoot = data.LdapSearchRoot.ToString().ToString();
            if (data.SynchronizedTime != null) SynchronizedTime = data.SynchronizedTime.ToDateTime().ToDateTime().ToUniversal();
            if (data.Comments != null) Comments.Prepend(data.Comments);
            if (data.VerUp != null) VerUp = data.VerUp.ToBool();
        }

        private void SetBySession()
        {
            if (!Forms.HasData("Users_UserSettings")) UserSettings = Session_UserSettings();
            if (!Forms.HasData("Users_MailAddresses")) MailAddresses = Session_MailAddresses();
        }

        private void Set(SiteSettings ss, DataTable dataTable)
        {
            switch (dataTable.Rows.Count)
            {
                case 1: Set(ss, dataTable.Rows[0]); break;
                case 0: AccessStatus = Databases.AccessStatuses.NotFound; break;
                default: AccessStatus = Databases.AccessStatuses.Overlap; break;
            }
        }

        private void Set(SiteSettings ss, DataRow dataRow, string tableAlias = null)
        {
            AccessStatus = Databases.AccessStatuses.Selected;
            foreach(DataColumn dataColumn in dataRow.Table.Columns)
            {
                var column = new ColumnNameInfo(dataColumn.ColumnName);
                if (column.TableAlias == tableAlias)
                {
                    switch (column.Name)
                    {
                        case "TenantId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                TenantId = dataRow[column.ColumnName].ToInt();
                                SavedTenantId = TenantId;
                            }
                            break;
                        case "UserId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                UserId = dataRow[column.ColumnName].ToInt();
                                SavedUserId = UserId;
                            }
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "LoginId":
                            LoginId = dataRow[column.ColumnName].ToString();
                            SavedLoginId = LoginId;
                            break;
                        case "GlobalId":
                            GlobalId = dataRow[column.ColumnName].ToString();
                            SavedGlobalId = GlobalId;
                            break;
                        case "Name":
                            Name = dataRow[column.ColumnName].ToString();
                            SavedName = Name;
                            break;
                        case "UserCode":
                            UserCode = dataRow[column.ColumnName].ToString();
                            SavedUserCode = UserCode;
                            break;
                        case "Password":
                            Password = dataRow[column.ColumnName].ToString();
                            SavedPassword = Password;
                            break;
                        case "LastName":
                            LastName = dataRow[column.ColumnName].ToString();
                            SavedLastName = LastName;
                            break;
                        case "FirstName":
                            FirstName = dataRow[column.ColumnName].ToString();
                            SavedFirstName = FirstName;
                            break;
                        case "Birthday":
                            Birthday = new Time(dataRow, column.ColumnName);
                            SavedBirthday = Birthday.Value;
                            break;
                        case "Gender":
                            Gender = dataRow[column.ColumnName].ToString();
                            SavedGender = Gender;
                            break;
                        case "Language":
                            Language = dataRow[column.ColumnName].ToString();
                            SavedLanguage = Language;
                            break;
                        case "TimeZone":
                            TimeZone = dataRow[column.ColumnName].ToString();
                            SavedTimeZone = TimeZone;
                            break;
                        case "DeptCode":
                            DeptCode = dataRow[column.ColumnName].ToString();
                            SavedDeptCode = DeptCode;
                            break;
                        case "DeptId":
                            DeptId = dataRow[column.ColumnName].ToInt();
                            SavedDeptId = DeptId;
                            break;
                        case "FirstAndLastNameOrder":
                            FirstAndLastNameOrder = (Names.FirstAndLastNameOrders)dataRow[column.ColumnName].ToInt();
                            SavedFirstAndLastNameOrder = FirstAndLastNameOrder.ToInt();
                            break;
                        case "Body":
                            Body = dataRow[column.ColumnName].ToString();
                            SavedBody = Body;
                            break;
                        case "LastLoginTime":
                            LastLoginTime = new Time(dataRow, column.ColumnName);
                            SavedLastLoginTime = LastLoginTime.Value;
                            break;
                        case "PasswordExpirationTime":
                            PasswordExpirationTime = new Time(dataRow, column.ColumnName);
                            SavedPasswordExpirationTime = PasswordExpirationTime.Value;
                            break;
                        case "PasswordChangeTime":
                            PasswordChangeTime = new Time(dataRow, column.ColumnName);
                            SavedPasswordChangeTime = PasswordChangeTime.Value;
                            break;
                        case "NumberOfLogins":
                            NumberOfLogins = dataRow[column.ColumnName].ToInt();
                            SavedNumberOfLogins = NumberOfLogins;
                            break;
                        case "NumberOfDenial":
                            NumberOfDenial = dataRow[column.ColumnName].ToInt();
                            SavedNumberOfDenial = NumberOfDenial;
                            break;
                        case "TenantManager":
                            TenantManager = dataRow[column.ColumnName].ToBool();
                            SavedTenantManager = TenantManager;
                            break;
                        case "ServiceManager":
                            ServiceManager = dataRow[column.ColumnName].ToBool();
                            SavedServiceManager = ServiceManager;
                            break;
                        case "Disabled":
                            Disabled = dataRow[column.ColumnName].ToBool();
                            SavedDisabled = Disabled;
                            break;
                        case "Developer":
                            Developer = dataRow[column.ColumnName].ToBool();
                            SavedDeveloper = Developer;
                            break;
                        case "UserSettings":
                            UserSettings = GetUserSettings(dataRow);
                            SavedUserSettings = UserSettings.RecordingJson();
                            break;
                        case "ApiKey":
                            ApiKey = dataRow[column.ColumnName].ToString();
                            SavedApiKey = ApiKey;
                            break;
                        case "ClassA":
                            ClassA = dataRow[column.ColumnName].ToString();
                            SavedClassA = ClassA;
                            break;
                        case "ClassB":
                            ClassB = dataRow[column.ColumnName].ToString();
                            SavedClassB = ClassB;
                            break;
                        case "ClassC":
                            ClassC = dataRow[column.ColumnName].ToString();
                            SavedClassC = ClassC;
                            break;
                        case "ClassD":
                            ClassD = dataRow[column.ColumnName].ToString();
                            SavedClassD = ClassD;
                            break;
                        case "ClassE":
                            ClassE = dataRow[column.ColumnName].ToString();
                            SavedClassE = ClassE;
                            break;
                        case "ClassF":
                            ClassF = dataRow[column.ColumnName].ToString();
                            SavedClassF = ClassF;
                            break;
                        case "ClassG":
                            ClassG = dataRow[column.ColumnName].ToString();
                            SavedClassG = ClassG;
                            break;
                        case "ClassH":
                            ClassH = dataRow[column.ColumnName].ToString();
                            SavedClassH = ClassH;
                            break;
                        case "ClassI":
                            ClassI = dataRow[column.ColumnName].ToString();
                            SavedClassI = ClassI;
                            break;
                        case "ClassJ":
                            ClassJ = dataRow[column.ColumnName].ToString();
                            SavedClassJ = ClassJ;
                            break;
                        case "ClassK":
                            ClassK = dataRow[column.ColumnName].ToString();
                            SavedClassK = ClassK;
                            break;
                        case "ClassL":
                            ClassL = dataRow[column.ColumnName].ToString();
                            SavedClassL = ClassL;
                            break;
                        case "ClassM":
                            ClassM = dataRow[column.ColumnName].ToString();
                            SavedClassM = ClassM;
                            break;
                        case "ClassN":
                            ClassN = dataRow[column.ColumnName].ToString();
                            SavedClassN = ClassN;
                            break;
                        case "ClassO":
                            ClassO = dataRow[column.ColumnName].ToString();
                            SavedClassO = ClassO;
                            break;
                        case "ClassP":
                            ClassP = dataRow[column.ColumnName].ToString();
                            SavedClassP = ClassP;
                            break;
                        case "ClassQ":
                            ClassQ = dataRow[column.ColumnName].ToString();
                            SavedClassQ = ClassQ;
                            break;
                        case "ClassR":
                            ClassR = dataRow[column.ColumnName].ToString();
                            SavedClassR = ClassR;
                            break;
                        case "ClassS":
                            ClassS = dataRow[column.ColumnName].ToString();
                            SavedClassS = ClassS;
                            break;
                        case "ClassT":
                            ClassT = dataRow[column.ColumnName].ToString();
                            SavedClassT = ClassT;
                            break;
                        case "ClassU":
                            ClassU = dataRow[column.ColumnName].ToString();
                            SavedClassU = ClassU;
                            break;
                        case "ClassV":
                            ClassV = dataRow[column.ColumnName].ToString();
                            SavedClassV = ClassV;
                            break;
                        case "ClassW":
                            ClassW = dataRow[column.ColumnName].ToString();
                            SavedClassW = ClassW;
                            break;
                        case "ClassX":
                            ClassX = dataRow[column.ColumnName].ToString();
                            SavedClassX = ClassX;
                            break;
                        case "ClassY":
                            ClassY = dataRow[column.ColumnName].ToString();
                            SavedClassY = ClassY;
                            break;
                        case "ClassZ":
                            ClassZ = dataRow[column.ColumnName].ToString();
                            SavedClassZ = ClassZ;
                            break;
                        case "NumA":
                            NumA = dataRow[column.ColumnName].ToDecimal();
                            SavedNumA = NumA;
                            break;
                        case "NumB":
                            NumB = dataRow[column.ColumnName].ToDecimal();
                            SavedNumB = NumB;
                            break;
                        case "NumC":
                            NumC = dataRow[column.ColumnName].ToDecimal();
                            SavedNumC = NumC;
                            break;
                        case "NumD":
                            NumD = dataRow[column.ColumnName].ToDecimal();
                            SavedNumD = NumD;
                            break;
                        case "NumE":
                            NumE = dataRow[column.ColumnName].ToDecimal();
                            SavedNumE = NumE;
                            break;
                        case "NumF":
                            NumF = dataRow[column.ColumnName].ToDecimal();
                            SavedNumF = NumF;
                            break;
                        case "NumG":
                            NumG = dataRow[column.ColumnName].ToDecimal();
                            SavedNumG = NumG;
                            break;
                        case "NumH":
                            NumH = dataRow[column.ColumnName].ToDecimal();
                            SavedNumH = NumH;
                            break;
                        case "NumI":
                            NumI = dataRow[column.ColumnName].ToDecimal();
                            SavedNumI = NumI;
                            break;
                        case "NumJ":
                            NumJ = dataRow[column.ColumnName].ToDecimal();
                            SavedNumJ = NumJ;
                            break;
                        case "NumK":
                            NumK = dataRow[column.ColumnName].ToDecimal();
                            SavedNumK = NumK;
                            break;
                        case "NumL":
                            NumL = dataRow[column.ColumnName].ToDecimal();
                            SavedNumL = NumL;
                            break;
                        case "NumM":
                            NumM = dataRow[column.ColumnName].ToDecimal();
                            SavedNumM = NumM;
                            break;
                        case "NumN":
                            NumN = dataRow[column.ColumnName].ToDecimal();
                            SavedNumN = NumN;
                            break;
                        case "NumO":
                            NumO = dataRow[column.ColumnName].ToDecimal();
                            SavedNumO = NumO;
                            break;
                        case "NumP":
                            NumP = dataRow[column.ColumnName].ToDecimal();
                            SavedNumP = NumP;
                            break;
                        case "NumQ":
                            NumQ = dataRow[column.ColumnName].ToDecimal();
                            SavedNumQ = NumQ;
                            break;
                        case "NumR":
                            NumR = dataRow[column.ColumnName].ToDecimal();
                            SavedNumR = NumR;
                            break;
                        case "NumS":
                            NumS = dataRow[column.ColumnName].ToDecimal();
                            SavedNumS = NumS;
                            break;
                        case "NumT":
                            NumT = dataRow[column.ColumnName].ToDecimal();
                            SavedNumT = NumT;
                            break;
                        case "NumU":
                            NumU = dataRow[column.ColumnName].ToDecimal();
                            SavedNumU = NumU;
                            break;
                        case "NumV":
                            NumV = dataRow[column.ColumnName].ToDecimal();
                            SavedNumV = NumV;
                            break;
                        case "NumW":
                            NumW = dataRow[column.ColumnName].ToDecimal();
                            SavedNumW = NumW;
                            break;
                        case "NumX":
                            NumX = dataRow[column.ColumnName].ToDecimal();
                            SavedNumX = NumX;
                            break;
                        case "NumY":
                            NumY = dataRow[column.ColumnName].ToDecimal();
                            SavedNumY = NumY;
                            break;
                        case "NumZ":
                            NumZ = dataRow[column.ColumnName].ToDecimal();
                            SavedNumZ = NumZ;
                            break;
                        case "DateA":
                            DateA = dataRow[column.ColumnName].ToDateTime();
                            SavedDateA = DateA;
                            break;
                        case "DateB":
                            DateB = dataRow[column.ColumnName].ToDateTime();
                            SavedDateB = DateB;
                            break;
                        case "DateC":
                            DateC = dataRow[column.ColumnName].ToDateTime();
                            SavedDateC = DateC;
                            break;
                        case "DateD":
                            DateD = dataRow[column.ColumnName].ToDateTime();
                            SavedDateD = DateD;
                            break;
                        case "DateE":
                            DateE = dataRow[column.ColumnName].ToDateTime();
                            SavedDateE = DateE;
                            break;
                        case "DateF":
                            DateF = dataRow[column.ColumnName].ToDateTime();
                            SavedDateF = DateF;
                            break;
                        case "DateG":
                            DateG = dataRow[column.ColumnName].ToDateTime();
                            SavedDateG = DateG;
                            break;
                        case "DateH":
                            DateH = dataRow[column.ColumnName].ToDateTime();
                            SavedDateH = DateH;
                            break;
                        case "DateI":
                            DateI = dataRow[column.ColumnName].ToDateTime();
                            SavedDateI = DateI;
                            break;
                        case "DateJ":
                            DateJ = dataRow[column.ColumnName].ToDateTime();
                            SavedDateJ = DateJ;
                            break;
                        case "DateK":
                            DateK = dataRow[column.ColumnName].ToDateTime();
                            SavedDateK = DateK;
                            break;
                        case "DateL":
                            DateL = dataRow[column.ColumnName].ToDateTime();
                            SavedDateL = DateL;
                            break;
                        case "DateM":
                            DateM = dataRow[column.ColumnName].ToDateTime();
                            SavedDateM = DateM;
                            break;
                        case "DateN":
                            DateN = dataRow[column.ColumnName].ToDateTime();
                            SavedDateN = DateN;
                            break;
                        case "DateO":
                            DateO = dataRow[column.ColumnName].ToDateTime();
                            SavedDateO = DateO;
                            break;
                        case "DateP":
                            DateP = dataRow[column.ColumnName].ToDateTime();
                            SavedDateP = DateP;
                            break;
                        case "DateQ":
                            DateQ = dataRow[column.ColumnName].ToDateTime();
                            SavedDateQ = DateQ;
                            break;
                        case "DateR":
                            DateR = dataRow[column.ColumnName].ToDateTime();
                            SavedDateR = DateR;
                            break;
                        case "DateS":
                            DateS = dataRow[column.ColumnName].ToDateTime();
                            SavedDateS = DateS;
                            break;
                        case "DateT":
                            DateT = dataRow[column.ColumnName].ToDateTime();
                            SavedDateT = DateT;
                            break;
                        case "DateU":
                            DateU = dataRow[column.ColumnName].ToDateTime();
                            SavedDateU = DateU;
                            break;
                        case "DateV":
                            DateV = dataRow[column.ColumnName].ToDateTime();
                            SavedDateV = DateV;
                            break;
                        case "DateW":
                            DateW = dataRow[column.ColumnName].ToDateTime();
                            SavedDateW = DateW;
                            break;
                        case "DateX":
                            DateX = dataRow[column.ColumnName].ToDateTime();
                            SavedDateX = DateX;
                            break;
                        case "DateY":
                            DateY = dataRow[column.ColumnName].ToDateTime();
                            SavedDateY = DateY;
                            break;
                        case "DateZ":
                            DateZ = dataRow[column.ColumnName].ToDateTime();
                            SavedDateZ = DateZ;
                            break;
                        case "DescriptionA":
                            DescriptionA = dataRow[column.ColumnName].ToString();
                            SavedDescriptionA = DescriptionA;
                            break;
                        case "DescriptionB":
                            DescriptionB = dataRow[column.ColumnName].ToString();
                            SavedDescriptionB = DescriptionB;
                            break;
                        case "DescriptionC":
                            DescriptionC = dataRow[column.ColumnName].ToString();
                            SavedDescriptionC = DescriptionC;
                            break;
                        case "DescriptionD":
                            DescriptionD = dataRow[column.ColumnName].ToString();
                            SavedDescriptionD = DescriptionD;
                            break;
                        case "DescriptionE":
                            DescriptionE = dataRow[column.ColumnName].ToString();
                            SavedDescriptionE = DescriptionE;
                            break;
                        case "DescriptionF":
                            DescriptionF = dataRow[column.ColumnName].ToString();
                            SavedDescriptionF = DescriptionF;
                            break;
                        case "DescriptionG":
                            DescriptionG = dataRow[column.ColumnName].ToString();
                            SavedDescriptionG = DescriptionG;
                            break;
                        case "DescriptionH":
                            DescriptionH = dataRow[column.ColumnName].ToString();
                            SavedDescriptionH = DescriptionH;
                            break;
                        case "DescriptionI":
                            DescriptionI = dataRow[column.ColumnName].ToString();
                            SavedDescriptionI = DescriptionI;
                            break;
                        case "DescriptionJ":
                            DescriptionJ = dataRow[column.ColumnName].ToString();
                            SavedDescriptionJ = DescriptionJ;
                            break;
                        case "DescriptionK":
                            DescriptionK = dataRow[column.ColumnName].ToString();
                            SavedDescriptionK = DescriptionK;
                            break;
                        case "DescriptionL":
                            DescriptionL = dataRow[column.ColumnName].ToString();
                            SavedDescriptionL = DescriptionL;
                            break;
                        case "DescriptionM":
                            DescriptionM = dataRow[column.ColumnName].ToString();
                            SavedDescriptionM = DescriptionM;
                            break;
                        case "DescriptionN":
                            DescriptionN = dataRow[column.ColumnName].ToString();
                            SavedDescriptionN = DescriptionN;
                            break;
                        case "DescriptionO":
                            DescriptionO = dataRow[column.ColumnName].ToString();
                            SavedDescriptionO = DescriptionO;
                            break;
                        case "DescriptionP":
                            DescriptionP = dataRow[column.ColumnName].ToString();
                            SavedDescriptionP = DescriptionP;
                            break;
                        case "DescriptionQ":
                            DescriptionQ = dataRow[column.ColumnName].ToString();
                            SavedDescriptionQ = DescriptionQ;
                            break;
                        case "DescriptionR":
                            DescriptionR = dataRow[column.ColumnName].ToString();
                            SavedDescriptionR = DescriptionR;
                            break;
                        case "DescriptionS":
                            DescriptionS = dataRow[column.ColumnName].ToString();
                            SavedDescriptionS = DescriptionS;
                            break;
                        case "DescriptionT":
                            DescriptionT = dataRow[column.ColumnName].ToString();
                            SavedDescriptionT = DescriptionT;
                            break;
                        case "DescriptionU":
                            DescriptionU = dataRow[column.ColumnName].ToString();
                            SavedDescriptionU = DescriptionU;
                            break;
                        case "DescriptionV":
                            DescriptionV = dataRow[column.ColumnName].ToString();
                            SavedDescriptionV = DescriptionV;
                            break;
                        case "DescriptionW":
                            DescriptionW = dataRow[column.ColumnName].ToString();
                            SavedDescriptionW = DescriptionW;
                            break;
                        case "DescriptionX":
                            DescriptionX = dataRow[column.ColumnName].ToString();
                            SavedDescriptionX = DescriptionX;
                            break;
                        case "DescriptionY":
                            DescriptionY = dataRow[column.ColumnName].ToString();
                            SavedDescriptionY = DescriptionY;
                            break;
                        case "DescriptionZ":
                            DescriptionZ = dataRow[column.ColumnName].ToString();
                            SavedDescriptionZ = DescriptionZ;
                            break;
                        case "CheckA":
                            CheckA = dataRow[column.ColumnName].ToBool();
                            SavedCheckA = CheckA;
                            break;
                        case "CheckB":
                            CheckB = dataRow[column.ColumnName].ToBool();
                            SavedCheckB = CheckB;
                            break;
                        case "CheckC":
                            CheckC = dataRow[column.ColumnName].ToBool();
                            SavedCheckC = CheckC;
                            break;
                        case "CheckD":
                            CheckD = dataRow[column.ColumnName].ToBool();
                            SavedCheckD = CheckD;
                            break;
                        case "CheckE":
                            CheckE = dataRow[column.ColumnName].ToBool();
                            SavedCheckE = CheckE;
                            break;
                        case "CheckF":
                            CheckF = dataRow[column.ColumnName].ToBool();
                            SavedCheckF = CheckF;
                            break;
                        case "CheckG":
                            CheckG = dataRow[column.ColumnName].ToBool();
                            SavedCheckG = CheckG;
                            break;
                        case "CheckH":
                            CheckH = dataRow[column.ColumnName].ToBool();
                            SavedCheckH = CheckH;
                            break;
                        case "CheckI":
                            CheckI = dataRow[column.ColumnName].ToBool();
                            SavedCheckI = CheckI;
                            break;
                        case "CheckJ":
                            CheckJ = dataRow[column.ColumnName].ToBool();
                            SavedCheckJ = CheckJ;
                            break;
                        case "CheckK":
                            CheckK = dataRow[column.ColumnName].ToBool();
                            SavedCheckK = CheckK;
                            break;
                        case "CheckL":
                            CheckL = dataRow[column.ColumnName].ToBool();
                            SavedCheckL = CheckL;
                            break;
                        case "CheckM":
                            CheckM = dataRow[column.ColumnName].ToBool();
                            SavedCheckM = CheckM;
                            break;
                        case "CheckN":
                            CheckN = dataRow[column.ColumnName].ToBool();
                            SavedCheckN = CheckN;
                            break;
                        case "CheckO":
                            CheckO = dataRow[column.ColumnName].ToBool();
                            SavedCheckO = CheckO;
                            break;
                        case "CheckP":
                            CheckP = dataRow[column.ColumnName].ToBool();
                            SavedCheckP = CheckP;
                            break;
                        case "CheckQ":
                            CheckQ = dataRow[column.ColumnName].ToBool();
                            SavedCheckQ = CheckQ;
                            break;
                        case "CheckR":
                            CheckR = dataRow[column.ColumnName].ToBool();
                            SavedCheckR = CheckR;
                            break;
                        case "CheckS":
                            CheckS = dataRow[column.ColumnName].ToBool();
                            SavedCheckS = CheckS;
                            break;
                        case "CheckT":
                            CheckT = dataRow[column.ColumnName].ToBool();
                            SavedCheckT = CheckT;
                            break;
                        case "CheckU":
                            CheckU = dataRow[column.ColumnName].ToBool();
                            SavedCheckU = CheckU;
                            break;
                        case "CheckV":
                            CheckV = dataRow[column.ColumnName].ToBool();
                            SavedCheckV = CheckV;
                            break;
                        case "CheckW":
                            CheckW = dataRow[column.ColumnName].ToBool();
                            SavedCheckW = CheckW;
                            break;
                        case "CheckX":
                            CheckX = dataRow[column.ColumnName].ToBool();
                            SavedCheckX = CheckX;
                            break;
                        case "CheckY":
                            CheckY = dataRow[column.ColumnName].ToBool();
                            SavedCheckY = CheckY;
                            break;
                        case "CheckZ":
                            CheckZ = dataRow[column.ColumnName].ToBool();
                            SavedCheckZ = CheckZ;
                            break;
                        case "LdapSearchRoot":
                            LdapSearchRoot = dataRow[column.ColumnName].ToString();
                            SavedLdapSearchRoot = LdapSearchRoot;
                            break;
                        case "SynchronizedTime":
                            SynchronizedTime = dataRow[column.ColumnName].ToDateTime();
                            SavedSynchronizedTime = SynchronizedTime;
                            break;
                        case "Comments":
                            Comments = dataRow[column.ColumnName].ToString().Deserialize<Comments>() ?? new Comments();
                            SavedComments = Comments.ToJson();
                            break;
                        case "Creator":
                            Creator = SiteInfo.User(dataRow[column.ColumnName].ToInt());
                            SavedCreator = Creator.Id;
                            break;
                        case "Updator":
                            Updator = SiteInfo.User(dataRow[column.ColumnName].ToInt());
                            SavedUpdator = Updator.Id;
                            break;
                        case "CreatedTime":
                            CreatedTime = new Time(dataRow, column.ColumnName);
                            SavedCreatedTime = CreatedTime.Value;
                            break;
                        case "UpdatedTime":
                            UpdatedTime = new Time(dataRow, column.ColumnName); Timestamp = dataRow.Field<DateTime>(column.ColumnName).ToString("yyyy/M/d H:m:s.fff");
                            SavedUpdatedTime = UpdatedTime.Value;
                            break;
                        case "IsHistory": VerType = dataRow[column.ColumnName].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                    }
                }
            }
        }

        public bool Updated()
        {
            return
                TenantId_Updated() ||
                UserId_Updated() ||
                Ver_Updated() ||
                LoginId_Updated() ||
                GlobalId_Updated() ||
                Name_Updated() ||
                UserCode_Updated() ||
                Password_Updated() ||
                LastName_Updated() ||
                FirstName_Updated() ||
                Birthday_Updated() ||
                Gender_Updated() ||
                Language_Updated() ||
                TimeZone_Updated() ||
                DeptId_Updated() ||
                FirstAndLastNameOrder_Updated() ||
                Body_Updated() ||
                LastLoginTime_Updated() ||
                PasswordExpirationTime_Updated() ||
                PasswordChangeTime_Updated() ||
                NumberOfLogins_Updated() ||
                NumberOfDenial_Updated() ||
                TenantManager_Updated() ||
                ServiceManager_Updated() ||
                Disabled_Updated() ||
                Developer_Updated() ||
                UserSettings_Updated() ||
                ApiKey_Updated() ||
                ClassA_Updated() ||
                ClassB_Updated() ||
                ClassC_Updated() ||
                ClassD_Updated() ||
                ClassE_Updated() ||
                ClassF_Updated() ||
                ClassG_Updated() ||
                ClassH_Updated() ||
                ClassI_Updated() ||
                ClassJ_Updated() ||
                ClassK_Updated() ||
                ClassL_Updated() ||
                ClassM_Updated() ||
                ClassN_Updated() ||
                ClassO_Updated() ||
                ClassP_Updated() ||
                ClassQ_Updated() ||
                ClassR_Updated() ||
                ClassS_Updated() ||
                ClassT_Updated() ||
                ClassU_Updated() ||
                ClassV_Updated() ||
                ClassW_Updated() ||
                ClassX_Updated() ||
                ClassY_Updated() ||
                ClassZ_Updated() ||
                NumA_Updated() ||
                NumB_Updated() ||
                NumC_Updated() ||
                NumD_Updated() ||
                NumE_Updated() ||
                NumF_Updated() ||
                NumG_Updated() ||
                NumH_Updated() ||
                NumI_Updated() ||
                NumJ_Updated() ||
                NumK_Updated() ||
                NumL_Updated() ||
                NumM_Updated() ||
                NumN_Updated() ||
                NumO_Updated() ||
                NumP_Updated() ||
                NumQ_Updated() ||
                NumR_Updated() ||
                NumS_Updated() ||
                NumT_Updated() ||
                NumU_Updated() ||
                NumV_Updated() ||
                NumW_Updated() ||
                NumX_Updated() ||
                NumY_Updated() ||
                NumZ_Updated() ||
                DateA_Updated() ||
                DateB_Updated() ||
                DateC_Updated() ||
                DateD_Updated() ||
                DateE_Updated() ||
                DateF_Updated() ||
                DateG_Updated() ||
                DateH_Updated() ||
                DateI_Updated() ||
                DateJ_Updated() ||
                DateK_Updated() ||
                DateL_Updated() ||
                DateM_Updated() ||
                DateN_Updated() ||
                DateO_Updated() ||
                DateP_Updated() ||
                DateQ_Updated() ||
                DateR_Updated() ||
                DateS_Updated() ||
                DateT_Updated() ||
                DateU_Updated() ||
                DateV_Updated() ||
                DateW_Updated() ||
                DateX_Updated() ||
                DateY_Updated() ||
                DateZ_Updated() ||
                DescriptionA_Updated() ||
                DescriptionB_Updated() ||
                DescriptionC_Updated() ||
                DescriptionD_Updated() ||
                DescriptionE_Updated() ||
                DescriptionF_Updated() ||
                DescriptionG_Updated() ||
                DescriptionH_Updated() ||
                DescriptionI_Updated() ||
                DescriptionJ_Updated() ||
                DescriptionK_Updated() ||
                DescriptionL_Updated() ||
                DescriptionM_Updated() ||
                DescriptionN_Updated() ||
                DescriptionO_Updated() ||
                DescriptionP_Updated() ||
                DescriptionQ_Updated() ||
                DescriptionR_Updated() ||
                DescriptionS_Updated() ||
                DescriptionT_Updated() ||
                DescriptionU_Updated() ||
                DescriptionV_Updated() ||
                DescriptionW_Updated() ||
                DescriptionX_Updated() ||
                DescriptionY_Updated() ||
                DescriptionZ_Updated() ||
                CheckA_Updated() ||
                CheckB_Updated() ||
                CheckC_Updated() ||
                CheckD_Updated() ||
                CheckE_Updated() ||
                CheckF_Updated() ||
                CheckG_Updated() ||
                CheckH_Updated() ||
                CheckI_Updated() ||
                CheckJ_Updated() ||
                CheckK_Updated() ||
                CheckL_Updated() ||
                CheckM_Updated() ||
                CheckN_Updated() ||
                CheckO_Updated() ||
                CheckP_Updated() ||
                CheckQ_Updated() ||
                CheckR_Updated() ||
                CheckS_Updated() ||
                CheckT_Updated() ||
                CheckU_Updated() ||
                CheckV_Updated() ||
                CheckW_Updated() ||
                CheckX_Updated() ||
                CheckY_Updated() ||
                CheckZ_Updated() ||
                LdapSearchRoot_Updated() ||
                SynchronizedTime_Updated() ||
                Comments_Updated() ||
                Creator_Updated() ||
                Updator_Updated();
        }

        public List<string> Mine()
        {
            var mine = new List<string>();
            var userId = Sessions.UserId();
            if (SavedCreator == userId) mine.Add("Creator");
            if (SavedUpdator == userId) mine.Add("Updator");
            return mine;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private UserSettings GetUserSettings(DataRow dataRow)
        {
            return dataRow["UserSettings"].ToString().Deserialize<UserSettings>() ??
                new UserSettings();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateMailAddresses()
        {
            var statements = new List<SqlStatement>();
            Session_MailAddresses()?.ForEach(mailAddress =>
                statements.Add(Rds.InsertMailAddresses(
                    param: Rds.MailAddressesParam()
                        .OwnerId(UserId)
                        .OwnerType("Users")
                        .MailAddress(mailAddress))));
            if (statements.Any())
            {
                statements.Insert(0, Rds.PhysicalDeleteMailAddresses(
                    where: Rds.MailAddressesWhere()
                        .OwnerId(UserId)
                        .OwnerType("Users")));
                Rds.ExecuteNonQuery(transactional: true, statements: statements.ToArray());
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetSiteInfo()
        {
            SiteInfo.Reflesh();
            if (Self()) SetSession();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void PasswordExpirationPeriod()
        {
            PasswordExpirationTime = Parameters.Authentication.PasswordExpirationPeriod != 0
                ? new Time(DateTime.Today.AddDays(
                    Parameters.Authentication.PasswordExpirationPeriod))
                : PasswordExpirationTime;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public RdsUser RdsUser()
        {
            return new RdsUser()
            {
                UserId = UserId,
                DeptId = DeptId
            };
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public UserModel(RdsUser.UserTypes userType)
        {
            OnConstructing();
            UserId = userType.ToInt();
            Get(SiteSettingsUtilities.UsersSiteSettings());
            OnConstructed();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public UserModel(string loginId)
        {
            var ss = SiteSettingsUtilities.UsersSiteSettings();
            SetByForm(ss);
            Get(ss, where: Rds.UsersWhere().LoginId(loginId));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public bool Self()
        {
            return Sessions.UserId() == UserId;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string Authenticate(string returnUrl)
        {
            if (Authenticate())
            {
                if (PasswordExpired())
                {
                    return OpenChangePasswordAtLoginDialog();
                }
                else
                {
                    return Allow(returnUrl);
                }
            }
            else
            {
                var tenantOptions = TenantOptions();
                if (tenantOptions?.Any() == true)
                {
                    return TenantsDropDown(tenantOptions);
                }
                else
                {
                    return Deny();
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public bool Authenticate()
        {
            var ret = false;
            if (!RejectUnregisteredUser())
            {
                switch (Parameters.Authentication.Provider)
                {
                    case "LDAP":
                        ret = Ldap.Authenticate(LoginId, Forms.Data("Users_Password"));
                        if (ret)
                        {
                            Get(SiteSettingsUtilities.UsersSiteSettings(),
                                where: Rds.UsersWhere().LoginId(LoginId));
                        }
                        break;
                    case "LDAP+Local":
                        ret = Ldap.Authenticate(LoginId, Forms.Data("Users_Password"));
                        if (ret)
                        {
                            Get(SiteSettingsUtilities.UsersSiteSettings(),
                                where: Rds.UsersWhere().LoginId(LoginId));
                        }
                        else
                        {
                            ret = GetByCredentials(LoginId, Password, Forms.Int("SelectedTenantId"));
                        }
                        break;
                    case "Extension":
                        var user = Extension.Authenticate(LoginId, Password);
                        ret = user != null;
                        if (ret)
                        {
                            Get(SiteSettingsUtilities.UsersSiteSettings(),
                                where: Rds.UsersWhere()
                                    .TenantId(user.TenantId)
                                    .UserId(user.Id));
                        }
                        break;
                    default:
                        ret = GetByCredentials(LoginId, Password, Forms.Int("SelectedTenantId"));
                        break;
                }
            }
            return ret;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private bool RejectUnregisteredUser()
        {
            return Parameters.Authentication.RejectUnregisteredUser &&
                Rds.ExecuteScalar_int(statements: Rds.SelectUsers(
                    column: Rds.UsersColumn().UsersCount(),
                    where: Rds.UsersWhere()
                        .LoginId(LoginId)
                        .Disabled(false))) != 1;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public bool GetByCredentials(string loginId, string password, int tenantId = 0)
        {
            Get(SiteSettingsUtilities.UsersSiteSettings(),
                where: Rds.UsersWhere()
                    .LoginId(loginId)
                    .Password(password)
                    .Disabled(0));
            return AccessStatus == Databases.AccessStatuses.Selected;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string TenantsDropDown(Dictionary<string, string> tenantOptions)
        {
            return new ResponseCollection()
                .Html("#Tenants", new HtmlBuilder().FieldDropDown(
                    controlId: "SelectedTenantId",
                    fieldCss: " field-wide",
                    controlCss: " always-send",
                    labelText: Displays.Tenants(),
                    optionCollection: tenantOptions)).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private Dictionary<string, string> TenantOptions()
        {
            return Rds.ExecuteScalar_string(statements:
                Rds.SelectLoginKeys(
                    column: Rds.LoginKeysColumn().TenantNames(),
                    where: Rds.LoginKeysWhere()
                        .LoginId(LoginId)))
                            .Deserialize<Dictionary<string, string>>();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string Allow(string returnUrl, bool atLogin = false)
        {
            Rds.ExecuteNonQuery(statements: Rds.UpdateUsers(
                where: Rds.UsersWhereDefault(this),
                param: Rds.UsersParam()
                    .NumberOfLogins(raw: "[NumberOfLogins] + 1")
                    .LastLoginTime(DateTime.Now)));
            SetFormsAuthentication(returnUrl);
            return new UsersResponseCollection(this)
                .CloseDialog(_using: atLogin)
                .Message(Messages.LoginIn())
                .Href(returnUrl == string.Empty
                    ? Locations.Top()
                    : returnUrl).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string Deny()
        {
            Rds.ExecuteNonQuery(statements: Rds.UpdateUsers(
                param: Rds.UsersParam()
                    .NumberOfDenial(raw: "[NumberOfDenial] + 1"),
                where: Rds.UsersWhere()
                    .LoginId(LoginId)));
            return Messages.ResponseAuthentication().Focus("#Password").ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string OpenChangePasswordAtLoginDialog()
        {
            return new ResponseCollection()
                .Invoke("openChangePasswordDialog")
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private bool PasswordExpired()
        {
            return
                PasswordExpirationTime.Value.InRange() &&
                PasswordExpirationTime.Value <= DateTime.Now;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void SetFormsAuthentication(string returnUrl)
        {
            System.Web.Security.FormsAuthentication.SetAuthCookie(
                UserId.ToString(), Forms.Bool("Users_RememberMe"));
            Sessions.SetTenantId(TenantId);
            SetSession();
            Libraries.Initializers.StatusesInitializer.Initialize(TenantId);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void SetSession()
        {
            Sessions.Set("UserId", UserId);
            Sessions.Set("Language", Language);
            Sessions.Set("Developer", Developer);
            Sessions.Set("TimeZoneInfo", TimeZoneInfo);
            Sessions.Set("RdsUser", RdsUser());
            Sessions.Set("UserSettings", UserSettings.ToJson());
            Contract.Set();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types ChangePassword()
        {
            Rds.ExecuteNonQuery(statements: Rds.UpdateUsers(
                where: Rds.UsersWhereDefault(this),
                param: ChangePasswordParam(ChangedPassword)));
            Get(SiteSettingsUtilities.UsersSiteSettings());
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types ChangePasswordAtLogin()
        {
            Rds.ExecuteNonQuery(statements: Rds.UpdateUsers(
                where: Rds.UsersWhereDefault(this),
                param: ChangePasswordParam(ChangedPassword, changeAtLogin: true)));
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types ResetPassword()
        {
            Rds.ExecuteNonQuery(statements: Rds.UpdateUsers(
                where: Rds.UsersWhereDefault(this),
                param: ChangePasswordParam(AfterResetPassword)));
            Get(SiteSettingsUtilities.UsersSiteSettings());
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public SqlParamCollection ChangePasswordParam(string password, bool changeAtLogin = false)
        {
            PasswordExpirationPeriod();
            var param = Rds.UsersParam()
                .Password(password)
                .PasswordChangeTime(raw: "getdate()");
            return Parameters.Authentication.PasswordExpirationPeriod > 0 || !changeAtLogin
                ? param.PasswordExpirationTime(PasswordExpirationTime.Value)
                : param.PasswordExpirationTime(raw: "null");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string ToControl(SiteSettings ss, Column column)
        {
            return UserId.ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string ToResponse()
        {
            return string.Empty;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public HtmlBuilder Td(HtmlBuilder hb, Column column)
        {
            return UserId != 0 ?
                hb.Td(action: () => hb
                    .HtmlUser(UserId)) :
                hb.Td(action: () => { });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string ToExport(Column column, ExportColumn exportColumn = null)
        {
            return SiteInfo.UserName(UserId);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types AddMailAddress(string mailAddress, IEnumerable<string> selected)
        {
            MailAddresses = Session_MailAddresses() ?? new List<string>();
            if (MailAddresses.Contains(mailAddress))
            {
                return Error.Types.AlreadyAdded;
            }
            else
            { 
                MailAddresses.Add(mailAddress);
                Session_MailAddresses(MailAddresses);
                return Error.Types.None;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types DeleteMailAddresses(IEnumerable<string> selected)
        {
            MailAddresses = Session_MailAddresses() ?? new List<string>();
            MailAddresses.RemoveAll(o => selected.Contains(o));
            Session_MailAddresses(MailAddresses);
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types CreateApiKey(SiteSettings ss)
        {
            ApiKey = Guid.NewGuid().ToString().Sha512Cng();
            return Update(ss);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types DeleteApiKey(SiteSettings ss)
        {
            ApiKey = string.Empty;
            return Update(ss);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public bool InitialValue()
        {
            return UserId == 0;
        }
    }
}

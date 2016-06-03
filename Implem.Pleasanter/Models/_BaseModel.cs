using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Items;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Web;
namespace Implem.Pleasanter.Models
{
    public class BaseModel
    {
        public enum MethodTypes
        {
            NotSet,
            Index,
            New,
            Edit
        }

        public HttpContext HttpContext;
        public Databases.AccessStatuses AccessStatus = Databases.AccessStatuses.Initialized;
        public MethodTypes MethodType = MethodTypes.NotSet;
        public SiteSettings SiteSettings;
        public Permissions.Types PermissionType;
        public virtual long UrlId { get { return 0; } }
        public Versions.VerTypes VerType = Versions.VerTypes.Latest;
        public int Ver = 1;
        public Comments Comments = new Comments();
        public User Creator = new User();
        public User Updator = new User();
        public Time CreatedTime = null;
        public Time UpdatedTime = null;
        public bool VerUp = false;
        public string Timestamp = string.Empty;
        public int DeleteCommentId;
        public int SavedVer = 1;
        public string SavedComments = string.Empty;
        public int SavedCreator = 0;
        public int SavedUpdator = 0;
        public DateTime SavedCreatedTime = 0.ToDateTime();
        public DateTime SavedUpdatedTime = 0.ToDateTime();
        public bool SavedVerUp = false;
        public string SavedTimestamp = string.Empty;
        public bool Ver_Updated { get { return Ver != SavedVer; } }
        public bool Comments_Updated { get { return Comments.ToJson() != SavedComments && Comments.ToJson() != null; } }
        public bool Creator_Updated { get { return Creator.Id != SavedCreator; } }
        public bool Updator_Updated { get { return Updator.Id != SavedUpdator; } }
        public bool CreatedTime_Updated { get { return CreatedTime.Value != SavedCreatedTime && CreatedTime.Value != null; } }
        public bool UpdatedTime_Updated { get { return UpdatedTime.Value != SavedUpdatedTime && UpdatedTime.Value != null; } }

        public BaseModel()
        {
            HttpContext = HttpContext.Current;
        }
    }

    public class BaseItemModel : BaseModel
    {
        public long SiteId = 0;
        public Title Title = new Title();
        public string Body = string.Empty;
        public long SavedSiteId = 0;
        public string SavedTitle = string.Empty;
        public string SavedBody = string.Empty;
        public bool SiteId_Updated { get { return SiteId != SavedSiteId; } }
        public bool Title_Updated { get { return Title.Value != SavedTitle && Title.Value != null; } }
        public bool Body_Updated { get { return Body != SavedBody && Body != null; } }
    }
}

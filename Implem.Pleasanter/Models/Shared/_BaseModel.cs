using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Models;
using System;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class BaseModel
    {
        public enum MethodTypes
        {
            NotSet,
            Index,
            New,
            Edit
        }

        public int? StatusCode;
        [NonSerialized] public Databases.AccessStatuses AccessStatus = Databases.AccessStatuses.Initialized;
        [NonSerialized] public MethodTypes MethodType = MethodTypes.NotSet;
        [NonSerialized] public Versions.VerTypes VerType = Versions.VerTypes.Latest;
        public int Ver = 1;
        public Comments Comments = new Comments();
        public User Creator = new User();
        public User Updator = new User();
        public Time CreatedTime = null;
        public Time UpdatedTime = null;
        public bool VerUp = false;
        public string Timestamp = string.Empty;
        [NonSerialized] public int DeleteCommentId;
        [NonSerialized] public int SavedVer = 1;
        [NonSerialized] public int SavedCreator = 0;
        [NonSerialized] public int SavedUpdator = 0;
        [NonSerialized] public DateTime SavedCreatedTime = 0.ToDateTime();
        [NonSerialized] public DateTime SavedUpdatedTime = 0.ToDateTime();
        [NonSerialized] public bool SavedVerUp = false;
        [NonSerialized] public string SavedTimestamp = string.Empty;
        [NonSerialized] public string SavedComments = "[]";

        public bool Ver_Updated
        {
            get
            {
                return Ver != SavedVer;
            }
        }

        public bool Comments_Updated
        {
            get
            {
                return Comments.ToJson() != SavedComments && Comments.ToJson() != null;
            }
        }

        public bool Creator_Updated
        {
            get
            {
                return Creator.Id != SavedCreator;
            }
        }

        public bool Updator_Updated
        {
            get
            {
                return Updator.Id != SavedUpdator;
            }
        }

        public bool CreatedTime_Updated
        {
            get
            {
                return CreatedTime.Value != SavedCreatedTime && CreatedTime.Value != null;
            }
        }

        public bool UpdatedTime_Updated
        {
            get
            {
                return UpdatedTime.Value != SavedUpdatedTime && UpdatedTime.Value != null;
            }
        }
    }

    public class BaseItemModel : BaseModel
    {
        public long SiteId = 0;
        public Title Title = new Title();
        public string Body = string.Empty;
        [NonSerialized] public long SavedSiteId = 0;
        [NonSerialized] public string SavedTitle = string.Empty;
        [NonSerialized] public string SavedBody = string.Empty;

        public bool SiteId_Updated
        {
            get
            {
                return SiteId != SavedSiteId;
            }
        }

        public bool Title_Updated
        {
            get
            {
                return Title.Value != SavedTitle && Title.Value != null;
            }
        }

        public bool Body_Updated
        {
            get
            {
                return Body != SavedBody && Body != null;
            }
        }
    }
}

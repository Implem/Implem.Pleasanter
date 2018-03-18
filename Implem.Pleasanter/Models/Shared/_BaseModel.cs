using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Settings;
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

        public bool Ver_Updated(Column column = null)
        {
            return Ver != SavedVer &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != Ver);
        }

        public bool Comments_Updated(Column column = null)
        {
            return Comments.ToJson() != SavedComments && Comments.ToJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != Comments.ToJson());
        }

        public bool Creator_Updated(Column column = null)
        {
            return Creator.Id != SavedCreator &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != Creator.Id);
        }

        public bool Updator_Updated(Column column = null)
        {
            return Updator.Id != SavedUpdator &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != Updator.Id);
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

        public bool SiteId_Updated(Column column = null)
        {
            return SiteId != SavedSiteId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToLong() != SiteId);
        }

        public bool Title_Updated(Column column = null)
        {
            return Title.Value != SavedTitle && Title.Value != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != Title.Value);
        }

        public bool Body_Updated(Column column = null)
        {
            return Body != SavedBody && Body != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != Body);
        }
    }
}

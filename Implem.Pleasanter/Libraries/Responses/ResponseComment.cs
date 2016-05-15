using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Utilities;
using Implem.Pleasanter.Libraries.ViewParts;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class ResponseComment
    {
        public static ResponseCollection PrependComment(
            this ResponseCollection responseCollection,
            Comments comments,
            Versions.VerTypes verType)
        {
            return Forms.Data("Comments").Trim() != string.Empty
                ? responseCollection
                    .Val("#Comments", string.Empty)
                    .Focus("#Comments")
                    .Prepend("#CommentList", Html.Builder()
                        .Comment(comment: comments[0], verType: verType))
                    .Markup()
                : responseCollection;
        }

        public static ResponseCollection RemoveComment(
            this ResponseCollection responseCollection, int commentId, bool _using)
        {
            return _using
                ? responseCollection
                    .Remove("#Comment" + commentId)
                    .Focus("#Comments")
                : responseCollection;
        }
    }
}
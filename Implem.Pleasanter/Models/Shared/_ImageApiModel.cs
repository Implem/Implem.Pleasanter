namespace Implem.Pleasanter.Models.Shared
{
    public class _ImageApiModel
    {
        public bool? HeadNewLine { get; set; }
        public bool? EndNewLine { get; set; }
        public int? Position { get; set; }
        public string Alt { get; set; }
        public string Extension { get; set; }
        public string Base64 { get; set; }

        public _ImageApiModel()
        {
            HeadNewLine = HeadNewLine ?? false;
            EndNewLine = EndNewLine ?? false;
            Position = Position ?? -1;
            Alt = Alt ?? "image";
            Extension = Extension ?? ".png";
        }
    }
}

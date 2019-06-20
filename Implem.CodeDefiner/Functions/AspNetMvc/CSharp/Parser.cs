using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
namespace Implem.CodeDefiner.Functions.AspNetMvc.CSharp
{
    internal class Parser
    {
        private string code;
        internal CodeTypes CodeType;
        internal string Id;
        internal string Name;
        internal bool Fixed;
        internal bool NotMerge;
        internal string Description;
        internal int IndexBase;
        internal int Indent;
        internal int IndentChildMember { get { return Indent + 4; } }
        internal Dictionary<int, string> TextCollection = new Dictionary<int, string>();
        internal Dictionary<int, Parser> MemberCollection = new Dictionary<int, Parser>();

        internal enum CodeTypes : int
        {
            Namespace,
            Class,
            Method
        }

        internal Parser(string code, int indexBase = 0, int indent = 0)
        {
            SetProperties(code, indexBase, indent);
            SetMemberCollection();
        }

        private void SetProperties(string code, int indexBase, int indent)
        {
            this.code = code.Replace("\r\n", "\n");
            IndexBase = indexBase;
            Indent = indent;
            SetId();
            SetDescription();
            SetFixed();
            SetNotMerge();
            SetCodeType();
            SetName();
        }

        private void SetId()
        {
            Id = code.RegexFirst(
                "^ {" + Indent + "}(public|private|protected|internal|namespace)[^{]+",
                RegexOptions.Multiline).Trim();
        }

        private void SetDescription()
        {
            Description = code.RegexFirst("(^ {" + Indent + "}///.*$\n)*", RegexOptions.Multiline);
        }

        private void SetFixed()
        {
            Fixed = Description.IndexOf("/// Fixed:") != -1;
        }

        private void SetNotMerge()
        {
            NotMerge = Description
                .RegexExists("^ {" + Indent + "}/// NotMerge:.*", RegexOptions.Multiline);
        }

        private void SetCodeType()
        {
            CodeType = Id.IndexOf(" class ") != -1
                ? CodeTypes.Class
                : Id.IndexOf("namespace ") != -1
                    ? CodeTypes.Namespace
                    : CodeTypes.Method;
        }

        private void SetName()
        {
            switch (CodeType)
            {
                case CodeTypes.Namespace:
                    Name = Id.RegexFirst("(?<=namespace )[A-Za-z0-9_.]+"); break;
                case CodeTypes.Class:
                    Name = Id.RegexFirst("(?<=class )[A-Za-z0-9_]+"); break;
                case CodeTypes.Method:
                    Name = Id.RegexFirst("(?<= )[A-Za-z0-9_]+(?=\\()"); break;
                default:
                    break;
            }
        }

        private void SetMemberCollection()
        {
            if (HasCodeBlocks())
            {
                var index = 0;
                var pattern = MemberCodePattern();
                foreach (Match codeBlock in code.RegexMatches(pattern, RegexOptions.Multiline))
                {
                    AddTextCollection(index, codeBlock.Index - index);
                    AddMemberCollection(codeBlock);
                    index = NextIndex(codeBlock);
                }
                AddTextCollection(index);
            }
            else
            {
                TextCollection.Add(IndexBase, code);
            }
        }

        private bool HasCodeBlocks()
        {
            return code.RegexExists(
                "^ {" + IndentChildMember + "}(public|private|protected|internal) .+[^;]$",
                RegexOptions.Multiline);
        }

        private string MemberCodePattern()
        {
            return
                "(^ {" + IndentChildMember + "}///.*$\n)*" +
                "(^ {" + IndentChildMember + "}\\[.*\\]$\n)*" +
                "^ {" + IndentChildMember + "}(public|private|protected|internal)[^{;]+?[ A-Za-z0-9_>\\)\\]]$\n" +
                "^ {" + IndentChildMember + "}{(?:.|\n)+?" +
                "^ {" + IndentChildMember + "}};?$";
        }

        private void AddTextCollection(int index, int length = -1)
        {
            string text;
            if (length != -1)
            {
                text = code.Substring(index, length);
            }
            else
            {
                text = code.Substring(index);
            }
            if (text.Trim() != string.Empty)
            {
                TextCollection.Add(IndexBase + index, text);
            }
        }

        private void AddMemberCollection(Match matchCode)
        {
            var index = IndexBase + matchCode.Index;
            MemberCollection.Add(index, new Parser(matchCode.Value, index, Indent + 4));
        }

        private int NextIndex(Match matchCode)
        {
            return matchCode.Index + matchCode.Value.Length;
        }

        internal string Code()
        {
            if (TextCollection.Count == 0)
            {
                return MemberCollection
                    .Take(1)
                    .ToDictionary(o => o.Key, o => o.Value.Code())
                    .Union(MemberCollection.Skip(1)
                    .ToDictionary(o => o.Key, o => "\r\n" + o.Value.Code()))
                    .OrderBy(o => o.Key)
                    .Select(o => o.Value)
                    .Join(string.Empty);
            }
            else
            {
                return TextCollection
                    .ToDictionary(o => o.Key, o => CrlfAdjustedCode(o.Key, o.Value))
                    .Union(MemberCollection
                    .ToDictionary(o => o.Key, o => o.Value.Code()))
                    .OrderBy(o => o.Key)
                    .Select(o => o.Value)
                    .Join(string.Empty);
            }
        }

        private string CrlfAdjustedCode(int key, string code)
        {
            var codeTemp = code.SplitReturn()
                .Where(o => o.Trim(' ', '\n') != string.Empty).Join("\r\n");
            if (IsNotRightCurlyBracket(key, codeTemp))
            {
                codeTemp = "\r\n" + codeTemp;
            }
            if (IsNotLeftCurlyBracket(codeTemp))
            {
                codeTemp += "\r\n";
            }
            return codeTemp;
        }

        private bool IsNotRightCurlyBracket(int key, string code)
        {
            return key != 0 && code != (new string(' ', Indent) + "}");
        }

        private bool IsNotLeftCurlyBracket(string code)
        {
            return !code.EndsWith("{");
        }

        internal void MergeCode(Parser margeCs, bool margeToExisting)
        {
            MergeCode(null, this, margeCs, margeToExisting);
        }

        private void MergeCode(
            Parser baseCsParent, Parser baseCs, Parser margeCs, bool margeToExisting)
        {
            if (margeCs.Fixed)
            {
                MergeFixedCode(baseCsParent, baseCs, margeCs);
            }
            if (margeToExisting)
            {
                MergeToExisting(baseCsParent, baseCs, margeCs);
            }
            margeCs.MemberCollection.Select(o => o.Value).ForEach(memberOld =>
                MergeCode(baseCs, SameMember(baseCs, memberOld), memberOld, margeToExisting));
        }

        private void MergeFixedCode(Parser baseCsParent, Parser baseCs, Parser margeCs)
        {
            if (TextCollectionCountIsSame(baseCs, margeCs))
            {
                MergeTextCollection(baseCs, margeCs);
            }
            else
            {
                AddMemberCollection(baseCsParent, margeCs);
            }
        }

        private void MergeToExisting(Parser baseCsParent, Parser baseCs, Parser margeCs)
        {
            if (HasMergeToExistingConditions(baseCs, margeCs))
            {
                MergeTextCollection(baseCs, margeCs);
            }
            else
            {
                if (HasAddToExistingConditions(baseCsParent, baseCs, margeCs))
                {
                    AddMemberCollection(baseCsParent, margeCs);
                }
            }
        }

        private bool HasMergeToExistingConditions(Parser baseCs, Parser margeCs)
        {
            if (baseCs == null)
            {
                return false;
            }
            else
            {
                return 
                    !baseCs.Fixed && !margeCs.NotMerge &&
                    TextCollectionCountIsSame(baseCs, margeCs);
            }
        }

        private bool HasAddToExistingConditions(
            Parser baseCsParent, Parser baseCs, Parser margeCs)
        {
            if (baseCsParent == null)
            {
                return false;
            }
            else
            {
                if (baseCs == null)
                {
                    return !margeCs.NotMerge;
                }
                else
                {
                    return !(baseCs.Fixed || margeCs.NotMerge);
                }
            }
        }

        private Parser SameMember(Parser baseCs, Parser margeCs)
        {
            return baseCs != null
                ? baseCs.MemberCollection
                    .Select(o => o.Value)
                    .FirstOrDefault(o => o.Id == margeCs.Id)
                : null;
        }

        private bool TextCollectionCountIsSame(Parser baseCs, Parser margeCs)
        {
            return
                baseCs != null &&
                baseCs.TextCollection.Count() == margeCs.TextCollection.Count();
        }

        private void MergeTextCollection(Parser baseCs, Parser margeCs)
        {
            var oldTextCollection = margeCs.TextCollection.Values.ToList();
            baseCs.TextCollection.Keys.ToList()
                .Select((o, i) => new { Index = i, Key = o })
                .ForEach(data =>
                    baseCs.TextCollection[data.Key] = oldTextCollection[data.Index]);
        }

        private void AddMemberCollection(Parser baseCsParent, Parser margeCs)
        {
            int index;
            if (baseCsParent.MemberCollection.Any())
            {
                index = baseCsParent.MemberCollection.Max(o => o.Key) + 1;
            }
            else
            {
                if (baseCsParent.TextCollection.Count() == 1)
                {
                    SplitTextCollection(baseCsParent);
                }
                index = baseCsParent.IndexBase + 1;
            }
            baseCsParent.MemberCollection.Add(index, margeCs);
        }

        private void SplitTextCollection(Parser baseCsParent)
        {
            var key = baseCsParent.TextCollection.First().Key;
            var lineCollection = baseCsParent.TextCollection[key].SplitReturn();
            baseCsParent.TextCollection[key] = lineCollection
                .Take(lineCollection.Count() - 1)
                .Join("\n");
            baseCsParent.TextCollection.Add(
                key + baseCsParent.TextCollection[key].Length,
                lineCollection.Last());
        }
    }
}

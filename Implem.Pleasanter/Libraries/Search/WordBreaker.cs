using CSharp.Japanese.Kanaxs;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Search
{
    public class WordBreaker
    {
        public List<string> Results = new List<string>();
        public bool CreateIndex;
        private List<string> Alphabets = new List<string>();

        private enum CharTypes
        {
            Space,
            Symbol,
            Numeric,
            Lower,
            Upper,
            Hiragana,
            Katakana,
            Macron,
            Other,
            NotSet
        }

        public WordBreaker(string str, bool createIndex)
        {
            CreateIndex = createIndex;
            Break(str);
        }

        private void Break(string str)
        {
            var word = string.Empty;
            var currentType = CharTypes.NotSet;
            var beforeType = CharTypes.NotSet;
            var work = Convert(str);
            work.ForEach(_char =>
            {
                currentType = Type(_char);
                if (beforeType != currentType)
                {
                    if (currentType == CharTypes.Lower && beforeType == CharTypes.Upper)
                    {
                        word += _char;
                    }
                    else if (currentType == CharTypes.Space)
                    {
                        Alphabets.Clear();
                        Add(beforeType, word);
                        word = string.Empty;
                    }
                    else if (currentType == CharTypes.Macron)
                    {
                        switch (beforeType)
                        {
                            case CharTypes.Hiragana:
                            case CharTypes.Katakana:
                                currentType = beforeType;
                                word += _char;
                                break;
                            default:
                                Add(beforeType, word);
                                word = _char.ToString();
                                break;
                        }
                    }
                    else
                    {
                        Add(beforeType, word);
                        word = _char.ToString();
                    }
                    beforeType = currentType;
                }
                else if (currentType == CharTypes.Space)
                {
                    Alphabets.Clear();
                    word = string.Empty;
                }
                else if (currentType == CharTypes.Symbol)
                {
                    Add(beforeType, word);
                    word = _char.ToString();
                }
                else
                {
                    word += _char;
                }
            });
            Add(beforeType, word);
        }

        private void Add(CharTypes type, string word)
        {
            if (word != string.Empty)
            {
                switch (type)
                {
                    case CharTypes.Hiragana:
                        word = KanaEx.ToKatakana(word);
                        break;
                }
                if (word.Length >= 2 && word.EndsWith("ー"))
                {
                    word = word.Substring(0, word.Length - 1);
                }
                Results.Add(word);
                if (CreateIndex)
                {
                    AddAlphabets(type, word);
                    AddOthers(type, word);
                }
            }
        }

        private void AddAlphabets(CharTypes type, string word)
        {
            if (type == CharTypes.Lower || type == CharTypes.Upper)
            {
                Alphabets.Select((o, i) => new { Alphabet = o, Index = i }).ForEach(data =>
                {
                    for (var i = data.Index + 1; i <= Alphabets.Count() && i <= 3; i++)
                    {
                        Results.Add(Alphabets
                            .Skip(data.Index)
                            .Take(i)
                            .Join(string.Empty) + word);
                    }
                });
                Alphabets.Add(word);
            }
            else
            {
                Alphabets.Clear();
            }
        }

        private void AddOthers(CharTypes type, string word)
        {
            if (type == CharTypes.Other && word.Length > 2)
            {
                for (var i = 0; i < word.Length; i++)
                {
                    for (var j = 1; i + j <= word.Length; j++)
                    {
                        Results.Add(word.Substring(i, j));
                    }
                }
            }
        }

        private string Convert(string str)
        {
            if (str == null) return string.Empty;
            var ret = str.Trim();
            ret = KanaEx.ToHankaku(ret);
            ret = KanaEx.ToZenkakuKana(ret);
            return ret;
        }

        private CharTypes Type(char _char)
        {
            switch (_char)
            {
                case ' ':
                case '\r':
                case '\n':
                case '\t':
                    return CharTypes.Space;
                case '!':
                case '"':
                case '#':
                case '$':
                case '%':
                case '&':
                case '\'':
                case '(':
                case ')':
                case '*':
                case '+':
                case ',':
                case '-':
                case '.':
                case '/':
                case ':':
                case ';':
                case '<':
                case '=':
                case '>':
                case '?':
                case '@':
                case '[':
                case '\\':
                case ']':
                case '^':
                case '_':
                case '`':
                case '{':
                case '|':
                case '}':
                case '~':
                case '、':
                case '。':
                case '，':
                case '．':
                case '・':
                case '：':
                case '；':
                case '？':
                case '！':
                case '゛':
                case '゜':
                case '´':
                case '｀':
                case '¨':
                case '＾':
                case '￣':
                case '＿':
                case 'ヽ':
                case 'ヾ':
                case 'ゝ':
                case 'ゞ':
                case '〃':
                case '仝':
                case '々':
                case '〆':
                case '〇':
                case '―':
                case '‐':
                case '／':
                case '＼':
                case '～':
                case '∥':
                case '｜':
                case '…':
                case '‥':
                case '‘':
                case '’':
                case '“':
                case '”':
                case '（':
                case '）':
                case '〔':
                case '〕':
                case '［':
                case '］':
                case '｛':
                case '｝':
                case '〈':
                case '〉':
                case '《':
                case '》':
                case '「':
                case '」':
                case '『':
                case '』':
                case '【':
                case '】':
                case '＋':
                case '－':
                case '±':
                case '×':
                case '÷':
                case '＝':
                case '≠':
                case '＜':
                case '＞':
                case '≦':
                case '≧':
                case '∞':
                case '∴':
                case '♂':
                case '♀':
                case '°':
                case '′':
                case '″':
                case '℃':
                case '￥':
                case '＄':
                case '￠':
                case '￡':
                case '％':
                case '＃':
                case '＆':
                case '＊':
                case '＠':
                case '§':
                case '☆':
                case '★':
                case '○':
                case '●':
                case '◎':
                case '◇':
                case '◆':
                case '□':
                case '■':
                case '△':
                case '▲':
                case '▽':
                case '▼':
                case '※':
                case '〒':
                case '→':
                case '←':
                case '↑':
                case '↓':
                case '〓':
                case '∈':
                case '∋':
                case '⊆':
                case '⊇':
                case '⊂':
                case '⊃':
                case '∪':
                case '∩':
                case '∧':
                case '∨':
                case '￢':
                case '⇒':
                case '⇔':
                case '∀':
                case '∃':
                case '∠':
                case '⊥':
                case '⌒':
                case '∂':
                case '∇':
                case '≡':
                case '≒':
                case '≪':
                case '≫':
                case '√':
                case '∽':
                case '∝':
                case '∵':
                case '∫':
                case '∬':
                case 'Å':
                case '‰':
                case '♯':
                case '♭':
                case '♪':
                case '─':
                case '│':
                case '┌':
                case '┐':
                case '┘':
                case '└':
                case '├':
                case '┬':
                case '┤':
                case '┴':
                case '┼':
                case '━':
                case '┃':
                case '┏':
                case '┓':
                case '┛':
                case '┗':
                case '┣':
                case '┳':
                case '┫':
                case '┻':
                case '╋':
                case '┠':
                case '┯':
                case '┨':
                case '┷':
                case '┿':
                case '┝':
                case '┰':
                case '┥':
                case '┸':
                case '╂':
                case '①':
                case '②':
                case '③':
                case '④':
                case '⑤':
                case '⑥':
                case '⑦':
                case '⑧':
                case '⑨':
                case '⑩':
                case '⑪':
                case '⑫':
                case '⑬':
                case '⑭':
                case '⑮':
                case '⑯':
                case '⑰':
                case '⑱':
                case '⑲':
                case '⑳':
                case 'Ⅰ':
                case 'Ⅱ':
                case 'Ⅲ':
                case 'Ⅳ':
                case 'Ⅴ':
                case 'Ⅵ':
                case 'Ⅶ':
                case 'Ⅷ':
                case 'Ⅸ':
                case 'Ⅹ':
                case '㍉':
                case '㌔':
                case '㌢':
                case '㍍':
                case '㌘':
                case '㌧':
                case '㌃':
                case '㌶':
                case '㍑':
                case '㍗':
                case '㌍':
                case '㌦':
                case '㌣':
                case '㌫':
                case '㍊':
                case '㌻':
                case '㎜':
                case '㎝':
                case '㎞':
                case '㎎':
                case '㎏':
                case '㏄':
                case '㎡':
                case '　':
                case '㍻':
                case '〝':
                case '〟':
                case '№':
                case '㏍':
                case '℡':
                case '㊤':
                case '㊥':
                case '㊦':
                case '㊧':
                case '㊨':
                case '㈱':
                case '㈲':
                case '㈹':
                case '㍾':
                case '㍽':
                case '㍼':
                case '∮':
                case '∑':
                case '∟':
                case '⊿':
                    return CharTypes.Symbol;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return CharTypes.Numeric;
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'h':
                case 'i':
                case 'j':
                case 'k':
                case 'l':
                case 'm':
                case 'n':
                case 'o':
                case 'p':
                case 'q':
                case 'r':
                case 's':
                case 't':
                case 'u':
                case 'v':
                case 'w':
                case 'x':
                case 'y':
                case 'z':
                    return CharTypes.Lower;
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                case 'G':
                case 'H':
                case 'I':
                case 'J':
                case 'K':
                case 'L':
                case 'M':
                case 'N':
                case 'O':
                case 'P':
                case 'Q':
                case 'R':
                case 'S':
                case 'T':
                case 'U':
                case 'V':
                case 'W':
                case 'X':
                case 'Y':
                case 'Z':
                    return CharTypes.Upper;
                case 'ぁ':
                case 'あ':
                case 'ぃ':
                case 'い':
                case 'ぅ':
                case 'う':
                case 'ぇ':
                case 'え':
                case 'ぉ':
                case 'お':
                case 'か':
                case 'が':
                case 'き':
                case 'ぎ':
                case 'く':
                case 'ぐ':
                case 'け':
                case 'げ':
                case 'こ':
                case 'ご':
                case 'さ':
                case 'ざ':
                case 'し':
                case 'じ':
                case 'す':
                case 'ず':
                case 'せ':
                case 'ぜ':
                case 'そ':
                case 'ぞ':
                case 'た':
                case 'だ':
                case 'ち':
                case 'ぢ':
                case 'っ':
                case 'つ':
                case 'づ':
                case 'て':
                case 'で':
                case 'と':
                case 'ど':
                case 'な':
                case 'に':
                case 'ぬ':
                case 'ね':
                case 'の':
                case 'は':
                case 'ば':
                case 'ぱ':
                case 'ひ':
                case 'び':
                case 'ぴ':
                case 'ふ':
                case 'ぶ':
                case 'ぷ':
                case 'へ':
                case 'べ':
                case 'ぺ':
                case 'ほ':
                case 'ぼ':
                case 'ぽ':
                case 'ま':
                case 'み':
                case 'む':
                case 'め':
                case 'も':
                case 'ゃ':
                case 'や':
                case 'ゅ':
                case 'ゆ':
                case 'ょ':
                case 'よ':
                case 'ら':
                case 'り':
                case 'る':
                case 'れ':
                case 'ろ':
                case 'ゎ':
                case 'わ':
                case 'ゐ':
                case 'ゑ':
                case 'を':
                case 'ん':
                case 'ゔ':
                case 'ゕ':
                case 'ゖ':
                case 'ゟ':
                    return CharTypes.Hiragana;
                case 'ァ':
                case 'ア':
                case 'ィ':
                case 'イ':
                case 'ゥ':
                case 'ウ':
                case 'ェ':
                case 'エ':
                case 'ォ':
                case 'オ':
                case 'カ':
                case 'ガ':
                case 'キ':
                case 'ギ':
                case 'ク':
                case 'グ':
                case 'ケ':
                case 'ゲ':
                case 'コ':
                case 'ゴ':
                case 'サ':
                case 'ザ':
                case 'シ':
                case 'ジ':
                case 'ス':
                case 'ズ':
                case 'セ':
                case 'ゼ':
                case 'ソ':
                case 'ゾ':
                case 'タ':
                case 'ダ':
                case 'チ':
                case 'ヂ':
                case 'ッ':
                case 'ツ':
                case 'ヅ':
                case 'テ':
                case 'デ':
                case 'ト':
                case 'ド':
                case 'ナ':
                case 'ニ':
                case 'ヌ':
                case 'ネ':
                case 'ノ':
                case 'ハ':
                case 'バ':
                case 'パ':
                case 'ヒ':
                case 'ビ':
                case 'ピ':
                case 'フ':
                case 'ブ':
                case 'プ':
                case 'ヘ':
                case 'ベ':
                case 'ペ':
                case 'ホ':
                case 'ボ':
                case 'ポ':
                case 'マ':
                case 'ミ':
                case 'ム':
                case 'メ':
                case 'モ':
                case 'ャ':
                case 'ヤ':
                case 'ュ':
                case 'ユ':
                case 'ョ':
                case 'ヨ':
                case 'ラ':
                case 'リ':
                case 'ル':
                case 'レ':
                case 'ロ':
                case 'ヮ':
                case 'ワ':
                case 'ヰ':
                case 'ヱ':
                case 'ヲ':
                case 'ン':
                case 'ヴ':
                case 'ヵ':
                case 'ヶ':
                case 'ヷ':
                case 'ヸ':
                case 'ヹ':
                case 'ヺ':
                case 'ヿ':
                    return CharTypes.Katakana;
                case 'ー':
                    return CharTypes.Macron;
                default:
                    return CharTypes.Other;
            }
        }
    }
}
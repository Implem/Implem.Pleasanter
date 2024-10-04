using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System.Linq;
using Xunit;

namespace Implem.PleasanterTest.Tests.Users
{
    [Collection(nameof(UsersGeneratePassword))]
    public class UsersGeneratePassword
    {
        private static readonly string passwordObject = "#Users_ChangedPassword";
        private static readonly string passwordValidateObject = "#Users_ChangedPassword";
        private static readonly int minLength = 6;
        private static readonly char[] alphabet = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'];
        private static readonly char[] number = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];
        private static readonly char[] symbol = [' ', '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.', '/', ':', ';', '<', '=', '>', '?', '@', '[', '\\', ']', '^', '_', '`', '{', '|', '}', '~'];

        [Fact]
        public void PasswordPolicies0_Test()
        {
            //Security.json PasswordPolicies[0] のみが有効（default）
            for (int i = 0; i < 10; i++)
            {
                var minLength = 6;
                var pw = GeneratedPassword();
                //生成されるパスワードが6文字以上あること
                Assert.True(pw.Length >= minLength);
            }
        }

        [Fact]
        public void PasswordPolicies1_Test()
        {
            //Security.json PasswordPolicies[0],[1]が有効
            Parameters.Security.PasswordPolicies[1].Enabled = true;
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    var pw = GeneratedPassword();
                    //生成されるパスワードが6文字以上あること
                    Assert.True(pw.Length >= minLength);
                    //小文字のアルファベットが１文字以上あること
                    Assert.Contains(pw, c => alphabet.Contains(c));
                }
            }
            finally
            {
                Parameters.Security.PasswordPolicies[1].Enabled = false;
            }
        }

        [Fact]
        public void PasswordPolicies3_Test()
        {
            //Security.json PasswordPolicies[0],[1],[2]が有効
            Parameters.Security.PasswordPolicies[1].Enabled = true;
            Parameters.Security.PasswordPolicies[2].Enabled = true;
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    var pw = GeneratedPassword();
                    //生成されるパスワードが6文字以上あること
                    Assert.True(pw.Length >= minLength);
                    //小文字のアルファベットが１文字以上あること
                    Assert.Contains(pw, c => alphabet.Contains(c));
                    //大文字のアルファベットが１文字以上あること
                    Assert.Contains(pw, c => alphabet.Select(a => char.ToUpper(a)).Contains(c));
                }
            }
            finally
            {
                Parameters.Security.PasswordPolicies[1].Enabled = false;
                Parameters.Security.PasswordPolicies[2].Enabled = false;
            }
        }

        [Fact]
        public void PasswordPolicies4_Test()
        {
            //Security.json PasswordPolicies[0],[1],[2],[3]が有効
            Parameters.Security.PasswordPolicies[1].Enabled = true;
            Parameters.Security.PasswordPolicies[2].Enabled = true;
            Parameters.Security.PasswordPolicies[3].Enabled = true;
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    var pw = GeneratedPassword();
                    //生成されるパスワードが6文字以上あること
                    Assert.True(pw.Length >= minLength);
                    //小文字のアルファベットが１文字以上あること
                    Assert.Contains(pw, c => alphabet.Contains(c));
                    //大文字のアルファベットが１文字以上あること
                    Assert.Contains(pw, c => alphabet.Select(a => char.ToUpper(a)).Contains(c));
                    //数字が１文字以上あること
                    Assert.Contains(pw, c => number.Contains(c));
                }
            }
            finally
            {
                Parameters.Security.PasswordPolicies[1].Enabled = false;
                Parameters.Security.PasswordPolicies[2].Enabled = false;
                Parameters.Security.PasswordPolicies[3].Enabled = false;
            }
        }

        [Fact]
        public void PasswordPolicies5_Test()
        {
            //Security.json PasswordPolicies[0],[1],[2],[3],[4]が有効
            Parameters.Security.PasswordPolicies[1].Enabled = true;
            Parameters.Security.PasswordPolicies[2].Enabled = true;
            Parameters.Security.PasswordPolicies[3].Enabled = true;
            Parameters.Security.PasswordPolicies[4].Enabled = true;
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    var pw = GeneratedPassword();
                    //生成されるパスワードが6文字以上あること
                    Assert.True(pw.Length >= minLength);
                    //小文字のアルファベットが１文字以上あること
                    Assert.Contains(pw, c => alphabet.Contains(c));
                    //大文字のアルファベットが１文字以上あること
                    Assert.Contains(pw, c => alphabet.Select(a => char.ToUpper(a)).Contains(c));
                    //数字が１文字以上あること
                    Assert.Contains(pw, c => number.Contains(c));
                    //記号が１文字以上あること
                    Assert.Contains(pw, c => symbol.Contains(c));
                }
            }
            finally
            {
                Parameters.Security.PasswordPolicies[1].Enabled = false;
                Parameters.Security.PasswordPolicies[2].Enabled = false;
                Parameters.Security.PasswordPolicies[3].Enabled = false;
                Parameters.Security.PasswordPolicies[4].Enabled = false;
            }
        }

        [Fact]
        public void PasswordPolicies6_Test()
        {
            //Security.json PasswordPolicies[0] のみが有効（default）
            var saveRegex = Parameters.Security.PasswordPolicies[0].Regex;
            try
            {
                var maxLen = 24;
                for (int i = 6; i <= maxLen; i++)
                {
                    Parameters.Security.PasswordPolicies[0].Regex = $".{{{i},{maxLen}}}";
                    var pw = GeneratedPassword();
                    //生成されるパスワードが i 以上 maxLen 以下であること
                    Assert.True(pw.Length >=i && pw.Length <= 24);
                }
            }
            finally
            {
                Parameters.Security.PasswordPolicies[0].Regex = saveRegex;
            }
        }


        private static string GeneratedPassword()
        {
            var results = UserUtilities.GeneratePassword(passwordObject, passwordValidateObject);
            var response = results.Deserialize<ResponseCollection>();
            var pw1 = (string)response.FirstOrDefault(o => o.Method == "SetValue" && o.Target == passwordObject)?.Value;
            var pw2 = (string)response.FirstOrDefault(o => o.Method == "SetValue" && o.Target == passwordValidateObject)?.Value;
            //Nullでない&２つの値が一致していること
            Assert.NotNull(pw1);
            Assert.NotNull(pw2);
            Assert.True(pw1 == pw2);
            return pw1;
        }

    }
}

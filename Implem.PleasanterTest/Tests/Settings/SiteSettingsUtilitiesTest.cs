using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Implem.PleasanterTest.Tests.Settings
{
    public class SiteSettingsUtilitiesTest: IDisposable
    {
        private ParameterAccessor.Parts.User savedUser;
        public SiteSettingsUtilitiesTest()
        {
            //Parameters を変更する場合は値を保持しておき終了時に元に戻す
            savedUser = Parameters.User;
            Parameters.User = new ParameterAccessor.Parts.User();
        }

        public void Dispose()
        {
            Parameters.User = savedUser;
        }

        [Theory]
        [MemberData(nameof(GetData))]
        public void UsersSiteSettingsTest(
            bool? contractSettingsApi,
            bool parametersUserDisableApi,
            bool contextDisableApi,
            bool displayUserCheckBox)
        {
            var context = ContextData.Get(
                userId: 1,
                routeData: RouteData.UsersEdit(1));
            context.ContractSettings = new ContractSettings() { Api = contractSettingsApi };
            context.DisableApi = contextDisableApi;
            Parameters.User = new ParameterAccessor.Parts.User() { DisableApi= parametersUserDisableApi };
            var ss = SiteSettingsUtilities.UsersSiteSettings(context: context);
            var hideCheckBox = ss.ReadColumnAccessControls?
                .Any(o => o.ColumnName == "AllowApi"
                    && o.Type == Permissions.Types.ManageService) ?? false;
            Assert.Equal(!hideCheckBox, displayUserCheckBox);
        }

        public static IEnumerable<object[]> GetData()
        {
            var contractSettingsApi = new bool?[]
            {
                null,true,false,
                null,true,false,
                null,true,false,
                null,true,false,
                null,true,false,
                null,true,false,
                null,true,false,
                null,true,false,
            };
            var parametersUserdisableApi = new bool[]
            {
                true,true,true,
                false,false,false,
                true,true,true,
                false,false,false,
                true,true,true,
                false,false,false,
                true,true,true,
                false,false,false,
            };
            var contextDisableApi = new bool[]
            {
                true,true,true,
                true,true,true,
                true,true,true,
                true,true,true,
                false,false,false,
                false,false,false,
                false,false,false,
                false,false,false,
            };
            var displayUserCheckBox = new bool[]
            {
                true,true,false,
                true,true,false,
                true,true,false,
                true,true,false,
                true,true,false,
                false,false,false,
                true,true,false,
                false,false,false,
            };
            return contractSettingsApi
                .Zip(parametersUserdisableApi,
                    (a, b) => new object[] { a, b })
                .Zip(contextDisableApi,
                    (ab, c) => new object[] { ab[0], ab[1], c })
                .Zip(displayUserCheckBox,
                    (abc, d) => new object[] { abc[0], abc[1], abc[2], d });
        }

    }
}

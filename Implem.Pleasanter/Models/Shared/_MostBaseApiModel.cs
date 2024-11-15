using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace Implem.Pleasanter.Models.Shared
{
    [Serializable]
    public class _MostBaseApiModel
    {
        public decimal ApiVersion { get; set; } = Parameters.Api.Version;
        public bool? VerUp { get; set; }

        public _MostBaseApiModel()
        {
        }
    }
}
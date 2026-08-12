using System.Collections.Generic;
using System.Linq;

namespace Implem.ParameterAccessor.Parts
{
    public static class EnvironmentVariablesUtilities
    {
        public static bool IsMatchedEnvironment(
            List<string> environmentVariables,
            string deploymentEnvironment)
        {
            if (environmentVariables == null)
            {
                return true;
            }
            return environmentVariables.Any(o => o == deploymentEnvironment);
        }
    }
}

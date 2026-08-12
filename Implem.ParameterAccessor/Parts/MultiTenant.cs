namespace Implem.ParameterAccessor.Parts
{
    public class MultiTenant
    {
        public int DefaultTenantId { get; set; } = 1;

        public bool IsProtectedTenant(int tenantId)
        {
            return tenantId == DefaultTenantId;
        }
    }
}

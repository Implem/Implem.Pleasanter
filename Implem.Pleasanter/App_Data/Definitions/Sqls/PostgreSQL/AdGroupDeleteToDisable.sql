update "Groups" 
set "Disabled" = true, "Updator" = @ipU, "UpdatedTime" = CURRENT_TIMESTAMP
where ("TenantId" = @ipT)
    and ("Disabled" = false)
    and ("SynchronizedTime" is not null)
    and ("SynchronizedTime" <> @SynchronizedTime#CommandCount#);
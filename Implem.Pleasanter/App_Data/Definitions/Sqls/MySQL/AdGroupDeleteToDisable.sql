update "Groups" 
set "Disabled" = 1, "Updator" = @ipU, "UpdatedTime" = current_timestamp(3)
where ("TenantId" = @ipT)
    and ("Disabled" = false)
    and ("SynchronizedTime" is not null)
    and ("SynchronizedTime" <> @SynchronizedTime#CommandCount#);
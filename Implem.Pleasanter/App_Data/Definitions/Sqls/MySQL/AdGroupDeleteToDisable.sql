update "Groups" 
set "Disabled" = 1, "Updator" = @ipU, "UpdatedTime" = CURRENT_TIMESTAMP
where ("TenantId" = @ipT)
    and ("Disabled" = false)
    and ("SynchronizedTime" is not null)
    and ("SynchronizedTime" <> str_to_date(@SynchronizedTime#CommandCount#, '%Y/%m/%d %H:%i:%s'));
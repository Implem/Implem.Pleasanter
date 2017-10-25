while (1)
{
    try
    {
        Invoke-RestMethod http://localhost/pleasanter/reminderschedules/remind?NoLog=1
        [DateTime]::Now.ToString() + ": " + "success"
    }
    catch
    {
        [DateTime]::Now.ToString() + ": " + $error
    }
    Start-Sleep -s 5
}
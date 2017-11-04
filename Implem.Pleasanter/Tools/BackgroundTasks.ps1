while (1)
{
    try
    {
        Invoke-RestMethod http://localhost/pleasanter/backgroundtasks/do?NoLog=1
        [DateTime]::Now.ToString() + ": " + "success"
    }
    catch
    {
        [DateTime]::Now.ToString() + ": " + $error
    }
	Start-Sleep -s 5
}
#Requires -Version 3.0

<#
.SYNOPSIS
Crea e implementa un sitio web de Microsoft Azure para un proyecto web de Visual Studio.
Para obtener documentación más detallada, vaya a http://go.microsoft.com/fwlink/?LinkID=394471 

.EXAMPLE
PS C:\> .\Publish-WebApplicationWebSite.ps1 `
-Configuration .\Configurations\WebApplication1-WAWS-dev.json `
-WebDeployPackage ..\WebApplication1\WebApplication1.zip `
-Verbose

#>
[CmdletBinding(HelpUri = 'http://go.microsoft.com/fwlink/?LinkID=391696')]
param
(
    [Parameter(Mandatory = $true)]
    [ValidateScript({Test-Path $_ -PathType Leaf})]
    [String]
    $Configuration,

    [Parameter(Mandatory = $false)]
    [String]
    $SubscriptionName,

    [Parameter(Mandatory = $false)]
    [ValidateScript({Test-Path $_ -PathType Leaf})]
    [String]
    $WebDeployPackage,

    [Parameter(Mandatory = $false)]
    [ValidateScript({ !($_ | Where-Object { !$_.Contains('Name') -or !$_.Contains('Password')}) })]
    [Hashtable[]]
    $DatabaseServerPassword,

    [Parameter(Mandatory = $false)]
    [Switch]
    $SendHostMessagesToOutput = $false
)


function New-WebDeployPackage
{
    #Escriba una función para compilar y empaquetar la aplicación web.

    #Para compilar la aplicación web, use MsBuild.exe. Si necesita ayuda, consulte la referencia de la línea de comandos de MSBuild en http://go.microsoft.com/fwlink/?LinkId=391339.
}

function Test-WebApplication
{
    #Editar esta función para ejecutar pruebas unitarias en la aplicación web

    #Escriba una función para ejecutar pruebas unitarias en la aplicación web. Use VSTest.Console.exe. Si necesita ayuda, consulte la referencia de la línea de comandos de VSTest.Console en http://go.microsoft.com/fwlink/?LinkId=391340.
}

function New-AzureWebApplicationWebsiteEnvironment
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [Object]
        $Configuration,

        [Parameter (Mandatory = $false)]
        [AllowNull()]
        [Hashtable[]]
        $DatabaseServerPassword
    )
       
    Add-AzureWebsite -Name $Config.name -Location $Config.location | Out-String | Write-HostWithTime
    # Cree las bases de datos SQL. La cadena de conexión se usa en la implementación.
    $connectionString = New-Object -TypeName Hashtable
    
    if ($Config.Contains('databases'))
    {
        @($Config.databases) |
            Where-Object {$_.connectionStringName -ne ''} |
            Add-AzureSQLDatabases -DatabaseServerPassword $DatabaseServerPassword -CreateDatabase |
            ForEach-Object { $connectionString.Add($_.Name, $_.ConnectionString) }           
    }
    
    return @{ConnectionString = $connectionString}   
}

function Publish-AzureWebApplicationToWebsite
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [Object]
        $Configuration,

        [Parameter(Mandatory = $false)]
        [AllowNull()]
        [Hashtable]
        $ConnectionString,

        [Parameter(Mandatory = $true)]
        [ValidateScript({Test-Path $_ -PathType Leaf})]
        [String]
        $WebDeployPackage
    )

    if ($ConnectionString -and $ConnectionString.Count -gt 0)
    {
        Publish-AzureWebsiteProject `
            -Name $Config.name `
            -Package $WebDeployPackage `
            -ConnectionString $ConnectionString
    }
    else
    {
        Publish-AzureWebsiteProject `
            -Name $Config.name `
            -Package $WebDeployPackage
    }
}


# Rutina principal de script
Set-StrictMode -Version 3

Remove-Module AzureWebSitePublishModule -ErrorAction SilentlyContinue
$scriptDirectory = Split-Path -Parent $PSCmdlet.MyInvocation.MyCommand.Definition
Import-Module ($scriptDirectory + '\AzureWebSitePublishModule.psm1') -Scope Local -Verbose:$false

New-Variable -Name VMWebDeployWaitTime -Value 30 -Option Constant -Scope Script 
New-Variable -Name AzureWebAppPublishOutput -Value @() -Scope Global -Force
New-Variable -Name SendHostMessagesToOutput -Value $SendHostMessagesToOutput -Scope Global -Force

try
{
    $originalErrorActionPreference = $Global:ErrorActionPreference
    $originalVerbosePreference = $Global:VerbosePreference
    
    if ($PSBoundParameters['Verbose'])
    {
        $Global:VerbosePreference = 'Continue'
    }
    
    $scriptName = $MyInvocation.MyCommand.Name + ':'
    
    Write-VerboseWithTime ($scriptName + ' Inicio')
    
    $Global:ErrorActionPreference = 'Stop'
    Write-VerboseWithTime ('{0} $ErrorActionPreference está establecido en {1}' -f $scriptName, $ErrorActionPreference)
    
    Write-Debug ('{0}: $PSCmdlet.ParameterSetName = {1}' -f $scriptName, $PSCmdlet.ParameterSetName)

    # Guarde la suscripción actual. Más tarde, se restaurará el valor Actual de la suscripción en el script.
    Backup-Subscription -UserSpecifiedSubscription $SubscriptionName
    
    # Compruebe que tiene la versión 0.7.4 u otra posterior del módulo de Azure.
    if (-not (Test-AzureModule))
    {
         throw 'Tiene una versión obsoleta de Microsoft Azure PowerShell. Para instalar la última versión, visite http://go.microsoft.com/fwlink/?LinkID=320552.'
    }
    
    if ($SubscriptionName)
    {

        # Si proporcionó un nombre de suscripción, asegúrese de que la suscripción existe en la cuenta.
        if (!(Get-AzureSubscription -SubscriptionName $SubscriptionName))
        {
            throw ("{0}: No se encuentra el nombre de suscripción $SubscriptionName" -f $scriptName)

        }

        # Establezca la suscripción especificada como la actual
        Select-AzureSubscription -SubscriptionName $SubscriptionName | Out-Null

        Write-VerboseWithTime ('{0}: la suscripción está establecida en {1}' -f $scriptName, $SubscriptionName)
    }

    $Config = Read-ConfigFile $Configuration 

    #Compile y empaquete la aplicación web.
    New-WebDeployPackage

    #Ejecutar pruebas unitarias en su aplicación web
    Test-WebApplication

    #Cree el entorno de Azure que se describe en el archivo de configuración JSON.
    $newEnvironmentResult = New-AzureWebApplicationWebsiteEnvironment -Configuration $Config -DatabaseServerPassword $DatabaseServerPassword

    #Implemente el paquete de aplicación web si el usuario especifica $WebDeployPackage. 
    if($WebDeployPackage)
    {
        Publish-AzureWebApplicationToWebsite `
            -Configuration $Config `
            -ConnectionString $newEnvironmentResult.ConnectionString `
            -WebDeployPackage $WebDeployPackage
    }
}
finally
{
    $Global:ErrorActionPreference = $originalErrorActionPreference
    $Global:VerbosePreference = $originalVerbosePreference

    # Restaure la suscripción actual original en el estado Actual.
    Restore-Subscription

    Write-Output $Global:AzureWebAppPublishOutput    
    $Global:AzureWebAppPublishOutput = @()
}

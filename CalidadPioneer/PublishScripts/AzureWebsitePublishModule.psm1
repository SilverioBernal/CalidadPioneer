#  AzureWebSitePublishModule.psm1 es un módulo de script de Windows PowerShell. Este módulo exporta funciones de Windows PowerShell que automatizan la administración del ciclo de vida para aplicaciones web. Puede usar las funciones como están o personalizarlas para su aplicación y entorno de publicación.

Set-StrictMode -Version 3

# Variable en la que se va a guardar la suscripción original.
$Script:originalCurrentSubscription = $null

# Variable en la que se va a guardar la cuenta de almacenamiento original.
$Script:originalCurrentStorageAccount = $null

# Variable en la que se va a guardar la cuenta de almacenamiento de la suscripción especificada por el usuario.
$Script:originalStorageAccountOfUserSpecifiedSubscription = $null

# Variable en la que se va a guardar el nombre de la suscripción.
$Script:userSpecifiedSubscription = $null


<#
.SYNOPSIS
Antepone la hora y la fecha a un mensaje.

.DESCRIPTION
Antepone la hora y la fecha a un mensaje. Esta función está diseñada para mensajes escritos en los flujos Error y Verbose.

.PARAMETER  Message
Especifica los mensajes sin la fecha.

.INPUTS
System.String

.OUTPUTS
System.String

.EXAMPLE
PS C:\> Format-DevTestMessageWithTime -Message "Agregando el archivo $filename al directorio"
2/5/2014 1:03:08 PM - Agregando el archivo $filename al directorio

.LINK
Write-VerboseWithTime

.LINK
Write-ErrorWithTime
#>
function Format-DevTestMessageWithTime
{
    [CmdletBinding()]
    param
    (
        [Parameter(Position=0, Mandatory = $true, ValueFromPipeline = $true)]
        [String]
        $Message
    )

    return ((Get-Date -Format G)  + ' - ' + $Message)
}


<#

.SYNOPSIS
Escribe un mensaje de error utilizando como prefijo la hora actual.

.DESCRIPTION
Escribe un mensaje de error utilizando como prefijo la hora actual. Esta función llama a la función Format-DevTestMessageWithTime para anteponer la hora antes de escribir el mensaje en el flujo Error.

.PARAMETER  Message
Especifica el mensaje de la llamada a un mensaje de error. Puede canalizar la cadena de mensaje a la función.

.INPUTS
System.String

.OUTPUTS
Ninguno. La función escribe en el flujo Error.

.EXAMPLE
PS C:> Write-ErrorWithTime -Message "Failed. Cannot find the file."

Write-Error: 2/6/2014 8:37:29 AM - Failed. Cannot find the file.
 + CategoryInfo     : NotSpecified: (:) [Write-Error], WriteErrorException
 + FullyQualifiedErrorId : Microsoft.PowerShell.Commands.WriteErrorException

.LINK
Write-Error

#>
function Write-ErrorWithTime
{
    [CmdletBinding()]
    param
    (
        [Parameter(Position=0, Mandatory = $true, ValueFromPipeline = $true)]
        [String]
        $Message
    )

    $Message | Format-DevTestMessageWithTime | Write-Error
}


<#
.SYNOPSIS
Escribe un mensaje detallado utilizando como prefijo la hora actual.

.DESCRIPTION
Escribe un mensaje detallado utilizando como prefijo la hora actual. Como llama a Write-Verbose, el mensaje solo se muestra cuando el script se ejecuta con el parámetro Verbose o cuando la preferencia VerbosePreference está establecida en Continue.

.PARAMETER  Message
Especifica el mensaje de la llamada a un mensaje detallado. Puede canalizar la cadena de mensaje a la función.

.INPUTS
System.String

.OUTPUTS
Ninguno. La función escribe en el flujo Verbose.

.EXAMPLE
PS C:> Write-VerboseWithTime -Message "The operation succeeded."
PS C:>
PS C:\> Write-VerboseWithTime -Message "The operation succeeded." -Verbose
VERBOSE: 1/27/2014 11:02:37 AM - The operation succeeded.

.EXAMPLE
PS C:\ps-test> "The operation succeeded." | Write-VerboseWithTime -Verbose
VERBOSE: 1/27/2014 11:01:38 AM - The operation succeeded.

.LINK
Write-Verbose
#>
function Write-VerboseWithTime
{
    [CmdletBinding()]
    param
    (
        [Parameter(Position=0, Mandatory = $true, ValueFromPipeline = $true)]
        [String]
        $Message
    )

    $Message | Format-DevTestMessageWithTime | Write-Verbose
}


<#
.SYNOPSIS
Escribe un mensaje de host utilizando como prefijo la hora actual.

.DESCRIPTION
Esta función escribe un mensaje en el programa host (Write-Host) utilizando como prefijo la hora actual. Los efectos de escribir en el programa host no son siempre iguales. La mayoría de los programas que hospedan Windows PowerShell escriben estos mensajes como salida estándar.

.PARAMETER  Message
Especifica el mensaje base sin fecha. Puede canalizar la cadena de mensaje a la función.

.INPUTS
System.String

.OUTPUTS
Ninguno. La función escribe el mensaje en el programa host.

.EXAMPLE
PS C:> Write-HostWithTime -Message "La operación se realizó correctamente."
1/27/2014 11:02:37 AM - La operación se realizó correctamente.

.LINK
Write-Host
#>
function Write-HostWithTime
{
    [CmdletBinding()]
    param
    (
        [Parameter(Position=0, Mandatory = $true, ValueFromPipeline = $true)]
        [String]
        $Message
    )
    
    if ((Get-Variable SendHostMessagesToOutput -Scope Global -ErrorAction SilentlyContinue) -and $Global:SendHostMessagesToOutput)
    {
        if (!(Get-Variable -Scope Global AzureWebAppPublishOutput -ErrorAction SilentlyContinue) -or !$Global:AzureWebAppPublishOutput)
        {
            New-Variable -Name AzureWebAppPublishOutput -Value @() -Scope Global -Force
        }

        $Global:AzureWebAppPublishOutput += $Message | Format-DevTestMessageWithTime
    }
    else 
    {
        $Message | Format-DevTestMessageWithTime | Write-Host
    }
}


<#
.SYNOPSIS
Devuelve $true si una propiedad o método es miembro del objeto. De lo contrario, $false.

.DESCRIPTION
Devuelve $true si la propiedad o método es miembro del objeto. Esta función devuelve $false para los métodos estáticos de la clase y para las vistas, como PSBase y PSObject.

.PARAMETER  Object
Especifica el objeto de la prueba. Escriba una variable que contenga un objeto o una expresión que devuelva un objeto. No puede especificar tipos, como [DateTime], ni canalizar objetos a esta función.

.PARAMETER  Member
Especifica el nombre de la propiedad o método de la prueba. Si especifica un método, deben omitirse los paréntesis que siguen al nombre del método.

.INPUTS
Ninguno. Esta función no toma datos de entrada de la canalización.

.OUTPUTS
System.Boolean

.EXAMPLE
PS C:\> Test-Member -Object (Get-Date) -Member DayOfWeek
True

.EXAMPLE
PS C:\> $date = Get-Date
PS C:\> Test-Member -Object $date -Member AddDays
True

.EXAMPLE
PS C:\> [DateTime]::IsLeapYear((Get-Date).Year)
True
PS C:\> Test-Member -Object (Get-Date) -Member IsLeapYear
False

.LINK
Get-Member
#>
function Test-Member
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [Object]
        $Object,

        [Parameter(Mandatory = $true)]
        [String]
        $Member
    )

    return $null -ne ($Object | Get-Member -Name $Member)
}


<#
.SYNOPSIS
Devuelve $true si la versión del módulo de Azure es 0.7.4 o posterior. De lo contrario, $false.

.DESCRIPTION
Test-AzureModuleVersion devuelve $true si la versión del módulo de Azure es 0.7.4 o posterior. Devuelve $false si el módulo no está instalado o la versión es anterior. Esta función no tiene parámetros.

.INPUTS
Ninguno

.OUTPUTS
System.Boolean

.EXAMPLE
PS C:\> Get-Module Azure -ListAvailable
PS C:\> #No module
PS C:\> Test-AzureModuleVersion
False

.EXAMPLE
PS C:\> (Get-Module Azure -ListAvailable).Version

Major  Minor  Build  Revision
-----  -----  -----  --------
0      7      4      -1

PS C:\> Test-AzureModuleVersion
True

.LINK
Get-Module

.LINK
PSModuleInfo object (http://msdn.microsoft.com/en-us/library/system.management.automation.psmoduleinfo(v=vs.85).aspx)
#>
function Test-AzureModuleVersion
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [ValidateNotNull()]
        [System.Version]
        $Version
    )

    return ($Version.Major -gt 0) -or ($Version.Minor -gt 7) -or ($Version.Minor -eq 7 -and $Version.Build -ge 4)
}


<#
.SYNOPSIS
Devuelve $true si la versión del módulo de Azure instalada es 0.7.4 o posterior.

.DESCRIPTION
Test-AzureModule devuelve $true si la versión instalada del módulo de Azure es 0.7.4 o posterior. Devuelve $false si el módulo no está instalado o la versión es anterior. Esta función no tiene parámetros.

.INPUTS
Ninguno

.OUTPUTS
System.Boolean

.EXAMPLE
PS C:\> Get-Module Azure -ListAvailable
PS C:\> #No module
PS C:\> Test-AzureModule
False

.EXAMPLE
PS C:\> (Get-Module Azure -ListAvailable).Version

Major  Minor  Build  Revision
-----  -----  -----  --------
    0      7      4      -1

PS C:\> Test-AzureModule
True

.LINK
Get-Module

.LINK
PSModuleInfo object (http://msdn.microsoft.com/en-us/library/system.management.automation.psmoduleinfo(v=vs.85).aspx)
#>
function Test-AzureModule
{
    [CmdletBinding()]

    $module = Get-Module -Name Azure

    if (!$module)
    {
        $module = Get-Module -Name Azure -ListAvailable

        if (!$module -or !(Test-AzureModuleVersion $module.Version))
        {
            return $false;
        }
        else
        {
            $ErrorActionPreference = 'Continue'
            Import-Module -Name Azure -Global -Verbose:$false
            $ErrorActionPreference = 'Stop'

            return $true
        }
    }
    else
    {
        return (Test-AzureModuleVersion $module.Version)
    }
}


<#
.SYNOPSIS
Guarda la suscripción de Microsoft Azure actual en la variable $Script:originalSubscription del ámbito de script.

.DESCRIPTION
La función Backup-Subscription guarda la suscripción de Microsoft Azure actual (Get-AzureSubscription -Current) y su cuenta de almacenamiento, así como la suscripción modificada por este script ($UserSpecifiedSubscription) y su cuenta de almacenamiento, en el ámbito de script. Al guardar los valores, puede usar una función, como Restore-Subscription, para restaurar el estado actual de la suscripción y la cuenta de almacenamiento originales si el estado ha cambiado.

.PARAMETER UserSpecifiedSubscription
Especifica el nombre de la suscripción en la que se crearán y publicarán los nuevos recursos. La función guarda los nombres de la suscripción y sus cuentas de almacenamiento en el ámbito de script. Este parámetro es obligatorio.

.INPUTS
Ninguno

.OUTPUTS
Ninguno

.EXAMPLE
PS C:\> Backup-Subscription -UserSpecifiedSubscription Contoso
PS C:\>

.EXAMPLE
PS C:\> Backup-Subscription -UserSpecifiedSubscription Contoso -Verbose
VERBOSE: Backup-Subscription: Start
VERBOSE: Backup-Subscription: Original subscription is Microsoft Azure MSDN - Visual Studio Ultimate
VERBOSE: Backup-Subscription: End
#>
function Backup-Subscription
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]
        $UserSpecifiedSubscription
    )

    Write-VerboseWithTime 'Backup-Subscription: Inicio'

    $Script:originalCurrentSubscription = Get-AzureSubscription -Current -ErrorAction SilentlyContinue
    if ($Script:originalCurrentSubscription)
    {
        Write-VerboseWithTime ('Backup-Subscription: La suscripción original es ' + $Script:originalCurrentSubscription.SubscriptionName)
        $Script:originalCurrentStorageAccount = $Script:originalCurrentSubscription.CurrentStorageAccountName
    }
    
    $Script:userSpecifiedSubscription = $UserSpecifiedSubscription
    if ($Script:userSpecifiedSubscription)
    {        
        $userSubscription = Get-AzureSubscription -SubscriptionName $Script:userSpecifiedSubscription -ErrorAction SilentlyContinue
        if ($userSubscription)
        {
            $Script:originalStorageAccountOfUserSpecifiedSubscription = $userSubscription.CurrentStorageAccountName
        }        
    }

    Write-VerboseWithTime 'Backup-Subscription: Fin'
}


<#
.SYNOPSIS
Restaura el estado "actual" en la suscripción de Microsoft Azure que se guardó en la variable $Script:originalSubscription del ámbito de script.

.DESCRIPTION
La función Restore-Subscription convierte la suscripción guardada en la variable $Script:originalSubscription en la suscripción actual (de nuevo). Si la suscripción original tiene una cuenta de almacenamiento, esta función la convierte en la cuenta de almacenamiento actual de la suscripción actual. La función restaura la suscripción solo si hay una variable $SubscriptionName que no es null en el entorno. De lo contrario, termina. Si $SubscriptionName contiene un valor pero $Script:originalSubscription es $null, Restore-Subscription usa el cmdlet Select-AzureSubscription para borrar las opciones Actual y Predeterminada de las suscripciones de Microsoft Azure PowerShell. Esta función no tiene parámetros; toma los datos de entrada y no devuelve nada (void). Puede usar -Verbose para escribir mensajes en el flujo detallado Verbose.

.INPUTS
Ninguno

.OUTPUTS
Ninguno

.EXAMPLE
PS C:\> Restore-Subscription
PS C:\>

.EXAMPLE
PS C:\> Restore-Subscription -Verbose
VERBOSE: Restore-Subscription: Start
VERBOSE: Restore-Subscription: End
#>
function Restore-Subscription
{
    [CmdletBinding()]
    param()

    Write-VerboseWithTime 'Restore-Subscription: Inicio'

    if ($Script:originalCurrentSubscription)
    {
        if ($Script:originalCurrentStorageAccount)
        {
            Set-AzureSubscription `
                -SubscriptionName $Script:originalCurrentSubscription.SubscriptionName `
                -CurrentStorageAccountName $Script:originalCurrentStorageAccount
        }

        Select-AzureSubscription -SubscriptionName $Script:originalCurrentSubscription.SubscriptionName
    }
    else 
    {
        Select-AzureSubscription -NoCurrent
        Select-AzureSubscription -NoDefault
    }
    
    if ($Script:userSpecifiedSubscription -and $Script:originalStorageAccountOfUserSpecifiedSubscription)
    {
        Set-AzureSubscription `
            -SubscriptionName $Script:userSpecifiedSubscription `
            -CurrentStorageAccountName $Script:originalStorageAccountOfUserSpecifiedSubscription
    }

    Write-VerboseWithTime 'Restore-Subscription: Fin'
}


<#
.SYNOPSIS
Valida el archivo de configuración y devuelve una tabla hash con los valores del archivo de configuración.

.DESCRIPTION
La función Read-ConfigFile valida el archivo de configuración JSON y devuelve una tabla hash de valores seleccionados.
-- Comienza convirtiendo el archivo JSON a PSCustomObject. La tabla hash del sitio web tiene las claves siguientes:
-- Location: Ubicación de sitio web
-- Databases: Bases de datos SQL de sitio web

.PARAMETER  ConfigurationFile
Especifica la ruta de acceso y el nombre del archivo de configuración JSON del proyecto web. Visual Studio genera automáticamente el archivo de configuración JSON cuando se crea un proyecto web y se guarda en la carpeta PublishScripts de la solución.

.PARAMETER HasWebDeployPackage
Indica que hay un archivo ZIP de paquete de Web Deploy para la aplicación web. Para especificar un valor de $true, use -HasWebDeployPackage o HasWebDeployPackage:$true. Para especificar un valor de false, use HasWebDeployPackage:$false. Este parámetro es obligatorio.

.INPUTS
Ninguno. No puede canalizar datos de entrada a esta función.

.OUTPUTS
System.Collections.Hashtable

.EXAMPLE
PS C:\> Read-ConfigFile -ConfigurationFile <path> -HasWebDeployPackage


Name                           Value                                                                                                                                                                     
----                           -----                                                                                                                                                                     
databases                      {@{connectionStringName=; databaseName=; serverName=; user=; password=}}                                                                                                  
website                        @{name="mysite"; location="West US";}                                                      
#>
function Read-ConfigFile
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [ValidateScript({Test-Path $_ -PathType Leaf})]
        [String]
        $ConfigurationFile
    )

    Write-VerboseWithTime 'Read-ConfigFile: Inicio'

    # Obtenga el contenido del archivo JSON (-raw omite los saltos de línea) y conviértalo en un objeto PSCustomObject.
    $config = Get-Content $ConfigurationFile -Raw | ConvertFrom-Json

    if (!$config)
    {
        throw ('Read-ConfigFile: Error en ConvertFrom-Json: ' + $error[0])
    }

    # Determinar si el objeto environmentSettings tiene propiedades 'webSite' (independientemente del valor de las mismas)
    $hasWebsiteProperty =  Test-Member -Object $config.environmentSettings -Member 'webSite'

    if (!$hasWebsiteProperty)
    {
        throw 'Read-ConfigFile: El archivo de configuración no tiene una propiedad webSite.'
    }

    # Cree una tabla hash a partir de los valores de PSCustomObject.
    $returnObject = New-Object -TypeName Hashtable

    $returnObject.Add('name', $config.environmentSettings.webSite.name)
    $returnObject.Add('location', $config.environmentSettings.webSite.location)

    if (Test-Member -Object $config.environmentSettings -Member 'databases')
    {
        $returnObject.Add('databases', $config.environmentSettings.databases)
    }

    Write-VerboseWithTime 'Read-ConfigFile: Fin'

    return $returnObject
}


<#
.SYNOPSIS
Crea un sitio web de Microsoft Azure.

.DESCRIPTION
Crea un sitio web de Microsoft Azure con el nombre y la ubicación específicos. Esta función llama al cmdlet New-AzureWebsite en el módulo de Azure. Si la suscripción todavía no tiene un sitio web con el nombre especificado, esta función lo crea y devuelve un objeto de sitio web. De lo contrario, devuelve el sitio web existente.

.PARAMETER  Name
Especifica un nombre para el nuevo sitio web. El nombre debe ser único en Microsoft Azure. Este parámetro es obligatorio.

.PARAMETER  Location
Especifica la ubicación del sitio web. Los valores válidos son las ubicaciones de Microsoft Azure, por ejemplo, "Oeste de EE. UU.". Este parámetro es obligatorio.

.INPUTS
NINGUNO.

.OUTPUTS
Microsoft.WindowsAzure.Commands.Utilities.Websites.Services.WebEntities.Site

.EXAMPLE
Add-AzureWebsite -Name TestSite -Location "West US"

Name       : contoso
State      : Running
Host Names : contoso.azurewebsites.net

.LINK
New-AzureWebsite
#>
function Add-AzureWebsite
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [String]
        $Name,

        [Parameter(Mandatory = $true)]
        [String]
        $Location
    )

    Write-VerboseWithTime 'Add-AzureWebsite: Inicio'
    $website = Get-AzureWebsite -Name $Name -ErrorAction SilentlyContinue

    if ($website)
    {
        Write-HostWithTime ('Add-AzureWebsite: Sitio web existente ' +
        $website.Name + ' existente')
    }
    else
    {
        if (Test-AzureName -Website -Name $Name)
        {
            Write-ErrorWithTime ('El sitio web {0} ya existe.' -f $Name)
        }
        else
        {
            $website = New-AzureWebsite -Name $Name -Location $Location
        }
    }

    $website | Out-String | Write-VerboseWithTime
    Write-VerboseWithTime 'Add-AzureWebsite: Fin'

    return $website
}

<#
.SYNOPSIS
Devuelve $True cuando la dirección URL es absoluta y su esquema es https.

.DESCRIPTION
La función Test-HttpsUrl convierte la dirección URL de entrada en un objeto System.Uri. Devuelve $True cuando la dirección URL es absoluta (no relativa) y su esquema es https. Si es false o la cadena de entrada no puede convertirse en una dirección URL, la función devuelve $false.

.PARAMETER Url
Especifica la dirección URL que se va a probar. Escriba una cadena de dirección URL.

.INPUTS
NINGUNO.

.OUTPUTS
System.Boolean

.EXAMPLE
PS C:\>$profile.publishUrl
waws-prod-bay-001.publish.azurewebsites.windows.net:443

PS C:\>Test-HttpsUrl -Url 'waws-prod-bay-001.publish.azurewebsites.windows.net:443'
False
#>
function Test-HttpsUrl
{

    param
    (
        [Parameter(Mandatory = $true)]
        [String]
        $Url
    )

    # Si $uri no se puede convertir en un objeto System.Uri, Test-HttpsUrl devuelve $false.
    $uri = $Url -as [System.Uri]

    return $uri.IsAbsoluteUri -and $uri.Scheme -eq 'https'
}


<#
.SYNOPSIS
Crea una cadena que permite conectarse a una base de datos SQL de Microsoft Azure.

.DESCRIPTION
La función Get-AzureSQLDatabaseConnectionString ensambla una cadena de conexión que se va a conectar a una base de datos SQL de Microsoft Azure.

.PARAMETER  DatabaseServerName
Especifica el nombre de un servidor de bases de datos existente en la suscripción de Microsoft Azure. Todas las bases de datos SQL de Microsoft Azure deben estar asociadas a un servidor de bases de datos SQL. Para obtener el nombre del servidor, use el cmdlet Get-AzureSqlDatabaseServer (módulo de Azure). Este parámetro es obligatorio.

.PARAMETER  DatabaseName
Especifica el nombre de la base de datos SQL. Puede ser una base de datos SQL existente o el nombre de una base de datos SQL nueva. Este parámetro es obligatorio.

.PARAMETER  Username
Especifica el nombre del administrador de bases de datos SQL. El nombre de usuario será $Username@DatabaseServerName. Este parámetro es obligatorio.

.PARAMETER  Password
Especifica una contraseña para el administrador de bases de datos SQL. Escriba la contraseña sin formato. No se permiten cadenas seguras. Este parámetro es obligatorio.

.INPUTS
Ninguno.

.OUTPUTS
System.String

.EXAMPLE
PS C:\> $ServerName = (Get-AzureSqlDatabaseServer).ServerName[0]
PS C:\> Get-AzureSQLDatabaseConnectionString -DatabaseServerName $ServerName `
        -DatabaseName 'testdb' -UserName 'admin'  -Password 'password'

Server=tcp:testserver.database.windows.net,1433;Database=testdb;User ID=admin@testserver;Password=password;Trusted_Connection=False;Encrypt=True;Connection Timeout=20;
#>
function Get-AzureSQLDatabaseConnectionString
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [String]
        $DatabaseServerName,

        [Parameter(Mandatory = $true)]
        [String]
        $DatabaseName,

        [Parameter(Mandatory = $true)]
        [String]
        $UserName,

        [Parameter(Mandatory = $true)]
        [String]
        $Password
    )

    return ('Server=tcp:{0}.database.windows.net,1433;Database={1};' +
           'User ID={2}@{0};' +
           'Password={3};' +
           'Trusted_Connection=False;' +
           'Encrypt=True;' +
           'Connection Timeout=20;') `
           -f $DatabaseServerName, $DatabaseName, $UserName, $Password
}


<#
.SYNOPSIS
Crea bases de datos SQL de Microsoft Azure a partir de los valores del archivo de configuración JSON que Visual Studio genera.

.DESCRIPTION
La función Add-AzureSQLDatabases toma la información de la sección de bases de datos del archivo JSON. Esta función, Add-AzureSQLDatabases (plural), llama a la función Add-AzureSQLDatabase (singular) de cada base de datos SQL del archivo JSON. Add-AzureSQLDatabase (singular) llama al cmdlet New-AzureSqlDatabase (módulo de Azure), que crea las bases de datos SQL. Esta función no devuelve un objeto de base de datos. Devuelve una tabla hash de los valores que se utilizaron para crear las bases de datos.

.PARAMETER DatabaseConfig
 Toma una matriz de PSCustomObjects que tiene su origen en el archivo JSON que la función Read-ConfigFile devuelve cuando el archivo JSON tiene una propiedad de sitio web. Incluye las propiedades de environmentSettings.databases. Puede canalizar la lista a esta función.
PS C:\> $config = Read-ConfigFile <name>.json
PS C:\> $DatabaseConfig = $config.databases| where {$_.connectionStringName}
PS C:\> $DatabaseConfig
connectionStringName: Default Connection
databasename : TestDB1
edition   :
size     : 1
collation  : SQL_Latin1_General_CP1_CI_AS
servertype  : New SQL Database Server
servername  : r040tvt2gx
user     : dbuser
password   : Test.123
location   : West US

.PARAMETER  DatabaseServerPassword
Especifica la contraseña para el administrador del servidor de bases de datos SQL. Escriba una tabla hash con las claves Name y Password. El valor de Name es el nombre del servidor de bases de datos SQL. El valor de Password es la contraseña de administrador. Por ejemplo: @Name = "TestDB1"; Password = "password" Este parámetro es opcional. Si lo omite o el nombre del servidor de bases de datos SQL no coincide con el valor de la propiedad serverName del objeto $DatabaseConfig, la función usa la propiedad Password del objeto $DatabaseConfig para la base de datos SQL en la cadena de conexión.

.PARAMETER CreateDatabase
Se asegura de que se pretende crear una base de datos. Este parámetro es opcional.

.INPUTS
System.Collections.Hashtable[]

.OUTPUTS
System.Collections.Hashtable

.EXAMPLE
PS C:\> $config = Read-ConfigFile <name>.json
PS C:\> $DatabaseConfig = $config.databases| where {$_.connectionStringName}
PS C:\> $DatabaseConfig | Add-AzureSQLDatabases

Name                           Value
----                           -----
ConnectionString               Server=tcp:testdb1.database.windows.net,1433;Database=testdb;User ID=admin@testdb1;Password=password;Trusted_Connection=False;Encrypt=True;Connection Timeout=20;
Name                           Default Connection
Type                           SQLAzure

.LINK
Get-AzureSQLDatabaseConnectionString

.LINK
Create-AzureSQLDatabase
#>
function Add-AzureSQLDatabases
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [PSCustomObject]
        $DatabaseConfig,

        [Parameter(Mandatory = $false)]
        [AllowNull()]
        [Hashtable[]]
        $DatabaseServerPassword,

        [Parameter(Mandatory = $false)]
        [Switch]
        $CreateDatabase = $false
    )

    begin
    {
        Write-VerboseWithTime 'Add-AzureSQLDatabases: Inicio'
    }
    process
    {
        Write-VerboseWithTime ('Add-AzureSQLDatabases: Creando ' + $DatabaseConfig.databaseName)

        if ($CreateDatabase)
        {
            # Cree una nueva base de datos SQL con los valores de DatabaseConfig (a menos que exista una).
            # Se suprimió la salida del comando.
            Add-AzureSQLDatabase -DatabaseConfig $DatabaseConfig | Out-Null
        }

        $serverPassword = $null
        if ($DatabaseServerPassword)
        {
            foreach ($credential in $DatabaseServerPassword)
            {
               if ($credential.Name -eq $DatabaseConfig.serverName)
               {
                   $serverPassword = $credential.password             
                   break
               }
            }               
        }

        if (!$serverPassword)
        {
            $serverPassword = $DatabaseConfig.password
        }

        return @{
            Name = $DatabaseConfig.connectionStringName;
            Type = 'SQLAzure';
            ConnectionString = Get-AzureSQLDatabaseConnectionString `
                -DatabaseServerName $DatabaseConfig.serverName `
                -DatabaseName $DatabaseConfig.databaseName `
                -UserName $DatabaseConfig.user `
                -Password $serverPassword }
    }
    end
    {
        Write-VerboseWithTime 'Add-AzureSQLDatabases: Fin'
    }
}


<#
.SYNOPSIS
Crea una nueva base de datos SQL de Microsoft Azure.

.DESCRIPTION
La función Add-AzureSQLDatabase crea una base de datos SQL de Microsoft Azure a partir de los datos del archivo de configuración JSON que Visual Studio genera y devuelve la nueva base de datos. Si la suscripción ya tiene una base de datos SQL con el nombre especificado en el servidor de bases de datos SQL indicado, la función devuelve la base de datos existente. Esta función llama al cmdlet New-AzureSqlDatabase (módulo de Azure), que es el que en realidad crea la base de datos SQL.

.PARAMETER DatabaseConfig
Toma un objeto PSCustomObject que tiene su origen en el archivo de configuración JSON que la función Read-ConfigFile devuelve cuando el archivo JSON tiene una propiedad de sitio web. Incluye las propiedades de environmentSettings.databases. No se puede canalizar el objeto a esta función. Visual Studio genera un archivo de configuración JSON para todos los proyectos web y lo guarda en la carpeta PublishScripts de la solución.

.INPUTS
Ninguno. Esta función no toma datos de entrada de la canalización.

.OUTPUTS
Microsoft.WindowsAzure.Commands.SqlDatabase.Services.Server.Database

.EXAMPLE
PS C:\> $config = Read-ConfigFile <name>.json
PS C:\> $DatabaseConfig = $config.databases | where connectionStringName
PS C:\> $DatabaseConfig

connectionStringName    : Default Connection
databasename : TestDB1
edition      :
size         : 1
collation    : SQL_Latin1_General_CP1_CI_AS
servertype   : New SQL Database Server
servername   : r040tvt2gx
user         : dbuser
password     : Test.123
location     : West US

PS C:\> Add-AzureSQLDatabase -DatabaseConfig $DatabaseConfig

.LINK
Add-AzureSQLDatabases

.LINK
New-AzureSQLDatabase
#>
function Add-AzureSQLDatabase
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [ValidateNotNull()]
        [Object]
        $DatabaseConfig
    )

    Write-VerboseWithTime 'Add-AzureSQLDatabase: Inicio'

    # Si el valor de parámetro no tiene la propiedad serverName o si la propiedad serverName no tiene ningún valor, se produce un error.
    if (-not (Test-Member $DatabaseConfig 'serverName') -or -not $DatabaseConfig.serverName)
    {
        throw 'Add-AzureSQLDatabase: Falta el valor serverName del servidor de bases de datos (obligatorio) en el valor de DatabaseConfig.'
    }

    # Si el valor de parámetro no tiene la propiedad databasename o si la propiedad databasename no tiene ningún valor, se produce un error.
    if (-not (Test-Member $DatabaseConfig 'databaseName') -or -not $DatabaseConfig.databaseName)
    {
        throw 'Add-AzureSQLDatabase: Falta el nombre de base de datos (obligatorio) en el valor de DatabaseConfig.'
    }

    $DbServer = $null

    if (Test-HttpsUrl $DatabaseConfig.serverName)
    {
        $absoluteDbServer = $DatabaseConfig.serverName -as [System.Uri]
        $subscription = Get-AzureSubscription -Current -ErrorAction SilentlyContinue

        if ($subscription -and $subscription.ServiceEndpoint -and $subscription.SubscriptionId)
        {
            $absoluteDbServerRegex = 'https:\/\/{0}\/{1}\/services\/sqlservers\/servers\/(.+)\.database\.windows\.net\/databases' -f `
                                     $subscription.serviceEndpoint.Host, $subscription.SubscriptionId

            if ($absoluteDbServer -match $absoluteDbServerRegex -and $Matches.Count -eq 2)
            {
                 $DbServer = $Matches[1]
            }
        }
    }

    if (!$DbServer)
    {
        $DbServer = $DatabaseConfig.serverName
    }

    $db = Get-AzureSqlDatabase -ServerName $DbServer -DatabaseName $DatabaseConfig.databaseName -ErrorAction SilentlyContinue

    if ($db)
    {
        Write-HostWithTime ('Create-AzureSQLDatabase: Se está usando la base de datos existente ' + $db.Name)
        $db | Out-String | Write-VerboseWithTime
    }
    else
    {
        $param = New-Object -TypeName Hashtable
        $param.Add('serverName', $DbServer)
        $param.Add('databaseName', $DatabaseConfig.databaseName)

        if ((Test-Member $DatabaseConfig 'size') -and $DatabaseConfig.size)
        {
            $param.Add('MaxSizeGB', $DatabaseConfig.size)
        }
        else
        {
            $param.Add('MaxSizeGB', 1)
        }

        # Si el objeto $DatabaseConfig tiene una propiedad de intercalación y no es null o está vacía
        if ((Test-Member $DatabaseConfig 'collation') -and $DatabaseConfig.collation)
        {
            $param.Add('Collation', $DatabaseConfig.collation)
        }

        # Si el objeto $DatabaseConfig tiene una propiedad de edición y no es null o está vacía
        if ((Test-Member $DatabaseConfig 'edition') -and $DatabaseConfig.edition)
        {
            $param.Add('Edition', $DatabaseConfig.edition)
        }

        # Escriba la tabla hash en el flujo detallado (Verbose).
        $param | Out-String | Write-VerboseWithTime
        # Llame a New-AzureSqlDatabase utilizando el empaquetamiento (suprima la salida).
        $db = New-AzureSqlDatabase @param
    }

    Write-VerboseWithTime 'Add-AzureSQLDatabase: Fin'
    return $db
}

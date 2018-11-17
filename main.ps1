param( $csFile )

if (-Not($csFile)) {
    Write-Host "Usage : ./main.ps1 CSharp_Source"
    exit
}

add-type -path $csFile -passThru
$sl = New-Object Program

$sl.Main()

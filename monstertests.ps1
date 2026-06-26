# 1. Läs in din kompilerade .dll-fil
Add-Type -Path "D:\GitRepos\PerilousSwamp\PerilousSwamp\MrSwampMonster\bin\Debug\MrSwampMonster.dll"

$monsterStrength = 20

for ($i = 0; $i -lt 40; $i++) {
    $result = [MrSwampMonster.Monster]::ResolveCombat($i, $monsterStrength)
    Write-Output "Attack $($i) results in $($result)."
}
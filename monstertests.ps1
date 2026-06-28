Add-Type -Path "D:\GitRepos\PerilousSwamp\PerilousSwamp\MrSwampMonster\bin\Debug\MrSwampMonster.dll"

$monsterStrength = 80
$monster = [MrSwampMonster.Monster]::new($monsterStrength)

for ($i = 0; $i -lt 200; $i++) {
    $result = $monster.ResolveCombat($i)
    Write-Output "Monster strength: $($monsterStrength) Attack with strength $($i) results in $($result)."
}

for ($i = 0; $i -lt 50; $i++) {
    $monster = [MrSwampMonster.Monster]::new($monsterStrength)
    Write-Output $monster.MonsterName
}
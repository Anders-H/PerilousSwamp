Public Class Monster

    Public Shared ReadOnly Rnd As New Random()
    Public ReadOnly MonsterCombatStrength As Integer
    Public ReadOnly MonsterName As String
    Public ReadOnly Treasure As Treasure
    Public IsAlive As Boolean
    Public IsGone As Boolean
    Public AliveImageIndex As Integer
    Public AttackImageIndex As Integer

    Public Sub New(monsterCombatStrength As Integer)
        Me.MonsterCombatStrength = monsterCombatStrength
        IsAlive = True
        IsGone = False
        Dim monsterNameFirstParts As String() = {"Fiendish", "Green", "Lean", "Hungry", "Nasty", "Tough", "Horrible", "Dirty", "Vile", "Feral", "Grimy", "Sinister", "Savage"}
        Dim monsterNameLastParts As String() = {"Werewolf", "Phoenix", "Bunyip", "Troll"} ', "Goblin", "Ghoul", "Gorgon", "Dragon", "Orge", "Wizard", "Manticore", "Wraith", "Basilisk", "Harpy", "Minotaur"}
        Dim monsterFirstIndex = Rnd.Next(0, monsterNameFirstParts.Length)
        Dim monsterSecondIndex = Rnd.Next(0, monsterNameFirstParts.Length)

        While monsterSecondIndex = monsterFirstIndex
            monsterSecondIndex = Rnd.Next(0, monsterNameLastParts.Length)
        End While

        Dim monsterThirdIndex = Rnd.Next(0, monsterNameLastParts.Length)
        MonsterName = String.Format("{0}, {1} {2}", monsterNameFirstParts(monsterFirstIndex), monsterNameFirstParts(monsterSecondIndex), monsterNameLastParts(monsterThirdIndex))
        AliveImageIndex = monsterThirdIndex * 2
        AttackImageIndex = AliveImageIndex + 1
        Treasure = New Treasure()
    End Sub

    Function ResolveCombat(playerCombatPoints As Integer) As Integer
        Dim baseDifference As Integer = playerCombatPoints - MonsterCombatStrength
        Dim randomRange = CType(Math.Round(Math.Abs(baseDifference) * 0.9), Integer)

        If randomRange < 2 Then
            randomRange = 2
        ElseIf randomRange > 30 Then
            randomRange = 30
        End If

        Dim randomFactor As Integer = Rnd.Next(-randomRange, randomRange + 1)
        Dim result As Integer = baseDifference + randomFactor

        If result = 0 Then
            If Rnd.Next(0, 2) = 0 Then
                result = 1
            Else
                result = -1
            End If
        End If

        If result > 30 Then
            result /= 3
        ElseIf result > 15 Then
            result /= 2
        End If

        Return result
    End Function

End Class
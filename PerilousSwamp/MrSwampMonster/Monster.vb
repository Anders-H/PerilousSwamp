Public Class Monster

    Public Shared ReadOnly Rnd As New Random()
    Public ReadOnly MonsterCombatStrength As Integer
    Public ReadOnly MonsterName As String
    Public IsAlive As Boolean
    Public IsGone As Boolean
    Public AliveImageIndex As Integer
    Public AttackImageIndex As Integer

    Public Sub New(monsterCombatStrength As Integer)
        Me.MonsterCombatStrength = monsterCombatStrength
        IsAlive = True
        IsGone = False
        Dim monsterNameFirstParts As String() = {"Fiendish", "Green", "Lean", "Hungry", "Nasty", "Tough", "Horrible", "Dirty", "Vile", "Feral", "Grimy", "Sinister", "Savage"}
        Dim monsterNameLastParts As String() = {"Werewolf", "Phoenix"} ', "Troll", "Goblin", "Ghoul", "Gorgon", "Dragon", "Orge", "Wizard", "Manticore", "Wraith", "Basilisk", "Harpy", "Minotaur"}
        Dim monsterFirstIndex = Rnd.Next(0, monsterNameFirstParts.Length)
        Dim monsterSecondIndex = Rnd.Next(0, monsterNameFirstParts.Length)

        While monsterSecondIndex = monsterFirstIndex
            monsterSecondIndex = Rnd.Next(0, monsterNameLastParts.Length)
        End While

        Dim monsterThirdIndex = Rnd.Next(0, monsterNameLastParts.Length)
        MonsterName = String.Format("{0}, {1} {2}", monsterNameFirstParts(monsterFirstIndex), monsterNameFirstParts(monsterSecondIndex), monsterNameLastParts(monsterThirdIndex))
        AliveImageIndex = monsterThirdIndex * 2
        AttackImageIndex = AliveImageIndex + 1
    End Sub

    Function ResolveCombat(playerStrength As Integer) As Integer
        Dim baseDifference As Integer = playerStrength - MonsterCombatStrength
        Dim randomFactor As Integer = Rnd.Next(-50, 51)
        Dim result As Integer = baseDifference + randomFactor

        If result = 0 Then
            If Rnd.Next(0, 2) = 0 Then
                result = 1
            Else
                result = -1
            End If
        End If

        Return result
    End Function

End Class
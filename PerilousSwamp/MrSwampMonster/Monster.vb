Public Class Monster

    Private Shared ReadOnly Rnd As New Random()
    Public ReadOnly MonsterCombatStrength As Integer
    Public ReadOnly MonsterName As String

    Public Sub New(monasterCombatStrength As Integer)
        MonsterCombatStrength = monasterCombatStrength
        Dim monsterFirstIndex = Rnd.Next(0, 8)
        Dim monsterSecondIndex = Rnd.Next(0, 8)

        While monsterSecondIndex = monsterFirstIndex
            monsterSecondIndex = Rnd.Next(0, 8)
        End While

        Dim monsterNameFirstPart = ""
        Dim monsterNameSecondPart = ""

        Select Case monsterFirstIndex
            Case 0
                monsterNameFirstPart = "Fiendish, "
            Case 1
                monsterNameFirstPart = "Green, "
            Case 2
                monsterNameFirstPart = "Lean, "
            Case 3
                monsterNameFirstPart = "Hungry, "
            Case 4
                monsterNameFirstPart = "Nasty, "
            Case 5
                monsterNameFirstPart = "Tough, "
            Case 6
                monsterNameFirstPart = "Horrible, "
            Case 7
                monsterNameFirstPart = "Dirty, "
        End Select

        Select Case monsterSecondIndex
            Case 0
                monsterNameSecondPart = "Fiendish "
            Case 1
                monsterNameSecondPart = "Green "
            Case 2
                monsterNameSecondPart = "Lean "
            Case 3
                monsterNameSecondPart = "Hungry "
            Case 4
                monsterNameSecondPart = "Nasty "
            Case 5
                monsterNameSecondPart = "Tough "
            Case 6
                monsterNameSecondPart = "Horrible "
            Case 7
                monsterNameSecondPart = "Dirty "
        End Select


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
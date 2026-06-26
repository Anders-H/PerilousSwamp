Public Class Monster

    Private Shared ReadOnly Rnd As New Random()
    Public CombatStrength As Integer

    Public Sub New(combatStrength As Integer)
        combatStrength = combatStrength
    End Sub

    Public Shared Function ResolveCombat(playerSacrifice As Integer, monsterStrength As Integer) As Integer

        If monsterStrength <= 0 Then
            monsterStrength = 1
        End If

        If playerSacrifice < 0 Then
            playerSacrifice = 0
        End If

        Dim ratio As Double = CDbl(playerSacrifice) / monsterStrength
        Dim winChance = CInt(ratio * 50)
        winChance = Clamp(winChance, 5, 95)
        Dim roll As Integer = Rnd.[Next](0, 100)
        Dim isWin As Boolean = roll < winChance
        Dim baseDifference As Integer = playerSacrifice - monsterStrength
        Dim variance As Integer = Rnd.[Next](-3, 4)

        If isWin Then
            Dim winValue As Integer = baseDifference + variance
            Return Math.Max(1, winValue)
        Else
            Dim lossValue As Integer = baseDifference + variance
            Return Math.Min(-1, lossValue)
        End If

    End Function

    Private Shared Function Clamp(value As Integer, min As Integer, max As Integer) As Integer

        If value < min Then
            Return min
        End If

        If value > max Then
            Return max
        End If

        Return value

    End Function

End Class
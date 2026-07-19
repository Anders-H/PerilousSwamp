Public Class MonsterMap

    Private ReadOnly _monsters As Dictionary(Of String, Monster)

    Public Sub New()
        _monsters = New Dictionary(Of String, Monster)
    End Sub

    Public Function GetMonsterFromMapPosition(x As Integer, y As Integer) As Monster
        Dim locationString = String.Format("{0}x{1}", x, y)
        Dim monster As Monster = Nothing

        If _monsters.TryGetValue(locationString, monster) Then
            Return monster
        End If

        Dim strength As Double = Monster.Rnd.Next(1, 13)
        strength *= 3.5
        strength += Monster.Rnd.Next(1, 27)
        strength = Math.Round(strength)
        monster = New Monster(CType(strength, Integer))
        _monsters.Add(locationString, monster)
        Return monster
    End Function

End Class
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

        Dim strength = Monster.Rnd.Next(0, 25)
        strength *= 3 + 20
        monster = New Monster(strength)
        _monsters.Add(locationString, monster)
        Return monster
    End Function

End Class
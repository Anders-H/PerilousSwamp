Public Class Treasure
    Public ReadOnly TreasureName As String

    Public Sub New()
        Dim treasureNames As String() = {"10 Silver Spoons", "Excalibur Sword", "A jar of Rubies", "A Treasure Chest", "50 Silver Pieces", "100 Gold Pieces", "A Box of Jewels", "A Solid Silver Chalice", "An Ivory Statue", "A Jewelled Sword"}
        TreasureName = treasureNames(Monster.Rnd.Next(0, treasureNames.Length))
    End Sub
End Class
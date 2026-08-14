Attribute VB_Name = "UndoBridge"
Option Explicit

Public Sub UndoLastTranslation()
    On Error GoTo Handler

    Dim addIn As Object
    Set addIn = Application.COMAddIns("TarjimonOfficeUZ.Excel")

    If addIn Is Nothing Then
        Err.Raise vbObjectError + 2201, "TarjimonOfficeUZ", "TarjimonOfficeUZ.Excel COM add-in topilmadi."
    End If

    If addIn.Object Is Nothing Then
        Err.Raise vbObjectError + 2202, "TarjimonOfficeUZ", "TarjimonOfficeUZ.Excel Undo xizmati mavjud emas."
    End If

    addIn.Object.UndoLastTranslation
    Exit Sub

Handler:
    MsgBox "Tarjimon Office UZ: tarjimani bekor qilib bo'lmadi." & vbCrLf & vbCrLf & _
           "Sabab: " & Err.Description, vbExclamation, "Tarjimon Office UZ"
End Sub

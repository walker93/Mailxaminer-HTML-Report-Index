Imports System.IO
Module extensions

    Public Sub AddDirectory(path As String, ByRef nodoP As TreeNode)
        Try
            Dim directories = Directory.GetDirectories(path)
            For Each dire In directories
                Dim info As New DirectoryInfo(dire)
                If info.GetFiles("*.html").Length > 0 Or info.GetDirectories.Length > 0 Then
                    Dim nodoC As New TreeNode(info.Name)
                    AddDirectory(dire, nodoC)
                    nodoP.Nodes.Add(nodoC)
                End If
            Next
            Application.DoEvents()
        Catch ex As Exception

        End Try
    End Sub
    Public Function getcommonpath(path1 As String, path2 As String) As String
        Dim commonpath As String = ""
        Dim folders1() = path1.Split("\"c)
        Dim folders2() = path2.Split("\"c)
        If folders1.Length >= folders2.Length Then
            For i = 0 To folders2.Length - 1
                If folders2(i) = folders1(i) Then commonpath &= folders2(i) & "\"
            Next
        Else
            For i = 0 To folders1.Length - 1
                If folders2(i) = folders1(i) Then commonpath &= folders1(i) & "\"
            Next
        End If
        Return commonpath
    End Function

    Public emailEdit As String = "
<style>
    @media print{
        input {
            display: none;
        }
    }
</style>
<input type = 'button' title='Stampa E-mail' onclick='window.print()' value='🖨️ Stampa'>
<input type = 'button' title='Apri E-mail' onclick=""window.open(location.href, '_blank')"" value='🔎 Apri'>
"



End Module

Imports System.IO
Module extensions

    Dim supported_extensions() As String = {".html"}
    Public Function isSupportedFile(file As FileInfo) As Boolean
        Return supported_extensions.Contains(file.Extension.ToLower)
    End Function

    Public Sub AddFiles(path As String, ByRef nodoP As TreeNode)
        Dim file = New FileInfo(path)
        If isSupportedFile(file) Then
            'nodoP.Nodes.Add(file.Name)
            Application.DoEvents()
        Else
            'Form1.PrintLog("Not supported:  " + file.FullName)
        End If
    End Sub

    Public Sub AddFiles(paths As String(), ByRef nodoP As TreeNode)
        For Each p In paths
            Try
                AddFiles(p, nodoP)
            Catch ex As Exception
                'Form1.PrintLog("ERROR: " & ex.Message)
            End Try
        Next
    End Sub
    Public Sub AddDirectory(path As String, ByRef nodoP As TreeNode)
        Try
            Dim directories = Directory.GetDirectories(path)
            Dim files = Directory.GetFiles(path)
            AddFiles(files, nodoP)

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
            'Form1.PrintLog("ERROR: " & ex.Message)
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
End Module

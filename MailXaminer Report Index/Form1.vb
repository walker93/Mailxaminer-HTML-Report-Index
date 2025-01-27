﻿Public Class Form1
    Dim destDir As IO.DirectoryInfo
    Dim provider As New Globalization.CultureInfo("it-IT")
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        TreeView1.Nodes.Clear()
        Button2.Enabled = False
        If FBD1.ShowDialog() = DialogResult.OK Then
            Dim selected_path = New IO.DirectoryInfo(FBD1.SelectedPath)
            Dim Root = New TreeNode(selected_path.Name)
            AddDirectory(selected_path.FullName, Root)
            TreeView1.Nodes.Add(Root)
            Button2.Enabled = True
            Label1.Text = selected_path.FullName
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim data = getStringChilds(TreeView1.Nodes)
        destDir = New IO.DirectoryInfo(FBD1.SelectedPath).Parent
        'If IO.Directory.Exists(destDir.FullName & "\resources") Then IO.Directory.Delete(destDir.FullName & "\resources", True)
        My.Computer.FileSystem.CopyDirectory("resources", destDir.FullName & "\resources", True)
        My.Computer.FileSystem.MoveFile(destDir.FullName & "\resources\index.html", destDir.FullName & "\index.html", True)
        Dim folderTreeHTML = IO.File.ReadAllText(destDir.FullName & "\resources\foldertree.html")
        IO.File.WriteAllText(destDir.FullName & "\resources\foldertree.html", folderTreeHTML.Replace("FOLDERTREE_PLACEHOLDER", data))

        getFolderChilds(TreeView1.Nodes)
        MsgBox("Export Completed successfully!", MsgBoxStyle.OkOnly, "Export complete")
    End Sub

    Function getStringChilds(tree As TreeNodeCollection) As String
        Dim treeBuilder As New Text.StringBuilder
        For Each child As TreeNode In tree
            treeBuilder.AppendLine("{")
            treeBuilder.AppendLine(" 'text': '" & child.Text & "',")
            treeBuilder.AppendLine(" 'children': [" & getStringChilds(child.Nodes) & "]")
            treeBuilder.AppendLine("},")
        Next
        Return treeBuilder.ToString
    End Function

    Function getFolderChilds(tree As TreeNodeCollection) As String
        IO.Directory.CreateDirectory(destDir.FullName & "\resources\folders\")
        For Each child As TreeNode In tree
            Dim exportHTML = IO.File.ReadAllText(destDir.FullName & "\resources\folder.html")
            Dim treeBuilder As New Text.StringBuilder
            Dim searchPath = destDir.FullName & "\" & child.FullPath.Replace("=-=", "\")
            Dim files = New IO.DirectoryInfo(searchPath).GetFiles
            For Each file In files
                Application.DoEvents()
                ProgressLBL.Text = String.Format("{0}/{1} cartella - File {2}/{3}", tree.IndexOf(child), tree.Count, Array.IndexOf(files, file), files.Length)

                Dim rows As HtmlAgilityPack.HtmlNodeCollection
                Dim from_email As String = ""
                Dim to_email As String = ""
                Dim sent_on As String = ""
                Dim subject As String = ""
                Dim attachemnts = "false"
                Try
                    Dim HTML As New HtmlAgilityPack.HtmlDocument()
                    Dim contentHTML As String = IO.File.ReadAllText(file.FullName)
                    contentHTML = contentHTML.Replace(emailEdit, "")
                    contentHTML = contentHTML.Insert(contentHTML.IndexOf("<font"), emailEdit)
                    IO.File.WriteAllText(file.FullName, contentHTML)
                    HTML.LoadHtml(contentHTML.Substring(0, contentHTML.IndexOf("<hr>")))
                    Dim table = HTML.DocumentNode.SelectNodes("//table")(0)
                    rows = table.SelectNodes("//tr")
                Catch ex As Exception
                    Continue For
                End Try

                For Each row As HtmlAgilityPack.HtmlNode In rows
                    Dim header = row.FirstChild.InnerText.Trim
                    Dim content = row.ChildNodes(1).InnerText.Trim
                    Select Case header
                        Case "From:"
                            from_email = content.Replace("<", "&lt;").Replace(">", "&gt;")
                        Case "Sent On:"
                            Try
                                Dim substring = content.Substring(0, content.IndexOf("(")).Trim
                                Dim data = Date.ParseExact(substring, "dd/MM/yyyy HH:mm:ss", provider)
                                sent_on = data.ToString("yyyy/MM/dd HH:mm:ss")
                            Catch ex As Exception
                                Try
                                    Dim substring = content.Substring(0, content.IndexOf("(")).Trim
                                    Dim data = Date.ParseExact(substring, "dd/MM/yyyy HH:mm", provider)
                                    sent_on = data.ToString("yyyy/MM/dd HH:mm")
                                Catch exc As Exception
                                    sent_on = "1970/01/01 00:00:00"
                                End Try
                            End Try
                        Case "Subject:"
                            subject = content
                        Case "To:"
                            to_email = content.Replace("<", "&lt;").Replace(">", "&gt;")
                        Case "Attachments:"
                            attachemnts = "true"
                    End Select
                Next
                Dim link = file.FullName.Replace(getcommonpath(destDir.FullName, file.FullName), "")
                treeBuilder.AppendLine("<tr>")
                treeBuilder.AppendLine("<td>" & subject & "</td>")
                treeBuilder.AppendLine("<td>" & from_email & "</td>")
                treeBuilder.AppendLine("<td>" & to_email & "</td>")
                treeBuilder.AppendLine("<td>" & sent_on & "</td>")
                treeBuilder.AppendLine("<td>" & attachemnts & "</td>")
                treeBuilder.AppendLine("<td>" & link & "</td>")
                treeBuilder.AppendLine("</tr>")
            Next
            IO.File.WriteAllText(destDir.FullName & "\resources\folders\" & child.FullPath & ".html", exportHTML.Replace("ROWS_PLACEHOLDER", treeBuilder.ToString).Replace("INDEX_PAGE_TTILE_PLACEHOLDER", searchPath))
            Application.DoEvents()
            getFolderChilds(child.Nodes)
        Next
        Return ""
    End Function
End Class

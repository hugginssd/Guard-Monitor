Public Class frmDeployment

    Private Sub frmDeployment_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim siteSource As New Dictionary(Of String, String)()
        With rs("select * from tbl_site")
            If Not .EOF Or Not .BOF Then
                Do
                    siteSource.Add(.Fields("id").Value, .Fields("name").Value)
                    .MoveNext()
                Loop While Not .EOF
            End If
        End With

        cmbSiteID.DataSource = New BindingSource(siteSource, Nothing)
        cmbSiteID.DisplayMember = "Value"
        cmbSiteID.ValueMember = "Key"

        loadListview()
    End Sub

    Private Sub cmbSiteID_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbSiteID.SelectedIndexChanged
        Try
            With rs("select * from tbl_site where id = " & cmbSiteID.SelectedValue)
                txtSiteName.Text = .Fields("name").Value
                txtSiteAddress.Text = .Fields("address").Value
                txtContactName.Text = .Fields("contact_name").Value
                txtContactNumber.Text = .Fields("contact_number").Value
                txtCoordinates.Text = .Fields("coordinates").Value

                loadListview()
            End With
        Catch ex As Exception
        End Try
    End Sub

    Sub loadListview()
        ListView1.Items.Clear()
        With rs("select * from tbl_employee ")
            If Not .EOF Or Not .BOF Then
                Do
                    Dim lvx As ListViewItem = New ListViewItem

                    lvx.Text = .Fields("employee_number").Value.ToString()
                    lvx.SubItems.Add(.Fields("firstnames").Value.ToString())
                    lvx.SubItems.Add(.Fields("lastname").Value.ToString())
                    lvx.SubItems.Add(.Fields("contact_number").Value.ToString())
                    lvx.SubItems.Add(.Fields("address").Value).ToString()

                    If cmbSiteID.SelectedValue <> "" Then
                        If employeeDeployed(cmbSiteID.SelectedValue, .Fields("id").Value()) Then
                            lvx.Checked = True
                        End If
                    End If

                    ListView1.Items.Add(lvx)
                    .MoveNext()
                Loop While Not .EOF
            End If
        End With
    End Sub

    Private Sub cmdClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdClose.Click
        Me.Dispose()
    End Sub

    Private Sub cmdSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSave.Click
        If cmbSiteID.Text = "" Then
            MsgBox("Please select a site", vbExclamation)
            Exit Sub
        End If

        If ListView1.CheckedItems.Count = 0 Then
            MsgBox("Please select employees to deploy", vbExclamation)
            Exit Sub
        End If

        If Val(txtDays.Text) = 0 Then
            MsgBox("Please enter number of days", vbExclamation)
            Exit Sub
        End If

        rs("delete from tbl_deployment where site_id = " & cmbSiteID.SelectedValue)

        Dim checkedItems As ListView.CheckedListViewItemCollection = ListView1.CheckedItems
        For Each item In checkedItems
           
            With rs("tbl_deployment")
                .AddNew()
                .Fields("site_id").Value = cmbSiteID.SelectedValue
                .Fields("employee_id").Value = getEmployeeID(item.SubItems(0).text)
                .Fields("date_deployed").Value = dtDeployment.Value.Year & "-" & dtDeployment.Value.Month & "-" & dtDeployment.Value.Day
                .Fields("days").Value = cmbSiteID.SelectedValue
                .Fields("instructions").Value = txtSiteInstructions.Text
                .Update()

                MsgBox("Deployment record successfully saved", vbInformation)
                Me.Dispose()
            End With
        Next

    End Sub


End Class
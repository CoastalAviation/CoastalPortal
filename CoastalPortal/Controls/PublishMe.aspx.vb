Imports System.Web.Script.Serialization
Imports Telerik.Web.UI
Imports Optimizer.AirTaxi
Imports Optimizer.DataAccess

Public Class PublishMe
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.Page.Title = "Publish Schedule "
        If Session("AcceptModelID") = "" Then
            Exit Sub
        End If

        If Session("carrierid") = 100 Then


            Dim cutofftime As String
            Dim cutoffdate As DateTime
            If Not IsPostBack Then
                cutoffdate = DateAdd(DateInterval.Hour, 24, Now.ToUniversalTime)
                cutofftime = cutoffdate.Month & "/" & cutoffdate.Day & "/" & cutoffdate.Year & "   " & "05:00"
                RadDateTimeFrom.SelectedDate = CDate(cutofftime)
                cutoffdate = DateAdd(DateInterval.Hour, 72, Now.ToUniversalTime)
                cutofftime = cutoffdate.Month & "/" & cutoffdate.Day & "/" & cutoffdate.Year & "   " & "23:00"
                RadDateTimeTo.SelectedDate = CDate(cutofftime)
            Else
                If Session("overridepublish") = True Then
                    override.Visible = True
                    Session("ovrridepublish") = False
                    Label1.Text = "<script type='text/javascript'>resizepage();</script>"
                End If

            End If

            '    RaddatetimeTo.SelectedDate = DateAdd(DateInterval.Hour, 48, Now.ToUniversalTime)
        End If

    End Sub

    Protected Sub btnPublish_Click(sender As Object, e As EventArgs) Handles btnPublish.Click
        Dim publish As String
        Dim Bestmodelid As String

        If RBPublishList.SelectedValue.ToString = "Publish All" Then
            publish = "ftype [Publish All]"
        ElseIf RBPublishList.SelectedValue.ToString = "Publish King Air Only" Then
            publish = "ftype[BE35]"
        Else
            publish = "ftype[CEXC]"
        End If

        Try

            If IsNothing(Session("carrierid")) Then
                lblmsg.Text = "Credentials Lost, Please Sign in Again"
                Exit Sub
            End If

            Dim carrierid As String = Session("carrierid")


            If carrierid = 0 Then
                lblmsg.Text = "Credentials Lost, Please Sign in Again"
                Exit Sub
            End If

            Dim customrun As String
            For i = 1 To 6
                If Mid(Session("AcceptModelID"), i, 1) = "-" Then Exit For
                customrun = customrun & Mid(Session("AcceptModelID"), i, 1)

            Next i



            If Not (IsNumeric(customrun)) Then
                lblmsg.Text = "Model ID Lost, Please Sign in Again"
                Exit Sub
            End If


            Dim cnsgazure As New ADODB.Connection

            If cnsgazure.State = 0 Then
                cnsgazure.ConnectionString = ConnectionStringHelper.GetConnectionStringSQLMKAzure
                cnsgazure.Open()
            End If

            Dim req As String
            Dim rs As New ADODB.Recordset

            req = "select * from OptimizerRequest where id = " & CInt(customrun)



            If rs.State = 1 Then rs.Close()
            rs.Open(req, cnsgazure, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)


            If rs.EOF Then
                lblmsg.Text = "Model ID Not Found, Please Sign in Again"
                Exit Sub

            End If


            If rs.Fields("declaredcomplete").Value <> True Then
                lblmsg.Text = "Model Not Declared Complete, Please Wait"
                Exit Sub
            End If

            req = "SELECT top 1 * FROM [OptimizerLog]  where casrevenuemiles <> 0 and  left (modelrunid, 4) = '8183' and caslinebreaks <= (foslinebreaks + 7)  and  modelrunid not like '%Q-%' and ModelRunID not like '%R11%' and ModelRunID not like '%R12%'   and ModelRunID not like '%R0%'  and CASrevenueexpense <> 0 and carrierid = abc and FOSrevenuelegs - 25 <= CASrevenuelegs order by CAStotalrevenueexpense asc"
            req = Replace(req, "8183", customrun)
            req = Replace(req, "(modelrunid, 4)", "(modelrunid, " & Len(customrun) & ")")
            req = Replace(req, "abc", Session("carrierid"))
            If rs.State = 1 Then rs.Close()
            rs.Open(req, cnsgazure, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
            If Not rs.EOF Then
                Bestmodelid = Trim(rs.Fields("modelrunid").Value)
            End If

            If Session("AcceptModelID") <> Bestmodelid Then
                If Not override.Checked Then
                    Session("overridepublish") = True
                    'Label1.Text = "<script type='text/javascript'>resizepage();</script>"
                    Lblmsg.Text = "Model selected " & Session("AcceptModelID") & ".  Best model " & Bestmodelid & ". To publish selected model check override and Publish, otherwise cancel and load new model."
                    Exit Sub
                Else
                    sendemailtemplate("dhacket@coastalavtech.com,rkane@coastalavtech.com", Session("AcceptModelID") & " model published with Override", "Model " & Session("AcceptModelID") & " was published with override when best model & " & Bestmodelid & " was available", 100)
                End If
            End If

            Dim tup As String
            tup = "notbefore[" & RadDateTimeFrom.SelectedDate & "]"
            tup = tup & "notafter[" & RadDateTimeTo.SelectedDate & "]"

            tup = tup & "carrierid[" & Session("carrierid") & "]"
            tup = tup & "ck24[" & chkDH24.Checked & "]"

            tup = tup & "cascalendarmodelid[" & Session("AcceptModelID") & "]"

            tup = tup & "daterangefrom[" & daterangefrom & "]"

            tup = tup & "daterangeto[" & daterangeto & "]"

            tup = tup & publish

            tup = tup

            Dim Post_Result As String
            If ConnectionStringHelper.ts = "Test" Then
                Post_Result = postToServiceBusQueue("AcceptAllTest", tup, 5)
            Else
                Post_Result = postToServiceBusQueue("acceptall", tup, 5)
            End If
            Session("overridepublish") = False
            If Left(Post_Result, 12) = "Added record" Then 'rk checking left 12  7.19.17
                Lblmsg.Text = "Job Submitted "
            Else
                Lblmsg.Text = "Problem Submitting Job - Please Log off and Log on again."
                Lblmsg.ForeColor = System.Drawing.Color.Red
            End If
            Exit Sub

        Catch ex As Exception
            Insertsys_log(Session("carrierid"), appName, ex.Message & Now & " -1348 - ", "", "")

            lblmsg.Text = "Error Encountered " & ex.Message
            Exit Sub
        End Try

        Exit Sub


    End Sub

End Class
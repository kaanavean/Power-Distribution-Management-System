Imports System.Data.Common
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Xml.Serialization
Imports Core_Dictionary_Module

Public Class DCL 'Dictionary Communication Layer (CDM and XELA)
    Dim xela = New Main()
    Dim batterySvc As New BatteryInformation()
    Dim chargeSvc As New ChargeInformation()
    Dim _bin As String = "C:\KAVN\%xela.arc%\system.dict\pdms.arc\"

    Private Class NativeMethods
        <DllImport("ntdll.dll")>
        Public Shared Function NtShutdownSystem(ByVal Action As Integer) As Integer
        End Function

        <DllImport("advapi32.dll", SetLastError:=True)>
        Public Shared Function OpenProcessToken(ByVal ProcessHandle As IntPtr, ByVal DesiredAccess As Integer, ByRef TokenHandle As IntPtr) As Boolean
        End Function

        <DllImport("advapi32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
        Public Shared Function LookupPrivilegeValue(ByVal lpSystemName As String, ByVal lpName As String, ByRef lpLuid As LUID) As Boolean
        End Function

        <DllImport("advapi32.dll", SetLastError:=True)>
        Public Shared Function AdjustTokenPrivileges(ByVal TokenHandle As IntPtr, ByVal DisableAllPrivileges As Boolean, ByRef NewState As TOKEN_PRIVILEGES, ByVal BufferLength As Integer, ByVal PreviousState As IntPtr, ByVal ReturnLength As IntPtr) As Boolean
        End Function

        <StructLayout(LayoutKind.Sequential)>
        Public Structure LUID
            Public LowPart As UInt32
            Public HighPart As Int32
        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Public Structure TOKEN_PRIVILEGES
            Public PrivilegeCount As Integer
            Public Luid As LUID
            Public Attributes As Integer
        End Structure

        Public Const SE_PRIVILEGE_ENABLED As Integer = &H2
        Public Const TOKEN_QUERY As Integer = &H8
        Public Const TOKEN_ADJUST_PRIVILEGES As Integer = &H20
        Public Const SE_SHUTDOWN_NAME As String = "SeShutdownPrivilege"
    End Class

    ' <Kanote>
    ' This is a layer made for XELA, and is going to be a background layer and most important API for XELA, MaidOS (XRS/Experimental Research Stage) and the upcoming MaidNVI Runtime
    ' I am currently trying to integrate LTE into the DCL to limit external usage and create a XELA-only environment.
    ' </Kanote>

    Public Sub ForceShutdown() ' Missing - key As String
        Dim _active As Boolean = xela.MRead_Boolean(_bin & "force_shutdown.sta")
        Dim _minimumLevel As Integer = xela.MRead_Integer(_bin & "force_shutdown.val")
        Dim _current As Integer = batterySvc.CapacityPercent

        If _active AndAlso _current > -1 AndAlso _current <= _minimumLevel Then

            If Not chargeSvc.IsCharging Then
                SystemAction_Emergency(0)
            End If

        End If
    End Sub

    Public Function MinimumReq() As Boolean ' True if the battery level is above the minimum requirement, false if not. If the feature is not active, it will return false.
        Dim _active As Boolean = xela.MRead_Boolean(_bin & "minimum_req.sta")
        Dim _minimumLevel As Integer = xela.MRead_Integer(_bin & "minimum_req.val")
        Dim _current As Integer = batterySvc.CapacityPercent

        If Not _active Then Return False

        If _current <= -1 Then Return False

        If _current > _minimumLevel Then
            Return True
        Else
            SystemAction_Emergency(0)
            Return False
        End If
    End Function

    Private Sub SystemAction_Emergency(ByVal actionType As Integer) 'Set private to prevent misuse, this is only for internal use of the class, and will not be exposed to XELA or any other API. ActionType: 0 = Shutdown, 1 = Reboot, 2 = Sleep, 3 = Hibernate
        Dim hToken As IntPtr
        Dim luid As New NativeMethods.LUID()
        Dim tp As New NativeMethods.TOKEN_PRIVILEGES()

        If NativeMethods.OpenProcessToken(Process.GetCurrentProcess().Handle, NativeMethods.TOKEN_ADJUST_PRIVILEGES Or NativeMethods.TOKEN_QUERY, hToken) Then
            If NativeMethods.LookupPrivilegeValue(Nothing, NativeMethods.SE_SHUTDOWN_NAME, luid) Then
                tp.PrivilegeCount = 1
                tp.Luid = luid
                tp.Attributes = NativeMethods.SE_PRIVILEGE_ENABLED
                NativeMethods.AdjustTokenPrivileges(hToken, False, tp, 0, IntPtr.Zero, IntPtr.Zero)
            End If
        End If

        NativeMethods.NtShutdownSystem(actionType)
    End Sub

    Public Function LastSecureLevel() As Boolean 'last secure level - True if the battery level is at or below the last secure level, false if not. If the feature is not active, it will return false.
        Dim _active As Boolean = xela.MRead_Boolean(_bin & "last_secure.sta")
        Dim _secureLevel As Integer = xela.MRead_Integer(_bin & "last_secure.val")
        Dim _current As Integer = batterySvc.CapacityPercent

        If _active Then
            If _current <= 6 Then
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function

    Public Enum Reports
        ForceShutdown_Active
        ForceShutdown_MinimumLevel
        MinimumReq_Active
        MinimumReq_Level
        LastSecure_Active
        LastSecure_Level
    End Enum

    Public Function SystemReport(obj As Reports) As Object ' This function is used to get the current settings of the system, it will return the value of the setting based on the object passed in. If the object is not valid, it will return Nothing.
        If obj = Reports.ForceShutdown_Active Then
            Return xela.MRead_Boolean(_bin & "force_shutdown.sta")
        ElseIf obj = Reports.ForceShutdown_MinimumLevel Then
            Return xela.MRead_Integer(_bin & "force_shutdown.val")
        ElseIf obj = Reports.MinimumReq_Active Then
            Return xela.MRead_Boolean(_bin & "minimum_req.sta")
        ElseIf obj = Reports.MinimumReq_Level Then
            Return xela.MRead_Integer(_bin & "minimum_req.val")
        ElseIf obj = Reports.LastSecure_Active Then
            Return xela.MRead_Boolean(_bin & "last_secure.sta")
        ElseIf obj = Reports.LastSecure_Level Then
            Return xela.MRead_Integer(_bin & "last_secure.val")
        Else
            Return Nothing
        End If
    End Function
End Class
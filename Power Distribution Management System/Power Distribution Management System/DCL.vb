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
    ' </Kanote>

    Public Sub ForceShutdown()
        Dim _active As Boolean = xela.MRead_Boolean(_bin & "force_shutdown.sta")
        Dim _minimumLevel As Integer = xela.MRead_Integer(_bin & "force_shutdown.val")
        Dim _current As Integer = batterySvc.CapacityPercent

        If _active AndAlso _current > -1 AndAlso _current <= _minimumLevel Then

            If Not chargeSvc.IsCharging Then
                TriggerEmergencyAction(0)
            End If

        End If
    End Sub

    Public Function MinimumReq() As Boolean
        Dim _active As Boolean = xela.MRead_Boolean(_bin & "minimum_req.sta")
        Dim _minimumLevel As Integer = xela.MRead_Integer(_bin & "minimum_req.val")
        Dim _current As Integer = batterySvc.CapacityPercent

        If Not _active Then Return False

        If _current <= -1 Then Return False

        If _current > _minimumLevel Then
            Return True
        Else
            TriggerEmergencyAction(0)
            Return False
        End If
    End Function

    Public Sub TriggerEmergencyAction(ByVal actionType As Integer)
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
End Class
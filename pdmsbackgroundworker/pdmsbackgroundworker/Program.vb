Imports System
Imports System.Threading
Imports Power_Distribution_Management_System

Module Programm
    Sub Main()
        Dim running As Boolean = True
        Dim isBooted As Boolean = False
        Dim pdms As New DCL

        If isBooted = False Then
            If pdms.MinimumReq = True Then
                isBooted = True
            End If
        End If

        Task.Run(Sub()
                     Do While running
                         If isBooted Then
                             pdms.ForceShutdown() ' Checks if the system is in a critical state and forces shutdown if necessary
                         End If

                         Thread.Sleep(5000) ' 5 Sekunden warten
                     Loop
                 End Sub)

        Do While running
            Thread.Sleep(1000)
        Loop
    End Sub
End Module
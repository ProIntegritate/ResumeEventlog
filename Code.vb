' Run as Admin. Counters https://github.com/jfmaes/SharpNukeEventLog

Imports System
Imports System.Diagnostics
Imports System.Threading
Imports System.Runtime.InteropServices

Module Program

    Public Enum ThreadAccess As Integer
        Terminate = &H1
        SuspendResume = &H2
        GetContext = &H8
        SetContext = &H10
        SetInformation = &H20
        QueryInformation = &H40
        SetThreadToken = &H80
        Impersonate = &H100
        DirectImpersonation = &H200
    End Enum

    <DllImport("kernel32.dll")>
    Private Function OpenThread(ByVal dwDesiredAccess As ThreadAccess, ByVal bInheritHandle As Boolean, ByVal dwThreadId As Integer) As IntPtr
    End Function

    Declare Function ResumeThread Lib "kernel32" (ByVal hThread As String) As Integer

    Sub Main(args As String())

        Dim p() As System.Diagnostics.Process = Process.GetProcessesByName("svchost")
        Dim pmc As System.Diagnostics.ProcessModuleCollection

        Dim ThreadHandle As IntPtr
        Dim bDone As Boolean = False

        Try
            For n = 0 To UBound(p)
                pmc = p(n).Modules

                For m = 0 To pmc.Count - 1
                    If LCase(pmc(m).ModuleName) = "wevtsvc.dll" Then
                        Console.WriteLine("wevtsvc.dll found in PID:" & p(n).Id.ToString & "->" & p(n).ProcessName)
                        Dim ptc As System.Diagnostics.ProcessThreadCollection
                        Dim pt As System.Diagnostics.ProcessThread

                        ptc = p(0).Threads
                        For o = 0 To ptc.Count - 1
                            pt = ptc(o)

                            ThreadHandle = OpenThread(ThreadAccess.SuspendResume, False, pt.Id)
                            If ThreadHandle <> IntPtr.Zero Then
                                ResumeThread(ThreadHandle)
                                Console.WriteLine("ThreadHandle: " & ThreadHandle.ToString & " - Resumed.")
                            End If
                            bDone = True
                        Next

                    End If
                    If bDone = True Then Exit For
                Next
                If bDone = True Then Exit For
            Next
        Catch ex As Exception
        End Try

    End Sub

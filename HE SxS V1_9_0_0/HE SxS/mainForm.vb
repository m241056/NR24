Option Strict Off
Option Explicit Off
Imports NationalInstruments
Imports NationalInstruments.DAQmx
Imports NationalInstruments.UI
Imports NationalInstruments.UI.WindowsForms
Imports System
Imports System.Threading
Imports Ixxat.Vci4
Imports Microsoft.Office.Interop
Imports System.IO

Public Class mainForm
    'DAQmx Objects
    Dim myTask As Task
    Dim runningTask As Task
    Dim analogInReader As AnalogMultiChannelReader
    Dim samples As Integer = 10

    'Microsoft Excel Objects
    Dim intRow As Integer
    Dim oXL As Excel.Application
    Dim oWB As Excel.Workbook
    Dim oSheet As Excel.Worksheet
    Dim oRng As Excel.Range
    Dim oInt As Integer = 0

    'Vehicle Component Warning/Error Flags
    Dim RDMotorError As Boolean
    Dim RDMotorWarning As Boolean
    Dim RPMotorError As Boolean
    Dim RPMotorWarning As Boolean
    Dim FDMotorError As Boolean
    Dim FDMotorWarning As Boolean
    Dim FPMotorError As Boolean
    Dim FPMotorWarning As Boolean
    Dim GenMotorError As Boolean
    Dim GenMotorWarning As Boolean
    Dim RDControllerError As Boolean
    Dim RDControllerWarning As Boolean
    Dim RPControllerError As Boolean
    Dim RPControllerWarning As Boolean
    Dim FDControllerError As Boolean
    Dim FDControllerWarning As Boolean
    Dim FPControllerError As Boolean
    Dim FPControllerWarning As Boolean
    Dim GenControllerError As Boolean
    Dim GenControllerWarning As Boolean
    Dim DriverBatteryError As Boolean
    Dim DriverBatteryWarning As Boolean
    Dim MiddleBatteryError As Boolean
    Dim MiddleBatteryWarning As Boolean
    Dim PassengerBatteryError As Boolean
    Dim PassengerBatteryWarning As Boolean
    Dim RearGearboxError As Boolean
    Dim RearGearboxWarning As Boolean
    Dim FrontGearboxError As Boolean
    Dim FrontGearboxWarning As Boolean
    Dim GenEngineError As Boolean
    Dim GenEngineWarning As Boolean
    Dim ErrorDisplayState As Boolean = False
    Dim powerReading(19) As Single
    Dim currentPowerReading As Integer = 0
    Dim odometerMultiplier As Single = 0.25913497    '0.621426796 // Changed on 01-May to match GPS mileage
    Dim motorSpeedLimit As Integer = 7500
    Dim maxMotorSpeed As Integer = 7500
    Dim minMotorSpeed As Integer = 0
    Dim int16Value As UInt16
    Dim int16OValue As UInt16
    Dim int16LSBThrottle As UInt16
    Dim int16MSBThrottle As UInt16
    Dim int16LSBSpeed As UInt16
    Dim int16MSBSpeed As UInt16
    Dim throttleActive As Boolean = False
    Dim vehicleMileageUpgrade As Integer = 385

    Dim Sevcon0 As sevconClass
    Dim Sevcon1 As sevconClass
    Dim Sevcon2 As sevconClass
    Dim Sevcon3 As sevconClass
    Dim Sevcon4 As sevconClass
    Dim Battery0 As BatteryClass
    Dim Battery1 As BatteryClass
    Dim Battery2 As BatteryClass
    Dim Battery3 As BatteryClass
    Dim CanIO0 As CANio500Class
    Dim CanIO1 As CANio500Class

    Dim vehicleControlStatus As String = "ECU"
    Dim generatorEnabled As Boolean = False
    Dim generatorChargingRequest As Boolean = True
    Dim generatorSOCEnabled As Boolean = False
    Dim generatorControllerOn As Boolean = False
    Dim generatorEngineStarting As Boolean = False
    Dim generatorEngineRunning As Boolean = False
    Dim generatorEngineQuitting As Boolean = False
    Dim ErrorFlashingState As Boolean = False
    Dim InactiveComponentColor As Color = Color.Gray
    Dim ActiveComponentColor As Color = Color.LightGreen
    Dim WarningComponentColor As Color = Color.Yellow
    Dim ErrorComponentColor As Color = Color.Red
    Dim DataLoggingActive As Boolean = False
    Dim RDMotorActive As Boolean
    Dim RPMotorActive As Boolean
    Dim FDMotorActive As Boolean
    Dim FPMotorActive As Boolean
    Dim GenMotorActive As Boolean
    Dim RDControllerActive As Boolean
    Dim RPControllerActive As Boolean
    Dim FDControllerActive As Boolean
    Dim FPControllerActive As Boolean
    Dim GenControllerActive As Boolean
    Dim DriverBatteryActive As Boolean
    Dim MiddleBatteryActive As Boolean
    Dim PassengerBatteryActive As Boolean
    Dim RearGearboxActive As Boolean = True
    Dim FrontGearboxActive As Boolean = True
    Dim EngineActive As Boolean
    Dim numOfCANMessage As Integer
    Dim intHeartbeat As Integer
    Dim intHourCounter As Integer
    Dim zeroTurnEnabled As Boolean
    Dim zeroTurnActive As Boolean
    Dim zeroTurnDirection As String
    Dim RDTravelDirection As String = "Neutral"
    Dim RPTravelDirection As String = "Neutral"
    Dim FDTravelDirection As String = "Neutral"
    Dim FPTravelDirection As String = "Neutral"
    Dim RDDirectionChange As Boolean
    Dim RPDirectionChange As Boolean
    Dim FDDirectionChange As Boolean
    Dim FPDirectionChange As Boolean
    Dim emptyFuelVoltage As Single = 4.27
    Dim fullFuelVoltage As Single = 8.53
    Dim currentFuelVoltage As Single
    Dim fuelReading(100) As Single
    Dim currentFuelReading As Integer
    Dim fuelReadingDataLooped As Boolean = False
    Dim calculatingFuelIndicatorBar As Integer
    Dim fullBatteryVoltage As Single = 115.42
    Dim emptyBatteryVoltage As Single = 89.5
    Dim SevconTempWarningLevel As Integer = 75
    Dim SevconTempErrorLevel As Integer = 85
    Dim SevconCurrentWarningLevel As Integer = 550
    Dim SevconCurrentErrorLevel As Integer = 220
    Dim MotorTempWarningLevel As Integer = 135
    Dim MotorTempErrorLevel As Integer = 150
    Dim strErrorLogFile As String = "C:\HE SxS\Code\HE SxS V1_8_0_0\HE SxS\Log Files\ErrorLog_" & DateTime.Today.ToString("dd-MMM-yyy") & ".txt"
    Dim sw As StreamWriter
    Dim LogFileOpened As Boolean
    Dim avgMiles As Single
    Dim AllowedDischargeCurrentPerSevcon As Int16
    Dim AllowedChargeCurrentPerSevcon As Int32

    'IXXAT USB-to-CAN objects
    Private mDevice As Ixxat.Vci4.IVciDevice = Nothing
    Private mCanChn1 As Ixxat.Vci4.Bal.Can.ICanChannel = Nothing
    Private mCanChn2 As Ixxat.Vci4.Bal.Can.ICanChannel = Nothing
    Private mReader1 As Ixxat.Vci4.Bal.Can.ICanMessageReader = Nothing
    Private mReader2 As Ixxat.Vci4.Bal.Can.ICanMessageReader = Nothing
    Private mWriter1 As Ixxat.Vci4.Bal.Can.ICanMessageWriter = Nothing
    Private mWriter2 As Ixxat.Vci4.Bal.Can.ICanMessageWriter = Nothing
    Private mCanCtl1 As Ixxat.Vci4.Bal.Can.ICanControl = Nothing
    Private mCanCtl2 As Ixxat.Vci4.Bal.Can.ICanControl = Nothing
    Private mRxEvent1 As System.Threading.AutoResetEvent = Nothing
    Private mRxEvent2 As System.Threading.AutoResetEvent = Nothing
    Private rxThread1 As System.Threading.Thread = Nothing
    Private rxThread2 As System.Threading.Thread = Nothing
    Private deviceManager As Ixxat.Vci4.IVciDeviceManager = Nothing
    Private deviceList As Ixxat.Vci4.IVciDeviceList = Nothing
    Private deviceEnum As IEnumerator = Nothing
    Private changeEvent1 As New AutoResetEvent(True)
    Private changeEvent2 As New AutoResetEvent(True)
    Private interfaceChangeThread1 As System.Threading.Thread = Nothing
    Private interfaceChangeThread2 As System.Threading.Thread = Nothing

    'This delegate enables asynchronous calls for setting the text property on a TextBox control
    Delegate Sub SetTextCallback1(ByVal [text] As String)
    Delegate Sub SetTextCallback2(ByVal [text] As String)

    'This delegate enables the thread safe call for the interface ListBox
    Delegate Sub availableInterfacesListViewCallBack()

    Private Sub TransferControl()
        'Transfer control back to ECU
        controlSourceSwitch.Value = True
        vehicleControlStatus = "ECU"
        SendCanMessage("Send Heartbeat", 1, 5)
        SendCanMessage("Send Heartbeat", 2, 5)
    End Sub
    Private Sub FillListBoxWithInterfaces()

        If availableInterfacesListView.InvokeRequired Then
            Dim d As New availableInterfacesListViewCallBack(AddressOf FillListBoxWithInterfaces)
            Me.Invoke(d, New Object() {})
        Else
            'Remove all items from the ListBox
            availableInterfacesListView.Items.Clear()

            Try
                deviceManager = Ixxat.Vci4.VciServer.Instance().DeviceManager
                deviceList = deviceManager.GetDeviceList()
                deviceEnum = deviceList.GetEnumerator()
                deviceEnum.Reset()
                Dim intI As Integer = 0
                Do While deviceEnum.MoveNext() = True
                    intI += 1
                    If intI = 1 Then
                        mDevice = deviceEnum.Current
                    ElseIf intI = 2 Then

                    End If


                    Dim CurListViewItem = New ListViewItem
                    If intI = 1 Then
                        CurListViewItem.Tag = mDevice.VciObjectId
                        CurListViewItem.Text = mDevice.Description
                    ElseIf intI = 2 Then

                    End If

                    availableInterfacesListView.Items.Add(CurListViewItem)

                Loop
            Catch ex As Exception

            End Try
        End If
    End Sub

    Private Sub InterfaceChangeThreadFunc()
        Do
            If changeEvent1.WaitOne(-1, False) Then
                FillListBoxWithInterfaces()
            End If
        Loop
    End Sub


    Private Sub mainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        My.Settings.Save()
        sw.WriteLine("NORMAL - Tablet Program Stopped - " & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
        sw.Close()
        CloseAll()
        runningTask = Nothing
        myTask.Dispose()
    End Sub

    Private Sub CloseAll()
        If mCanCtl1 IsNot Nothing Then
            'Stop the CAN1 controller
            Try
                mCanCtl1.StopLine()
            Catch ex As Exception

            End Try
        End If

        If mCanCtl2 IsNot Nothing Then
            'Stop the CAN2 controller
            Try
                mCanCtl2.StopLine()
            Catch ex As Exception

            End Try
        End If

        If rxThread1 IsNot Nothing Then
            'Tell receive thread 1 to quit
            rxThread1.Abort()

            'Wait for termination of receive thread
            rxThread1.Join()

            rxThread1 = Nothing
        End If

        If rxThread2 IsNot Nothing Then
            'Tell receive thread 1 to quit
            rxThread2.Abort()

            'Wait for termination of receive thread
            rxThread2.Join()

            rxThread2 = Nothing
        End If

        If interfaceChangeThread1 IsNot Nothing Then
            'Tell the interface change thread to quit
            interfaceChangeThread1.Abort()

            'Wait for termination of interface change thread
            interfaceChangeThread1.Join()

            interfaceChangeThread1 = Nothing
        End If

        If interfaceChangeThread2 IsNot Nothing Then
            'Tell the interface change thread to quit
            interfaceChangeThread2.Abort()

            'Wait for termination of interface change thread
            interfaceChangeThread2.Join()

            interfaceChangeThread2 = Nothing
        End If
        'Dispose all open objects inclusding the VCI object itself
        CloseVciObjects(True)
        'sw.WriteLine("NORMAL - Ixxat Stopped - " & DateTime.Today.ToString)
    End Sub
    Private Sub StartIxxat()
        deviceManager = Ixxat.Vci4.VciServer.Instance().DeviceManager
        deviceList = deviceManager.GetDeviceList()

        FillListBoxWithInterfaces()

        'Set 500kBit/s as default CAN1 baud rate
        baudrate1ListBox.SelectedIndex = 2

        'Set 500kBit/s as default CAN2 baud rate
        baudrate2ListBox.SelectedIndex = 2

        'Start an own thread which will wait for an interface change message e.g. a USB device was plugged in or out
        interfaceChangeThread1 = New System.Threading.Thread(New System.Threading.ThreadStart(AddressOf InterfaceChangeThreadFunc))
        interfaceChangeThread1.Start()

        init1Button_Click(Me, e)
        init2Button_Click(Me, e)
        sw.WriteLine("NORMAL - Ixxat Started - " & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
    End Sub

    Private Sub mainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            Me.Text = "HE SxS - Version " & Me.ProductVersion.ToString
            currentFuelReading = 1

            'Start Error Log File
            If (Not File.Exists(strErrorLogFile)) Then
                sw = File.CreateText(strErrorLogFile)
                sw.WriteLine("NORMAL - Tablet Program Started - " & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                LogFileOpened = True
            Else
                sw = File.AppendText(strErrorLogFile)
                sw.WriteLine("NORMAL - Tablet Program Started - " & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                LogFileOpened = True
            End If

            StartIxxat()

            Sevcon0 = New sevconClass
            Sevcon1 = New sevconClass
            Sevcon2 = New sevconClass
            Sevcon3 = New sevconClass
            Sevcon4 = New sevconClass

            Sevcon0.Description = "REAR DRIVER"
            Sevcon1.Description = "REAR PASSENGER"
            Sevcon2.Description = "FRONT DRIVER"
            Sevcon3.Description = "FRONT PASSENGER"
            Sevcon4.Description = "GENERATOR"

            Battery0 = New BatteryClass
            Battery1 = New BatteryClass
            Battery2 = New BatteryClass
            Battery3 = New BatteryClass

            Battery0.Description = "DRIVER"
            Battery1.Description = "PASSENGER"
            Battery2.Description = "MIDDLE"
            Battery3.Description = "SPARE"

            CanIO0 = New CANio500Class
            CanIO1 = New CANio500Class

            CanIO0.Description = "Rear CanIO Module"
            CanIO1.Description = "Front CanIO Module"

            CANInfoGroupBox.Visible = False
            CANInformationToolStripMenuItem.Checked = True
            CANInformationToolStripMenuItem_Click(Me, e)
            VehicleOverviewToolStripMenuItem.Checked = False
            VehicleOverviewToolStripMenuItem_Click(Me, e)
            AnalogToolStripMenuItem_Click(Me, e)

            'Load saved application settings
            minSOCActiveSlide.Value = My.Settings.genChargeActivateMin
            maxSOCActiveSlide.Value = My.Settings.genChargeActivateMax
            tractionControlMinSpeedSlide.Value = My.Settings.tractionControlLowSpeedLimit

            batteryBusTimer.Enabled = True
            intHeartbeat = 5
            intHourCounter = 150

            myTask = New Task()
            With myTask
                .AIChannels.CreateVoltageChannel("USB-6343_018D53BA/ai0", "Temp_Front_Gearbox", AITerminalConfiguration.Differential, -40, 179, "SENSOR_01-06")
                .AIChannels.CreateVoltageChannel("USB-6343_018D53BA/ai1", "Temp_Front_Heatsink", AITerminalConfiguration.Differential, -40, 179, "SENSOR_01-06")
                .AIChannels.CreateVoltageChannel("USB-6343_018D53BA/ai2", "Temp_Rear_Gearbox", AITerminalConfiguration.Differential, -40, 179, "SENSOR_01-06")
                .AIChannels.CreateVoltageChannel("USB-6343_018D53BA/ai3", "Temp_Rear_Heatsink", AITerminalConfiguration.Differential, -40, 179, "SENSOR_01-06")
                .AIChannels.CreateVoltageChannel("USB-6343_018D53BA/ai4", "Temp_Ambient", AITerminalConfiguration.Differential, 32, 212, "TEMP_AMBIENT")
                .AIChannels.CreateVoltageChannel("USB-6343_018D53BA/ai5", "Humidity_Ambient", AITerminalConfiguration.Differential, 0, 100, "HUMIDITY_AMBIENT")
                .Control(TaskAction.Verify)
            End With

            runningTask = myTask
            analogInReader = New AnalogMultiChannelReader(myTask.Stream)
        Catch ex As Exception
            sw.WriteLine("ERROR - mainForm_Load_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
        End Try
        
    End Sub

    Private Sub ReceiveThreadFunc1()
        Dim canMessage As Ixxat.Vci4.Bal.Can.ICanMessage

        Try
            Do
                'Wait 10msec for a message reception
                If mRxEvent1.WaitOne(10, False) Then
                    'Read a CAN message from the receive FIFO
                    If mReader1.ReadMessage(canMessage) Then
                        ShowReceivedMessage(canMessage, 1)
                    End If
                End If
            Loop
        Catch ex As Exception
            'sw.WriteLine("ERROR - ReceiveThreadFunc1_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
        End Try


    End Sub

    Private Sub ReceiveThreadFunc2()
        Dim canMessage As Ixxat.Vci4.Bal.Can.ICanMessage

        Try
            Do
                'Wait 10msec for a message reception
                If mRxEvent2.WaitOne(100, False) Then
                    'Read a CAN message from the receive FIFO
                    If mReader2.ReadMessage(canMessage) Then
                        ShowReceivedMessage(canMessage, 2)
                    End If
                End If
            Loop
        Catch ex As Exception
            'sw.WriteLine("ERROR - ReceiveThreadFunc2_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
        End Try


    End Sub
    Private Sub ShowReceivedMessage(ByVal canMessage As Ixxat.Vci4.Bal.Can.CanMessage, ByVal CanBusNum As Integer)

        Try
            Select Case canMessage.FrameType
                Case Ixxat.Vci4.Bal.Can.CanMsgFrameType.Data
                    If CanBusNum = 1 Then
                        ShowDataMessage(canMessage, 1)
                    ElseIf CanBusNum = 2 Then
                        ShowDataMessage(canMessage, 2)
                    End If


                Case Ixxat.Vci4.Bal.Can.CanMsgFrameType.Error
                    ShowErrorMessage(canMessage, CanBusNum)
                Case Ixxat.Vci4.Bal.Can.CanMsgFrameType.Info
                    ShowInfoMessage(canMessage, CanBusNum)
                Case Ixxat.Vci4.Bal.Can.CanMsgFrameType.Status
                    ShowStatusMessage(canMessage, CanBusNum)
                Case Ixxat.Vci4.Bal.Can.CanMsgFrameType.TimeOverrun
                    ShowTimerOverrunMessage(canMessage, CanBusNum)
                Case Ixxat.Vci4.Bal.Can.CanMsgFrameType.TimeReset
                    ShowTimerResetMessage(canMessage, CanBusNum)
                Case Ixxat.Vci4.Bal.Can.CanMsgFrameType.Wakeup
                    ShowWakeUpMessage(canMessage, CanBusNum)
            End Select
        Catch ex As Exception
            sw.WriteLine("ERROR - ShowReceivedMessage_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
        End Try

    End Sub

    Private Sub ShowDataMessage(ByVal canMessage As Ixxat.Vci4.Bal.Can.ICanMessage, ByVal CanBusNum As Integer)
        Dim textLine As String
        Dim textPacket As Single
        Dim CANdata(7) As Byte
        Dim CobId As String
        Dim intI As Integer

        Try


            'For intMessageNum = 0 To numOfCANMessage - 1
            For intI = 1 To canMessage.DataLength
                CANdata(intI - 1) = canMessage.Item(intI - 1)
                'Debug.Print("CANdata" & intI.ToString & " - " & CANdata(intI - 1).ToString)
            Next
            CobId = canMessage.Identifier.ToString("X2")
            'Debug.Print(intMessageNum & " - " & CobId.ToString)
            If CanBusNum = 1 Then       'Rear Sevcons, rear CanIO, driver battery, & passenger battery
                Select Case CobId
                    Case &H8A.ToString("X2")    'CanIO0 (NODE A - REAR MODULE) EMERGENCY MESSAGES
                    Case &H181.ToString("X2")   'Sevcon0 (NODE 1 - REAR DRIVER) DIGITAL INPUTS
                        Sevcon0.DigitalInputs = CANdata(2)
                        Sevcon0.TimeStamp = DateTime.Now
                    Case &H182.ToString("X2")   'Sevcon1 (NODE 2 - REAR PASSENGER) DIGITAL INPUTS
                        Sevcon1.DigitalInputs = CANdata(2)
                        Sevcon1.TimeStamp = DateTime.Now
                    Case &H188.ToString("X2")   'Battery1 (NODE 8 - PASSENGER) PACK_STATUS
                        Battery1.SoC = ConvertHexString(CANdata(0))
                        Battery1.BmsStatusBits = ConvertHexString(CANdata(1), CANdata(2))
                        Battery1.TimeStamp = DateTime.Now
                    Case &H189.ToString("X2")   'Battery0 (NODE 9 - DRIVER) PACK_STATUS
                        Battery0.SoC = ConvertHexString(CANdata(0))
                        Battery0.BmsStatusBits = ConvertHexString(CANdata(1), CANdata(2))
                        Battery0.TimeStamp = DateTime.Now
                    Case &H18A.ToString("X2")   'CanIO0 (NODE A - REAR MODULE) DIGITAL INPUTS
                        'deltaTime = DateTime.Now - CanIO1.TimeStamp180
                        'If deltaTime.TotalSeconds > 0.1 Then
                        CanIO0.ReadDigitalInputs(CANdata(0), CANdata(1))
                        If vehicleControlStatus = "Tablet" Then
                            If CanIO0.DigitalInput1 = True Then         '//Key On
                                CanIO0.DigitalOutput4 = True
                                CanIO1.DigitalOutput1 = True
                            Else
                                CanIO0.DigitalOutput4 = False
                                CanIO1.DigitalOutput1 = False
                            End If
                            If CanIO0.DigitalInput2 = True Then         '//Generator Enable
                                If generatorSOCEnabled = True Then
                                    If generatorEngineRunning = False Then
                                        If generatorEngineStarting = False Then
                                            If generatorEngineQuitting = False Then
                                                generatorEnabled = True
                                                CanIO0.DigitalOutput1 = True 'Enable the Generator Sevcon Contactor
                                                CanIO1.DigitalOutput2 = True 'Enable the Kohler Fuel Solenoid and cooling fans
                                                EngineActive = True
                                            End If
                                        End If
                                    End If
                                Else
                                    If generatorEngineRunning = True Then
                                        CanIO1.DigitalOutput2 = False
                                        EngineActive = False
                                    End If
                                    If generatorEngineRunning = False And generatorEngineQuitting = False And generatorEngineStarting = False Then
                                        CanIO0.DigitalOutput1 = False
                                    End If
                                End If
                            Else
                                If generatorEngineRunning = True Then
                                    CanIO1.DigitalOutput2 = False
                                End If
                                If generatorEngineRunning = False And generatorEngineQuitting = False And generatorEngineStarting = False Then
                                    CanIO0.DigitalOutput1 = False
                                End If
                            End If
                            If CanIO0.DigitalInput3 = True Then         '//Spare
                            End If
                            If CanIO0.DigitalInput4 = True Then         '//Spare
                            End If
                            If chargePortEnableSwitch.Value = True And CanIO0.DigitalInput2 = False Then
                                CanIO0.DigitalOutput3 = True
                            Else
                                CanIO0.DigitalOutput3 = False
                            End If
                            CanIO0.SetDigitalOutputs()
                            SendCanMessage("Digital Outputs", 1, 0)
                        End If
                        CanIO0.TimeStamp180 = DateTime.Now
                        CanIO0.TimeStamp = DateTime.Now
                        'End If
                    Case &H20A.ToString("X2")   'CanIO0 (NODE A - REAR MODULE) DIGITAL OUTPUTS
                    Case &H281.ToString("X2")   'Sevcon0 (NODE 1 - REAR DRIVER) RPM_THROTTLE_MOTTEMP
                        Sevcon0.Velocity = ConvertHexString(CANdata(0), CANdata(1), CANdata(2), CANdata(3))
                        Sevcon0.ThrottleInputVoltage = ConvertHexString(CANdata(4), CANdata(5))
                        Sevcon0.MotorTemp = ConvertHexString(CANdata(6), CANdata(7))
                        If Sevcon0.MotorTemp > MotorTempWarningLevel Then
                            RDMotorWarning = True
                        Else
                            RDMotorWarning = False
                        End If
                        If Sevcon0.MotorTemp > MotorTempErrorLevel Then
                            RDMotorError = True
                        Else
                            RDMotorError = False
                        End If
                        Sevcon0.TimeStamp = DateTime.Now
                    Case &H282.ToString("X2")   'Sevcon1 (NODE 2 - REAR PASSENGER) RPM_THROTTLE_MOTTEMP
                        Sevcon1.Velocity = ConvertHexString(CANdata(0), CANdata(1), CANdata(2), CANdata(3))
                        Sevcon1.ThrottleInputVoltage = ConvertHexString(CANdata(4), CANdata(5))
                        Sevcon1.MotorTemp = ConvertHexString(CANdata(6), CANdata(7))
                        If Sevcon1.MotorTemp > MotorTempWarningLevel Then
                            RPMotorWarning = True
                        Else
                            RPMotorWarning = False
                        End If
                        If Sevcon1.MotorTemp > MotorTempErrorLevel Then
                            RPMotorError = True
                        Else
                            RPMotorError = False
                        End If
                        Sevcon1.TimeStamp = DateTime.Now
                    Case &H288.ToString("X2")   'Battery1 (NODE 8 - PASSENGER) PACK_CONFIG
                        Battery1.PackCapacityAh = ConvertHexString(CANdata(5), CANdata(6))
                    Case &H289.ToString("X2")   'Battery0 (NODE 9 - DRIVER) PACK_CONFIG
                        Battery0.PackCapacityAh = ConvertHexString(CANdata(5), CANdata(6))
                    Case &H28A.ToString("X2")   'CanIO0 (NODE A - REAR MODULE) ANALOG INPUTS
                        Dim rawAI1 As Integer
                        Dim rawAI2 As Integer
                        Dim rawAI3 As Integer
                        Dim rawAI4 As Integer
                        rawAI1 = ConvertHexString(CANdata(0), CANdata(1))
                        rawAI2 = ConvertHexString(CANdata(2), CANdata(3))
                        rawAI3 = ConvertHexString(CANdata(4), CANdata(5))
                        rawAI4 = ConvertHexString(CANdata(6), CANdata(7))
                        CanIO0.ReadAnalogInputs(rawAI1, rawAI2, rawAI3, rawAI4)
                        CanIO0.TimeStamp = DateTime.Now
                        currentFuelVoltage = CanIO0.AnalogInput1
                        fuelReading(currentFuelReading) = currentFuelVoltage
                        Dim myTemp As Single
                        myTemp = (CanIO0.AnalogInput2 * 54.75) - 94.75
                        If myTemp >= GeneratorThermometer.Range.Minimum And myTemp <= GeneratorThermometer.Range.Maximum Then
                            GeneratorThermometer.Enabled = True
                            GeneratorThermometer.Value = myTemp
                        Else
                            GeneratorThermometer.Enabled = False
                        End If

                        '2GeneratorTempLabel.Text = ((CanIO0.AnalogInput2 * 54.75) - 94.75).ToString("F1") & " F"
                        If currentFuelReading < 100 Then
                            currentFuelReading += 1
                        Else
                            currentFuelReading = 1
                            fuelReadingDataLooped = True
                        End If
                    Case &H30A.ToString("X2")   'CanIO0 (NODE A - REAR MODULE) ANALOG OUTPUTS
                    Case &H308.ToString("X2")   'Battery1 (NODE 8 - PASSENGER) PACK_STATUS

                    Case &H309.ToString("X2")   'Battery0 (NODE 9 - DRIVER) PACK_STATUS

                    Case &H381.ToString("X2")   'Sevcon0 (NODE 1 - REAR DRIVER) CONTROLLER_TEMP_DIGITALIN18
                        Sevcon0.BatteryVoltage = Convert.ToSingle((ConvertHexString(CANdata(0), CANdata(1))) * 0.0625)
                        Sevcon0.ControllerHeatsinkTemp = ConvertHexString(CANdata(2))
                        textPacket = Convert.ToSingle(ConvertHexString(CANdata(3), CANdata(4)))
                        If textPacket < 32767 Then
                            Sevcon0.BatteryCurrent = Convert.ToSingle(textPacket * 0.0625)
                        Else
                            Sevcon0.BatteryCurrent = Convert.ToSingle((65535 - textPacket) * -0.0625)
                        End If
                        Sevcon0.CapacitorVoltage = Convert.ToSingle((ConvertHexString(CANdata(5), CANdata(6))) * 0.0625)
                        Sevcon0.TimeStamp = DateTime.Now
                    Case &H382.ToString("X2")   'Sevcon1 (NODE 2 - REAR PASSENGER) CONTROLLER_TEMP_DIGITALIN18
                        Sevcon1.BatteryVoltage = Convert.ToSingle((ConvertHexString(CANdata(0), CANdata(1))) * 0.0625)
                        Sevcon1.ControllerHeatsinkTemp = ConvertHexString(CANdata(2))
                        textPacket = Convert.ToSingle(ConvertHexString(CANdata(3), CANdata(4)))
                        If textPacket < 32767 Then
                            Sevcon1.BatteryCurrent = Convert.ToSingle(textPacket * 0.0625)
                        Else
                            Sevcon1.BatteryCurrent = Convert.ToSingle((65535 - textPacket) * -0.0625)
                        End If
                        Sevcon1.CapacitorVoltage = Convert.ToSingle((ConvertHexString(CANdata(5), CANdata(6))) * 0.0625)
                        Sevcon1.TimeStamp = DateTime.Now
                    Case &H388.ToString("X2")   'Battery1 (NODE 8 - PASSENGER) CELL_VOLTAGE
                        If CanBusNum = 1 Then
                            Battery1.PackVoltage = Convert.ToSingle((ConvertHexString(CANdata(3), CANdata(4), CANdata(5), CANdata(6))) / 1000)
                        ElseIf CanBusNum = 2 Then
                            Battery2.PackVoltage = Convert.ToSingle((ConvertHexString(CANdata(3), CANdata(4), CANdata(5), CANdata(6))) / 1000)
                        End If
                        Battery1.TimeStamp = DateTime.Now
                    Case &H389.ToString("X2")   'Battery0 (NODE 9 - DRIVER) CELL_VOLTAGE
                        If CanBusNum = 1 Then
                            Battery0.PackVoltage = Convert.ToSingle((ConvertHexString(CANdata(3), CANdata(4), CANdata(5), CANdata(6))) / 1000)
                        ElseIf CanBusNum = 2 Then
                            Battery3.PackVoltage = Convert.ToSingle((ConvertHexString(CANdata(3), CANdata(4), CANdata(5), CANdata(6))) / 1000)
                        End If
                        Battery0.TimeStamp = DateTime.Now
                    Case &H38A.ToString("X2")   'CanIO0 (NODE A - REAR MODULE) PWR(+)
                        Dim rawAI As Integer
                        rawAI = ConvertHexString(CANdata(0), CANdata(1))
                        CanIO0.ReadPowerSupplyVoltage(rawAI)
                        CanIO0.TimeStamp = DateTime.Now
                        CanIO0.TimeStamp = DateTime.Now
                    Case &H408.ToString("X2")   'Battery1 (NODE 8 - PASSENGER) PACK_ACTIVE_DATA
                        Battery1.HighestPackTempC = ConvertHexString(CANdata(1))
                        Battery1.LowestPackTempC = ConvertHexString(CANdata(2))
                        Battery1.TimeStamp = DateTime.Now
                        textPacket = ConvertHexString(CANdata(3), CANdata(4))
                        If textPacket < 32767 Then
                            Battery1.PackDischageAmps = Convert.ToSingle(textPacket)
                        Else
                            Battery1.PackDischageAmps = Convert.ToSingle((65535 - textPacket) * -1)
                        End If
                        Battery1.PackCapacityRemainingAh = ConvertHexString(CANdata(5), CANdata(6))
                        Battery1.AllowedCurrentPercentage = ConvertHexString(CANdata(7))
                        Battery1.TimeStamp = DateTime.Now
                    Case &H409.ToString("X2")   'Battery0 (NODE 9 - DRIVER) PACK_ACTIVE_DATA
                        Battery0.HighestPackTempC = ConvertHexString(CANdata(1))
                        Battery0.LowestPackTempC = ConvertHexString(CANdata(2))
                        Battery0.TimeStamp = DateTime.Now
                        textPacket = ConvertHexString(CANdata(3), CANdata(4))
                        If textPacket < 32767 Then
                            Battery0.PackDischageAmps = Convert.ToSingle(textPacket)
                        Else
                            Battery0.PackDischageAmps = Convert.ToSingle((65535 - textPacket) * -1)
                        End If
                        Battery0.PackCapacityRemainingAh = ConvertHexString(CANdata(5), CANdata(6))
                        Battery0.AllowedCurrentPercentage = ConvertHexString(CANdata(7))
                        Battery0.TimeStamp = DateTime.Now
                    Case &H481.ToString("X2")   'Sevcon0 (NODE 1 - REAR DRIVER) VOLTMOD_INDUCTANCE_TEMPEST
                    Case &H482.ToString("X2")   'Sevcon1 (NODE 2 - REAR PASSENGER) VOLTMOD_INDUCTANCE_TEMPEST
                    Case &H488.ToString("X2")   'Battery1 (NODE 8 - PASSENGER) PACK_TEMP_DATA
                        If BatteryTempGroupBox.Visible = True And BatteryTempGroupBox.Text = "Passenger Battery Temperatures" Then
                            Select Case CANdata(0)
                                Case Is = 0
                                    Battery1.PackSensorTempC(0) = ConvertHexString(CANdata(1))
                                    Battery1.PackSensorStatus(0) = ConvertHexString(CANdata(5))
                                Case Is = 1
                                    Battery1.PackSensorTempC(1) = ConvertHexString(CANdata(1))
                                    Battery1.PackSensorStatus(1) = ConvertHexString(CANdata(5))
                                Case Is = 2
                                    Battery1.PackSensorTempC(2) = ConvertHexString(CANdata(1))
                                    Battery1.PackSensorStatus(2) = ConvertHexString(CANdata(5))
                                Case Is = 3
                                    Battery1.PackSensorTempC(3) = ConvertHexString(CANdata(1))
                                    Battery1.PackSensorStatus(3) = ConvertHexString(CANdata(5))
                                Case Is = 4
                                    Battery1.PackSensorTempC(4) = ConvertHexString(CANdata(1))
                                    Battery1.PackSensorStatus(4) = ConvertHexString(CANdata(5))
                                Case Is = 5
                                    Battery1.PackSensorTempC(5) = ConvertHexString(CANdata(1))
                                    Battery1.PackSensorStatus(5) = ConvertHexString(CANdata(5))
                                Case Is = 6
                                    Battery1.PackSensorTempC(6) = ConvertHexString(CANdata(1))
                                    Battery1.PackSensorStatus(6) = ConvertHexString(CANdata(5))
                                Case Is = 7
                                    Battery1.PackSensorTempC(7) = ConvertHexString(CANdata(1))
                                    Battery1.PackSensorStatus(7) = ConvertHexString(CANdata(5))
                            End Select
                        End If
                       
                    Case &H489.ToString("X2")   'Battery0 (NODE 9 - DRIVER) PACK_TEMP_DATA
                        If BatteryTempGroupBox.Visible = True And BatteryTempGroupBox.Text = "Driver Battery Temperatures" Then
                            Select Case CANdata(0)
                                Case Is = 0
                                    Battery0.PackSensorTempC(0) = ConvertHexString(CANdata(1))
                                    Battery0.PackSensorStatus(0) = ConvertHexString(CANdata(5))
                                Case Is = 1
                                    Battery0.PackSensorTempC(1) = ConvertHexString(CANdata(1))
                                    Battery0.PackSensorStatus(1) = ConvertHexString(CANdata(5))
                                Case Is = 2
                                    Battery0.PackSensorTempC(2) = ConvertHexString(CANdata(1))
                                    Battery0.PackSensorStatus(2) = ConvertHexString(CANdata(5))
                                Case Is = 3
                                    Battery0.PackSensorTempC(3) = ConvertHexString(CANdata(1))
                                    Battery0.PackSensorStatus(3) = ConvertHexString(CANdata(5))
                                Case Is = 4
                                    Battery0.PackSensorTempC(4) = ConvertHexString(CANdata(1))
                                    Battery0.PackSensorStatus(4) = ConvertHexString(CANdata(5))
                                Case Is = 5
                                    Battery0.PackSensorTempC(5) = ConvertHexString(CANdata(1))
                                    Battery0.PackSensorStatus(5) = ConvertHexString(CANdata(5))
                                Case Is = 6
                                    Battery0.PackSensorTempC(6) = ConvertHexString(CANdata(1))
                                    Battery0.PackSensorStatus(6) = ConvertHexString(CANdata(5))
                                Case Is = 7
                                    Battery0.PackSensorTempC(7) = ConvertHexString(CANdata(1))
                                    Battery0.PackSensorStatus(7) = ConvertHexString(CANdata(5))
                            End Select
                        End If
                    Case &H48A.ToString("X2")   'CanIO0 (NODE A - REAR MODULE) DIGITAL INPUT EDGE EVENTS

                    Case &H501.ToString("X2")   'Sevcon0 (NODE 1 - REAR DRIVER) TARGETVEL_MAXIQ_CONTVOLT
                        textPacket = ConvertHexString(CANdata(0), CANdata(1))
                        If textPacket < 32767 Then
                            Sevcon0.Torque = Convert.ToSingle(textPacket * 0.0625)
                        Else
                            Sevcon0.Torque = Convert.ToSingle((65535 - textPacket) * -0.0625)
                        End If
                        ' CANdata(2), CANdata(3))
                        Sevcon0.MaximumCurrentAllowed = ConvertHexString(CANdata(4), CANdata(5))
                        Sevcon0.LineContactorVoltage = ConvertHexString(CANdata(6), CANdata(7))
                        If Sevcon0.LineContactorVoltage > 0 Then
                            Sevcon0.ContactorClosed = True
                        Else
                            Sevcon0.ContactorClosed = False
                        End If
                        'Debug.Print("Rear Driver Line Contactor Closed = " & Sevcon0.ContactorClosed)
                        Sevcon0.TimeStamp = DateTime.Now
                    Case &H502.ToString("X2")   'Sevcon1 (NODE 2 - REAR PASSENGER) TARGETVEL_MAXIQ_CONTVOLT
                        textPacket = ConvertHexString(CANdata(0), CANdata(1))
                        If textPacket < 32767 Then
                            Sevcon1.Torque = Convert.ToSingle(textPacket * 0.0625)
                        Else
                            Sevcon1.Torque = Convert.ToSingle((65535 - textPacket) * -0.0625)
                        End If
                        ' CANdata(2), CANdata(3))
                        Sevcon1.MaximumCurrentAllowed = ConvertHexString(CANdata(4), CANdata(5))
                        Sevcon1.LineContactorVoltage = ConvertHexString(CANdata(6), CANdata(7))
                        If Sevcon1.LineContactorVoltage > 0 Then
                            Sevcon1.ContactorClosed = True
                        Else
                            Sevcon1.ContactorClosed = False
                        End If
                        'Debug.Print("Rear Passenger Line Contactor Closed = " & Sevcon1.ContactorClosed)
                        Sevcon1.TimeStamp = DateTime.Now
                    Case &H506.ToString("X2")   'BMS RPDO CONTROL
                    Case &H508.ToString("X2")   'Battery1 (NODE 8 - PASSENGER) PACK_TIME
                        Dim tempInt As Integer
                        tempInt = ConvertHexString(CANdata(4), CANdata(5))
                        Battery1.MaxChargeCRate = tempInt / 10
                        tempInt = ConvertHexString(CANdata(6), CANdata(7))
                        Battery1.MaxDischargeCRate = tempInt / 10
                    Case &H509.ToString("X2")   'Battery0 (NODE 9 - DRIVER) PACK_TIME
                        Dim tempInt As Integer
                        tempInt = ConvertHexString(CANdata(4), CANdata(5))
                        Battery0.MaxChargeCRate = tempInt / 10
                        tempInt = ConvertHexString(CANdata(6), CANdata(7))
                        Battery0.MaxDischargeCRate = tempInt / 10
                    Case &H581.ToString("X2")   'Sevcon0 (NODE 1 - REAR DRIVER) SEVCON SDO REPLY
                        Dim tempInt As Integer
                        tempInt = ConvertHexString(CANdata(1), CANdata(2))
                        Select Case tempInt
                            Case Is = 10113                     '&H2781
                                If CANdata(3) = 1 Then          'Key On Hours
                                    Sevcon0.KeyOnHours = ConvertHexString(CANdata(4), CANdata(5))
                                ElseIf CANdata(3) = 2 Then      'Key On Minutes (.25 minutes)
                                    Sevcon0.KeyOnMinutes = (ConvertHexString(CANdata(4))) * 0.25
                                End If
                            Case Is = 10114                     '&H2782
                                If CANdata(3) = 1 Then          'Traction Hours
                                    Sevcon0.TractionHours = ConvertHexString(CANdata(4), CANdata(5))
                                ElseIf CANdata(3) = 2 Then      'Traction Minutes (.25 minutes)
                                    Sevcon0.TractionMinutes = (ConvertHexString(CANdata(4))) * 0.25
                                End If
                            Case Is = 10656                     '&H29A0
                                If CANdata(3) = 3 Then          'Odometer (0.1 miles)
                                    Sevcon0.OdometerTenthMiles = ConvertHexString(CANdata(4), CANdata(5), CANdata(6), CANdata(7))
                                End If
                            Case Is = 10657                     '&H29A1
                                If CANdata(3) = 3 Then
                                    Sevcon0.TripTenthMiles = CANdata(4)
                                End If
                        End Select
                    Case &H582.ToString("X2")   'Sevcon1 (NODE 2 - REAR PASSENGER) SEVCON SDO REPLY
                        Dim tempInt As Integer
                        tempInt = ConvertHexString(CANdata(1), CANdata(2))
                        Select Case tempInt
                            Case Is = 10113                     '&H2781
                                If CANdata(3) = 1 Then          'Key On Hours
                                    Sevcon1.KeyOnHours = ConvertHexString(CANdata(4), CANdata(5))
                                ElseIf CANdata(3) = 2 Then      'Key On Minutes (.25 minutes)
                                    Sevcon1.KeyOnMinutes = (ConvertHexString(CANdata(4))) * 0.25
                                End If
                            Case Is = 10114                     '&H2782
                                If CANdata(3) = 1 Then          'Traction Hours
                                    Sevcon1.TractionHours = ConvertHexString(CANdata(4), CANdata(5))
                                ElseIf CANdata(3) = 2 Then      'Traction Minutes (.25 minutes)
                                    Sevcon1.TractionMinutes = (ConvertHexString(CANdata(4))) * 0.25
                                End If
                            Case Is = 10656                     '&H29A0
                                If CANdata(3) = 3 Then          'Odometer (0.1 miles)
                                    Sevcon1.OdometerTenthMiles = ConvertHexString(CANdata(4), CANdata(5), CANdata(6), CANdata(7))
                                End If
                        End Select
                    Case &H601.ToString("X2")   'Sevcon SDO REQUEST
                    Case &H701.ToString("X2")   'Sevcon0 (NODE 1 - REAR DRIVER) HEARTBEAT MESSAGE
                    Case &H702.ToString("X2")   'Sevcon1 (NODE 2 - REAR PASSENGER) HEARTBEAT MESSAGE
                    Case &H70A.ToString("X2")   'CanIO0 (NODE A - REAR MODULE) HEARTBEAT MESSAGE
                End Select
            ElseIf CanBusNum = 2 Then     'Front Sevcons, front CanIO, middle battery, & spare battery
                Select Case CobId
                    Case &H181.ToString("X2")   'Sevcon2 (NODE 1 - FRONT DRIVER) DIGITAL INPUTS
                        Sevcon2.DigitalInputs = CANdata(2)
                        Sevcon2.TimeStamp = DateTime.Now
                    Case &H182.ToString("X2")   'Sevcon3 (NODE 2 - FRONT PASSENGER) DIGITAL INPUTS
                        Sevcon3.DigitalInputs = CANdata(2)
                        Sevcon3.TimeStamp = DateTime.Now
                    Case &H183.ToString("X2")   'Sevcon4 (NODE 3 - GENERATOR) DIGITAL INPUTS
                        Sevcon4.DigitalInputs = CANdata(2)
                        Sevcon4.TimeStamp = DateTime.Now
                    Case &H188.ToString("X2")   'Battery3 (NODE 8 - SPARE) PACK_STATUS
                        Battery3.SoC = ConvertHexString(CANdata(0))
                        Battery3.BmsStatusBits = ConvertHexString(CANdata(1), CANdata(2))
                        Battery3.TimeStamp180 = DateTime.Now
                        Battery3.TimeStamp = DateTime.Now
                    Case &H189.ToString("X2")   'Battery2 (NODE 9 - MIDDLE) PACK_STATUS
                        Battery2.SoC = ConvertHexString(CANdata(0))
                        Battery2.BmsStatusBits = ConvertHexString(CANdata(1), CANdata(2))
                        Battery2.TimeStamp180 = DateTime.Now
                        Battery2.TimeStamp = DateTime.Now
                    Case &H18A.ToString("X2")   'CanIO1 (NODE A - FRONT MODULE) DIGITAL INPUTS
                        CanIO1.ReadDigitalInputs(CANdata(0), CANdata(1))
                        If vehicleControlStatus = "Tablet" Then
                            If CanIO1.DigitalInput1 = True Then         '//Forward Switch

                            Else

                            End If
                            If CanIO1.DigitalInput2 = True Then         '//Reverse Switch

                            Else

                            End If
                            If CanIO1.DigitalInput3 = True Then         '//Brake Light Switch
                                CanIO1.DigitalOutput4 = True            '//Brake Light Relay Coil
                                brakeLightsLed.Value = True
                            Else
                                CanIO1.DigitalOutput4 = False
                                brakeLightsLed.Value = False
                            End If
                            If CanIO1.DigitalInput4 = True Then         '//Headlight Switch
                                CanIO1.DigitalOutput3 = True            '//Low Beam Headlight Relay Coil
                                runningLightsLed.Value = True
                            Else
                                CanIO1.DigitalOutput3 = False
                                runningLightsLed.Value = False
                            End If
                            CanIO1.SetDigitalOutputs()
                            SendCanMessage("Digital Outputs", 2, 0)
                        End If
                        CanIO1.TimeStamp180 = DateTime.Now
                        CanIO1.TimeStamp = DateTime.Now
                    Case &H281.ToString("X2")   'Sevcon2 (NODE 1 - FRONT DRIVER) RPM_THROTTLE_MOTTEMP
                        Sevcon2.Velocity = ConvertHexString(CANdata(0), CANdata(1), CANdata(2), CANdata(3))
                        Sevcon2.ThrottleInputVoltage = ConvertHexString(CANdata(4), CANdata(5))
                        Sevcon2.MotorTemp = ConvertHexString(CANdata(6), CANdata(7))
                        If Sevcon2.MotorTemp > MotorTempWarningLevel Then
                            FDMotorWarning = True
                        Else
                            FDMotorWarning = False
                        End If
                        If Sevcon2.MotorTemp > MotorTempErrorLevel Then
                            FDMotorError = True
                        Else
                            FDMotorError = False
                        End If
                        Sevcon2.TimeStamp = DateTime.Now
                    Case &H282.ToString("X2")   'Sevcon3 (NODE 2 - FRONT PASSENGER) RPM_THROTTLE_MOTTEMP
                        Sevcon3.Velocity = ConvertHexString(CANdata(0), CANdata(1), CANdata(2), CANdata(3))
                        Sevcon3.ThrottleInputVoltage = ConvertHexString(CANdata(4), CANdata(5))
                        Sevcon3.MotorTemp = ConvertHexString(CANdata(6), CANdata(7))
                        If Sevcon3.MotorTemp > MotorTempWarningLevel Then
                            FPMotorWarning = True
                        Else
                            FPMotorWarning = False
                        End If
                        If Sevcon3.MotorTemp > MotorTempErrorLevel Then
                            FPMotorError = True
                        Else
                            FPMotorError = False
                        End If
                        Sevcon3.TimeStamp = DateTime.Now
                    Case &H283.ToString("X2")   'Sevcon4 (NODE 3 - GENERATOR) RPM_THROTTLE_MOTTEMP
                        Sevcon4.Velocity = ConvertHexString(CANdata(0), CANdata(1), CANdata(2), CANdata(3))
                        If Sevcon4.Velocity >= 0 And Sevcon4.Velocity <= 3000 Then
                            If CanIO1.DigitalOutput2 = True Then
                                generatorEngineStarting = True
                                generatorEngineRunning = False
                                generatorEngineQuitting = False
                            Else
                                generatorEngineStarting = False
                                generatorEngineRunning = False
                                generatorEngineQuitting = True
                            End If
                            If Sevcon4.Velocity >= 0 And Sevcon4.Velocity <= 10 And CanIO1.DigitalOutput2 = False Then
                                generatorEngineStarting = False
                                generatorEngineRunning = False
                                generatorEngineQuitting = False
                            End If
                        ElseIf Sevcon4.Velocity >= 3050 Then
                            generatorEngineStarting = False
                            generatorEngineRunning = True
                            generatorEngineQuitting = False
                        End If
                        Sevcon4.ThrottleInputVoltage = ConvertHexString(CANdata(4), CANdata(5))
                        Sevcon4.MotorTemp = ConvertHexString(CANdata(6), CANdata(7))
                        If Sevcon4.MotorTemp > MotorTempWarningLevel Then
                            GenMotorWarning = True
                        Else
                            GenMotorWarning = False
                        End If
                        If Sevcon4.MotorTemp > MotorTempErrorLevel Then
                            GenMotorError = True
                        Else
                            GenMotorError = False
                        End If
                        Sevcon4.TimeStamp = DateTime.Now
                    Case &H288.ToString("X2")   'Battery3 (NODE 8 - SPARE) PACK_CONFIG
                        Battery3.PackCapacityAh = ConvertHexString(CANdata(5), CANdata(6))
                    Case &H289.ToString("X2")   'Battery2 (NODE 9 - MIDDLE) PACK_CONFIG
                        Battery2.PackCapacityAh = ConvertHexString(CANdata(5), CANdata(6))
                    Case &H28A.ToString("X2")   'CanIO1 (NODE A - FRONT MODULE) ANALOG INPUTS
                        Dim rawAI1, rawAI2, rawAI3, rawAI4 As Integer
                        rawAI1 = ConvertHexString(CANdata(0), CANdata(1))
                        rawAI2 = ConvertHexString(CANdata(2), CANdata(3))
                        rawAI3 = ConvertHexString(CANdata(4), CANdata(5))
                        rawAI4 = ConvertHexString(CANdata(6), CANdata(7))
                        CanIO1.ReadAnalogInputs(rawAI1, rawAI2, rawAI3, rawAI4)
                        CanIO1.TimeStamp280 = DateTime.Now
                        CanIO1.TimeStamp = DateTime.Now

                        'Case &H308.ToString("X2")   'Battery3 (NODE 8 - SPARE) PACK_STATUS
                        'Case &H309.ToString("X2")   'Battery2 (NODE 9 - MIDDLE) PACK_STATUS
                    Case &H381.ToString("X2")   'Sevcon2 (NODE 1 - FRONT DRIVER) CONTROLLER_TEMP_DIGITALIN18
                        Sevcon2.BatteryVoltage = Convert.ToSingle((ConvertHexString(CANdata(0), CANdata(1))) * 0.0625)
                        Sevcon2.ControllerHeatsinkTemp = ConvertHexString(CANdata(2))
                        textPacket = Convert.ToSingle(ConvertHexString(CANdata(3), CANdata(4)))
                        If textPacket < 32767 Then
                            Sevcon2.BatteryCurrent = Convert.ToSingle(textPacket * 0.0625)
                        Else
                            Sevcon2.BatteryCurrent = Convert.ToSingle((65535 - textPacket) * -0.0625)
                        End If
                        Sevcon2.CapacitorVoltage = Convert.ToSingle((ConvertHexString(CANdata(5), CANdata(6))) * 0.0625)
                        Sevcon2.TimeStamp = DateTime.Now
                    Case &H382.ToString("X2")   'Sevcon3 (NODE 2 - FRONT PASSENGER) CONTROLLER_TEMP_DIGITALIN18
                        Sevcon3.BatteryVoltage = Convert.ToSingle((ConvertHexString(CANdata(0), CANdata(1))) * 0.0625)
                        Sevcon3.ControllerHeatsinkTemp = ConvertHexString(CANdata(2))
                        textPacket = Convert.ToSingle(ConvertHexString(CANdata(3), CANdata(4)))
                        If textPacket < 32767 Then
                            Sevcon3.BatteryCurrent = Convert.ToSingle(textPacket * 0.0625)
                        Else
                            Sevcon3.BatteryCurrent = Convert.ToSingle((65535 - textPacket) * -0.0625)
                        End If
                        Sevcon3.CapacitorVoltage = Convert.ToSingle((ConvertHexString(CANdata(5), CANdata(6))) * 0.0625)
                        Sevcon3.DigitalInputs = CANdata(7)
                        Sevcon3.TimeStamp = DateTime.Now
                    Case &H383.ToString("X2")   'Sevcon4 (NODE 3 - GENERATOR) CONTROLLER_TEMP_DIGITALIN18
                        Sevcon4.BatteryVoltage = Convert.ToSingle((ConvertHexString(CANdata(0), CANdata(1))) * 0.0625)
                        Sevcon4.ControllerHeatsinkTemp = ConvertHexString(CANdata(2))
                        textPacket = Convert.ToSingle(ConvertHexString(CANdata(3), CANdata(4)))
                        If textPacket < 32767 Then
                            Sevcon4.BatteryCurrent = Convert.ToSingle(textPacket * 0.0625)
                        Else
                            Sevcon4.BatteryCurrent = Convert.ToSingle((65535 - textPacket) * -0.0625)
                        End If
                        Sevcon4.CapacitorVoltage = Convert.ToSingle((ConvertHexString(CANdata(5), CANdata(6))) * 0.0625)
                        Sevcon4.DigitalInputs = CANdata(7)
                        Sevcon4.TimeStamp = DateTime.Now
                    Case &H388.ToString("X2")   'Battery3 (NODE 8 - SPARE) CELL_VOLTAGE
                        Battery3.PackVoltage = Convert.ToSingle((ConvertHexString(CANdata(3), CANdata(4), CANdata(5), CANdata(6))) / 1000)
                        Battery3.TimeStamp380 = DateTime.Now
                        Battery3.TimeStamp = DateTime.Now
                    Case &H389.ToString("X2")   'Battery2 (NODE 9 - MIDDLE) CELL_VOLTAGE
                        Battery2.PackVoltage = Convert.ToSingle((ConvertHexString(CANdata(3), CANdata(4), CANdata(5), CANdata(6))) / 1000)
                        Battery2.TimeStamp380 = DateTime.Now
                        Battery2.TimeStamp = DateTime.Now
                    Case &H38A.ToString("X2")   'CanIO1 (NODE A - FRONT MODULE) PWR(+)
                        Dim rawAI As Integer
                        rawAI = ConvertHexString(CANdata(0), CANdata(1))
                        CanIO1.ReadPowerSupplyVoltage(rawAI)
                        CanIO1.TimeStamp380 = DateTime.Now
                        CanIO1.TimeStamp = DateTime.Now
                    Case &H408.ToString("X2")   'Battery3 (NODE 8 - SPARE) PACK_ACTIVE_DATA
                        Battery3.HighestPackTempC = ConvertHexString(CANdata(1))
                        Battery3.LowestPackTempC = ConvertHexString(CANdata(2))
                        Battery3.TimeStamp400 = DateTime.Now
                        Battery3.TimeStamp = DateTime.Now
                        textPacket = ConvertHexString(CANdata(3), CANdata(4))
                        If textPacket < 32767 Then
                            Battery3.PackDischageAmps = Convert.ToSingle(textPacket)
                        Else
                            Battery3.PackDischageAmps = Convert.ToSingle((65535 - textPacket) * -1)
                        End If
                        Battery3.PackCapacityRemainingAh = ConvertHexString(CANdata(5), CANdata(6))
                        Battery3.AllowedCurrentPercentage = ConvertHexString(CANdata(7))
                    Case &H409.ToString("X2")   'Battery2 (NODE 9 - MIDDLE) PACK_ACTIVE_DATA
                        Battery2.HighestPackTempC = ConvertHexString(CANdata(1))
                        Battery2.LowestPackTempC = ConvertHexString(CANdata(2))
                        Battery2.TimeStamp400 = DateTime.Now
                        Battery2.TimeStamp = DateTime.Now
                        textPacket = ConvertHexString(CANdata(3), CANdata(4))
                        If textPacket < 32767 Then
                            Battery2.PackDischageAmps = Convert.ToSingle(textPacket)
                        Else
                            Battery2.PackDischageAmps = Convert.ToSingle((65535 - textPacket) * -1)
                        End If
                        Battery2.PackCapacityRemainingAh = ConvertHexString(CANdata(5), CANdata(6))
                        Battery2.AllowedCurrentPercentage = ConvertHexString(CANdata(7))
                        'Case &H481.ToString("X2")   'Sevcon2 (NODE 1 - FRONT DRIVER) VOLTMOD_INDUCTANCE_TEMPEST
                        'Case &H482.ToString("X2")   'Sevcon3 (NODE 2 - FRONT PASSENGER) VOLTMOD_INDUCTANCE_TEMPEST
                        'Case &H488.ToString("X2")   'Battery3 (NODE 8 - SPARE) PACK_TEMP_DATA
                    Case &H489.ToString("X2")   'Battery2 (NODE 9 - MIDDLE) PACK_TEMP_DATA
                        If BatteryTempGroupBox.Visible = True And BatteryTempGroupBox.Text = "Middle Battery Temperatures" Then
                            Select Case CANdata(0)
                                Case Is = 0
                                    Battery2.PackSensorTempC(0) = ConvertHexString(CANdata(1))
                                    Battery2.PackSensorStatus(0) = ConvertHexString(CANdata(5))
                                Case Is = 1
                                    Battery2.PackSensorTempC(1) = ConvertHexString(CANdata(1))
                                    Battery2.PackSensorStatus(1) = ConvertHexString(CANdata(5))
                                Case Is = 2
                                    Battery2.PackSensorTempC(2) = ConvertHexString(CANdata(1))
                                    Battery2.PackSensorStatus(2) = ConvertHexString(CANdata(5))
                                Case Is = 3
                                    Battery2.PackSensorTempC(3) = ConvertHexString(CANdata(1))
                                    Battery2.PackSensorStatus(3) = ConvertHexString(CANdata(5))
                                Case Is = 4
                                    Battery2.PackSensorTempC(4) = ConvertHexString(CANdata(1))
                                    Battery2.PackSensorStatus(4) = ConvertHexString(CANdata(5))
                                Case Is = 5
                                    Battery2.PackSensorTempC(5) = ConvertHexString(CANdata(1))
                                    Battery2.PackSensorStatus(5) = ConvertHexString(CANdata(5))
                                Case Is = 6
                                    Battery2.PackSensorTempC(6) = ConvertHexString(CANdata(1))
                                    Battery2.PackSensorStatus(6) = ConvertHexString(CANdata(5))
                                Case Is = 7
                                    Battery2.PackSensorTempC(7) = ConvertHexString(CANdata(1))
                                    Battery2.PackSensorStatus(7) = ConvertHexString(CANdata(5))
                            End Select
                        End If
                        'Case &H48A.ToString("X2")   'CanIO1 (NODE A - FRONT MODULE) DIGITAL INPUT EDGE EVENTS
                    Case &H501.ToString("X2")   'Sevcon2 (NODE 1 - FRONT DRIVER) TARGETVEL_MAXIQ_CONTVOLT
                        textPacket = ConvertHexString(CANdata(0), CANdata(1))
                        If textPacket < 32767 Then
                            Sevcon2.Torque = Convert.ToSingle(textPacket * 0.0625)
                        Else
                            Sevcon2.Torque = Convert.ToSingle((65535 - textPacket) * -0.0625)
                        End If
                        ' CANdata(2), CANdata(3))
                        Sevcon2.MaximumCurrentAllowed = ConvertHexString(CANdata(4), CANdata(5))
                        Sevcon2.LineContactorVoltage = ConvertHexString(CANdata(6), CANdata(7))
                        If Sevcon2.LineContactorVoltage > 0 Then
                            Sevcon2.ContactorClosed = True
                        Else
                            Sevcon2.ContactorClosed = False
                        End If
                        Sevcon2.TimeStamp = DateTime.Now
                    Case &H502.ToString("X2")   'Sevcon3 (NODE 2 - FRONT PASSENGER) TARGETVEL_MAXIQ_CONTVOLT
                        textPacket = ConvertHexString(CANdata(0), CANdata(1))
                        If textPacket < 32767 Then
                            Sevcon3.Torque = Convert.ToSingle(textPacket * 0.0625)
                        Else
                            Sevcon3.Torque = Convert.ToSingle((65535 - textPacket) * -0.0625)
                        End If
                        'CANdata(2), CANdata(3))
                        Sevcon3.MaximumCurrentAllowed = ConvertHexString(CANdata(4), CANdata(5))
                        Sevcon3.LineContactorVoltage = ConvertHexString(CANdata(6), CANdata(7))
                        If Sevcon3.LineContactorVoltage > 0 Then
                            Sevcon3.ContactorClosed = True
                        Else
                            Sevcon3.ContactorClosed = False
                        End If
                    Case &H503.ToString("X2")   'Sevcon4 (NODE 3 - GENERATOR) TARGETVEL_MAXIQ_CONTVOLT
                        textPacket = ConvertHexString(CANdata(0), CANdata(1))
                        If textPacket < 32767 Then
                            Sevcon4.Torque = Convert.ToSingle(textPacket * 0.0625)
                        Else
                            Sevcon4.Torque = Convert.ToSingle((65535 - textPacket) * -0.0625)
                        End If
                        ' CANdata(2), CANdata(3))
                        Sevcon4.MaximumCurrentAllowed = ConvertHexString(CANdata(4), CANdata(5))
                        Sevcon4.LineContactorVoltage = ConvertHexString(CANdata(6), CANdata(7))
                        If Sevcon4.LineContactorVoltage > 0 Then
                            Sevcon4.ContactorClosed = True
                        Else
                            Sevcon4.ContactorClosed = False
                        End If
                        Sevcon4.TimeStamp = DateTime.Now
                        'Case &H506.ToString("X2")   'BMS RPDO CONTROL
                    Case &H508.ToString("X2")   'Battery3 (NODE 8 - SPARE) PACK_TIME
                        Dim tempInt As Integer
                        tempInt = ConvertHexString(CANdata(4), CANdata(5))
                        Battery3.MaxChargeCRate = tempInt / 10
                        tempInt = ConvertHexString(CANdata(6), CANdata(7))
                        Battery3.MaxDischargeCRate = tempInt / 10
                    Case &H509.ToString("X2")   'Battery2 (NODE 9 - MIDDLE) PACK_TIME
                        Dim tempInt As Integer
                        tempInt = ConvertHexString(CANdata(4), CANdata(5))
                        Battery2.MaxChargeCRate = tempInt / 10
                        tempInt = ConvertHexString(CANdata(6), CANdata(7))
                        Battery2.MaxDischargeCRate = tempInt / 10
                    Case &H581.ToString("X2")   'Sevcon2 (NODE 1 - FRONT DRIVER) SEVCON SDO REPLY
                        Dim tempInt As Integer
                        tempInt = ConvertHexString(CANdata(1), CANdata(2))
                        Select Case tempInt
                            Case Is = 10113                     '&H2781
                                If CANdata(3) = 1 Then          'Key On Hours
                                    Sevcon2.KeyOnHours = ConvertHexString(CANdata(4), CANdata(5))
                                ElseIf CANdata(3) = 2 Then      'Key On Minutes (.25 minutes)
                                    Sevcon2.KeyOnMinutes = (ConvertHexString(CANdata(4))) * 0.25
                                End If
                            Case Is = 10114                     '&H2782
                                If CANdata(3) = 1 Then          'Traction Hours
                                    Sevcon2.TractionHours = ConvertHexString(CANdata(4), CANdata(5))
                                ElseIf CANdata(3) = 2 Then      'Traction Minutes (.25 minutes)
                                    Sevcon2.TractionMinutes = (ConvertHexString(CANdata(4))) * 0.25
                                End If
                            Case Is = 10656                     '&H29A0
                                If CANdata(3) = 3 Then          'Odometer (0.1 miles)
                                    Sevcon2.OdometerTenthMiles = ConvertHexString(CANdata(4), CANdata(5), CANdata(6), CANdata(7))
                                End If
                        End Select
                    Case &H582.ToString("X2")   'Sevcon3 (NODE 2 - FRONT PASSENGER) SEVCON SDO REPLY
                        Dim tempInt As Integer
                        tempInt = ConvertHexString(CANdata(1), CANdata(2))
                        Select Case tempInt
                            Case Is = 10113                     '&H2781
                                If CANdata(3) = 1 Then          'Key On Hours
                                    Sevcon3.KeyOnHours = ConvertHexString(CANdata(4), CANdata(5))
                                ElseIf CANdata(3) = 2 Then      'Key On Minutes (.25 minutes)
                                    Sevcon3.KeyOnMinutes = (ConvertHexString(CANdata(4))) * 0.25
                                End If
                            Case Is = 10114                     '&H2782
                                If CANdata(3) = 1 Then          'Traction Hours
                                    Sevcon3.TractionHours = ConvertHexString(CANdata(4), CANdata(5))
                                ElseIf CANdata(3) = 2 Then      'Traction Minutes (.25 minutes)
                                    Sevcon3.TractionMinutes = (ConvertHexString(CANdata(4))) * 0.25
                                End If
                            Case Is = 10656                     '&H29A0
                                If CANdata(3) = 3 Then          'Odometer (0.1 miles)
                                    Sevcon3.OdometerTenthMiles = ConvertHexString(CANdata(4), CANdata(5), CANdata(6), CANdata(7))
                                End If
                        End Select
                    Case &H583.ToString("X2")   'Sevcon4 (NODE 3 - GENERATOR) SEVCON SDO REPLY
                        Dim tempInt As Integer
                        tempInt = ConvertHexString(CANdata(1), CANdata(2))
                        Select Case tempInt
                            Case Is = 10113                     '&H2781
                                If CANdata(3) = 1 Then          'Key On Hours
                                    Sevcon4.KeyOnHours = ConvertHexString(CANdata(4), CANdata(5))
                                ElseIf CANdata(3) = 2 Then      'Key On Minutes (.25 minutes)
                                    Sevcon4.KeyOnMinutes = (ConvertHexString(CANdata(4))) * 0.25
                                End If
                            Case Is = 10114                     '&H2782
                                If CANdata(3) = 1 Then          'Traction Hours
                                    Sevcon4.TractionHours = ConvertHexString(CANdata(4), CANdata(5))
                                ElseIf CANdata(3) = 2 Then      'Traction Minutes (.25 minutes)
                                    Sevcon4.TractionMinutes = (ConvertHexString(CANdata(4))) * 0.25
                                End If
                            Case Is = 10656                     '&H29A0
                                If CANdata(3) = 3 Then          'Odometer (0.1 miles)
                                    Sevcon4.OdometerTenthMiles = ConvertHexString(CANdata(4), CANdata(5), CANdata(6), CANdata(7))
                                End If
                        End Select
                        'Case &H601.ToString("X2")   'Sevcon SDO REQUEST
                        'Case &H701.ToString("X2")
                End Select
            End If


            textLine = "Time: " + canMessage.TimeStamp.ToString + " ID: " + canMessage.Identifier.ToString("X3") + "h"

            If canMessage.RemoteTransmissionRequest Then
                textLine = textLine + "Remote Request Data Length: " + canMessage.DataLength.ToString()
            Else
                Dim i As Byte
                For i = 1 To canMessage.DataLength
                    textLine = textLine + " " + canMessage.Item(i - 1).ToString("X2")
                Next
            End If

            If canMessage.SelfReceptionRequest Then
                textLine = textLine + " Self Reception"
            End If

            'Set the text thread safe to the label
            If CanBusNum = 1 Then
                SetText1(textLine)
            ElseIf CanBusNum = 2 Then
                SetText2(textLine)
            End If
            'Next



        Catch ex As Exception
            'Dim myMessageBoxResponse As MsgBoxResult
            'myMessageBoxResponse = MessageBox.Show(ex.ToString, "CAN Parsing Error", MessageBoxButtons.OK)
            sw.WriteLine("ERROR - ShowDataMessage_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
        End Try

    End Sub
    Private Sub SendCanMessage(ByVal Type As String, ByVal CanBusNum As Integer, ByVal Node As Integer)

        Try
            Dim myInt As Integer = 0
            If CanBusNum = 1 Then
                If mWriter1 IsNot Nothing Then
                    Dim myCanMessage As Ixxat.Vci4.Bal.Can.CanMessage

                    Select Case Type
                        Case Is = "Query Main Odometer"
                            With myCanMessage
                                .TimeStamp = 0                                      '//No delayed transmission
                                If Node = 1 Then
                                    .Identifier = 1537                              '//&H601 - Sevcon SDO Request
                                Else
                                    .Identifier = 1538                              '//&H602 - Sevcon SDO Request
                                End If
                                .FrameType = Bal.Can.CanMsgFrameType.Data
                                .DataLength = 8
                                .ExtendedFrameFormat = False                        '//Standard Frame
                                .SelfReceptionRequest = False                       '//Self-Reception

                                .Item(0) = 79                                       '//&H4F
                                .Item(1) = 160                                      '//Register LSB - &HA0
                                .Item(2) = 41                                       '//Register MSB - &H29
                                .Item(3) = 3                                        '//Register Index - &H03
                                .Item(4) = 0
                                .Item(5) = 0
                                .Item(6) = 0
                                .Item(7) = 0
                            End With

                        Case Is = "Query Trip Odometer"
                            With myCanMessage
                                .TimeStamp = 0                                      '//No delayed transmission
                                If Node = 1 Then
                                    .Identifier = 1537                              '//&H601 - Sevcon SDO Request
                                Else
                                    .Identifier = 1538                              '//&H602 - Sevcon SDO Request
                                End If
                                .FrameType = Bal.Can.CanMsgFrameType.Data
                                .DataLength = 8
                                .ExtendedFrameFormat = False                        '//Standard Frame
                                .SelfReceptionRequest = False                       '//Self-Reception

                                .Item(0) = 79                                       '//&H4F
                                .Item(1) = 161                                      '//Register LSB - &HA1
                                .Item(2) = 41                                       '//Register MSB - &H29
                                .Item(3) = 3                                        '//Register Index - &H03
                                .Item(4) = 0
                                .Item(5) = 0
                                .Item(6) = 0
                                .Item(7) = 0
                            End With

                        Case Is = "Query Key On Hours"
                            With myCanMessage
                                .TimeStamp = 0                                      '//No delayed transmission
                                If Node = 1 Then
                                    .Identifier = 1537                              '//&H601 - Sevcon SDO Request
                                Else
                                    .Identifier = 1538                              '//&H602 - Sevcon SDO Request
                                End If
                                .FrameType = Bal.Can.CanMsgFrameType.Data
                                .DataLength = 8
                                .ExtendedFrameFormat = False                        '//Standard Frame
                                .SelfReceptionRequest = False                       '//Self-Reception

                                .Item(0) = 79                                       '//&H4F
                                .Item(1) = 129                                      '//Register LSB - &H81
                                .Item(2) = 39                                       '//Register MSB - &H27
                                .Item(3) = 1                                        '//Register Index - &H01
                                .Item(4) = 0
                                .Item(5) = 0
                                .Item(6) = 0
                                .Item(7) = 0
                            End With

                        Case Is = "Query Key On Minutes"
                            With myCanMessage
                                .TimeStamp = 0                                      '//No delayed transmission
                                If Node = 1 Then
                                    .Identifier = 1537                              '//&H601 - Sevcon SDO Request
                                Else
                                    .Identifier = 1538                              '//&H602 - Sevcon SDO Request
                                End If
                                .FrameType = Bal.Can.CanMsgFrameType.Data
                                .DataLength = 8
                                .ExtendedFrameFormat = False                        '//Standard Frame
                                .SelfReceptionRequest = False                       '//Self-Reception

                                .Item(0) = 79                                       '//&H4F
                                .Item(1) = 129                                      '//Register LSB - &H81
                                .Item(2) = 39                                       '//Register MSB - &H27
                                .Item(3) = 2                                        '//Register Index - &H02
                                .Item(4) = 0
                                .Item(5) = 0
                                .Item(6) = 0
                                .Item(7) = 0
                            End With

                        Case Is = "Query Traction Hours"
                            With myCanMessage
                                .TimeStamp = 0                                      '//No delayed transmission
                                If Node = 1 Then
                                    .Identifier = 1537                              '//&H601 - Sevcon SDO Request
                                Else
                                    .Identifier = 1538                              '//&H602 - Sevcon SDO Request
                                End If
                                .FrameType = Bal.Can.CanMsgFrameType.Data
                                .DataLength = 8
                                .ExtendedFrameFormat = False                        '//Standard Frame
                                .SelfReceptionRequest = False                       '//Self-Reception

                                .Item(0) = 79                                       '//&H4F
                                .Item(1) = 130                                      '//Register LSB - &H82
                                .Item(2) = 39                                       '//Register MSB - &H27
                                .Item(3) = 1                                        '//Register Index - &H01
                                .Item(4) = 0
                                .Item(5) = 0
                                .Item(6) = 0
                                .Item(7) = 0
                            End With

                        Case Is = "Query Traction Minutes"
                            With myCanMessage
                                .TimeStamp = 0                                      '//No delayed transmission
                                If Node = 1 Then
                                    .Identifier = 1537                              '//&H601 - Sevcon SDO Request
                                Else
                                    .Identifier = 1538                              '//&H602 - Sevcon SDO Request
                                End If
                                .FrameType = Bal.Can.CanMsgFrameType.Data
                                .DataLength = 8
                                .ExtendedFrameFormat = False                        '//Standard Frame
                                .SelfReceptionRequest = False                       '//Self-Reception

                                .Item(0) = 79                                       '//&H4F
                                .Item(1) = 130                                      '//Register LSB - &H82
                                .Item(2) = 39                                       '//Register MSB - &H27
                                .Item(3) = 2                                        '//Register Index - &H02
                                .Item(4) = 0
                                .Item(5) = 0
                                .Item(6) = 0
                                .Item(7) = 0
                            End With

                        Case Is = "Send Heartbeat"
                            With myCanMessage
                                .TimeStamp = 0                                      '//No delayed transmission
                                .Identifier = 1798                                  '//&H706 - Tablet Heartbeat
                                .FrameType = Bal.Can.CanMsgFrameType.Data
                                .DataLength = 8
                                .ExtendedFrameFormat = False                        '//Standard Frame
                                .SelfReceptionRequest = False                       '//Self-Reception

                                .Item(0) = 5                                        '//Normal Heartbeat Value
                                If vehicleControlStatus = "ECU" Then
                                    .Item(1) = 0                                    '//ECU Control of vehicle
                                Else
                                    .Item(1) = 4                                    '//Tablet Control of vehicle
                                End If

                                '//Traction state of vehicle 1-AWD 2-4WD HI 3- 4WD LO
                                If drivetrainAWDSwitch.Value = True Then
                                    .Item(2) = 1
                                Else
                                    If _4WDHighSwitch.Value = True Then
                                        .Item(2) = 2
                                    Else
                                        .Item(2) = 3
                                    End If
                                End If
                                
                                .Item(3) = minSOCActiveSlide.Value                  '//Generator Min SOC Value
                                .Item(4) = maxSOCActiveSlide.Value                  '//Generator Max SOC Value
                                .Item(5) = 0
                                .Item(6) = 0
                                .Item(7) = 0
                            End With

                        Case Is = "Digital Outputs"
                            With myCanMessage
                                .TimeStamp = 0                                      '//No delayed transmission
                                .Identifier = 522                                   '//&H20A - CANio Digital Outputs
                                .FrameType = Bal.Can.CanMsgFrameType.Data
                                .DataLength = 2
                                .ExtendedFrameFormat = False                        '//Standard Frame
                                .SelfReceptionRequest = False                       '//Self-Reception

                                .Item(0) = CanIO0.x200_0
                                .Item(1) = CanIO0.x200_1
                            End With

                        Case Is = "Open Battery Contactor"
                            If Node = 8 Then
                                With myCanMessage
                                    .TimeStamp = 0                                  '//No delayed transmission
                                    .Identifier = &H506                             '//Message ID (CAN-ID)
                                    .FrameType = Bal.Can.CanMsgFrameType.Data
                                    .DataLength = 5
                                    .ExtendedFrameFormat = False                    '//Standard Frame
                                    .SelfReceptionRequest = False                   '//Self-Reception

                                    .Item(0) = 8                                    '//Target Node ID
                                    .Item(1) = 88                                   '//&H58 Control Bits
                                    .Item(2) = 0
                                    .Item(3) = 1
                                    .Item(4) = 17
                                End With

                            ElseIf Node = 9 Then
                                With myCanMessage
                                    .TimeStamp = 0                                  '//No delayed transmission
                                    .Identifier = &H506                             '//Message ID (CAN-ID)
                                    .FrameType = Bal.Can.CanMsgFrameType.Data
                                    .DataLength = 5
                                    .ExtendedFrameFormat = False                    '//Standard Frame
                                    .SelfReceptionRequest = False                   '//Self-Reception

                                    .Item(0) = 9                                    '//Target Node ID
                                    .Item(1) = 88                                   '//&H58 Control Bits
                                    .Item(2) = 0
                                    .Item(3) = 1
                                    .Item(4) = 17
                                End With
                            End If

                        Case Is = "Close Battery Contactor"
                            If Node = 8 Then
                                With myCanMessage
                                    .TimeStamp = 0                                  '//No delayed transmission
                                    .Identifier = &H506                             '//Message ID (CAN-ID)
                                    .FrameType = Bal.Can.CanMsgFrameType.Data
                                    .DataLength = 5
                                    .ExtendedFrameFormat = False                    '//Standard Frame
                                    .SelfReceptionRequest = False                   '//Self-Reception

                                    .Item(0) = 8                                    '//Target Node ID
                                    .Item(1) = 100                                  '//&H64 Control Bits
                                    .Item(2) = 2
                                    .Item(3) = 1
                                    .Item(4) = 17
                                End With
                            ElseIf Node = 9 Then
                                With myCanMessage
                                    .TimeStamp = 0                                  '//No delayed transmission
                                    .Identifier = &H506                             '//Message ID (CAN-ID)
                                    .FrameType = Bal.Can.CanMsgFrameType.Data
                                    .DataLength = 5
                                    .ExtendedFrameFormat = False                    '//Standard Frame
                                    .SelfReceptionRequest = False                   '//Self-Reception

                                    .Item(0) = 9                                    '//Target Node ID
                                    .Item(1) = 100                                  '//&H64 Control Bits
                                    .Item(2) = 2
                                    .Item(3) = 1
                                    .Item(4) = 17
                                End With
                            End If

                        Case Is = "Send Rear Sevcon Current Limits"
                            With myCanMessage
                                .TimeStamp = 0                                  '//No delayed transmission
                                .Identifier = &H205                             '//Message ID (CAN-ID)
                                .FrameType = Bal.Can.CanMsgFrameType.Data
                                .DataLength = 8
                                .ExtendedFrameFormat = False                    '//Standard Frame
                                .SelfReceptionRequest = False                   '//Self-Reception

                                .Item(0) = (AllowedChargeCurrentPerSevcon And 255)          '//Charge Current LSB
                                .Item(1) = (AllowedChargeCurrentPerSevcon >> 8)             '//Charge Current MSB
                                .Item(2) = (AllowedDischargeCurrentPerSevcon And 255)       '//Discharge Current LSB
                                .Item(3) = (AllowedDischargeCurrentPerSevcon >> 8)          '//Discharge Current MSB
                                .Item(4) = 0
                                .Item(5) = 0
                                .Item(6) = 0
                                .Item(7) = 0
                            End With

                        Case Is = "Send Rear Sevcon Inputs"
                            'Don't allow direction change unless motor velocity is less than 2 RPM
                            If Sevcon0.Velocity < 2 And Sevcon1.Velocity < 2 Then
                                If CanIO1.DigitalInput1 = True Then
                                    myInt += 1
                                    If RDTravelDirection = "Reverse" Or RDTravelDirection = "Neutral" Then
                                        RDDirectionChange = True
                                    End If
                                    If RPTravelDirection = "Reverse" Or RPTravelDirection = "Neutral" Then
                                        RPDirectionChange = True
                                    End If
                                    RDTravelDirection = "Forward"
                                    RPTravelDirection = "Forward"
                                End If
                                If CanIO1.DigitalInput2 = True Then
                                    myInt += 2
                                    If RDTravelDirection = "Forward" Or RDTravelDirection = "Neutral" Then
                                        RDDirectionChange = True
                                    End If
                                    If RPTravelDirection = "Forward" Or RPTravelDirection = "Neutral" Then
                                        RPDirectionChange = True
                                    End If
                                    RDTravelDirection = "Reverse"
                                    RPTravelDirection = "Reverse"
                                End If
                                If CanIO1.DigitalInput1 = False And CanIO1.DigitalInput2 = False Then
                                    If RDTravelDirection = "Forward" Or RDTravelDirection = "Reverse" Then
                                        RDDirectionChange = True
                                    End If
                                    If RPTravelDirection = "Forward" Or RPTravelDirection = "Reverse" Then
                                        RPDirectionChange = True
                                    End If
                                    RDTravelDirection = "Neutral"
                                    RPTravelDirection = "Neutral"
                                End If
                            Else
                                Select Case RDTravelDirection
                                    Case Is = "Forward"
                                        myInt = 1
                                    Case Is = "Neutral"
                                        myInt = 0
                                    Case Is = "Reverse"
                                        myInt = 2
                                End Select
                            End If

                            Dim RDThrottleLSB As Int16
                            Dim RDThrottleMSB As Int16
                            If RDThrottleEnableSwitch.Value = True Then
                                If throttleActive Then
                                    RDThrottleLSB = int16LSBThrottle
                                    RDThrottleMSB = int16MSBThrottle
                                    myInt += 4
                                Else
                                    RDThrottleLSB = 0
                                    RDThrottleMSB = 0
                                End If
                            Else
                                RDThrottleLSB = 0
                                RDThrottleMSB = 0
                            End If

                            With myCanMessage
                                .HighPriorityMsg = True
                                .TimeStamp = 0                                      '//No delayed transmission
                                .Identifier = &H20D                                 '//Message ID (CAN-ID)
                                .FrameType = Bal.Can.CanMsgFrameType.Data
                                .DataLength = 5
                                .ExtendedFrameFormat = False                        '//Standard Frame
                                .SelfReceptionRequest = False                       '//Self-Reception

                                .Item(0) = RDThrottleLSB                            '//Throttle Value LSB
                                .Item(1) = RDThrottleMSB                            '//Throttle Value MSB
                                .Item(2) = int16LSBSpeed                            '//Speed Value LSB
                                .Item(3) = int16MSBSpeed                            '//Speed Value MSB
                                .Item(4) = myInt                                    '//Forward/Reverse/FS1 bits
                            End With
                    End Select

                    'Write the CAN message into the transmit FIFO
                    If mWriter1.Capacity > 0 Then
                        mWriter1.SendMessage(myCanMessage)
                    End If
                End If

            ElseIf CanBusNum = 2 Then
                If mWriter2 IsNot Nothing Then
                    Dim myCanMessage As Ixxat.Vci4.Bal.Can.CanMessage

                    Select Case Type
                        Case Is = "Query Main Odometer"
                            With myCanMessage
                                .TimeStamp = 0                                      '//No delayed transmission
                                Select Case Node
                                    Case Is = 1
                                        .Identifier = 1537                              '//&H601 - Sevcon SDO Request
                                    Case Is = 2
                                        .Identifier = 1538                              '//&H602 - Sevcon SDO Request
                                    Case Is = 3
                                        .Identifier = 1539                              '//&H603 - Sevcon SDO Request
                                End Select
                                .FrameType = Bal.Can.CanMsgFrameType.Data
                                .DataLength = 8
                                .ExtendedFrameFormat = False                        '//Standard Frame
                                .SelfReceptionRequest = False                       '//Self-Reception

                                .Item(0) = 79                                       '//&H4F
                                .Item(1) = 160                                      '//Register LSB - &HA0
                                .Item(2) = 41                                       '//Register MSB - &H29
                                .Item(3) = 3                                        '//Register Index - &H03
                                .Item(4) = 0
                                .Item(5) = 0
                                .Item(6) = 0
                                .Item(7) = 0
                            End With

                        Case Is = "Query Trip Odometer"
                            With myCanMessage
                                .TimeStamp = 0                                      '//No delayed transmission
                                Select Case Node
                                    Case Is = 1
                                        .Identifier = 1537                              '//&H601 - Sevcon SDO Request
                                    Case Is = 2
                                        .Identifier = 1538                              '//&H602 - Sevcon SDO Request
                                    Case Is = 3
                                        .Identifier = 1539                              '//&H603 - Sevcon SDO Request
                                End Select
                                .FrameType = Bal.Can.CanMsgFrameType.Data
                                .DataLength = 8
                                .ExtendedFrameFormat = False                        '//Standard Frame
                                .SelfReceptionRequest = False                       '//Self-Reception

                                .Item(0) = 79                                       '//&H4F
                                .Item(1) = 161                                      '//Register LSB - &HA1
                                .Item(2) = 41                                       '//Register MSB - &H29
                                .Item(3) = 3                                        '//Register Index - &H03
                                .Item(4) = 0
                                .Item(5) = 0
                                .Item(6) = 0
                                .Item(7) = 0
                            End With

                        Case Is = "Query Key On Hours"
                            With myCanMessage
                                .TimeStamp = 0                                      '//No delayed transmission
                                Select Case Node
                                    Case Is = 1
                                        .Identifier = 1537                              '//&H601 - Sevcon SDO Request
                                    Case Is = 2
                                        .Identifier = 1538                              '//&H602 - Sevcon SDO Request
                                    Case Is = 3
                                        .Identifier = 1539                              '//&H603 - Sevcon SDO Request
                                End Select
                                .FrameType = Bal.Can.CanMsgFrameType.Data
                                .DataLength = 8
                                .ExtendedFrameFormat = False                        '//Standard Frame
                                .SelfReceptionRequest = False                       '//Self-Reception

                                .Item(0) = 79                                       '//&H4F
                                .Item(1) = 129                                      '//Register LSB - &H81
                                .Item(2) = 39                                       '//Register MSB - &H27
                                .Item(3) = 1                                        '//Register Index - &H01
                                .Item(4) = 0
                                .Item(5) = 0
                                .Item(6) = 0
                                .Item(7) = 0
                            End With

                        Case Is = "Query Key On Minutes"
                            With myCanMessage
                                .TimeStamp = 0                                      '//No delayed transmission
                                Select Case Node
                                    Case Is = 1
                                        .Identifier = 1537                              '//&H601 - Sevcon SDO Request
                                    Case Is = 2
                                        .Identifier = 1538                              '//&H602 - Sevcon SDO Request
                                    Case Is = 3
                                        .Identifier = 1539                              '//&H603 - Sevcon SDO Request
                                End Select
                                .FrameType = Bal.Can.CanMsgFrameType.Data
                                .DataLength = 8
                                .ExtendedFrameFormat = False                        '//Standard Frame
                                .SelfReceptionRequest = False                       '//Self-Reception

                                .Item(0) = 79                                       '//&H4F
                                .Item(1) = 129                                      '//Register LSB - &H81
                                .Item(2) = 39                                       '//Register MSB - &H27
                                .Item(3) = 2                                        '//Register Index - &H02
                                .Item(4) = 0
                                .Item(5) = 0
                                .Item(6) = 0
                                .Item(7) = 0
                            End With

                        Case Is = "Query Traction Hours"
                            With myCanMessage
                                .TimeStamp = 0                                      '//No delayed transmission
                                Select Case Node
                                    Case Is = 1
                                        .Identifier = 1537                              '//&H601 - Sevcon SDO Request
                                    Case Is = 2
                                        .Identifier = 1538                              '//&H602 - Sevcon SDO Request
                                    Case Is = 3
                                        .Identifier = 1539                              '//&H603 - Sevcon SDO Request
                                End Select
                                .FrameType = Bal.Can.CanMsgFrameType.Data
                                .DataLength = 8
                                .ExtendedFrameFormat = False                        '//Standard Frame
                                .SelfReceptionRequest = False                       '//Self-Reception

                                .Item(0) = 79                                       '//&H4F
                                .Item(1) = 130                                      '//Register LSB - &H82
                                .Item(2) = 39                                       '//Register MSB - &H27
                                .Item(3) = 1                                        '//Register Index - &H01
                                .Item(4) = 0
                                .Item(5) = 0
                                .Item(6) = 0
                                .Item(7) = 0
                            End With

                        Case Is = "Query Traction Minutes"
                            With myCanMessage
                                .TimeStamp = 0                                      '//No delayed transmission
                                Select Case Node
                                    Case Is = 1
                                        .Identifier = 1537                              '//&H601 - Sevcon SDO Request
                                    Case Is = 2
                                        .Identifier = 1538                              '//&H602 - Sevcon SDO Request
                                    Case Is = 3
                                        .Identifier = 1539                              '//&H603 - Sevcon SDO Request
                                End Select
                                .FrameType = Bal.Can.CanMsgFrameType.Data
                                .DataLength = 8
                                .ExtendedFrameFormat = False                        '//Standard Frame
                                .SelfReceptionRequest = False                       '//Self-Reception

                                .Item(0) = 79                                       '//&H4F
                                .Item(1) = 130                                      '//Register LSB - &H82
                                .Item(2) = 39                                       '//Register MSB - &H27
                                .Item(3) = 2                                        '//Register Index - &H02
                                .Item(4) = 0
                                .Item(5) = 0
                                .Item(6) = 0
                                .Item(7) = 0
                            End With

                        Case Is = "Send Heartbeat"
                            With myCanMessage
                                .TimeStamp = 0                                      '//No delayed transmission
                                .Identifier = 1798                                  '//&H706 - Tablet Heartbeat
                                .FrameType = Bal.Can.CanMsgFrameType.Data
                                .DataLength = 8
                                .ExtendedFrameFormat = False                        '//Standard Frame
                                .SelfReceptionRequest = False                       '//Self-Reception

                                .Item(0) = 5                                        '//Normal Heartbeat Value
                                If vehicleControlStatus = "ECU" Then
                                    .Item(1) = 0                                    '//ECU Control of vehicle
                                Else
                                    .Item(1) = 4                                    '//Tablet Control of vehicle
                                End If

                                '//Traction state of vehicle 1-AWD 2-4WD HI 3- 4WD LO
                                If drivetrainAWDSwitch.Value = True Then
                                    .Item(2) = 1
                                Else
                                    If _4WDHighSwitch.Value = True Then
                                        .Item(2) = 2
                                    Else
                                        .Item(2) = 3
                                    End If
                                End If

                                .Item(3) = minSOCActiveSlide.Value                  '//Generator Min SOC Value
                                .Item(4) = maxSOCActiveSlide.Value                  '//Generator Max SOC Value
                                .Item(5) = 0
                                .Item(6) = 0
                                .Item(7) = 0
                            End With

                        Case Is = "Digital Outputs"
                            With myCanMessage
                                .TimeStamp = 0                                      '//No delayed transmission
                                .Identifier = 522                                   '//&H20A - CANio Digital Outputs
                                .FrameType = Bal.Can.CanMsgFrameType.Data
                                .DataLength = 2
                                .ExtendedFrameFormat = False                        '//Standard Frame
                                .SelfReceptionRequest = False                       '//Self-Reception

                                .Item(0) = CanIO1.x200_0
                                .Item(1) = CanIO1.x200_1
                            End With

                        Case Is = "Open Battery Contactor"
                            If Node = 8 Then
                                With myCanMessage
                                    .TimeStamp = 0                                  '//No delayed transmission
                                    .Identifier = &H506                             '//Message ID (CAN-ID)
                                    .FrameType = Bal.Can.CanMsgFrameType.Data
                                    .DataLength = 5
                                    .ExtendedFrameFormat = False                    '//Standard Frame
                                    .SelfReceptionRequest = False                   '//Self-Reception

                                    .Item(0) = 8                                    '//Target Node ID
                                    .Item(1) = 88                                   '//&H58 Control Bits
                                    .Item(2) = 2
                                    .Item(3) = 1
                                    .Item(4) = 17
                                End With
                            ElseIf Node = 9 Then
                                With myCanMessage
                                    .TimeStamp = 0                                  '//No delayed transmission
                                    .Identifier = &H506                             '//Message ID (CAN-ID)
                                    .FrameType = Bal.Can.CanMsgFrameType.Data
                                    .DataLength = 5
                                    .ExtendedFrameFormat = False                    '//Standard Frame
                                    .SelfReceptionRequest = False                   '//Self-Reception

                                    .Item(0) = 9                                    '//Target Node ID
                                    .Item(1) = 88                                   '//&H58 Control Bits
                                    .Item(2) = 2
                                    .Item(3) = 1
                                    .Item(4) = 17
                                End With
                            End If

                        Case Is = "Close Battery Contactor"
                            If Node = 8 Then
                                With myCanMessage
                                    .TimeStamp = 0                                  '//No delayed transmission
                                    .Identifier = &H506                             '//Message ID (CAN-ID)
                                    .FrameType = Bal.Can.CanMsgFrameType.Data
                                    .DataLength = 5
                                    .ExtendedFrameFormat = False                    '//Standard Frame
                                    .SelfReceptionRequest = False                   '//Self-Reception

                                    .Item(0) = 8                                    '//Target Node ID
                                    .Item(1) = 100                                  '//&H64 Control Bits
                                    .Item(2) = 2
                                    .Item(3) = 1
                                    .Item(4) = 17
                                End With
                            ElseIf Node = 9 Then
                                With myCanMessage
                                    .TimeStamp = 0                                  '//No delayed transmission
                                    .Identifier = &H506                             '//Message ID (CAN-ID)
                                    .FrameType = Bal.Can.CanMsgFrameType.Data
                                    .DataLength = 5
                                    .ExtendedFrameFormat = False                    '//Standard Frame
                                    .SelfReceptionRequest = False                   '//Self-Reception

                                    .Item(0) = 9                                    '//Target Node ID
                                    .Item(1) = 100                                  '//&H64 Control Bits
                                    .Item(2) = 2
                                    .Item(3) = 1
                                    .Item(4) = 17
                                End With
                            End If

                        Case Is = "Send Front Sevcon Current Limits"
                            With myCanMessage
                                .TimeStamp = 0                                  '//No delayed transmission
                                .Identifier = &H205                             '//Message ID (CAN-ID)
                                .FrameType = Bal.Can.CanMsgFrameType.Data
                                .DataLength = 8
                                .ExtendedFrameFormat = False                    '//Standard Frame
                                .SelfReceptionRequest = False                   '//Self-Reception

                                .Item(0) = (AllowedChargeCurrentPerSevcon And 255)          '//Charge Current LSB
                                .Item(1) = (AllowedChargeCurrentPerSevcon >> 8)             '//Charge Current MSB
                                .Item(2) = (AllowedDischargeCurrentPerSevcon And 255)       '//Discharge Current LSB
                                .Item(3) = (AllowedDischargeCurrentPerSevcon >> 8)          '//Discharge Current MSB
                                .Item(4) = 0
                                .Item(5) = 0
                                .Item(6) = 0
                                .Item(7) = 0
                            End With

                        Case Is = "Send Front Sevcon Inputs"
                            'Don't allow direction change unless motor velocity is less than 2 RPM
                            If Sevcon2.Velocity < 2 And Sevcon3.Velocity < 2 Then
                                If CanIO1.DigitalInput1 = True Then
                                    myInt += 1
                                    If FDTravelDirection = "Reverse" Or FDTravelDirection = "Neutral" Then
                                        FDDirectionChange = True
                                    End If
                                    If FPTravelDirection = "Reverse" Or FPTravelDirection = "Neutral" Then
                                        FPDirectionChange = True
                                    End If
                                    FDTravelDirection = "Forward"
                                    FPTravelDirection = "Forward"
                                End If
                                If CanIO1.DigitalInput2 = True Then
                                    myInt += 2
                                    If FDTravelDirection = "Forward" Or FDTravelDirection = "Neutral" Then
                                        FDDirectionChange = True
                                    End If
                                    If FPTravelDirection = "Forward" Or FPTravelDirection = "Neutral" Then
                                        FPDirectionChange = True
                                    End If
                                    FDTravelDirection = "Reverse"
                                    FPTravelDirection = "Reverse"
                                End If
                                If CanIO1.DigitalInput1 = False And CanIO1.DigitalInput2 = False Then
                                    If FDTravelDirection = "Forward" Or FDTravelDirection = "Reverse" Then
                                        FDDirectionChange = True
                                    End If
                                    If FPTravelDirection = "Forward" Or FPTravelDirection = "Reverse" Then
                                        FPDirectionChange = True
                                    End If
                                    FDTravelDirection = "Neutral"
                                    FPTravelDirection = "Neutral"
                                End If
                            Else
                                Select Case FDTravelDirection
                                    Case Is = "Forward"
                                        myInt = 1
                                    Case Is = "Neutral"
                                        myInt = 0
                                        FDPictureBox.Visible = False
                                    Case Is = "Reverse"
                                        myInt = 2
                                End Select
                            End If

                            Dim FDThrottleLSB As Int16
                            Dim FDThrottleMSB As Int16
                            If FDThrottleEnableSwitch.Value = True Then
                                If throttleActive Then
                                    FDThrottleLSB = int16LSBThrottle
                                    FDThrottleMSB = int16MSBThrottle
                                    myInt += 4
                                Else
                                    FDThrottleLSB = 0
                                    FDThrottleMSB = 0
                                End If
                            Else
                                FDThrottleLSB = 0
                                FDThrottleMSB = 0
                            End If

                            With myCanMessage
                                .HighPriorityMsg = True
                                .TimeStamp = 0                                      '//No delayed transmission
                                .Identifier = &H20D                                 '//Message ID (CAN-ID)
                                .FrameType = Bal.Can.CanMsgFrameType.Data
                                .DataLength = 5
                                .ExtendedFrameFormat = False                        '//Standard Frame
                                .SelfReceptionRequest = False                       '//Self-Reception

                                .Item(0) = FDThrottleLSB                            '//Throttle Value LSB
                                .Item(1) = FDThrottleMSB                            '//Throttle Value MSB
                                .Item(2) = int16LSBSpeed                            '//Speed Value LSB
                                .Item(3) = int16MSBSpeed                            '//Speed Value MSB
                                .Item(4) = myInt                                    '//Forward/Reverse/FS1 bits
                            End With

                        Case Is = "Send Gen Sevcon Inputs"
                            Dim GenThrottleLSB As Int16
                            Dim GenThrottleMSB As Int16
                            Dim GenFootbrakeLSB As Int16
                            Dim GenFootbrakeMSB As Int16
                            If Sevcon4.ContactorClosed = True Then
                                generatorControllerOn = True
                            Else
                                generatorControllerOn = False
                            End If
                            If generatorEnabled And generatorSOCEnabled And generatorChargingRequest Then           'Generator Swith is ON and charging request is active
                                If generatorControllerOn Then               'Generator Sevcon has been pre-charged
                                    GenFootbrakeLSB = 0
                                    GenFootbrakeMSB = 0
                                    If generatorEngineStarting Then         'Apply throttle to start generator engine
                                        int16OValue = Convert.ToInt16(My.Settings.genStartThrottle)
                                        int16Value = int16OValue
                                        GenThrottleMSB = (int16Value >> 8)
                                        int16OValue = Convert.ToInt16(My.Settings.genStartThrottle)
                                        int16Value = int16OValue
                                        GenThrottleLSB = (int16Value And 255)
                                        myInt += 3                          'Forward (1) + FS1 (4)
                                    End If
                                    If generatorEngineRunning Then          'Apply no throttle to activate motor regen
                                        GenThrottleLSB = 0
                                        GenThrottleMSB = 0
                                    End If
                                End If
                            End If

                            With myCanMessage
                                .TimeStamp = 0                                      '//No delayed transmission
                                .Identifier = &H20F                                 '//Message ID (CAN-ID)
                                .FrameType = Bal.Can.CanMsgFrameType.Data
                                .DataLength = 5
                                .ExtendedFrameFormat = False                        '//Standard Frame
                                .SelfReceptionRequest = False                       '//Self-Reception

                                .Item(0) = GenThrottleLSB                           '//Throttle Value LSB
                                .Item(1) = GenThrottleMSB                           '//Throttle Value MSB
                                .Item(2) = GenFootbrakeLSB                          '//Footbrake Value LSB
                                .Item(3) = GenFootbrakeMSB                          '//Footbrake Value LSB
                                .Item(4) = myInt
                            End With
                    End Select

                    'Write the CAN message into the transmit FIFO
                    If mWriter2.Capacity > 0 Then
                        mWriter2.SendMessage(myCanMessage)
                    End If
                End If
            End If
        Catch ex As Exception
            'MessageBox.Show(ex.Message.ToString, "SendCanMessage ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error)
            sw.WriteLine("ERROR - SendCanMessage_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
        End Try
    End Sub

    Private Overloads Function ConvertHexString(ByVal Byte0 As Byte) As Int32
        'Coverts one hex byte to an integer value
        Try
            ConvertHexString = Convert.ToInt32(Byte0.ToString("X2"), 16)
        Catch ex As Exception
            'MessageBox.Show(ex.Message, "ConvertHexString-1Byte Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            sw.WriteLine("ERROR - ConvertHexString_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
        End Try
        Return ConvertHexString
    End Function

    Private Overloads Function ConvertHexString(ByVal Byte0 As Byte, ByVal Byte1 As Byte) As Int32
        'Concatenates two hex bytes and converts the string into an integer value
        Try
            ConvertHexString = Convert.ToInt32((Byte1.ToString("X2") & Byte0.ToString("X2")), 16)
        Catch ex As Exception
            'MessageBox.Show(ex.Message, "ConvertHexString-2Bytes Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            sw.WriteLine("ERROR - ConvertHexString_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
        End Try
        Return ConvertHexString
    End Function

    Private Overloads Function ConvertHexString(ByVal Byte0 As Byte, ByVal Byte1 As Byte, ByVal Byte2 As Byte) As Int32
        'Concatenates three hex bytes and converts the string into an integer value
        Try
            ConvertHexString = Convert.ToInt32((Byte2.ToString("X2") & Byte1.ToString("X2") & Byte0.ToString("X2")), 16)
        Catch ex As Exception
            'MessageBox.Show(ex.Message, "ConvertHexString-3Bytes Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            sw.WriteLine("ERROR - ConvertHexString_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
        End Try
        Return ConvertHexString
    End Function

    Private Overloads Function ConvertHexString(ByVal Byte0 As Byte, ByVal Byte1 As Byte, ByVal Byte2 As Byte, Byte3 As Byte) As Int32
        'Concatenates four hex bytes and converts the string into an integer value
        Try
            ConvertHexString = Convert.ToInt32((Byte3.ToString("X2") & Byte2.ToString("X2") & Byte1.ToString("X2") & Byte0.ToString("X2")), 16)
        Catch ex As Exception
            'MessageBox.Show(ex.Message, "ConvertHexString-4Bytes Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            sw.WriteLine("ERROR - ConvertHexString_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
        End Try
        Return ConvertHexString
    End Function

    Private Sub ShowInfoMessage(ByVal canMessage As Ixxat.Vci4.Bal.Can.ICanMessage, ByVal CanBusNum As Integer)

    End Sub

    Private Sub ShowStatusMessage(ByVal canMessage As Ixxat.Vci4.Bal.Can.ICanMessage, ByVal CanBusNum As Integer)

    End Sub

    Private Sub ShowTimerOverrunMessage(ByVal canMessage As Ixxat.Vci4.Bal.Can.ICanMessage, ByVal CanBusNum As Integer)

    End Sub

    Private Sub ShowWakeUpMessage(ByVal canMessage As Ixxat.Vci4.Bal.Can.ICanMessage, ByVal CanBusNum As Integer)

    End Sub

    Private Sub ShowTimerResetMessage(ByVal canMessage As Ixxat.Vci4.Bal.Can.ICanMessage, ByVal CanBusNum As Integer)

    End Sub

    Private Sub availableInterfacesListView_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles availableInterfacesListView.SelectedIndexChanged
        init1Button.Enabled = True
        init2Button.Enabled = True
        ShowDeviceHardwareID()
    End Sub

    Private Sub init1Button_Click(sender As Object, e As EventArgs) Handles init1Button.Click
        Dim CurListViewItem As ListViewItem
        If availableInterfacesListView.Items.Count > 0 Then
            availableInterfacesListView.Items(0).Selected = True
            availableInterfacesListView_SelectedIndexChanged(Me, e)
            CurListViewItem = availableInterfacesListView.SelectedItems(0)

            getStatus1Timer.Enabled = False

            'CloseCurrentExistingController(1)

            Try
                If CurListViewItem IsNot Nothing Then
                    SelectDevice(CurListViewItem.Tag)
                    InitSocket(0, 1)

                    rxThread1 = New System.Threading.Thread(New System.Threading.ThreadStart(AddressOf ReceiveThreadFunc1))
                    rxThread1.Start()

                    init1Button.Enabled = False
                    transmitData1Button.Enabled = True
                    getStatus1Timer.Enabled = True
                End If
            Catch ex As Exception
                sw.WriteLine("ERROR - init1Button_Click_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            End Try
        Else
            Dim myMessageBoxResponse As MsgBoxResult
            sw.WriteLine("ERROR - init1Button_Click_Exception - No Tablet Dock Power (CAN1) " & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            myMessageBoxResponse = MsgBox("USB-to-CAN Device Is Disconnected and/or Tablet Dock Isn't Powered", MsgBoxStyle.OkOnly, "USB-TO-CAN DEVICE ERROR (CAN1)")
            If myMessageBoxResponse = MsgBoxResult.Ok Then
                sw.WriteLine("ACK OK - init1Button_Click_Exception - No Tablet Dock Power (CAN1) " & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            End If
        End If

    End Sub

    Private Sub init2Button_Click(sender As Object, e As EventArgs) Handles init2Button.Click

        Dim CurListViewItem As ListViewItem
        'availableInterfacesListView.Items(0).Selected = True
        'availableInterfacesListView_SelectedIndexChanged(Me, e)
        If availableInterfacesListView.Items.Count > 0 Then
            CurListViewItem = availableInterfacesListView.SelectedItems(0)

            getStatus2Timer.Enabled = False

            'CloseCurrentExistingController(1)

            Try
                If CurListViewItem IsNot Nothing Then
                    'SelectDevice(CurListViewItem.Tag)
                    InitSocket(1, 2)

                    rxThread2 = New System.Threading.Thread(New System.Threading.ThreadStart(AddressOf ReceiveThreadFunc2))
                    rxThread2.Start()

                    init2Button.Enabled = False
                    transmitData2Button.Enabled = True
                    getStatus2Timer.Enabled = True
                End If
            Catch ex As Exception
                sw.WriteLine("ERROR - init1Button_Click_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            End Try
        Else
            Dim myMessageBoxResponse As MsgBoxResult
            sw.WriteLine("ERROR - init1Button_Click_Exception - No Tablet Dock Power (CAN2) " & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            myMessageBoxResponse = MsgBox("USB-to-CAN Device Is Disconnected and/or Tablet Dock Isn't Powered", MsgBoxStyle.OkOnly, "USB-TO-CAN DEVICE ERROR (CAN2)")
            If myMessageBoxResponse = MsgBoxResult.Ok Then
                sw.WriteLine("ACK OK - init1Button_Click_Exception - No Tablet Dock Power (CAN2) " & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            End If
        End If


    End Sub

    Private Sub ShowErrorMessage(ByVal canMessage As Ixxat.Vci4.Bal.Can.ICanMessage, ByVal CanBusNum As Integer)
        Dim msgError As Ixxat.Vci4.Bal.Can.CanMsgError
        msgError = canMessage(0)

        Try
            If CanBusNum = 1 Then
                Select Case msgError
                    Case Ixxat.Vci4.Bal.Can.CanMsgError.Acknowledge
                        SetText1("Error: Acknowledge")
                    Case Ixxat.Vci4.Bal.Can.CanMsgError.Bit
                        SetText1("Error: Bit")
                    Case Ixxat.Vci4.Bal.Can.CanMsgError.Crc
                        SetText1("Error: Crc")
                    Case Ixxat.Vci4.Bal.Can.CanMsgError.Form
                        SetText1("Error: Form")
                    Case Ixxat.Vci4.Bal.Can.CanMsgError.Other
                        SetText1("Error: Other")
                    Case Ixxat.Vci4.Bal.Can.CanMsgError.Stuff
                        SetText1("Error: Stuff")
                End Select
            ElseIf CanBusNum = 2 Then
                Select Case msgError
                    Case Ixxat.Vci4.Bal.Can.CanMsgError.Acknowledge
                        SetText2("Error: Acknowledge")
                    Case Ixxat.Vci4.Bal.Can.CanMsgError.Bit
                        SetText2("Error: Bit")
                    Case Ixxat.Vci4.Bal.Can.CanMsgError.Crc
                        SetText2("Error: Crc")
                    Case Ixxat.Vci4.Bal.Can.CanMsgError.Form
                        SetText2("Error: Form")
                    Case Ixxat.Vci4.Bal.Can.CanMsgError.Other
                        SetText2("Error: Other")
                    Case Ixxat.Vci4.Bal.Can.CanMsgError.Stuff
                        SetText2("Error: Stuff")
                End Select
            End If
        Catch ex As Exception
            Dim myMessageBoxResponse As MsgBoxResult
            sw.WriteLine("ERROR - ShowErrorMessage_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            myMessageBoxResponse = MsgBox(ex.ToString, MsgBoxStyle.OkOnly, "ERROR - ShowErrorMessage")
            If myMessageBoxResponse = MsgBoxResult.Ok Then
                sw.WriteLine("ACK OK - ShowErrorMessage_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            End If
        End Try

    End Sub

    Private Sub SelectDevice(ByVal DeviceEnumNumber As Long)
        Dim mTempDevice As Ixxat.Vci4.IVciDevice = Nothing
        Dim deviceHardwareID As Object

        Try
            deviceEnum = deviceList.GetEnumerator()
            deviceEnum.Reset()
            Do While deviceEnum.MoveNext() = True
                mTempDevice = deviceEnum.Current
                deviceHardwareID = mTempDevice.UniqueHardwareId
                If TypeOf deviceHardwareID Is System.Guid Then
                    hwIdLabel.Text = GetSerialNumber(deviceHardwareID)
                Else
                    hwIdLabel.Text = deviceHardwareID.ToString()
                End If
            Loop
        Catch ex As Exception
            Dim myMessageBoxResponse As MsgBoxResult
            sw.WriteLine("ERROR - SelectDevice_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            myMessageBoxResponse = MsgBox(ex.ToString, MsgBoxStyle.OkOnly, "ERROR - SelectDevice")
            If myMessageBoxResponse = MsgBoxResult.Ok Then
                sw.WriteLine("ACK OK - SelectDevice_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            End If
        End Try

    End Sub

    Private Sub InitSocket(ByVal canNo As Byte, ByVal canBusNum As Integer)
        If canBusNum = 1 Then
            Dim bal1 As Ixxat.Vci4.Bal.IBalObject

            Dim balType1 As Type
            balType1 = GetType(Ixxat.Vci4.Bal.Can.ICanChannel)

            Try
                bal1 = mDevice.OpenBusAccessLayer()

                mCanChn1 = bal1.OpenSocket(canNo, balType1)

                'Intialize the message channel
                mCanChn1.Initialize(1024, 128, False)

                'Get a message reader object
                mReader1 = mCanChn1.GetMessageReader()

                'Initialize message reader
                mReader1.Threshold = 1

                'Create and assign the event that's set if at least one message was received
                mRxEvent1 = New System.Threading.AutoResetEvent(False)
                mReader1.AssignEvent(mRxEvent1)

                'Get a message writer object
                mWriter1 = mCanChn1.GetMessageWriter()

                'Initialize message writer
                mWriter1.Threshold = 1

                'Activate the message channel
                mCanChn1.Activate()

                'Open the CAN controller
                Dim canCtrlType1 As Type
                canCtrlType1 = GetType(Ixxat.Vci4.Bal.Can.ICanControl)
                mCanCtl1 = bal1.OpenSocket(canNo, canCtrlType1)

                'Initialize the CAN controller
                Dim operatingMode1 As Byte
                operatingMode1 = Ixxat.Vci4.Bal.Can.CanOperatingModes.Standard Or Ixxat.Vci4.Bal.Can.CanOperatingModes.Extended Or Ixxat.Vci4.Bal.Can.CanOperatingModes.ErrFrame

                Dim bitRate1 As Ixxat.Vci4.Bal.Can.CanBitrate
                bitRate1 = GetSelectedBaudRate()
                mCanCtl1.InitLine(operatingMode1, bitRate1)

                'Set the acceptance filter
                Dim accCode1 As UInteger
                Dim accMask1 As UInteger

                accCode1 = Ixxat.Vci4.Bal.Can.CanAccCode.All
                accMask1 = Ixxat.Vci4.Bal.Can.CanAccMask.All

                mCanCtl1.SetAccFilter(Ixxat.Vci4.Bal.Can.CanFilter.Std, accCode1, accMask1)

                'Start the CAN controller
                mCanCtl1.StartLine()

            Catch ex As Exception
                Dim myMessageBoxResponse As MsgBoxResult
                sw.WriteLine("ERROR - InitSocketCAN1_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                myMessageBoxResponse = MsgBox(ex.ToString, MsgBoxStyle.OkOnly, "ERROR - InitSocketCAN1")
                If myMessageBoxResponse = MsgBoxResult.Ok Then
                    sw.WriteLine("ACK OK - InitSocketCAN1_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                End If
                Return
            End Try
        ElseIf canBusNum = 2 Then
            Dim bal2 As Ixxat.Vci4.Bal.IBalObject

            Dim balType2 As Type
            balType2 = GetType(Ixxat.Vci4.Bal.Can.ICanChannel)

            Try
                bal2 = mDevice.OpenBusAccessLayer()

                mCanChn2 = bal2.OpenSocket(canNo, balType2)

                'Intialize the message channel
                mCanChn2.Initialize(1024, 128, False)

                'Get a message reader object
                mReader2 = mCanChn2.GetMessageReader()

                'Initialize message reader
                mReader2.Threshold = 1

                'Create and assign the event that's set if at least one message was received
                mRxEvent2 = New System.Threading.AutoResetEvent(False)
                mReader2.AssignEvent(mRxEvent2)

                'Get a message writer object
                mWriter2 = mCanChn2.GetMessageWriter()

                'Initialize message writer
                mWriter2.Threshold = 1

                'Activate the message channel
                mCanChn2.Activate()

                'Open the CAN controller
                Dim canCtrlType2 As Type
                canCtrlType2 = GetType(Ixxat.Vci4.Bal.Can.ICanControl)
                mCanCtl2 = bal2.OpenSocket(canNo, canCtrlType2)

                'Initialize the CAN controller
                Dim operatingMode2 As Byte
                operatingMode2 = Ixxat.Vci4.Bal.Can.CanOperatingModes.Standard Or Ixxat.Vci4.Bal.Can.CanOperatingModes.Extended Or Ixxat.Vci4.Bal.Can.CanOperatingModes.ErrFrame

                Dim bitRate2 As Ixxat.Vci4.Bal.Can.CanBitrate
                bitRate2 = GetSelectedBaudRate()
                mCanCtl2.InitLine(operatingMode2, bitRate2)

                'Set the acceptance filter
                Dim accCode2 As UInteger
                Dim accMask2 As UInteger

                accCode2 = Ixxat.Vci4.Bal.Can.CanAccCode.All
                accMask2 = Ixxat.Vci4.Bal.Can.CanAccMask.All

                mCanCtl2.SetAccFilter(Ixxat.Vci4.Bal.Can.CanFilter.Std, accCode2, accMask2)

                'Start the CAN controller
                mCanCtl2.StartLine()

            Catch ex As Exception
                Dim myMessageBoxResponse As MsgBoxResult
                sw.WriteLine("ERROR - InitSocketCAN2_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                myMessageBoxResponse = MsgBox(ex.ToString, MsgBoxStyle.OkOnly, "ERROR - InitSocketCAN2")
                If myMessageBoxResponse = MsgBoxResult.Ok Then
                    sw.WriteLine("ACK OK - InitSocketCAN2_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                End If
                Return
            End Try
        End If
    End Sub

    Private Sub ShowDeviceHardwareID()
        Dim CurListViewItem As ListViewItem

        Dim selIndex As Windows.Forms.ListView.SelectedIndexCollection

        selIndex = availableInterfacesListView.SelectedIndices()

        Try
            If selIndex.Count > 0 Then
                CurListViewItem = availableInterfacesListView.SelectedItems(0)
                If CurListViewItem IsNot Nothing Then
                    SelectDevice(CurListViewItem.Tag)
                End If
            End If
        Catch ex As Exception
            Dim myMessageBoxResponse As MsgBoxResult
            sw.WriteLine("ERROR - ShowDeviceHardwareID_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            myMessageBoxResponse = MsgBox(ex.ToString, MsgBoxStyle.OkOnly, "ERROR - ShowDeviceHardwareID")
            If myMessageBoxResponse = MsgBoxResult.Ok Then
                sw.WriteLine("ACK OK - ShowDeviceHardwareID_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            End If
        End Try

    End Sub

    Private Function GetSerialNumber(ByVal inputGuid As System.Guid)
        Dim resultString As String = ""

        'Convert the GUID to a byte array
        Dim byteArray() As Byte = inputGuid.ToByteArray

        Try
            'The first 2 bytes must have HW as data, then it is really a Serial Number
            If (Chr(byteArray(0)) = "H") And (Chr(byteArray(1)) = "W") Then
                resultString = System.Text.Encoding.ASCII.GetString(byteArray)
            Else
                resultString = inputGuid.ToString()
            End If

        Catch ex As Exception
            Dim myMessageBoxResponse As MsgBoxResult
            sw.WriteLine("ERROR - GetSerialNumber_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            myMessageBoxResponse = MsgBox(ex.ToString, MsgBoxStyle.OkOnly, "ERROR - GetSerialNumber")
            If myMessageBoxResponse = MsgBoxResult.Ok Then
                sw.WriteLine("ACK OK - GetSerialNumber_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            End If
        End Try
        Return resultString
    End Function

    Private Sub SetText1(ByVal [text] As String)

        Try
            If lastRxMsg1TextBox.InvokeRequired Then
                Dim d As New SetTextCallback1(AddressOf SetText1)
                Me.Invoke(d, New Object() {[text]})
            Else
                lastRxMsg1TextBox.Text = [text]
            End If
        Catch ex As Exception
            sw.WriteLine("ERROR - SetText1_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
        End Try

    End Sub

    Private Sub SetText2(ByVal [text] As String)

        Try
            If lastRxMsg1TextBox.InvokeRequired Then
                Dim d As New SetTextCallback2(AddressOf SetText2)
                Me.Invoke(d, New Object() {[text]})
            Else
                lastRxMsg2TextBox.Text = [text]
            End If
        Catch ex As Exception
            sw.WriteLine("ERROR - SetText2_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
        End Try

    End Sub

    Private Function GetSelectedBaudRate() As Ixxat.Vci4.Bal.Can.CanBitrate
        Dim resultBaud As Ixxat.Vci4.Bal.Can.CanBitrate

        Try
            Select Case baudrate1ListBox.SelectedIndex
                Case 0
                    resultBaud = Ixxat.Vci4.Bal.Can.CanBitrate.Cia125KBit
                Case 1
                    resultBaud = Ixxat.Vci4.Bal.Can.CanBitrate.Cia250KBit
                Case 2
                    resultBaud = Ixxat.Vci4.Bal.Can.CanBitrate.Cia500KBit
                Case 3
                    resultBaud = Ixxat.Vci4.Bal.Can.CanBitrate.Cia800KBit
                Case Else
                    resultBaud = Ixxat.Vci4.Bal.Can.CanBitrate.Cia1000KBit
            End Select

            Return resultBaud
        Catch ex As Exception
            Dim myMessageBoxResponse As MsgBoxResult
            sw.WriteLine("ERROR - GetSelectedBaudRate_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            myMessageBoxResponse = MsgBox(ex.ToString, MsgBoxStyle.OkOnly, "ERROR - GetSelectedBaudRate")
            If myMessageBoxResponse = MsgBoxResult.Ok Then
                sw.WriteLine("ACK OK - GetSelectedBaudRate_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            End If
        End Try

    End Function

    Private Sub CloseCurrentExistingController(ByVal canBusNum As Integer)

        Try
            If canBusNum = 1 Then
                If mCanCtl1 IsNot Nothing Then
                    'Stop the CAN controller
                    Try
                        mCanCtl1.StopLine()
                    Catch ex As Exception
                        MessageBox.Show(ex.ToString, "CloseCurrentExistingController CAN1 ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                End If
                'Close the receive thread. It will be reopened with another event
                If rxThread1 IsNot Nothing Then
                    'Tell the receive thread to quit
                    rxThread1.Abort()

                    'Wait for the termination of the receive thread
                    rxThread1.Join()
                    rxThread1 = Nothing
                End If
            ElseIf canBusNum = 2 Then
                If mCanCtl2 IsNot Nothing Then
                    'Stop the CAN controller
                    Try
                        mCanCtl2.StopLine()
                    Catch ex As Exception
                        MessageBox.Show(ex.ToString, "CloseCurrentExistingController CAN2 ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                End If
                'Close the receive thread. It will be reopened with another event
                If rxThread2 IsNot Nothing Then
                    'Tell the receive thread to quit
                    rxThread2.Abort()

                    'Wait for the termination of the receive thread
                    rxThread2.Join()
                    rxThread2 = Nothing
                End If
            End If
            CloseVciObjects(False)
        Catch ex As Exception
            Dim myMessageBoxResponse As MsgBoxResult
            sw.WriteLine("ERROR - CloseCurrentExistingController_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            myMessageBoxResponse = MsgBox(ex.ToString, MsgBoxStyle.OkOnly, "ERROR - CloseCurrentExistingController")
            If myMessageBoxResponse = MsgBoxResult.Ok Then
                sw.WriteLine("ACK OK - CloseCurrentExistingController_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            End If
        End Try

    End Sub

    Private Sub CloseVciObjects(closeVciObject As Boolean)
        'Dispose all hold VCI objects.

        Try
            'Dispose message reader1
            If (mReader1 IsNot Nothing) Then
                DisposeVciObject(mReader1)
                mReader1 = Nothing
            End If

            'Dispose message reader2
            If (mReader2 IsNot Nothing) Then
                DisposeVciObject(mReader2)
                mReader2 = Nothing
            End If

            'Dispose CAN channel1
            If (mCanChn1 IsNot Nothing) Then
                DisposeVciObject(mCanChn1)
                mCanChn1 = Nothing
            End If

            'Dispose CAN channel2
            If (mCanChn2 IsNot Nothing) Then
                DisposeVciObject(mCanChn2)
                mCanChn2 = Nothing
            End If

            'Dispose CAN controller1
            If (mCanCtl1 IsNot Nothing) Then
                DisposeVciObject(mCanCtl1)
                mCanCtl1 = Nothing
            End If

            'Dispose CAN controller2
            If (mCanCtl2 IsNot Nothing) Then
                DisposeVciObject(mCanCtl2)
                mCanCtl2 = Nothing
            End If

            If closeVciObject Then
                'Dispose VCI device1
                If (mDevice IsNot Nothing) Then
                    DisposeVciObject(mDevice)
                End If

            End If
        Catch ex As Exception
            Dim myMessageBoxResponse As MsgBoxResult
            sw.WriteLine("ERROR - CloseVciObjects_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            myMessageBoxResponse = MsgBox(ex.ToString, MsgBoxStyle.OkOnly, "ERROR - CloseVciObjects")
            If myMessageBoxResponse = MsgBoxResult.Ok Then
                sw.WriteLine("ACK OK - CloseVciObjects_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            End If
        End Try


    End Sub

    Private Sub DisposeVciObject(ByVal obj As Object)
        Try
            If obj IsNot Nothing Then
                Dim dispose As System.IDisposable
                dispose = obj
                If dispose IsNot Nothing Then
                    dispose.Dispose()
                    obj = Nothing
                End If
            End If
        Catch ex As Exception
            Dim myMessageBoxResponse As MsgBoxResult
            sw.WriteLine("ERROR - DisposeVciObjects_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            myMessageBoxResponse = MsgBox(ex.ToString, MsgBoxStyle.OkOnly, "ERROR - DisposeVciObjects")
            If myMessageBoxResponse = MsgBoxResult.Ok Then
                sw.WriteLine("ACK OK - DisposeVciObjects_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            End If
        End Try

    End Sub

    Private Sub getStatus1Timer_Tick(sender As Object, e As EventArgs) Handles getStatus1Timer.Tick

        If mCanCtl1 IsNot Nothing Then
            Dim lineStatus As Ixxat.Vci4.Bal.Can.CanLineStatus

            Try
                lineStatus = mCanCtl1.LineStatus

                If lineStatus.IsInInitMode Then
                    InitMode1Led.Value = False
                Else
                    InitMode1Led.Value = True
                End If

                If lineStatus.IsTransmitPending Then
                    txPending1Led.Value = False
                Else
                    txPending1Led.Value = True
                End If

                If lineStatus.HasDataOverrun Then
                    dataOverrun1Led.Value = False
                Else
                    dataOverrun1Led.Value = True
                End If

                If lineStatus.HasErrorOverrun Then
                    errorWarningLevel1Led.Value = False
                Else
                    errorWarningLevel1Led.Value = True
                End If

                If lineStatus.IsBusOff Then
                    busOff1Led.Value = False
                    CloseAll()
                    StartIxxat()
                Else
                    busOff1Led.Value = True

                End If
            Catch ex As Exception
                sw.WriteLine("ERROR - GetStatus1Timer_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                'MessageBox.Show(ex.ToString, "GetStatus1Timer ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If

    End Sub

    Private Sub VehicleStatusTimer_Tick(sender As Object, e As EventArgs) Handles VehicleStatusTimer.Tick

        Try
            Dim myTime As DateTime
            myTime = DateTime.Now
            If (myTime - Sevcon0.TimeStamp).Seconds < 5 And (myTime - Sevcon0.TimeStamp).Minutes = 0 Then
                RDControllerActive = True
                RDMotorActive = True
            Else
                RDControllerActive = False
                RDMotorActive = False
            End If
            If (myTime - Sevcon1.TimeStamp).Seconds < 5 And (myTime - Sevcon1.TimeStamp).Minutes = 0 Then
                RPControllerActive = True
                RPMotorActive = True
            Else
                RPControllerActive = False
                RPMotorActive = False
            End If
            If (myTime - Sevcon2.TimeStamp).Seconds < 5 And (myTime - Sevcon2.TimeStamp).Minutes = 0 Then
                FDControllerActive = True
                FDMotorActive = True
            Else
                FDControllerActive = False
                FDMotorActive = False
            End If
            If (myTime - Sevcon3.TimeStamp).Seconds < 5 And (myTime - Sevcon3.TimeStamp).Minutes = 0 Then
                FPControllerActive = True
                FPMotorActive = True
            Else
                FPControllerActive = False
                FPMotorActive = False
            End If
            If (myTime - Sevcon4.TimeStamp).Seconds < 5 And (myTime - Sevcon4.TimeStamp).Minutes = 0 Then
                GenControllerActive = True
                GenMotorActive = True
            Else
                GenControllerActive = False
                GenMotorActive = False
            End If
            If (myTime - Battery0.TimeStamp).Seconds < 5 And (myTime - Battery0.TimeStamp).Minutes = 0 Then
                DriverBatteryActive = True
            Else
                DriverBatteryActive = False
            End If
            If (myTime - Battery1.TimeStamp).Seconds < 5 And (myTime - Battery1.TimeStamp).Minutes = 0 Then
                PassengerBatteryActive = True
            Else
                PassengerBatteryActive = False
            End If
            If (myTime - Battery2.TimeStamp).Seconds < 5 And (myTime - Battery2.TimeStamp).Minutes = 0 Then
                MiddleBatteryActive = True
            Else
                MiddleBatteryActive = False
            End If
            If RDMotorActive Then
                If RDMotorError = False Then
                    If RDMotorWarning Then
                        RDMotorPictureBox.BackColor = WarningComponentColor
                    Else
                        RDMotorPictureBox.BackColor = ActiveComponentColor
                    End If
                Else
                    RDMotorPictureBox.BackColor = ErrorComponentColor
                End If
            Else
                RDMotorPictureBox.BackColor = InactiveComponentColor
            End If
            If RPMotorActive Then
                If RPMotorError = False Then
                    If RPMotorWarning Then
                        RPMotorPictureBox.BackColor = WarningComponentColor
                    Else
                        RPMotorPictureBox.BackColor = ActiveComponentColor
                    End If
                Else
                    RPMotorPictureBox.BackColor = ErrorComponentColor
                End If
            Else
                RPMotorPictureBox.BackColor = InactiveComponentColor
            End If
            If FDMotorActive Then
                If FDMotorError = False Then
                    If FDMotorWarning Then
                        FDMotorPictureBox.BackColor = WarningComponentColor
                    Else
                        FDMotorPictureBox.BackColor = ActiveComponentColor
                    End If
                Else
                    FDMotorPictureBox.BackColor = ErrorComponentColor
                End If
            Else
                FDMotorPictureBox.BackColor = InactiveComponentColor
            End If
            If FPMotorActive Then
                If FPMotorError = False Then
                    If RPMotorWarning Then
                        FPMotorPictureBox.BackColor = WarningComponentColor
                    Else
                        FPMotorPictureBox.BackColor = ActiveComponentColor
                    End If
                Else
                    FPMotorPictureBox.BackColor = ErrorComponentColor
                End If
            Else
                FPMotorPictureBox.BackColor = InactiveComponentColor
            End If
            If GenMotorActive = True Or CanIO0.DigitalInput2 = True Then
                If minSOCActiveSlide.Enabled = True Then
                    minSOCActiveSlide.Enabled = False
                End If
                If maxSOCActiveSlide.Enabled = True Then
                    maxSOCActiveSlide.Enabled = False
                End If
                If GenCurrentMeter.Enabled = False Then
                    GenCurrentMeter.Enabled = True
                End If
                If GenVoltageMeter.Enabled = False Then
                    GenVoltageMeter.Enabled = True
                End If
                If genTorqueMeter.Enabled = False Then
                    genTorqueMeter.Enabled = True
                End If
                If genPowerMeter.Enabled = False Then
                    genPowerMeter.Enabled = True
                End If
                If genControllerOnlineLed.Enabled = False Then
                    genControllerOnlineLed.Enabled = True
                End If
                If GenMotorThermometer.Enabled = False Then
                    GenMotorThermometer.Enabled = True
                End If
                If GenControllerThermometer.Enabled = False Then
                    GenMotorThermometer.Enabled = True
                End If
                If GenMotorError = False Then
                    If GenMotorWarning Then
                        GenMotorPictureBox.BackColor = WarningComponentColor
                    Else
                        GenMotorPictureBox.BackColor = ActiveComponentColor
                    End If
                Else
                    GenMotorPictureBox.BackColor = ErrorComponentColor
                End If
            Else
                GenMotorPictureBox.BackColor = InactiveComponentColor
                If minSOCActiveSlide.Enabled = False Then
                    minSOCActiveSlide.Enabled = True
                End If
                If maxSOCActiveSlide.Enabled = False Then
                    maxSOCActiveSlide.Enabled = True
                End If
                If GenCurrentMeter.Enabled = True Then
                    GenCurrentMeter.Enabled = False
                End If
                If GenVoltageMeter.Enabled = True Then
                    GenVoltageMeter.Enabled = False
                End If
                If genTorqueMeter.Enabled = True Then
                    genTorqueMeter.Enabled = False
                End If
                If genPowerMeter.Enabled = True Then
                    genPowerMeter.Enabled = False
                End If
                If genControllerOnlineLed.Enabled = True Then
                    genControllerOnlineLed.Enabled = False
                End If
                If GenMotorThermometer.Enabled = True Then
                    GenMotorThermometer.Enabled = False
                End If
                If GenControllerThermometer.Enabled = True Then
                    GenMotorThermometer.Enabled = False
                End If
            End If
            If GenControllerActive Then
                If GenControllerError = False Then
                    GenControllerPictureBox.BackColor = ActiveComponentColor
                Else
                    GenControllerPictureBox.BackColor = ErrorComponentColor
                End If
            Else
                GenControllerPictureBox.BackColor = InactiveComponentColor
            End If
            If RDControllerActive Then
                If RDControllerError = False Then
                    RDControllerPictureBox.BackColor = ActiveComponentColor
                Else
                    RDControllerPictureBox.BackColor = ErrorComponentColor
                End If
            Else
                RDControllerPictureBox.BackColor = InactiveComponentColor
            End If
            If RPControllerActive Then
                If RPControllerError = False Then
                    RPControllerPictureBox.BackColor = ActiveComponentColor
                Else
                    RPControllerPictureBox.BackColor = ErrorComponentColor
                End If
            Else
                RPControllerPictureBox.BackColor = InactiveComponentColor
            End If
            If FDControllerActive Then
                If FDControllerError = False Then
                    FDControllerPictureBox.BackColor = ActiveComponentColor
                Else
                    RDControllerPictureBox.BackColor = ErrorComponentColor
                End If
            Else
                FDControllerPictureBox.BackColor = InactiveComponentColor
            End If
            If FPControllerActive Then
                If FPControllerError = False Then
                    FPControllerPictureBox.BackColor = ActiveComponentColor
                Else
                    FPControllerPictureBox.BackColor = ErrorComponentColor
                End If
            Else
                FPControllerPictureBox.BackColor = InactiveComponentColor
            End If
            If RearGearboxActive Then
                If RearGearboxError = False Then
                    RearGearboxPictureBox.BackColor = ActiveComponentColor
                Else
                    RearGearboxPictureBox.BackColor = ErrorComponentColor
                End If
            Else
                RearGearboxPictureBox.BackColor = InactiveComponentColor
            End If
            If FrontGearboxActive Then
                If FrontGearboxError = False Then
                    FrontGearboxPictureBox.BackColor = ActiveComponentColor
                Else
                    FrontGearboxPictureBox.BackColor = ErrorComponentColor
                End If
            Else
                FrontGearboxPictureBox.BackColor = InactiveComponentColor
            End If
            If DriverBatteryActive Then
                If DriverBatteryError = False Then
                    DriverBatteryPictureBox.BackColor = ActiveComponentColor
                Else
                    DriverBatteryPictureBox.BackColor = ErrorComponentColor
                End If
            Else
                DriverBatteryPictureBox.BackColor = InactiveComponentColor
            End If
            If MiddleBatteryActive Then
                If MiddleBatteryError = False Then
                    MiddleBatteryPictureBox.BackColor = ActiveComponentColor
                Else
                    MiddleBatteryPictureBox.BackColor = ErrorComponentColor
                End If
            Else
                MiddleBatteryPictureBox.BackColor = InactiveComponentColor
            End If
            If PassengerBatteryActive Then
                If PassengerBatteryError = False Then
                    PassengerBatteryPictureBox.BackColor = ActiveComponentColor
                Else
                    PassengerBatteryPictureBox.BackColor = ErrorComponentColor
                End If
            Else
                PassengerBatteryPictureBox.BackColor = InactiveComponentColor
            End If
            If EngineActive Then
                If GenEngineError = False Then
                    EnginePictureBox.BackColor = ActiveComponentColor
                Else
                    EnginePictureBox.BackColor = ErrorComponentColor
                End If
            Else
                EnginePictureBox.BackColor = InactiveComponentColor
            End If

            Dim myInt As Integer = 0
            Dim numOfBatteriesConnected As Integer = 0
            If DriverBatteryActive = True Then
                numOfBatteriesConnected += 1
                myInt += Battery0.SoC
            End If
            If PassengerBatteryActive = True Then
                numOfBatteriesConnected += 1
                myInt += Battery1.SoC
            End If
            If MiddleBatteryActive = True Then
                numOfBatteriesConnected += 1
                myInt += Battery2.SoC
            End If
            Dim mySOC As Double
            If numOfBatteriesConnected <> 0 Then
                mySOC = Convert.ToDouble(myInt / numOfBatteriesConnected)
                If generatorEngineRunning = True Then
                    If mySOC > My.Settings.genChargeActivateMax Or mySOC > 95 Then
                        generatorSOCEnabled = False         'Shut down generator if SOC is greater than maximum threshold
                    End If
                Else
                    If mySOC < My.Settings.genChargeActivateMin Then
                        generatorSOCEnabled = True          'Allow generator to start if SOC is less than minimum threshold
                    End If
                End If

                If mySOC >= batteryCapacityTank.Range.Minimum And mySOC <= batteryCapacityTank.Range.Maximum Then
                    batteryCapacityTank.Enabled = True
                    batteryCapacityTank.Value = mySOC
                    batteryCapacityTank.Caption = "Total Battery Capacity = " & batteryCapacityTank.Value.ToString("F2") & "%"
                    If EngineeringDisplayToolStripMenuItem.Checked Then
                        SecondarySOCTank.Enabled = True
                        SecondarySOCTank.Value = mySOC
                        SecondarySOCTank.Caption = "Total Battery Capacity = " & SecondarySOCTank.Value.ToString("F2") & "%"
                    End If
                Else
                    batteryCapacityTank.Enabled = False
                    SecondarySOCTank.Enabled = False
                End If
            End If

            If Battery0.StatusCodeContactorClosed = True And Battery1.StatusCodeContactorClosed = True And Battery2.StatusCodeContactorClosed = True Then
                batteryCapacityTank.FillColor = Color.LimeGreen
                SecondarySOCTank.FillColor = Color.LimeGreen
            Else
                batteryCapacityTank.FillColor = Color.Blue
                SecondarySOCTank.FillColor = Color.Blue
            End If
            'If currentDisplaySwitch.Value = True Then
            '    WaveformGraph1.YAxes(0).Mode = AxisMode.Fixed
            '    WaveformGraph1.XAxes(0).Mode = AxisMode.StripChart
            '    WaveformGraph1.YAxes(0).Range = New Range(-10, 450)
            '    WaveformGraph1.XAxes(0).Range = New Range(0, 60)
            '    If Sevcon0.BatteryCurrent < 500 Then
            '        RDWaveformPlot.PlotYAppend(Sevcon0.BatteryCurrent)
            '    End If
            '    If Sevcon1.BatteryCurrent < 500 Then
            '        RPWaveformPlot.PlotYAppend(Sevcon1.BatteryCurrent)
            '    End If
            '    If Sevcon2.BatteryCurrent < 500 Then
            '        FDWaveformPlot.PlotYAppend(Sevcon2.BatteryCurrent)
            '    End If
            '    If Sevcon3.BatteryCurrent < 500 Then
            '        FPWaveformPlot.PlotYAppend(Sevcon3.BatteryCurrent)
            '    End If

            '    WaveformGraph1.Update()
            'End If
            'If powerDisplaySwitch.Value = True Then
            '    WaveformGraph1.YAxes(0).Mode = AxisMode.Fixed
            '    WaveformGraph1.XAxes(0).Mode = AxisMode.StripChart
            '    WaveformGraph1.YAxes(0).Range = New Range(0, 100)
            '    WaveformGraph1.XAxes(0).Range = New Range(0, 60)
            '    If Sevcon0.BatteryCurrent < 500 Then
            '        RDWaveformPlot.PlotYAppend(Sevcon0.BatteryCurrent)
            '    End If
            '    If Sevcon1.BatteryCurrent < 500 Then
            '        RPWaveformPlot.PlotYAppend(Sevcon1.BatteryCurrent)
            '    End If
            '    If Sevcon2.BatteryCurrent < 500 Then
            '        FDWaveformPlot.PlotYAppend(Sevcon2.BatteryCurrent)
            '    End If
            '    If Sevcon3.BatteryCurrent < 500 Then
            '        FPWaveformPlot.PlotYAppend(Sevcon3.BatteryCurrent)
            '    End If

            '    WaveformGraph1.Update()
            'End If
            'If torqueDisplaySwitch.Value = True Then

            'End If

            Dim mySpeed As Double
            mySpeed = ((Sevcon0.Velocity + Sevcon1.Velocity + Sevcon2.Velocity + Sevcon3.Velocity) / 4) * 0.00953846
            If mySpeed >= speedAnalogGauge.Range.Minimum And mySpeed <= speedAnalogGauge.Range.Maximum Then
                speedAnalogGauge.Enabled = True
                speedAnalogGauge.Value = Sevcon1.Velocity * 0.00953846
                speedDigitalLabel.Text = (Sevcon1.Velocity * 0.00953846).ToString("F1") & " MPH"
            Else
                speedAnalogGauge.Enabled = False
                speedDigitalLabel.Text = "N/A"
            End If

            Dim avgMiles As Single
            Dim Sevcon0CorrectedMiles As Single
            Dim Sevcon1CorrectedMiles As Single
            Dim Sevcon2CorrectedMiles As Single
            Dim Sevcon3CorrectedMiles As Single
            Sevcon0CorrectedMiles = (Sevcon0.OdometerTenthMiles / 10) * odometerMultiplier
            Sevcon1CorrectedMiles = (Sevcon1.OdometerTenthMiles / 10) * odometerMultiplier
            Sevcon2CorrectedMiles = ((Sevcon2.OdometerTenthMiles / 10) * odometerMultiplier) + vehicleMileageUpgrade
            Sevcon3CorrectedMiles = ((Sevcon3.OdometerTenthMiles / 10) * odometerMultiplier) + vehicleMileageUpgrade
            avgMiles = (Sevcon0CorrectedMiles + Sevcon1CorrectedMiles + Sevcon2CorrectedMiles + Sevcon3CorrectedMiles) / 4
            mileageLabel.Text = avgMiles.ToString("F1") & " mi. (M)"

            Dim maxHours As Integer
            If Sevcon0.KeyOnHours > maxHours Then
                maxHours = Sevcon0.KeyOnHours
            End If
            If Sevcon1.KeyOnHours > maxHours Then
                maxHours = Sevcon1.KeyOnHours
            End If
            If Sevcon2.KeyOnHours > maxHours Then
                maxHours = Sevcon2.KeyOnHours
            End If
            If Sevcon3.KeyOnHours > maxHours Then
                maxHours = Sevcon3.KeyOnHours
            End If
            keyOnHoursLabel.Text = maxHours.ToString("F0") & " hrs."

            If Trip1ToolStripMenuItem.Checked Then
                mileageLabel.Text = (avgMiles - My.Settings.trip1OdometerStartMiles).ToString("F1") & " mi. (1)"
            End If
            If Trip2ToolStripMenuItem.Checked Then
                mileageLabel.Text = (avgMiles - My.Settings.trip2OdometerStartMiles).ToString("F1") & " mi. (2)"
            End If
            If Trip3ToolStripMenuItem.Checked Then
                mileageLabel.Text = (avgMiles - My.Settings.trip3OdometerStartMiles).ToString("F1") & " mi. (3)"
            End If

            Dim myPower1 As Single
            Dim myPower2 As Single
            Dim myPower3 As Single
            Dim myNetPower As Single

            If BatteryTempGroupBox.Visible = True Then
                Select Case BatteryTempGroupBox.Text
                    Case Is = "Driver Battery Temperatures"
                        PackTemp0Thermometer.Value = Battery0.PackSensorTempC(0)
                        PackTemp1Thermometer.Value = Battery0.PackSensorTempC(1)
                        PackTemp2Thermometer.Value = Battery0.PackSensorTempC(2)
                        PackTemp3Thermometer.Value = Battery0.PackSensorTempC(3)
                        PackTemp4Thermometer.Value = Battery0.PackSensorTempC(4)
                        PackTemp5Thermometer.Value = Battery0.PackSensorTempC(5)
                        PackTemp6Thermometer.Value = Battery0.PackSensorTempC(6)
                        PackTemp7Thermometer.Value = Battery0.PackSensorTempC(7)
                    Case Is = "Passenger Battery Temperatures"
                        PackTemp0Thermometer.Value = Battery1.PackSensorTempC(0)
                        PackTemp1Thermometer.Value = Battery1.PackSensorTempC(1)
                        PackTemp2Thermometer.Value = Battery1.PackSensorTempC(2)
                        PackTemp3Thermometer.Value = Battery1.PackSensorTempC(3)
                        PackTemp4Thermometer.Value = Battery1.PackSensorTempC(4)
                        PackTemp5Thermometer.Value = Battery1.PackSensorTempC(5)
                        PackTemp6Thermometer.Value = Battery1.PackSensorTempC(6)
                        PackTemp7Thermometer.Value = Battery1.PackSensorTempC(7)
                    Case Is = "Middle Battery Temperatures"
                        PackTemp0Thermometer.Value = Battery2.PackSensorTempC(0)
                        PackTemp1Thermometer.Value = Battery2.PackSensorTempC(1)
                        PackTemp2Thermometer.Value = Battery2.PackSensorTempC(2)
                        PackTemp3Thermometer.Value = Battery2.PackSensorTempC(3)
                        PackTemp4Thermometer.Value = Battery2.PackSensorTempC(4)
                        PackTemp5Thermometer.Value = Battery2.PackSensorTempC(5)
                        PackTemp6Thermometer.Value = Battery2.PackSensorTempC(6)
                        PackTemp7Thermometer.Value = Battery2.PackSensorTempC(7)
                End Select
                PackTemp0Label.Text = PackTemp0Thermometer.Value.ToString("F0") & " C"
                PackTemp1Label.Text = PackTemp1Thermometer.Value.ToString("F0") & " C"
                PackTemp2Label.Text = PackTemp2Thermometer.Value.ToString("F0") & " C"
                PackTemp3Label.Text = PackTemp3Thermometer.Value.ToString("F0") & " C"
                PackTemp4Label.Text = PackTemp4Thermometer.Value.ToString("F0") & " C"
                PackTemp5Label.Text = PackTemp5Thermometer.Value.ToString("F0") & " C"
                PackTemp6Label.Text = PackTemp6Thermometer.Value.ToString("F0") & " C"
                PackTemp7Label.Text = PackTemp7Thermometer.Value.ToString("F0") & " C"
            End If

            myPower1 = (Battery0.PackVoltage * Battery0.PackDischageAmps) / 1000
            myPower2 = (Battery1.PackVoltage * Battery1.PackDischageAmps) / 1000
            myPower3 = (Battery2.PackVoltage * Battery2.PackDischageAmps) / 1000
            myNetPower = myPower1 + myPower2 + myPower3
            netPowerLabel.Text = (myNetPower * -1).ToString("F3") & " kW"
            If Engineering1TabControl.Visible = True Then
                averageVehiclePowerLabel.Text = (myNetPower * -1).ToString("F3") & " kW"
            End If

            If generatorEnabled Then
                fuelCapacityTank.FillColor = Color.LimeGreen
            Else
                fuelCapacityTank.FillColor = Color.Blue
            End If

            Dim sumOfFuelReadings As Single

            If fuelReadingDataLooped = True Then
                fuelCapacityTank.Caption = "Fuel Capacity = " & fuelCapacityTank.Value.ToString("F2") & "%"
                fuelCapacityTank.Enabled = True
                For intI As Integer = 1 To 100
                    sumOfFuelReadings += fuelReading(intI)
                Next
                currentFuelVoltage = sumOfFuelReadings / 100

                Select Case currentFuelVoltage
                    Case Is <= emptyFuelVoltage
                        fuelCapacityTank.Value = 0
                    Case Is > emptyFuelVoltage
                        If currentFuelVoltage < fullFuelVoltage Then
                            Dim myTempValue1 As Single
                            Dim myTempValue2 As Single
                            Dim myTempValue3 As Single
                            myTempValue1 = fullFuelVoltage - emptyFuelVoltage
                            myTempValue2 = currentFuelVoltage - myTempValue1
                            myTempValue3 = (myTempValue2 / myTempValue1) * 100
                            If myTempValue3 >= fuelCapacityTank.Range.Minimum And myTempValue3 <= fuelCapacityTank.Range.Maximum Then
                                fuelCapacityTank.Value = myTempValue3
                            End If
                        Else
                            fuelCapacityTank.Value = 100
                        End If
                End Select
            Else
                If calculatingFuelIndicatorBar < 5 Then
                    calculatingFuelIndicatorBar += 1
                Else
                    calculatingFuelIndicatorBar = 1
                End If
                Select Case calculatingFuelIndicatorBar
                    Case Is = 1
                        fuelCapacityTank.Caption = "Fuel Capacity (Calculating.    )"
                    Case Is = 2
                        fuelCapacityTank.Caption = "Fuel Capacity (Calculating..   )"
                    Case Is = 3
                        fuelCapacityTank.Caption = "Fuel Capacity (Calculating...  )"
                    Case Is = 4
                        fuelCapacityTank.Caption = "Fuel Capacity (Calculating.... )"
                    Case Is = 5
                        fuelCapacityTank.Caption = "Fuel Capacity (Calculating.....)"
                End Select
            End If



        Catch ex As Exception
            sw.WriteLine("ERROR - VehicleStatusTimer_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            'MessageBox.Show(ex.ToString, "VehicleStatusTimer ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub getStatus2Timer_Tick(sender As Object, e As EventArgs) Handles getStatus2Timer.Tick
        If mCanCtl2 IsNot Nothing Then
            Dim lineStatus As Ixxat.Vci4.Bal.Can.CanLineStatus

            Try
                lineStatus = mCanCtl2.LineStatus

                If lineStatus.IsInInitMode Then
                    InitMode2Led.Value = False
                Else
                    InitMode2Led.Value = True
                End If

                If lineStatus.IsTransmitPending Then
                    txPending2Led.Value = False
                Else
                    txPending2Led.Value = True
                End If

                If lineStatus.HasDataOverrun Then
                    dataOverrun2Led.Value = False
                Else
                    dataOverrun2Led.Value = True
                End If

                If lineStatus.HasErrorOverrun Then
                    errorWarningLevel2Led.Value = False
                Else
                    errorWarningLevel2Led.Value = True
                End If

                If lineStatus.IsBusOff Then
                    busOff2Led.Value = False
                    CloseAll()
                    StartIxxat()

                Else
                    busOff2Led.Value = True

                End If
            Catch ex As Exception
                sw.WriteLine("ERROR - GetStatus2Timer_Exception - " & ex.Message.ToString() & DateTime.Today.ToString)
                MessageBox.Show(ex.ToString, "GetStatus2Timer ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub CANInformationToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CANInformationToolStripMenuItem.Click
        If CANInformationToolStripMenuItem.Checked Then
            CANInformationToolStripMenuItem.Checked = False
            CANInfoGroupBox.Visible = False
        Else
            CANInformationToolStripMenuItem.Checked = True
            vehiclePerformanceGroupBox.Visible = False
            VehiclePerformanceToolStripMenuItem.Checked = False
            vehicleSupervisorGroupBox.Visible = False
            VehicleControlToolStripMenuItem.Checked = False
            CANInfoGroupBox.Visible = True
            '516,43
            Dim myPoint As Point
            myPoint.X = 516
            myPoint.Y = 43
            CANInfoGroupBox.Location = myPoint
        End If
    End Sub
    Private Sub ShowVehicleOverviewComponents(ByRef State As Boolean)

        Try
            RDControllerPictureBox.Visible = State
            RPControllerPictureBox.Visible = State
            FDControllerPictureBox.Visible = State
            FPControllerPictureBox.Visible = State
            RDMotorPictureBox.Visible = State
            RPMotorPictureBox.Visible = State
            FDMotorPictureBox.Visible = State
            FPMotorPictureBox.Visible = State
            DriverBatteryPictureBox.Visible = State
            MiddleBatteryPictureBox.Visible = State
            PassengerBatteryPictureBox.Visible = State
            RearGearboxPictureBox.Visible = State
            FrontGearboxPictureBox.Visible = State
            GenMotorPictureBox.Visible = State
            GenControllerPictureBox.Visible = State
            EnginePictureBox.Visible = State
            vehiclePerformanceTank.Visible = State
            Me.Refresh()
        Catch ex As Exception
            sw.WriteLine("ERROR - ShowVehicleOverviewComponents_Exception - " & ex.Message.ToString() & DateTime.Today.ToString)
            'MessageBox.Show(ex.ToString, "ShowVehicleOverviewComponents ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub
    Private Sub VehicleOverviewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles VehicleOverviewToolStripMenuItem.Click
        If VehicleOverviewToolStripMenuItem.Checked Then
            VehicleOverviewToolStripMenuItem.Checked = False
            vehicleOverviewPictureBox.Visible = False
            ShowVehicleOverviewComponents(False)
        Else
            VehicleOverviewToolStripMenuItem.Checked = True
            vehicleOverviewPictureBox.Visible = True
            ShowVehicleOverviewComponents(True)
        End If
    End Sub

    Private Sub AnalogToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AnalogToolStripMenuItem.Click
        speedAnalogGauge.Visible = True
        AnalogToolStripMenuItem.Checked = True
        DigitalToolStripMenuItem.Checked = False
        speedDigitalLabel.Visible = False
    End Sub

    Private Sub DigitalToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DigitalToolStripMenuItem.Click
        speedDigitalLabel.Visible = True
        AnalogToolStripMenuItem.Checked = False
        DigitalToolStripMenuItem.Checked = True
        speedAnalogGauge.Visible = False
    End Sub

    Private Sub EngineeringDisplayToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EngineeringDisplayToolStripMenuItem.Click
        If EngineeringDisplayToolStripMenuItem.Checked Then
            EngineeringDisplayToolStripMenuItem.Checked = False
            Engineering1GroupBox.Visible = False
        Else
            EngineeringDisplayToolStripMenuItem.Checked = True
            Engineering1GroupBox.Visible = True
            Dim myPoint As Point
            myPoint.X = 12
            myPoint.Y = 46
            Engineering1GroupBox.Location = myPoint
        End If
    End Sub

    Private Sub DriverBatteryThermometer_Click(sender As Object, e As EventArgs) Handles DriverBatteryThermometer.Click
        Dim Position As Point
        Position.X = 6
        Position.Y = 6

        MotorTempPanel.Visible = False
        ControllerTempPanel.Visible = False
        GearboxHeatsinkTempPanel.Visible = False
        BatteryGeneratorTempPanel.Visible = False
        AmbientPanel.Visible = False
        With BatteryTempGroupBox
            .Text = "Driver Battery Temperatures"
            .Location = Position
            .Visible = True
        End With

    End Sub

    Private Sub closeTempPanelButton_Click(sender As Object, e As EventArgs) Handles closeTempPanelButton.Click
        BatteryTempGroupBox.Visible = False
        MotorTempPanel.Visible = True
        ControllerTempPanel.Visible = True
        GearboxHeatsinkTempPanel.Visible = True
        BatteryGeneratorTempPanel.Visible = True
        AmbientPanel.Visible = True
    End Sub

    Private Sub MiddleBatteryThermometer_Click(sender As Object, e As EventArgs) Handles MiddleBatteryThermometer.Click
        Dim Position As Point
        Position.X = 6
        Position.Y = 6

        MotorTempPanel.Visible = False
        ControllerTempPanel.Visible = False
        GearboxHeatsinkTempPanel.Visible = False
        BatteryGeneratorTempPanel.Visible = False
        AmbientPanel.Visible = False
        With BatteryTempGroupBox
            .Text = "Middle Battery Temperatures"
            .Location = Position
            .Visible = True
        End With
    End Sub

    Private Sub PassengerBatteryThermometer_Click(sender As Object, e As EventArgs) Handles PassengerBatteryThermometer.Click
        Dim Position As Point
        Position.X = 6
        Position.Y = 6

        MotorTempPanel.Visible = False
        ControllerTempPanel.Visible = False
        GearboxHeatsinkTempPanel.Visible = False
        BatteryGeneratorTempPanel.Visible = False
        AmbientPanel.Visible = False
        With BatteryTempGroupBox
            .Text = "Passenger Battery Temperatures"
            .Location = Position
            .Visible = True
        End With
    End Sub

    Private Sub dataUpdateTimer_Tick(sender As Object, e As EventArgs) Handles dataUpdateTimer.Tick

        Try
            If Engineering1TabControl.Visible Or DataLoggingActive Then
                If DataLoggingActive Then
                    oInt += 1
                End If
                If CanIO1.AnalogInput1 >= throttle1Gauge.Range.Minimum And CanIO1.AnalogInput1 <= throttle1Gauge.Range.Maximum Then
                    throttle1Gauge.Enabled = True
                    throttle1Gauge.Value = CanIO1.AnalogInput1
                Else
                    throttle1Gauge.Enabled = False
                End If
                If CanIO1.AnalogInput2 >= throttle2Gauge.Range.Minimum And CanIO1.AnalogInput2 <= throttle2Gauge.Range.Maximum Then
                    throttle2Gauge.Enabled = True
                    throttle2Gauge.Value = CanIO1.AnalogInput2
                Else
                    throttle2Gauge.Enabled = False
                End If
                If CanIO1.PowerSupplyVoltage >= frontBatteryVoltageMeter.Range.Minimum And CanIO1.PowerSupplyVoltage <= frontBatteryVoltageMeter.Range.Maximum Then
                    frontBatteryVoltageMeter.Enabled = True
                    frontBatteryVoltageMeter.Value = CanIO1.PowerSupplyVoltage
                Else
                    frontBatteryVoltageMeter.Enabled = False
                End If
                If CanIO0.PowerSupplyVoltage >= rearBatteryVoltageMeter.Range.Minimum And CanIO0.PowerSupplyVoltage <= rearBatteryVoltageMeter.Range.Maximum Then
                    rearBatteryVoltageMeter.Enabled = True
                    rearBatteryVoltageMeter.Value = CanIO0.PowerSupplyVoltage
                Else
                    rearBatteryVoltageMeter.Enabled = False
                End If
                forwardSwitch.Value = CanIO1.DigitalInput1
                reverseSwitch.Value = CanIO1.DigitalInput2
                vehicleOnSwitch.Value = CanIO0.DigitalInput1
                If DataLoggingActive Then
                    oSheet.Cells(oInt, 3).value = vehicleOnSwitch.Value.ToString
                End If
                brakeLightsSwitch.Value = CanIO1.DigitalInput3
                If DataLoggingActive Then
                    oSheet.Cells(oInt, 9).value = brakeLightsSwitch.Value.ToString
                End If
                headlightSwitch.Value = CanIO1.DigitalInput4
                generatorSwitch.Value = CanIO0.DigitalInput2
                If Battery0.HighestPackTempC >= DriverBatteryThermometer.Range.Minimum And Battery0.HighestPackTempC <= DriverBatteryThermometer.Range.Maximum Then
                    DriverBatteryThermometer.Enabled = True
                    DriverBatteryThermometer.Value = Battery0.HighestPackTempC
                    DriverBatteryTempLabel.Text = Battery0.HighestPackTempC.ToString("F1") & " C"
                Else
                    DriverBatteryThermometer.Enabled = False
                    DriverBatteryTempLabel.Text = "N/A"
                End If
                If DataLoggingActive Then
                    oSheet.Cells(oInt, 35).value = Battery0.HighestPackTempC.ToString
                End If
                If Battery1.HighestPackTempC >= PassengerBatteryThermometer.Range.Minimum And Battery1.HighestPackTempC <= PassengerBatteryThermometer.Range.Maximum Then
                    PassengerBatteryThermometer.Enabled = True
                    PassengerBatteryThermometer.Value = Battery1.HighestPackTempC
                    PassengerBatteryTempLabel.Text = Battery1.HighestPackTempC.ToString("F1") & " C"
                Else
                    PassengerBatteryThermometer.Enabled = False
                    PassengerBatteryTempLabel.Text = "N/A"
                End If
                If DataLoggingActive Then
                    oSheet.Cells(oInt, 37).value = Battery1.HighestPackTempC.ToString
                End If
                If Battery2.HighestPackTempC >= MiddleBatteryThermometer.Range.Minimum And Battery2.HighestPackTempC <= MiddleBatteryThermometer.Range.Maximum Then
                    MiddleBatteryThermometer.Enabled = True
                    MiddleBatteryThermometer.Value = Battery2.HighestPackTempC
                    MiddleBatteryTempLabel.Text = Battery2.HighestPackTempC.ToString("F1") & " C"
                Else
                    MiddleBatteryThermometer.Enabled = False
                    MiddleBatteryTempLabel.Text = "N/A"
                End If
                If DataLoggingActive Then
                    oSheet.Cells(oInt, 36).value = Battery2.HighestPackTempC.ToString
                End If
                If Sevcon2.ControllerHeatsinkTemp >= FDControllerThermometer.Range.Minimum And Sevcon2.ControllerHeatsinkTemp <= FDControllerThermometer.Range.Maximum Then
                    FDControllerThermometer.Enabled = True
                    FDControllerThermometer.Value = Sevcon2.ControllerHeatsinkTemp
                    FDControllerTempLabel.Text = Sevcon2.ControllerHeatsinkTemp.ToString("F1") & " C"
                Else
                    FDControllerThermometer.Enabled = False
                    FDControllerTempLabel.Text = "N/A"
                End If
                If DataLoggingActive Then
                    oSheet.Cells(oInt, 40).value = Sevcon2.ControllerHeatsinkTemp.ToString
                End If
                If Sevcon2.MotorTemp >= FDMotorThermometer.Range.Minimum And Sevcon2.MotorTemp <= FDMotorThermometer.Range.Maximum Then
                    FDMotorThermometer.Enabled = True
                    FDMotorThermometer.Value = Sevcon2.MotorTemp
                    FDMotorTempLabel.Text = Sevcon2.MotorTemp.ToString("F1") & " C"
                Else
                    FDMotorThermometer.Enabled = False
                    FDMotorTempLabel.Text = "N/A"
                End If
                If DataLoggingActive Then
                    oSheet.Cells(oInt, 44).value = Sevcon2.MotorTemp.ToString
                End If
                If Sevcon3.ControllerHeatsinkTemp >= FDControllerThermometer.Range.Minimum And Sevcon3.ControllerHeatsinkTemp <= FDControllerThermometer.Range.Maximum Then
                    FPControllerThermometer.Enabled = True
                    FPControllerThermometer.Value = Sevcon3.ControllerHeatsinkTemp
                    FPControllerTempLabel.Text = Sevcon3.ControllerHeatsinkTemp.ToString("F1") & " C"
                Else
                    FPControllerThermometer.Enabled = False
                    FPControllerTempLabel.Text = "N/A"
                End If
                If DataLoggingActive Then
                    oSheet.Cells(oInt, 41).value = Sevcon3.ControllerHeatsinkTemp.ToString
                End If
                If Sevcon3.MotorTemp >= FPMotorThermometer.Range.Minimum And Sevcon3.MotorTemp <= FPMotorThermometer.Range.Maximum Then
                    FPMotorThermometer.Enabled = True
                    FPMotorThermometer.Value = Sevcon3.MotorTemp
                    FPMotorTempLabel.Text = Sevcon3.MotorTemp.ToString("F1") & " C"
                Else
                    FPMotorThermometer.Enabled = False
                    FPMotorTempLabel.Text = "N/A"
                End If
                If Sevcon4.ControllerHeatsinkTemp >= GenControllerThermometer.Range.Minimum And Sevcon4.ControllerHeatsinkTemp <= GenControllerThermometer.Range.Maximum Then
                    'GenControllerThermometer.Enabled = True
                    GenControllerThermometer.Value = Sevcon4.ControllerHeatsinkTemp
                    GenControllerTempLabel.Text = Sevcon4.ControllerHeatsinkTemp.ToString("F1") & " C"
                Else
                    'GenControllerThermometer.Enabled = False
                    GenControllerTempLabel.Text = "N/A"
                End If
                Dim myCurrent As Single
                myCurrent = Sevcon4.BatteryCurrent * -1
                If myCurrent >= GenCurrentMeter.Range.Minimum And myCurrent <= GenCurrentMeter.Range.Maximum Then
                    'GenCurrentMeter.Enabled = True
                    GenCurrentMeter.Value = myCurrent
                    GenCurrentLabel.Text = myCurrent.ToString("F2") & " A"
                Else
                    'GenCurrentMeter.Enabled = False
                    GenCurrentLabel.Text = Sevcon4.BatteryCurrent.ToString("F2") & " A"
                End If
                If Sevcon4.BatteryVoltage >= GenVoltageMeter.Range.Minimum And Sevcon4.BatteryVoltage <= GenVoltageMeter.Range.Maximum Then
                    'GenVoltageMeter.Enabled = True
                    GenVoltageMeter.Value = Sevcon4.BatteryVoltage
                    GenVoltageLabel.Text = Sevcon4.BatteryVoltage.ToString("F2") & " V"
                Else
                    'GenVoltageMeter.Enabled = False
                    GenVoltageLabel.Text = "N/A"
                End If
                If Sevcon4.MotorTemp >= GenMotorThermometer.Range.Minimum And Sevcon4.MotorTemp <= GenMotorThermometer.Range.Maximum Then
                    'GenMotorThermometer.Enabled = True
                    GenMotorThermometer.Value = Sevcon4.MotorTemp
                    GenMotorTempLabel.Text = Sevcon4.MotorTemp.ToString("F1") & " C"
                Else
                    'GenMotorThermometer.Enabled = False
                    GenMotorTempLabel.Text = "N/A"
                End If
                If Sevcon4.Torque >= genTorqueMeter.Range.Minimum And (Sevcon4.Torque) <= genTorqueMeter.Range.Maximum Then
                    'genTorqueMeter.Enabled = True
                    genTorqueMeter.Value = Sevcon4.Torque
                    GenTorqueLabel.Text = Sevcon4.Torque.ToString("F2") & " N-m"
                Else
                    'genTorqueMeter.Enabled = False
                    GenTorqueLabel.Text = "N/A"
                End If
                If GenCurrentMeter.Enabled And GenVoltageMeter.Enabled Then
                    'genPowerMeter.Enabled = True
                    genPowerMeter.Value = ((GenCurrentMeter.Value * GenVoltageMeter.Value) / 1000)
                    GenPowerLabel.Text = genPowerMeter.Value.ToString("F2") & " kW"
                Else
                    'genPowerMeter.Enabled = False
                    GenPowerLabel.Text = "N/A"
                End If
                GenRPMLabel.Text = Sevcon4.Velocity.ToString("F0") & " RPM"
                If Sevcon0.ControllerHeatsinkTemp >= RDControllerThermometer.Range.Minimum And Sevcon0.ControllerHeatsinkTemp <= RDControllerThermometer.Range.Maximum Then
                    RDControllerThermometer.Value = Sevcon0.ControllerHeatsinkTemp
                    RDControllerTempLabel.Text = Sevcon0.ControllerHeatsinkTemp.ToString("F1") & " C"
                End If
                If DataLoggingActive Then
                    oSheet.Cells(oInt, 38).value = Sevcon0.ControllerHeatsinkTemp.ToString
                End If
                If Sevcon0.MotorTemp >= RDMotorThermometer.Range.Minimum And Sevcon0.MotorTemp <= RDMotorThermometer.Range.Maximum Then
                    RDMotorThermometer.Enabled = True
                    RDMotorThermometer.Value = Sevcon0.MotorTemp
                    RDMotorTempLabel.Text = Sevcon0.MotorTemp.ToString("F1") & " C"
                Else
                    RDMotorThermometer.Enabled = False
                    RDMotorTempLabel.Text = "N/A"
                End If
                If DataLoggingActive Then
                    oSheet.Cells(oInt, 42).value = Sevcon0.MotorTemp.ToString
                End If
                If Sevcon1.ControllerHeatsinkTemp >= RPControllerThermometer.Range.Minimum And Sevcon1.ControllerHeatsinkTemp <= RPControllerThermometer.Range.Maximum Then
                    RPControllerThermometer.Enabled = True
                    RPControllerThermometer.Value = Sevcon1.ControllerHeatsinkTemp
                    RPControllerTempLabel.Text = Sevcon1.ControllerHeatsinkTemp.ToString("F1") & " C"
                Else
                    RPControllerThermometer.Enabled = False
                    RPControllerTempLabel.Text = "N/A"
                End If
                If DataLoggingActive Then
                    oSheet.Cells(oInt, 39).value = Sevcon1.ControllerHeatsinkTemp.ToString
                End If
                If Sevcon1.MotorTemp >= RPMotorThermometer.Range.Minimum And Sevcon1.MotorTemp <= RPMotorThermometer.Range.Maximum Then
                    RPMotorThermometer.Enabled = True
                    RPMotorThermometer.Value = Sevcon1.MotorTemp
                    RPMotorTempLabel.Text = Sevcon1.MotorTemp.ToString("F1") & " C"
                Else
                    RPMotorThermometer.Enabled = False
                    RPMotorTempLabel.Text = "N/A"
                End If
                If DataLoggingActive Then
                    oSheet.Cells(oInt, 43).value = Sevcon1.MotorTemp.ToString
                End If
                If Battery0.SoC >= driverBatteryTank.Range.Minimum And Battery0.SoC <= driverBatteryTank.Range.Maximum Then
                    driverBatteryTank.Enabled = True
                    driverBatteryTank.Value = Battery0.SoC
                Else
                    driverBatteryTank.Enabled = False
                End If
                If Battery0.StatusCodeContactorClosed = True Then
                    driverBatteryTank.FillColor = Color.LimeGreen
                Else
                    driverBatteryTank.FillColor = Color.Blue
                End If
                driverbatteryVoltageLabel.Text = Battery0.PackVoltage.ToString("F2") & " V"
                driverBatteryCurrentLabel.Text = Battery0.PackDischageAmps.ToString("F2") & " A"
                If DataLoggingActive Then
                    oSheet.Cells(oInt, 17).value = Battery0.PackVoltage.ToString
                    oSheet.Cells(oInt, 20).value = Battery0.PackDischageAmps.ToString
                End If
                driverBatteryPowerLabel.Text = ((Battery0.PackVoltage * Battery0.PackDischageAmps) / 1000).ToString("F2") & " kW"
                If Battery2.SoC >= middleBatteryTank.Range.Minimum And Battery2.SoC <= middleBatteryTank.Range.Maximum Then
                    middleBatteryTank.Enabled = True
                    middleBatteryTank.Value = Battery2.SoC
                Else
                    middleBatteryTank.Enabled = False
                End If
                If Battery2.StatusCodeContactorClosed = True Then
                    middleBatteryTank.FillColor = Color.LimeGreen
                Else
                    middleBatteryTank.FillColor = Color.Blue
                End If
                middleBatteryVoltageLabel.Text = Battery2.PackVoltage.ToString("F2") & " V"
                middleBatteryCurrentLabel.Text = Battery2.PackDischageAmps.ToString("F2") & " A"
                If DataLoggingActive Then
                    oSheet.Cells(oInt, 18).value = Battery2.PackVoltage.ToString
                    oSheet.Cells(oInt, 21).value = Battery2.PackDischageAmps.ToString
                End If
                middleBatteryPowerLabel.Text = ((Battery2.PackVoltage * Battery2.PackDischageAmps) / 1000).ToString("F2") & " kW"
                If Battery1.SoC >= passengerBatteryTank.Range.Minimum And Battery1.SoC <= passengerBatteryTank.Range.Maximum Then
                    passengerBatteryTank.Enabled = True
                    passengerBatteryTank.Value = Battery1.SoC
                Else
                    passengerBatteryTank.Enabled = False
                End If
                If Battery1.StatusCodeContactorClosed = True Then
                    passengerBatteryTank.FillColor = Color.LimeGreen
                Else
                    passengerBatteryTank.FillColor = Color.Blue
                End If
                passengerBatteryVoltageLabel.Text = Battery1.PackVoltage.ToString("F2") & " V"
                passengerBatteryCurrentLabel.Text = Battery1.PackDischageAmps.ToString("F2") & " A"
                If DataLoggingActive Then
                    oSheet.Cells(oInt, 19).value = Battery1.PackVoltage.ToString
                    oSheet.Cells(oInt, 22).value = Battery1.PackDischageAmps.ToString
                End If
                passengerBatteryPowerLabel.Text = ((Battery1.PackVoltage * Battery1.PackDischageAmps) / 1000).ToString("F2") & " kW"
                Dim batteryVoltageRange As Single
                Dim myTempValue1 As Single
                Dim myTempValue2 As Single
                batteryVoltageRange = fullBatteryVoltage - emptyBatteryVoltage
                myTempValue1 = Battery0.PackVoltage - emptyBatteryVoltage
                myTempValue2 = (myTempValue1 / batteryVoltageRange) * 100
                If myTempValue2 >= driverBatteryVSOCTank.Range.Minimum And myTempValue2 <= driverBatteryTank.Range.Maximum Then
                    driverBatteryVSOCTank.Value = myTempValue2
                End If
                myTempValue1 = Battery1.PackVoltage - emptyBatteryVoltage
                myTempValue2 = (myTempValue1 / batteryVoltageRange) * 100
                If myTempValue2 >= passengerBatteryVSOCTank.Range.Minimum And myTempValue2 <= passengerBatteryTank.Range.Maximum Then
                    passengerBatteryVSOCTank.Value = myTempValue2
                End If
                myTempValue1 = Battery2.PackVoltage - emptyBatteryVoltage
                myTempValue2 = (myTempValue1 / batteryVoltageRange) * 100
                If myTempValue2 >= middleBatteryVSOCTank.Range.Minimum And myTempValue2 <= middleBatteryTank.Range.Maximum Then
                    middleBatteryVSOCTank.Value = myTempValue2
                End If
                RDForwardInputLed.Value = Sevcon0.ForwardInput
                RDReverseInputLed.Value = Sevcon0.ReverseInput
                RDFS1InputLed.Value = Sevcon0.FS1Input
                'RDKeySwitchInputLed.Value = Sevcon0.KeySwitchInput
                'RDBrakeSwitchInputLed.Value = Sevcon0.HandbrakeInput
                RPForwardInputLed.Value = Sevcon1.ForwardInput
                RPReverseInputLed.Value = Sevcon1.ReverseInput
                RPFS1InputLed.Value = Sevcon1.FS1Input
                'RPKeySwitchInputLed.Value = Sevcon1.KeySwitchInput
                'RPBrakeSwitchInputLed.Value = Sevcon1.HandbrakeInput
                FDForwardInputLed.Value = Sevcon2.ForwardInput
                FDReverseInputLed.Value = Sevcon2.ReverseInput
                FDFS1InputLed.Value = Sevcon2.FS1Input
                'FDKeySwitchInputLed.Value = Sevcon2.KeySwitchInput
                'FDBrakeSwitchInputLed.Value = Sevcon2.HandbrakeInput
                FPForwardInputLed.Value = Sevcon3.ForwardInput
                FPReverseInputLed.Value = Sevcon3.ReverseInput
                FPFS1InputLed.Value = Sevcon3.FS1Input
                'FPKeySwitchInputLed.Value = Sevcon3.KeySwitchInput
                'FPBrakeSwitchInputLed.Value = Sevcon3.HandbrakeInput
                genControllerOnlineLed.Value = Sevcon4.ContactorClosed

                RDTorqueLabel.Text = Sevcon0.Torque.ToString("F2") & " N-m"
                RPTorqueLabel.Text = Sevcon1.Torque.ToString("F2") & " N-m"
                FDTorqueLabel.Text = Sevcon2.Torque.ToString("F2") & " N-m"
                FPTorqueLabel.Text = Sevcon3.Torque.ToString("F2") & " N-m"
                RDMilesLabel.Text = ((Sevcon0.OdometerTenthMiles / 10) * odometerMultiplier).ToString("F1") & " mi."
                RPMilesLabel.Text = ((Sevcon1.OdometerTenthMiles / 10) * odometerMultiplier).ToString("F1") & " mi."
                FDMilesLabel.Text = (((Sevcon2.OdometerTenthMiles / 10) * odometerMultiplier) + vehicleMileageUpgrade).ToString("F1") & " mi."
                FPMilesLabel.Text = (((Sevcon3.OdometerTenthMiles / 10) * odometerMultiplier) + vehicleMileageUpgrade).ToString("F1") & " mi."
                Dim RDMiles As Single
                Dim RPMiles As Single
                Dim FDMiles As Single
                Dim FPMiles As Single
                RDMiles = (Sevcon0.OdometerTenthMiles / 10) * odometerMultiplier
                RPMiles = (Sevcon1.OdometerTenthMiles / 10) * odometerMultiplier
                FDMiles = ((Sevcon2.OdometerTenthMiles / 10) * odometerMultiplier) + vehicleMileageUpgrade
                FPMiles = ((Sevcon3.OdometerTenthMiles / 10) * odometerMultiplier) + vehicleMileageUpgrade
                avgMiles = (RDMiles + RPMiles + FDMiles + FPMiles) / 4
                averageMilesLabel.Text = avgMiles.ToString("F1") & " mi."
                Trip1MilesLabel.Text = (avgMiles - My.Settings.trip1OdometerStartMiles).ToString("F1") & " mi."
                Trip2MilesLabel.Text = (avgMiles - My.Settings.trip2OdometerStartMiles).ToString("F1") & " mi."
                Trip3MilesLabel.Text = (avgMiles - My.Settings.trip3OdometerStartMiles).ToString("F1") & " mi."
                overallMotorTorqueLabel.Text = (Sevcon0.Torque + Sevcon1.Torque + Sevcon2.Torque + Sevcon3.Torque).ToString("F2") & " N-m"
                averageWheelSpeedLabel.Text = (((Sevcon0.Velocity + Sevcon1.Velocity + Sevcon2.Velocity + Sevcon3.Velocity) / 4) * 0.00953846).ToString("F0") & " MPH"
                RDKeyHoursLabel.Text = Sevcon0.KeyOnHours.ToString("F0") & " K hrs."
                RPKeyHoursLabel.Text = Sevcon1.KeyOnHours.ToString("F0") & " K hrs."
                FDKeyHoursLabel.Text = Sevcon2.KeyOnHours.ToString("F0") & " K hrs."
                FPKeyHoursLabel.Text = Sevcon3.KeyOnHours.ToString("F0") & " K hrs."
                RDTractionHoursLabel.Text = Sevcon0.TractionHours.ToString("F0") & " T hrs."
                RPTractionHoursLabel.Text = Sevcon1.TractionHours.ToString("F0") & " T hrs."
                FDTractionHoursLabel.Text = Sevcon2.TractionHours.ToString("F0") & " T hrs."
                FPTractionHoursLabel.Text = Sevcon3.TractionHours.ToString("F0") & " T hrs."
                GenTractionHoursLabel.Text = Sevcon4.TractionHours.ToString("F0") & " hrs."

                Dim myReadings(10) As Double
                Dim readingValue As Double
                Dim Temp As Double

                myReadings = analogInReader.ReadSingleSample()

                Temp = 0
                For inti As Integer = 1 To samples
                    Temp += Convert.ToDouble(myReadings(0).ToString)
                Next
                readingValue = Convert.ToDouble(Temp / samples)
                If readingValue >= FrontGearboxThermometer.Range.Minimum And readingValue <= FrontGearboxThermometer.Range.Maximum Then
                    FrontGearboxThermometer.Enabled = True
                    FrontGearboxThermometer.Value = readingValue
                    FrontGearboxTempLabel.Text = FrontGearboxThermometer.Value.ToString("F1") & " F"
                Else
                    FrontGearboxThermometer.Enabled = False
                    FrontGearboxTempLabel.Text = "N/A"
                End If
                If DataLoggingActive Then
                    oSheet.Cells(oInt, 49).value = readingValue.ToString("F2")
                End If

                Temp = 0
                For inti As Integer = 1 To samples
                    Temp += Convert.ToDouble(myReadings(1).ToString)
                Next
                readingValue = Convert.ToDouble(Temp / samples)
                If readingValue >= FrontHeatsinkPlateThermometer.Range.Minimum And readingValue <= FrontHeatsinkPlateThermometer.Range.Maximum Then
                    FrontHeatsinkPlateThermometer.Enabled = True
                    FrontHeatsinkPlateThermometer.Value = readingValue
                    FrontHeatsinkPlateTempLabel.Text = FrontHeatsinkPlateThermometer.Value.ToString("F1") & " F"
                Else
                    FrontHeatsinkPlateThermometer.Enabled = False
                    FrontHeatsinkPlateTempLabel.Text = "N/A"
                End If
                If DataLoggingActive Then
                    oSheet.Cells(oInt, 50).value = readingValue.ToString("F2")
                End If

                Temp = 0
                For inti As Integer = 1 To samples
                    Temp += Convert.ToDouble(myReadings(2).ToString)
                Next
                readingValue = Convert.ToDouble(Temp / samples)
                If readingValue >= RearGearboxThermometer.Range.Minimum And readingValue <= RearGearboxThermometer.Range.Maximum Then
                    RearGearboxThermometer.Enabled = True
                    RearGearboxThermometer.Value = readingValue
                    RearGearboxTempLabel.Text = RearGearboxThermometer.Value.ToString("F1") & " F"
                Else
                    RearGearboxThermometer.Enabled = False
                    RearGearboxTempLabel.Text = "N/A"
                End If
                If DataLoggingActive Then
                    oSheet.Cells(oInt, 47).value = readingValue.ToString("F2")
                End If

                Temp = 0
                For inti As Integer = 1 To samples
                    Temp += Convert.ToDouble(myReadings(3).ToString)
                Next
                readingValue = Convert.ToDouble(Temp / samples)
                If readingValue >= RearHeatsinkPlateThermometer.Range.Minimum And readingValue <= RearHeatsinkPlateThermometer.Range.Maximum Then
                    RearHeatsinkPlateThermometer.Enabled = True
                    RearHeatsinkPlateThermometer.Value = readingValue
                    RearHeatsinkPlateTempLabel.Text = RearHeatsinkPlateThermometer.Value.ToString("F1") & " F"
                Else
                    RearHeatsinkPlateThermometer.Enabled = False
                    RearHeatsinkPlateTempLabel.Text = "N/A"
                End If
                If DataLoggingActive Then
                    oSheet.Cells(oInt, 48).value = readingValue.ToString("F2")
                End If

                Temp = 0
                For inti As Integer = 1 To samples
                    Temp += Convert.ToDouble(myReadings(4).ToString)
                Next
                readingValue = Convert.ToDouble(Temp / samples)
                If readingValue >= AmbientTempMeter.Range.Minimum And readingValue <= AmbientTempMeter.Range.Maximum Then
                    AmbientTempMeter.Enabled = True
                    AmbientTempMeter.Value = readingValue
                    AmbientTempLabel.Text = AmbientTempMeter.Value.ToString("F1") & " F"
                Else
                    AmbientTempMeter.Enabled = False
                    AmbientTempLabel.Text = "N/A"
                End If
                If DataLoggingActive Then
                    oSheet.Cells(oInt, 46).value = readingValue.ToString("F2")
                End If

                Temp = 0
                For inti As Integer = 1 To samples
                    Temp += Convert.ToDouble(myReadings(5).ToString)
                Next
                readingValue = Convert.ToDouble(Temp / samples)
                If readingValue >= AmbientHumidityMeter.Range.Minimum And readingValue <= AmbientHumidityMeter.Range.Maximum Then
                    AmbientHumidityMeter.Enabled = True
                    AmbientHumidityMeter.Value = readingValue
                    AmbientHumidityLabel.Text = AmbientHumidityMeter.Value.ToString("F1") & " %RH"
                Else
                    AmbientHumidityMeter.Enabled = False
                    AmbientHumidityLabel.Text = "N/A"
                End If
                If DataLoggingActive Then
                    oSheet.Cells(oInt, 51).value = readingValue.ToString("F2")
                End If

            End If
            If DataLoggingActive Then
                With oSheet
                    .Cells(oInt, 1).value = DateTime.Now.Ticks.ToString
                    .Cells(oInt, 2).value = vehicleControlStatus.ToString
                    '.Cells(oInt, 3).value  SET ABOVE
                    .Cells(oInt, 4).value = RDTravelDirection.ToString
                    .Cells(oInt, 5).value = RPTravelDirection.ToString
                    .Cells(oInt, 6).value = FDTravelDirection.ToString
                    .Cells(oInt, 7).value = FPTravelDirection.ToString
                    .Cells(oInt, 8).value = "TBD"
                    '.Cells(oInt, 9).value  SET ABOVE
                    .Cells(oInt, 10).value = "TBD"
                    .Cells(oInt, 11).value = "TBD"
                    .Cells(oInt, 12).value = "TBD"
                    .Cells(oInt, 13).value = "TBD"
                    .Cells(oInt, 14).value = Battery0.SoC.ToString("F2")
                    .Cells(oInt, 15).value = Battery2.SoC.ToString("F2")
                    .Cells(oInt, 16).value = Battery1.SoC.ToString("F2")
                    '.Cells(oInt, 17).value  SET ABOVE
                    '.Cells(oInt, 18).value  SET ABOVE
                    '.Cells(oInt, 19).value  SET ABOVE
                    '.Cells(oInt, 20).value  SET ABOVE
                    '.Cells(oInt, 21).value  SET ABOVE
                    '.Cells(oInt, 22).value  SET ABOVE
                    .Cells(oInt, 23).value = Sevcon0.BatteryVoltage.ToString("F2")
                    .Cells(oInt, 24).value = Sevcon1.BatteryVoltage.ToString("F2")
                    .Cells(oInt, 25).value = Sevcon2.BatteryVoltage.ToString("F2")
                    .Cells(oInt, 26).value = Sevcon3.BatteryVoltage.ToString("F2")
                    .Cells(oInt, 27).value = Sevcon0.BatteryCurrent.ToString("F2")
                    .Cells(oInt, 28).value = Sevcon1.BatteryCurrent.ToString("F2")
                    .Cells(oInt, 29).value = Sevcon2.BatteryCurrent.ToString("F2")
                    .Cells(oInt, 30).value = Sevcon3.BatteryCurrent.ToString("F2")
                    .Cells(oInt, 31).value = Sevcon0.CapacitorVoltage.ToString("F2")
                    .Cells(oInt, 32).value = Sevcon1.CapacitorVoltage.ToString("F2")
                    .Cells(oInt, 33).value = Sevcon2.CapacitorVoltage.ToString("F2")
                    .Cells(oInt, 34).value = Sevcon3.CapacitorVoltage.ToString("F2")
                    '.Cells(oInt, 35).value  SET ABOVE
                    '.Cells(oInt, 36).value  SET ABOVE
                    '.Cells(oInt, 37).value  SET ABOVE
                    '.Cells(oInt, 38).value  SET ABOVE
                    '.Cells(oInt, 39).value  SET ABOVE
                    '.Cells(oInt, 40).value  SET ABOVE
                    '.Cells(oInt, 41).value  SET ABOVE
                    '.Cells(oInt, 42).value  SET ABOVE
                    '.Cells(oInt, 43).value  SET ABOVE
                    '.Cells(oInt, 44).value  SET ABOVE
                    '.Cells(oInt, 45).value  SET ABOVE
                    '.Cells(oInt, 46).value  SET ABOVE
                    '.Cells(oInt, 47).value  SET ABOVE
                    '.Cells(oInt, 48).value  SET ABOVE
                    '.Cells(oInt, 49).value  SET ABOVE
                    '.Cells(oInt, 50).value  SET ABOVE
                    '.Cells(oInt, 51).value  SET ABOVE
                    .Cells(oInt, 52).value = Sevcon0.Velocity.ToString("F2")
                    .Cells(oInt, 53).value = Sevcon1.Velocity.ToString("F2")
                    .Cells(oInt, 54).value = Sevcon2.Velocity.ToString("F2")
                    .Cells(oInt, 55).value = Sevcon3.Velocity.ToString("F2")
                    .Cells(oInt, 56).value = "TBD"
                    .Cells(oInt, 57).value = "TBD"
                    .Cells(oInt, 58).value = "TBD"
                    .Cells(oInt, 59).value = "TBD"

                End With
            End If
        Catch ex As Exception
            sw.WriteLine("ERROR - DataUpdateTimer_Exception - " & ex.Message.ToString() & DateTime.Today.ToString)
            'MessageBox.Show(ex.ToString, "DataUpdateTimer ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub transmitData2Button_Click(sender As Object, e As EventArgs) Handles transmitData2Button.Click

    End Sub

    Private Sub transmitData1Button_Click(sender As Object, e As EventArgs) Handles transmitData1Button.Click

    End Sub

    Private Sub DataLogging(ByVal Command As String)
        Try
            Select Case Command
                Case Is = "Start"
                    'Start Excel and get Application object
                    oXL = CreateObject("Excel.Application")
                    oXL.Visible = False

                    'Get a new workbook
                    oWB = oXL.Workbooks.Add
                    oSheet = oWB.ActiveSheet

                    'Add column headings to workbook
                    With oSheet
                        .Cells(1, 1).value = "Time Stamp"
                        .Cells(1, 2).value = "Veh Mode"
                        .Cells(1, 3).value = "Key On State"
                        .Cells(1, 4).value = "RD Direction"
                        .Cells(1, 5).value = "RP Direction"
                        .Cells(1, 6).value = "FD Direction"
                        .Cells(1, 7).value = "FP Direction"
                        .Cells(1, 8).value = "Charge Port State"
                        .Cells(1, 9).value = "Brake State"
                        .Cells(1, 10).value = "RD Throttle Value"
                        .Cells(1, 11).value = "RP Throttle Value"
                        .Cells(1, 12).value = "FD Throttle Value"
                        .Cells(1, 13).value = "FP Throttle Value"
                        .Cells(1, 14).value = "D Bat SOC%"
                        .Cells(1, 15).value = "M Bat SOC%"
                        .Cells(1, 16).value = "P Bat SOC%"
                        .Cells(1, 17).value = "D Bat V"
                        .Cells(1, 18).value = "M Bat V"
                        .Cells(1, 19).value = "P Bat V"
                        .Cells(1, 20).value = "D Bat A"
                        .Cells(1, 21).value = "M Bat A"
                        .Cells(1, 22).value = "P Bat A"
                        .Cells(1, 23).value = "RD Sev Bat V"
                        .Cells(1, 24).value = "RP Sev Bat V"
                        .Cells(1, 25).value = "FD Sev Bat V"
                        .Cells(1, 26).value = "FP Sev Bat V"
                        .Cells(1, 27).value = "RD Sev A"
                        .Cells(1, 28).value = "RP Sev A"
                        .Cells(1, 29).value = "FD Sev A"
                        .Cells(1, 30).value = "FP Sev A"
                        .Cells(1, 31).value = "RD Sev Cap V"
                        .Cells(1, 32).value = "RP Sev Cap V"
                        .Cells(1, 33).value = "FD Sev Cap V"
                        .Cells(1, 34).value = "FP Sev Cap V"
                        .Cells(1, 35).value = "D Bat T"
                        .Cells(1, 36).value = "M Bat T"
                        .Cells(1, 37).value = "P Bat T"
                        .Cells(1, 38).value = "RD Sev T"
                        .Cells(1, 39).value = "RP Sev T"
                        .Cells(1, 40).value = "FD Sev T"
                        .Cells(1, 41).value = "FP Sev T"
                        .Cells(1, 42).value = "RD Mot T"
                        .Cells(1, 43).value = "RP Mot T"
                        .Cells(1, 44).value = "FD Mot T"
                        .Cells(1, 45).value = "FP Mot T"
                        .Cells(1, 46).value = "Ambient T"
                        .Cells(1, 47).value = "R Gearbox T"
                        .Cells(1, 48).value = "R Heatsink T"
                        .Cells(1, 49).value = "F Gearbox T"
                        .Cells(1, 50).value = "F Heatsink T"
                        .Cells(1, 51).value = "Ambient H"
                        .Cells(1, 52).value = "RD Mot S"
                        .Cells(1, 53).value = "RP Mot S"
                        .Cells(1, 54).value = "FD Mot S"
                        .Cells(1, 55).value = "FP Mot S"
                        .Cells(1, 56).value = "RD Mot Tq"
                        .Cells(1, 57).value = "RP Mot Tq"
                        .Cells(1, 58).value = "FD Mot Tq"
                        .Cells(1, 59).value = "FP Mot Tq"
                    End With
                    oInt = 1
                    DataLoggingActive = True
                Case Is = "Stop"
                    DataLoggingActive = False
                    Dim myString As String
                    myString = DateTime.Now.Day.ToString & "-" & DateTime.Now.Month.ToString & "-" & DateTime.Now.Year.ToString & "-" & DateTime.Now.Ticks.ToString
                    oWB.SaveAs("C:\HE SxS\Data Logs\" & myString & ".xlsx")
                    oWB.Close()
                    oSheet = Nothing
                    oWB = Nothing
                    oXL = Nothing
            End Select
        Catch ex As Exception
            sw.WriteLine("ERROR - DataLogging_Exception - " & ex.Message.ToString() & DateTime.Today.ToString)
        End Try
        

    End Sub

    Private Sub batteryBusTimer_Tick(sender As Object, e As EventArgs) Handles batteryBusTimer.Tick
        'Dim dumpMessageArray(1255) As Ixxat.Vci4.Bal.Can.ICanMessage
        'numOfCANMessage = mReader1.ReadMessages(dumpMessageArray)
        'numOfCANMessage = mReader2.ReadMessages(dumpMessageArray)
        Try
            Dim MaxDischargeCurrent As Single
            Dim TotalMaxDischargeCurrent As Single = 0
            Dim MaxChargeCurrent As Single
            Dim TotalMaxChargeCurrent As Single = 0
            If Battery0.StatusCodeContactorClosed = True Then
                MaxDischargeCurrent = Battery0.PackCapacityAh * Battery0.MaxDischargeCRate * (Battery0.AllowedCurrentPercentage / 255)
                TotalMaxDischargeCurrent += MaxDischargeCurrent
                MaxChargeCurrent = Battery0.PackCapacityAh * Battery0.MaxChargeCRate * 0.75
                TotalMaxChargeCurrent += MaxChargeCurrent
            End If
            If Battery1.StatusCodeContactorClosed = True Then
                MaxDischargeCurrent = Battery1.PackCapacityAh * Battery1.MaxDischargeCRate * (Battery1.AllowedCurrentPercentage / 255)
                TotalMaxDischargeCurrent += MaxDischargeCurrent
                MaxChargeCurrent = Battery1.PackCapacityAh * Battery1.MaxChargeCRate * 0.75
                TotalMaxChargeCurrent += MaxChargeCurrent
            End If
            If Battery2.StatusCodeContactorClosed = True Then
                MaxDischargeCurrent = Battery2.PackCapacityAh * Battery2.MaxDischargeCRate * (Battery2.AllowedCurrentPercentage / 255)
                TotalMaxDischargeCurrent += MaxDischargeCurrent
                MaxChargeCurrent = Battery2.PackCapacityAh * Battery2.MaxChargeCRate * 0.75
                TotalMaxChargeCurrent += MaxChargeCurrent
            End If
            If Battery3.StatusCodeContactorClosed = True Then
                MaxDischargeCurrent = Battery3.PackCapacityAh * Battery3.MaxDischargeCRate * (Battery3.AllowedCurrentPercentage / 255)
                TotalMaxDischargeCurrent += MaxDischargeCurrent
                MaxChargeCurrent = Battery3.PackCapacityAh * Battery3.MaxChargeCRate * 0.75
                TotalMaxChargeCurrent += MaxChargeCurrent
            End If
            If Battery0.SoC > 15 And Battery1.SoC > 15 And Battery2.SoC > 15 Then
                AllowedDischargeCurrentPerSevcon = Convert.ToInt16((TotalMaxDischargeCurrent) / 4)
            Else
                AllowedDischargeCurrentPerSevcon = Convert.ToInt16((TotalMaxDischargeCurrent * 0.85) / 4)
            End If
            allowedDischargePerSevconLabel.Text = AllowedDischargeCurrentPerSevcon.ToString("F2")
            AllowedChargeCurrentPerSevcon = 65536 - Convert.ToInt16(TotalMaxChargeCurrent)
            vehiclePerformanceTank.Value = (AllowedDischargeCurrentPerSevcon / 390) * 100
            If vehiclePerformanceTank.Value <= 15 Then
                vehiclePerformanceTank.FillColor = Color.Red
            ElseIf vehiclePerformanceTank.Value > 15 And vehiclePerformanceTank.Value <= 70 Then
                vehiclePerformanceTank.FillColor = Color.Yellow
            ElseIf vehiclePerformanceTank.Value > 70 And vehiclePerformanceTank.Value <= 100 Then
                vehiclePerformanceTank.FillColor = Color.Green
            End If

            If vehicleControlStatus = "Tablet" Then
                If CanIO0.DigitalInput1 = True Then     'Key ON switch has been turned
                    ''Turn on Low Power B+ SSR to enable Sevcon logic power if not already ON
                    'If CanIO0.DigitalOutput2 = False Then
                    '    CanIO0.DigitalOutput2 = True
                    '    CanIO0.SetDigitalOutputs()
                    '    Thread.Sleep(250)
                    '    Exit Sub
                    'End If
                    ''Send RPDO to Front Driver Sevcon to enable VCap charging
                    'SendCanMessage("Send FD Sevcon Inputs", 2, 1)
                    'If Sevcon2.ContactorClosed = True Then
                    '    SendCanMessage("Close Battery Contactor", 1, 9)
                    '    SendCanMessage("Close Battery Contactor", 2, 9)
                    '    SendCanMessage("Close Battery Contactor", 1, 8)
                    '    'SendCanMessage("Close Battery Contactor", 2, 8)
                    'End If
                    'If Battery0.StatusCodeContactorClosed = True And Battery1.StatusCodeContactorClosed = True And Battery2.StatusCodeContactorClosed = True Then
                    '    'Send RPDO to Rear Driver Sevcon to enable VCap charging
                    '    SendCanMessage("Send RD Sevcon Inputs", 1, 1)
                    '    If Sevcon0.ContactorClosed = False Then
                    '        Thread.Sleep(250)
                    '    End If
                    '    'Send RPDO to Rear Passenger Sevcon to enable VCap charging
                    '    SendCanMessage("Send RP Sevcon Inputs", 1, 2)
                    '    If Sevcon1.ContactorClosed = False Then
                    '        Thread.Sleep(250)
                    '    End If
                    '    'Send RPDO to Front Passenger Sevcon to enable VCap charging
                    '    SendCanMessage("Send FP Sevcon Inputs", 2, 2)
                    '    If Sevcon1.ContactorClosed = False Then
                    '        Thread.Sleep(250)
                    '    End If
                    'End If
                    If Sevcon0.CapacitorVoltage > (Battery1.PackVoltage * 0.85) And Sevcon1.CapacitorVoltage > (Battery1.PackVoltage * 0.85) Then
                        SendCanMessage("Close Battery Contactor", 1, 8)
                        SendCanMessage("Close Battery Contactor", 1, 9)
                        SendCanMessage("Close Battery Contactor", 2, 8)
                        SendCanMessage("Close Battery Contactor", 2, 9)
                        CanIO0.DigitalOutput2 = True
                        CanIO0.SetDigitalOutputs()
                    Else
                        SendCanMessage("Open Battery Contactor", 1, 8)
                        SendCanMessage("Open Battery Contactor", 1, 9)
                        SendCanMessage("Open Battery Contactor", 2, 8)
                        SendCanMessage("Open Battery Contactor", 2, 9)
                        CanIO0.DigitalOutput2 = False
                        CanIO0.SetDigitalOutputs()
                    End If
                    'Calculate maximum forward motor speed for Traction Control
                    minMotorSpeed = maxMotorSpeed
                    If Sevcon0.Velocity < minMotorSpeed Then
                        minMotorSpeed = Sevcon0.Velocity
                    End If
                    If Sevcon1.Velocity < minMotorSpeed Then
                        minMotorSpeed = Sevcon1.Velocity
                    End If
                    If Sevcon2.Velocity < minMotorSpeed Then
                        minMotorSpeed = Sevcon2.Velocity
                    End If
                    If Sevcon3.Velocity < minMotorSpeed Then
                        minMotorSpeed = Sevcon3.Velocity
                    End If
                    If drivetrainAWDSwitch.Value = True Then
                        minMotorSpeed = maxMotorSpeed
                    Else
                        If _4WDHighSwitch.Value = True Then
                            If minMotorSpeed <= 800 Then
                                minMotorSpeed = 800
                            Else
                                minMotorSpeed = minMotorSpeed * 1.43
                                If minMotorSpeed >= maxMotorSpeed Then
                                    minMotorSpeed = maxMotorSpeed
                                End If
                            End If
                        Else
                            If minMotorSpeed <= 300 Then
                                minMotorSpeed = 300
                            Else
                                minMotorSpeed = minMotorSpeed * 1.15
                                If minMotorSpeed >= 2800 Then
                                    minMotorSpeed = 2800
                                End If
                            End If
                        End If
                    End If
                    'Debug.Print("MinMotorSpeed = " & minMotorSpeed.ToString)
                    minMotorSpeedLabel.Text = minMotorSpeed.ToString
                    If CanIO1.DigitalInput1 = True Or CanIO1.DigitalInput2 = True Then
                        Dim mySingle As Single
                        Dim throttleMultiplier As Single = 0.00390625
                        mySingle = CanIO1.AnalogInput1 - 1.2
                        If mySingle > 0 Then
                            'Calculate throttle value and create LSB/MSB bytes
                            int16OValue = Convert.ToInt16((mySingle / 0.3) / throttleMultiplier)
                            int16OValue = int16OValue * (ThrottleLimitSlide.Value / 100)
                            int16Value = int16OValue
                            int16MSBThrottle = (int16Value >> 8)
                            int16Value = int16OValue
                            int16LSBThrottle = (int16Value And 255)
                            throttleActive = True
                        Else
                            int16LSBThrottle = 0
                            int16MSBThrottle = 0
                            throttleActive = False
                        End If
                    Else
                        int16LSBThrottle = 0
                        int16MSBThrottle = 0
                        throttleActive = False
                    End If
                    If drivetrainAWDSwitch.Value = False Then
                        'Calculate maximum forward motor speed value and create LSB/MSB bytes
                        int16OValue = Convert.ToInt16(minMotorSpeed)
                        int16Value = int16OValue
                        int16MSBSpeed = (int16Value >> 8)
                        int16Value = int16OValue
                        int16LSBSpeed = (int16Value And 255)
                    Else
                        int16MSBSpeed = 29
                        int16LSBSpeed = 76
                    End If
                    SendCanMessage("Send Rear Sevcon Inputs", 1, 1)
                    SendCanMessage("Send Front Sevcon Inputs", 2, 1)
                    SendCanMessage("Send Gen Sevcon Inputs", 2, 3)
                    SendCanMessage("Send Rear Sevcon Current Limits", 1, 1)
                    SendCanMessage("Send Front Sevcon Current Limits", 2, 1)

                Else
                    SendCanMessage("Open Battery Contactor", 1, 8)
                    SendCanMessage("Open Battery Contactor", 1, 9)
                    SendCanMessage("Open Battery Contactor", 2, 8)
                    SendCanMessage("Open Battery Contactor", 2, 9)
                End If
            Else
                If CanIO1.DigitalInput1 = True Then
                    If RDTravelDirection = "Reverse" Or RDTravelDirection = "Neutral" Then
                        RDDirectionChange = True
                        RDTravelDirection = "Forward"
                    End If
                    If RPTravelDirection = "Reverse" Or RPTravelDirection = "Neutral" Then
                        RPDirectionChange = True
                        RPTravelDirection = "Forward"
                    End If
                    If FDTravelDirection = "Reverse" Or FDTravelDirection = "Neutral" Then
                        FDDirectionChange = True
                        FDTravelDirection = "Forward"
                    End If
                    If FPTravelDirection = "Reverse" Or FPTravelDirection = "Neutral" Then
                        FPDirectionChange = True
                        FPTravelDirection = "Forward"
                    End If
                End If
                If CanIO1.DigitalInput2 = True Then
                    If RDTravelDirection = "Forward" Or RDTravelDirection = "Neutral" Then
                        RDDirectionChange = True
                        RDTravelDirection = "Reverse"
                    End If
                    If RPTravelDirection = "Forward" Or RPTravelDirection = "Neutral" Then
                        RPDirectionChange = True
                        RPTravelDirection = "Reverse"
                    End If
                    If FDTravelDirection = "Forward" Or FDTravelDirection = "Neutral" Then
                        FDDirectionChange = True
                        FDTravelDirection = "Reverse"
                    End If
                    If FPTravelDirection = "Forward" Or FPTravelDirection = "Neutral" Then
                        FPDirectionChange = True
                        FPTravelDirection = "Reverse"
                    End If
                End If
                If CanIO1.DigitalInput1 = False And CanIO1.DigitalInput2 = False Then
                    If RDTravelDirection = "Reverse" Or RDTravelDirection = "Forward" Then
                        RDDirectionChange = True
                        RDTravelDirection = "Neutral"
                    End If
                    If RPTravelDirection = "Reverse" Or RPTravelDirection = "Forward" Then
                        RPDirectionChange = True
                        RPTravelDirection = "Neutral"
                    End If
                    If FDTravelDirection = "Reverse" Or FDTravelDirection = "Forward" Then
                        FDDirectionChange = True
                        FDTravelDirection = "Neutral"
                    End If
                    If FPTravelDirection = "Reverse" Or FPTravelDirection = "Forward" Then
                        FPDirectionChange = True
                        FPTravelDirection = "Neutral"
                    End If
                End If
            End If

            Select Case RDTravelDirection
                Case Is = "Forward"
                    If RDDirectionChange = True Then
                        RDPictureBox.Visible = True
                        RDPictureBox.Image = HE_SxS.My.Resources.Green_animated_arrow_up
                        RDDirectionChange = False
                    End If
                Case Is = "Neutral"
                    If RDDirectionChange = True Then
                        RDPictureBox.Visible = False
                        RDDirectionChange = False
                    End If
                Case Is = "Reverse"
                    If RDDirectionChange = True Then
                        RDPictureBox.Visible = True
                        RDPictureBox.Image = HE_SxS.My.Resources.Red_animated_arrow_down
                        RDDirectionChange = False
                    End If
            End Select
            Select Case RPTravelDirection
                Case Is = "Forward"
                    If RPDirectionChange = True Then
                        RPPictureBox.Visible = True
                        RPPictureBox.Image = HE_SxS.My.Resources.Green_animated_arrow_up
                        RPDirectionChange = False
                    End If
                Case Is = "Neutral"
                    If RPDirectionChange = True Then
                        RPPictureBox.Visible = False
                        RPDirectionChange = False
                    End If
                Case Is = "Reverse"
                    If RPDirectionChange = True Then
                        RPPictureBox.Visible = True
                        RPPictureBox.Image = HE_SxS.My.Resources.Red_animated_arrow_down
                        RPDirectionChange = False
                    End If
            End Select
            Select Case FDTravelDirection
                Case Is = "Forward"
                    If FDDirectionChange = True Then
                        FDPictureBox.Visible = True
                        FDPictureBox.Image = HE_SxS.My.Resources.Green_animated_arrow_up
                        FDDirectionChange = False
                    End If
                Case Is = "Neutral"
                    If FDDirectionChange = True Then
                        FDPictureBox.Visible = False
                        FDDirectionChange = False
                    End If
                Case Is = "Reverse"
                    If FDDirectionChange = True Then
                        FDPictureBox.Visible = True
                        FDPictureBox.Image = HE_SxS.My.Resources.Red_animated_arrow_down
                        FDDirectionChange = False
                    End If
            End Select
            Select Case FPTravelDirection
                Case Is = "Forward"
                    If FPDirectionChange = True Then
                        FPPictureBox.Visible = True
                        FPPictureBox.Image = HE_SxS.My.Resources.Green_animated_arrow_up
                        FPDirectionChange = False
                    End If
                Case Is = "Neutral"
                    If FPDirectionChange = True Then
                        FPPictureBox.Visible = False
                        FPDirectionChange = False
                    End If
                Case Is = "Reverse"
                    If FPDirectionChange = True Then
                        FPPictureBox.Visible = True
                        FPPictureBox.Image = HE_SxS.My.Resources.Red_animated_arrow_down
                        FPDirectionChange = False
                    End If
            End Select

            Select Case intHeartbeat
                Case Is = 5
                    SendCanMessage("Send Heartbeat", 1, 6)
                    SendCanMessage("Send Heartbeat", 2, 6)
                    intHeartbeat = 1

                Case Is < 5
                    intHeartbeat += 1
            End Select


            Select Case intHourCounter
                Case Is = 0, 75
                    If generatorSOCEnabled = True Then
                        sw.WriteLine("NORMAL - Auto SOC Generator Allowed - " & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    Else
                        sw.WriteLine("NORMAL - Auto SOC Generator Not Allowed - " & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    End If
                    sw.WriteLine("NORMAL - Total System SOC = " & batteryCapacityTank.Value.ToString & " - " & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    sw.WriteLine("NORMAL - Total Fuel Capacity = " & fuelCapacityTank.Value.ToString & " - " & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                    sw.WriteLine("NORMAL - Vehicle Speed = " & speedAnalogGauge.Value.ToString & " - " & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                Case Is = 150
                    SendCanMessage("Query Main Odometer", 1, 1)
                    SendCanMessage("Query Main Odometer", 1, 2)
                    SendCanMessage("Query Main Odometer", 2, 1)
                    SendCanMessage("Query Main Odometer", 2, 2)
                    SendCanMessage("Query Key On Hours", 1, 1)
                    SendCanMessage("Query Key On Hours", 1, 2)
                    SendCanMessage("Query Key On Hours", 2, 1)
                    SendCanMessage("Query Key On Hours", 2, 2)
                    SendCanMessage("Query Key On Hours", 2, 3)
                    SendCanMessage("Query Traction Hours", 1, 1)
                    SendCanMessage("Query Traction Hours", 1, 2)
                    SendCanMessage("Query Traction Hours", 2, 1)
                    SendCanMessage("Query Traction Hours", 2, 2)
                    SendCanMessage("Query Traction Hours", 2, 3)
                    intHourCounter = 1
                Case Is < 150
                    intHourCounter += 1

            End Select
        Catch ex As Exception
            sw.WriteLine("ERROR - BatteryBusTimer_Exception - " & ex.Message.ToString() & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            'MessageBox.Show(ex.ToString, "BatteryBusTimer ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub currentDisplaySwitch_StateChanged(sender As Object, e As ActionEventArgs) Handles currentDisplaySwitch.StateChanged
        If currentDisplaySwitch.Value = True Then
            powerDisplaySwitch.Value = False
            torqueDisplaySwitch.Value = False
        End If
    End Sub

    Private Sub powerDisplaySwitch_StateChanged(sender As Object, e As ActionEventArgs) Handles powerDisplaySwitch.StateChanged
        If powerDisplaySwitch.Value = True Then
            currentDisplaySwitch.Value = False
            torqueDisplaySwitch.Value = False
        End If
    End Sub

    Private Sub torqueDisplaySwitch_StateChanged(sender As Object, e As ActionEventArgs) Handles torqueDisplaySwitch.StateChanged
        If torqueDisplaySwitch.Value = True Then
            currentDisplaySwitch.Value = False
            powerDisplaySwitch.Value = False
        End If
    End Sub

    Private Sub vehicleControlSwitch_StateChanged(sender As Object, e As ActionEventArgs) Handles vehicleControlSwitch.StateChanged
        If vehicleControlSwitch.Value = True Then
            vehicleControlStatus = "ECU"
            If LogFileOpened Then
                sw.WriteLine("NORMAL - ECU Mode Selected - " & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            End If
            chargePortEnableGroupBox.Visible = False
            throttleEnableGroupBox.Visible = False
            tractionControlGroupBox.Visible = True
        Else
            vehicleControlStatus = "Tablet"
            If LogFileOpened Then
                sw.WriteLine("NORMAL - Tablet Mode Selected - " & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            End If
            chargePortEnableGroupBox.Visible = True
            throttleEnableGroupBox.Visible = True
            tractionControlGroupBox.Visible = True
        End If
        controlSourceSwitch.Value = vehicleControlSwitch.Value
    End Sub

    Private Sub VehiclePerformanceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles VehiclePerformanceToolStripMenuItem.Click
        If VehiclePerformanceToolStripMenuItem.Checked Then
            DataLoggingToolStripMenuItem.Enabled = True
            VehiclePerformanceToolStripMenuItem.Checked = False
            vehiclePerformanceGroupBox.Visible = False
        Else
            DataLoggingToolStripMenuItem.Enabled = False
            CANInfoGroupBox.Visible = False
            CANInformationToolStripMenuItem.Checked = False
            vehicleSupervisorGroupBox.Visible = False
            VehicleControlToolStripMenuItem.Checked = False
            VehiclePerformanceToolStripMenuItem.Checked = True
            vehiclePerformanceGroupBox.Visible = True
        End If
    End Sub

    Private Sub DataLoggingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DataLoggingToolStripMenuItem.Click
        If DataLoggingToolStripMenuItem.Checked Then
            VehiclePerformanceToolStripMenuItem.Enabled = True
            DataLoggingToolStripMenuItem.Checked = False
            dataLoggingGroupBox.Visible = False
        Else
            VehiclePerformanceToolStripMenuItem.Enabled = False
            DataLoggingToolStripMenuItem.Checked = True
            dataLoggingGroupBox.Visible = True
        End If
    End Sub

    Private Sub logDataSwitch_StateChanged(sender As Object, e As ActionEventArgs) Handles logDataSwitch.StateChanged
        If logDataSwitch.Value = True Then
            DataLogging("Start")
            sw.WriteLine("NORMAL - Data Logging Started - " & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            DataLogging("Stop")
            sw.WriteLine("NORMAL - Data Logging Stopped - " & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
        End If
    End Sub

    Private Sub resetTrip1Button_Click(sender As Object, e As EventArgs) Handles resetTrip1Button.Click
        Dim myResponse = DialogResult

        myResponse = MessageBox.Show("Do you really want to reset Trip 1 to 0?", "TRIP 1 RESET", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If myResponse = Windows.Forms.DialogResult.Yes Then
            My.Settings.trip1OdometerStartMiles = avgMiles
        End If
    End Sub

    Private Sub resetTrip2Button_Click(sender As Object, e As EventArgs) Handles resetTrip2Button.Click
        Dim myResponse = DialogResult

        myResponse = MessageBox.Show("Do you really want to reset Trip 2 to 0?", "TRIP 1 RESET", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If myResponse = Windows.Forms.DialogResult.Yes Then
            My.Settings.trip2OdometerStartMiles = avgMiles
        End If
    End Sub

    Private Sub resetTrip3Button_Click(sender As Object, e As EventArgs) Handles resetTrip3Button.Click
        Dim myResponse = DialogResult

        myResponse = MessageBox.Show("Do you really want to reset Trip 3 to 0?", "TRIP 1 RESET", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If myResponse = Windows.Forms.DialogResult.Yes Then
            My.Settings.trip3OdometerStartMiles = avgMiles
        End If
    End Sub

    Private Sub Trip1ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles Trip1ToolStripMenuItem.Click
        MainToolStripMenuItem.Checked = False
        Trip1ToolStripMenuItem.Checked = True
        Trip2ToolStripMenuItem.Checked = False
        Trip3ToolStripMenuItem.Checked = False
    End Sub

    Private Sub MainToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MainToolStripMenuItem.Click
        MainToolStripMenuItem.Checked = True
        Trip1ToolStripMenuItem.Checked = False
        Trip2ToolStripMenuItem.Checked = False
        Trip3ToolStripMenuItem.Checked = False
    End Sub

    Private Sub Trip2ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles Trip2ToolStripMenuItem.Click
        MainToolStripMenuItem.Checked = False
        Trip1ToolStripMenuItem.Checked = False
        Trip2ToolStripMenuItem.Checked = True
        Trip3ToolStripMenuItem.Checked = False
    End Sub

    Private Sub Trip3ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles Trip3ToolStripMenuItem.Click
        MainToolStripMenuItem.Checked = False
        Trip1ToolStripMenuItem.Checked = False
        Trip2ToolStripMenuItem.Checked = False
        Trip3ToolStripMenuItem.Checked = True
    End Sub

   
    Private Sub minSOCActiveSlide_AfterChangeValue(sender As Object, e As AfterChangeNumericValueEventArgs) Handles minSOCActiveSlide.AfterChangeValue
        If maxSOCActiveSlide.Value < (minSOCActiveSlide.Value + 5) Then
            maxSOCActiveSlide.Value = minSOCActiveSlide.Value + 5
        End If
        My.Settings.genChargeActivateMin = minSOCActiveSlide.Value
        My.Settings.genChargeActivateMax = maxSOCActiveSlide.Value
        If LogFileOpened Then
            sw.WriteLine("NORMAL - Min SOC Active Changed To " & minSOCActiveSlide.Value.ToString & " - " & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            sw.WriteLine("NORMAL - Max SOC Active Changed To " & maxSOCActiveSlide.Value.ToString & " - " & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
        End If
        My.Settings.Save()
    End Sub

    Private Sub maxSOCActiveSlide_AfterChangeValue(sender As Object, e As AfterChangeNumericValueEventArgs) Handles maxSOCActiveSlide.AfterChangeValue
        If minSOCActiveSlide.Value > (maxSOCActiveSlide.Value - 5) Then
            minSOCActiveSlide.Value = maxSOCActiveSlide.Value - 5
        End If
        My.Settings.genChargeActivateMin = minSOCActiveSlide.Value
        My.Settings.genChargeActivateMax = maxSOCActiveSlide.Value
        If LogFileOpened Then
            sw.WriteLine("NORMAL - Min SOC Active Changed To " & minSOCActiveSlide.Value.ToString & " - " & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            sw.WriteLine("NORMAL - Max SOC Active Changed To " & maxSOCActiveSlide.Value.ToString & " - " & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
        End If
        My.Settings.Save()
    End Sub

    Private Sub tractionControlMinSpeedSlide_AfterChangeValue(sender As Object, e As AfterChangeNumericValueEventArgs) Handles tractionControlMinSpeedSlide.AfterChangeValue
        My.Settings.tractionControlLowSpeedLimit = tractionControlMinSpeedSlide.Value
    End Sub

    Private Sub VehicleControlToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles VehicleControlToolStripMenuItem.Click
        If VehicleControlToolStripMenuItem.Checked Then
            VehicleControlToolStripMenuItem.Checked = False
            vehicleSupervisorGroupBox.Visible = False
        Else
            VehicleControlToolStripMenuItem.Checked = True
            CANInfoGroupBox.Visible = False
            CANInformationToolStripMenuItem.Checked = False
            vehiclePerformanceGroupBox.Visible = False
            VehiclePerformanceToolStripMenuItem.Checked = False
            vehicleSupervisorGroupBox.Visible = True
            Dim myPoint As Point
            myPoint.X = 12
            myPoint.Y = 46
            Engineering1GroupBox.Location = myPoint
        End If
    End Sub

    Private Sub controlSourceSwitch_StateChanged(sender As Object, e As ActionEventArgs) Handles controlSourceSwitch.StateChanged
        vehicleControlSwitch.Value = controlSourceSwitch.Value
        If vehicleControlSwitch.Value = False Then
            drivetrainGroupBox.Visible = True
            If drivetrainAWDSwitch.Value = False Then
                _4WDGroupBox.Visible = True
                sw.WriteLine("NORMAL - Tablet 4WD Mode Selected - " & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            End If
        Else
            drivetrainGroupBox.Visible = True
            _4WDGroupBox.Visible = False
            If LogFileOpened Then
                sw.WriteLine("NORMAL - Tablet AWD Mode Selected - " & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            End If
        End If
    End Sub

    Private Sub drivetrainAWDSwitch_StateChanged(sender As Object, e As ActionEventArgs) Handles drivetrainAWDSwitch.StateChanged
        If drivetrainAWDSwitch.Value = False Then
            _4WDGroupBox.Visible = True
        Else
            _4WDGroupBox.Visible = False
        End If
    End Sub

    Private Sub VehicleErrorTimer_Tick(sender As Object, e As EventArgs) Handles VehicleErrorTimer.Tick
        If ErrorDisplayState = True Then
            ErrorDisplayState = False
            If RDMotorError Or RDMotorWarning Then
                RDMotorPictureBox.Visible = False
            End If
            If RPMotorError Or RPMotorWarning Then
                RPMotorPictureBox.Visible = False
            End If
            If FDMotorError Or FDMotorWarning Then
                FDMotorPictureBox.Visible = False
            End If
            If FPMotorError Or FPMotorWarning Then
                FPMotorPictureBox.Visible = False
            End If
        Else
            ErrorDisplayState = True
            RDMotorPictureBox.Visible = True
            RPMotorPictureBox.Visible = True
            FDMotorPictureBox.Visible = True
            FPMotorPictureBox.Visible = True
            GenMotorPictureBox.Visible = True
            RDControllerPictureBox.Visible = True
            RPControllerPictureBox.Visible = True
            FDControllerPictureBox.Visible = True
            FPControllerPictureBox.Visible = True
            GenControllerPictureBox.Visible = True
            DriverBatteryPictureBox.Visible = True
            MiddleBatteryPictureBox.Visible = True
            PassengerBatteryPictureBox.Visible = True
            FrontGearboxPictureBox.Visible = True
            RearGearboxPictureBox.Visible = True
            EnginePictureBox.Visible = True
        End If
    End Sub

    Private Sub chargePortEnableSwitch_StateChanged(sender As Object, e As ActionEventArgs) Handles chargePortEnableSwitch.StateChanged
        If chargePortEnableSwitch.Value = True Then
            sw.WriteLine("NORMAL - Charge Port Enable Switch Selected ON - " & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
        Else
            sw.WriteLine("NORMAL - Charge Port Enable Switch Selected OFF - " & DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
        End If

    End Sub
End Class

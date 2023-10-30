Public Class CANio500Class
    Private mstrDescription As String
    Private mstrHWDescription As String
    Private mblnDO1 As Boolean
    Private mblnDO2 As Boolean
    Private mblnDO3 As Boolean
    Private mblnDO4 As Boolean
    Private mstrUsrLED1 As String
    Private mstrUsrLED2 As String
    Private msngPwrSupplyVoltage As Single
    Private mblnDI1 As Boolean
    Private mblnDI2 As Boolean
    Private mblnDI3 As Boolean
    Private mblnDI4 As Boolean
    Private mblnDIA As Boolean
    Private msngAO1 As Single
    Private msngAO2 As Single
    Private msngAO3 As Single
    Private msngAO4 As Single
    Private msngAI1 As Single
    Private msngAI2 As Single
    Private msngAI3 As Single
    Private msngAI4 As Single
    Private mbyt200_0 As Byte                                           'Digital Outputs 1-4
    Private mbyt200_1 As Byte                                           'Digital Outputs to Control USR 1/2 LEDs
    Private mbyt300_0 As Byte                                           'Ananlog Output 1 LSB
    Private mbyt300_1 As Byte                                           'Ananlog Output 1 MSB
    Private mbyt300_2 As Byte                                           'Ananlog Output 2 LSB
    Private mbyt300_3 As Byte                                           'Ananlog Output 2 MSB
    Private mbyt300_4 As Byte                                           'Ananlog Output 3 LSB
    Private mbyt300_5 As Byte                                           'Ananlog Output 3 MSB
    Private mbyt300_6 As Byte                                           'Ananlog Output 4 LSB
    Private mbyt300_7 As Byte                                           'Ananlog Output 4 MSB
    Private mbyt180_0 As Byte                                           'Digital Inputs 1-4
    Private mbyt180_1 As Byte                                           'Additional Digital Input & HW Info
    Private mbyt480_0 As Byte                                           'Edge Events of Digital Input 1
    Private mbyt480_1 As Byte                                           'Edge Events of Digital Input 2
    Private mbyt480_2 As Byte                                           'Edge Events of Digital Input 3
    Private mbyt480_3 As Byte                                           'Edge Events of Digital Input 4
    Private mbyt280_0 As Byte                                           'Data of Analog Input 1 LSB
    Private mbyt280_1 As Byte                                           'Data of Analog Input 1 MSB
    Private mbyt280_2 As Byte                                           'Data of Analog Input 2 LSB
    Private mbyt280_3 As Byte                                           'Data of Analog Input 2 MSB
    Private mbyt280_4 As Byte                                           'Data of Analog Input 3 LSB
    Private mbyt280_5 As Byte                                           'Data of Analog Input 3 MSB
    Private mbyt280_6 As Byte                                           'Data of Analog Input 4 LSB
    Private mbyt280_7 As Byte                                           'Data of Analog Input 4 MSB
    Private mbyt380_0 As Byte                                           'Value of PWR(+) LSB
    Private mbyt380_1 As Byte                                           'Value of PWR(+) MSB
    Private mbyt700_0 As Byte                                           'Heartbeat Message
    Private mbyt80_0 As Byte                                            'Emergency Message
    Private Const AnalogInMultiplier As Single = 0.0026812              'AD Value * 2.6812 [mV]
    Private Const AnalogOutMultiplier As Single = 0.00244140625         'AD Value / 4096 * Output Voltage [V]
    Private Const PowerSupplyVoltageMultiplier As Single = 0.0080586    'AD Value * 8.0586 [mV]
    Private mdateTimeStamp As Date
    Private mdateTimeStamp180 As Date
    Private mdateTimeStamp280 As Date
    Private mdateTimeStamp380 As Date

    Property Description As String
        Get
            Return mstrDescription
        End Get
        Set(ByVal value As String)
            mstrDescription = value
        End Set
    End Property
    Property DigitalOutput1() As Boolean
        Get
            Return mblnDO1
        End Get
        Set(ByVal value As Boolean)
            mblnDO1 = value
        End Set
    End Property
    Property DigitalOutput2() As Boolean
        Get
            Return mblnDO2
        End Get
        Set(ByVal value As Boolean)
            mblnDO2 = value
        End Set
    End Property
    Property DigitalOutput3() As Boolean
        Get
            Return mblnDO3
        End Get
        Set(ByVal value As Boolean)
            mblnDO3 = value
        End Set
    End Property
    Property DigitalOutput4() As Boolean
        Get
            Return mblnDO4
        End Get
        Set(ByVal value As Boolean)
            mblnDO4 = value
        End Set
    End Property
    Property UserLED1() As String
        Get
            Return mstrUsrLED1
        End Get
        Set(ByVal value As String)
            mstrUsrLED1 = value
        End Set
    End Property
    Property UserLED2() As String
        Get
            Return mstrUsrLED2
        End Get
        Set(ByVal value As String)
            mstrUsrLED2 = value
        End Set
    End Property
    ReadOnly Property PowerSupplyVoltage() As Single
        Get
            Return msngPwrSupplyVoltage
        End Get
    End Property
    ReadOnly Property DigitalInput1() As Boolean
        Get
            Return mblnDI1
        End Get
    End Property
    ReadOnly Property DigitalInput2() As Boolean
        Get
            Return mblnDI2
        End Get
    End Property
    ReadOnly Property DigitalInput3() As Boolean
        Get
            Return mblnDI3
        End Get
    End Property
    ReadOnly Property DigitalInput4() As Boolean
        Get
            Return mblnDI4
        End Get
    End Property
    ReadOnly Property DigitalInputAdditional() As Boolean
        Get
            Return mblnDIA
        End Get
    End Property
    ReadOnly Property AnalogInput1() As Single
        Get
            Return msngAI1
        End Get
    End Property
    ReadOnly Property AnalogInput2() As Single
        Get
            Return msngAI2
        End Get
    End Property
    ReadOnly Property AnalogInput3() As Single
        Get
            Return msngAI3
        End Get
    End Property
    ReadOnly Property AnalogInput4() As Single
        Get
            Return msngAI4
        End Get
    End Property
    ReadOnly Property x200_0() As Byte
        Get
            Return mbyt200_0
        End Get
    End Property
    ReadOnly Property x200_1() As Byte
        Get
            Return mbyt200_1
        End Get
    End Property

    Public Sub SetDigitalOutputs()
        Dim myByte As Byte
        Dim myInteger As Integer
        If mblnDO1 = True Then
            myInteger += 1
        End If
        If mblnDO2 = True Then
            myInteger += 2
        End If
        If mblnDO3 = True Then
            myInteger += 4
        End If
        If mblnDO4 = True Then
            myInteger += 8
        End If
        myByte = Convert.ToByte(myInteger)
        mbyt200_0 = myByte
    End Sub
    Public Sub SetAnalogOutputs()

    End Sub
    Public Sub ReadDigitalInputs(ByVal byte0 As Byte, ByVal byte1 As Byte)
        Try
            Select Case byte0
                Case Is = 0
                    mblnDI1 = False
                    mblnDI2 = False
                    mblnDI3 = False
                    mblnDI4 = False
                Case Is = 1
                    mblnDI1 = True
                    mblnDI2 = False
                    mblnDI3 = False
                    mblnDI4 = False
                Case Is = 2
                    mblnDI1 = False
                    mblnDI2 = True
                    mblnDI3 = False
                    mblnDI4 = False
                Case Is = 3
                    mblnDI1 = True
                    mblnDI2 = True
                    mblnDI3 = False
                    mblnDI4 = False
                Case Is = 4
                    mblnDI1 = False
                    mblnDI2 = False
                    mblnDI3 = True
                    mblnDI4 = False
                Case Is = 5
                    mblnDI1 = True
                    mblnDI2 = False
                    mblnDI3 = True
                    mblnDI4 = False
                Case Is = 6
                    mblnDI1 = False
                    mblnDI2 = True
                    mblnDI3 = True
                    mblnDI4 = False
                Case Is = 7
                    mblnDI1 = True
                    mblnDI2 = True
                    mblnDI3 = True
                    mblnDI4 = False
                Case Is = 8
                    mblnDI1 = False
                    mblnDI2 = False
                    mblnDI3 = False
                    mblnDI4 = True
                Case Is = 9
                    mblnDI1 = True
                    mblnDI2 = False
                    mblnDI3 = False
                    mblnDI4 = True
                Case Is = 10
                    mblnDI1 = False
                    mblnDI2 = True
                    mblnDI3 = False
                    mblnDI4 = True
                Case Is = 11
                    mblnDI1 = True
                    mblnDI2 = True
                    mblnDI3 = False
                    mblnDI4 = True
                Case Is = 12
                    mblnDI1 = False
                    mblnDI2 = False
                    mblnDI3 = True
                    mblnDI4 = True
                Case Is = 13
                    mblnDI1 = True
                    mblnDI2 = False
                    mblnDI3 = True
                    mblnDI4 = True
                Case Is = 14
                    mblnDI1 = False
                    mblnDI2 = True
                    mblnDI3 = True
                    mblnDI4 = True
                Case Is = 15
                    mblnDI1 = True
                    mblnDI2 = True
                    mblnDI3 = True
                    mblnDI4 = True
            End Select
            Select Case byte1
                Case Is = 0
                    mblnDIA = False
                    mstrHWDescription = "Not Defined"
                Case Is = 1
                    mblnDIA = True
                    mstrHWDescription = "Not Defined"
                Case Is = 2
                    mstrHWDescription = "+/- 5V"
                Case Is = 3
                    mstrHWDescription = "+/- 5V"
                Case Is = 4
                    mstrHWDescription = "+/- 100mA"
                Case Is = 5
                    mstrHWDescription = "+/- 100mA"
                Case Is = 6
                    mstrHWDescription = "0 - 10V"
                Case Is = 7
                    mstrHWDescription = "0 - 10V"
            End Select
        Catch ex As Exception

        End Try
    End Sub
    Public Sub ReadAnalogInputs(ByVal analog1 As Integer, ByVal analog2 As Integer, ByVal analog3 As Integer, ByVal analog4 As Integer)
        msngAI1 = analog1 * AnalogInMultiplier
        msngAI2 = analog2 * AnalogInMultiplier
        msngAI3 = analog3 * AnalogInMultiplier
        msngAI4 = analog4 * AnalogInMultiplier
    End Sub
    Public Sub ReadPowerSupplyVoltage(ByVal analog As Integer)
        msngPwrSupplyVoltage = analog * PowerSupplyVoltageMultiplier
    End Sub
    Property TimeStamp() As Date
        Get
            Return mdateTimeStamp
        End Get
        Set(ByVal value As Date)
            mdateTimeStamp = value
        End Set
    End Property

    Property TimeStamp180() As Date
        Get
            Return mdateTimeStamp180
        End Get
        Set(ByVal value As Date)
            mdateTimeStamp180 = value
        End Set
    End Property

    Property TimeStamp280() As Date
        Get
            Return mdateTimeStamp
        End Get
        Set(ByVal value As Date)
            mdateTimeStamp280 = value
        End Set
    End Property

    Property TimeStamp380() As Date
        Get
            Return mdateTimeStamp
        End Get
        Set(ByVal value As Date)
            mdateTimeStamp380 = value
        End Set
    End Property
End Class

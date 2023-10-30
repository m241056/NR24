Public Class BatteryClass
    Private mintSoC As Integer
    Private mstrDescription As String
    Private mintBmsStatusBits As Integer
    Private mintNumOfCharges As Integer
    Private mdateTimeStamp As Date
    Private mdateTimeStamp180 As Date
    Private mdateTimeStamp300 As Date
    Private mdateTimeStamp380 As Date
    Private mdateTimeStamp400 As Date
    Private mdateTimeStamp480 As Date
    Private mdateTimeStamp500 As Date
    Private msngPackVoltage As Single
    Private mintHighestPackTempC As Integer
    Private mintLowestPackTempC As Integer
    Private mintPackSensorTempC(8) As Integer
    Private mintPackSensorStatus(8) As Integer
    Private msngPackDischargeAmps As Single
    Private mintPackCapacityRemainingAh As Integer
    Private mintAllowedCurrentPercentage As Integer
    Private mintPackCapacityAh As Integer
    Private msngMaxDischargeCRate As Single
    Private msngMaxChargeCRate As Single
    Private mblnBmsStatusCodeOkay As Boolean               '0x0000
    Private mblnBmsStatusCodeCharger As Boolean            '0x0001
    Private mblnBmsStatusCodeBattTempTooHigh As Boolean    '0x0002
    Private mblnBmsStatusCodeBattTempHigh As Boolean       '0x0004
    Private mblnBmsStatusCodeBattTempTooLow As Boolean     '0x0008
    Private mblnBmsStatusCodeLowBatt As Boolean            '0x0010
    Private mblnBmsStatusCodeCriticalBatt As Boolean       '0x0020
    Private mblnBmsStatusCodeImbalance As Boolean          '0x0040
    Private mblnBmsStatusCodeInternalFault As Boolean      '0x0080
    Private mblnBmsStatusCode0x0100 As Boolean             '0x0100
    Private mblnBmsStatusCodeContactorClosed As Boolean    '0x0200
    Private mblnBmsStatusCodeIsolationFault As Boolean     '0x0400
    Private mblnBmsStatusCodeCellTooHigh As Boolean        '0x0800
    Private mblnBmsStatusCodeCellTooLow As Boolean         '0x1000
    Private mblnBmsStatusCodeChargeHalt As Boolean         '0x2000
    Private mblnBmsStatusCodeFull As Boolean               '0x4000
    Private mblnBmsStatusCodeInternalDisable As Boolean    '0x8000

    Property SoC() As Integer
        Get
            Return mintSoC
        End Get
        Set(ByVal value As Integer)
            mintSoC = value
        End Set
    End Property
    Property Description() As String
        Get
            Return mstrDescription
        End Get
        Set(ByVal value As String)
            mstrDescription = value
        End Set
    End Property
    Property BmsStatusBits() As Int32
        Get
            Return mintBmsStatusBits
        End Get
        Set(ByVal value As Int32)
            mintBmsStatusBits = value
            Dim tempValue As Int32
            Dim statusBytes As Int32
            Try
                statusBytes = Convert.ToInt32(value)
                tempValue = Convert.ToInt32(&H8000)     'Code Internal Disable
                If (statusBytes And tempValue) = tempValue Then
                    mblnBmsStatusCodeInternalDisable = True
                Else
                    mblnBmsStatusCodeInternalDisable = False
                End If

                tempValue = Convert.ToInt32(&H4000)     'Code Full
                If (statusBytes And tempValue) = tempValue Then
                    mblnBmsStatusCodeFull = True
                Else
                    mblnBmsStatusCodeFull = False
                End If

                tempValue = Convert.ToInt32(&H2000)     'Code Charge Halt
                If (statusBytes And tempValue) = tempValue Then
                    mblnBmsStatusCodeChargeHalt = True
                Else
                    mblnBmsStatusCodeChargeHalt = False
                End If

                tempValue = Convert.ToInt32(&H1000)     'Code Cell Too Low
                If (statusBytes And tempValue) = tempValue Then
                    mblnBmsStatusCodeCellTooLow = True
                Else
                    mblnBmsStatusCodeCellTooLow = False
                End If

                tempValue = Convert.ToInt32(&H800)     'Code Cell Too High
                If (statusBytes And tempValue) = tempValue Then
                    mblnBmsStatusCodeCellTooHigh = True
                Else
                    mblnBmsStatusCodeCellTooHigh = False
                End If

                tempValue = Convert.ToInt32(&H400)     'Code Isolation Fault
                If (statusBytes And tempValue) = tempValue Then
                    mblnBmsStatusCodeIsolationFault = True
                Else
                    mblnBmsStatusCodeIsolationFault = False
                End If

                tempValue = Convert.ToInt32(&H200)     'Code Contactor Closed
                If (statusBytes And tempValue) = tempValue Then
                    mblnBmsStatusCodeContactorClosed = True
                Else
                    mblnBmsStatusCodeContactorClosed = False
                End If

                tempValue = Convert.ToInt32(&H100)     'Code 0x0100 AVAILABLE
                If (statusBytes And tempValue) = tempValue Then
                    mblnBmsStatusCode0x0100 = True
                Else
                    mblnBmsStatusCode0x0100 = False
                End If

                tempValue = Convert.ToInt32(&H80)     'Code Internal Fault
                If (statusBytes And tempValue) = tempValue Then
                    mblnBmsStatusCodeInternalFault = True
                Else
                    mblnBmsStatusCodeInternalFault = False
                End If

                tempValue = Convert.ToInt32(&H40)     'Code Imbalance
                If (statusBytes And tempValue) = tempValue Then
                    mblnBmsStatusCodeImbalance = True
                Else
                    mblnBmsStatusCodeImbalance = False
                End If

                tempValue = Convert.ToInt32(&H20)     'Code Critical Batt
                If (statusBytes And tempValue) = tempValue Then
                    mblnBmsStatusCodeCriticalBatt = True
                Else
                    mblnBmsStatusCodeCriticalBatt = False
                End If

                tempValue = Convert.ToInt32(&H10)     'Code Low Batt
                If (statusBytes And tempValue) = tempValue Then
                    mblnBmsStatusCodeLowBatt = True
                Else
                    mblnBmsStatusCodeLowBatt = False
                End If

                tempValue = Convert.ToInt32(&H8)     'Code Batt Temp Too Low
                If (statusBytes And tempValue) = tempValue Then
                    mblnBmsStatusCodeBattTempTooLow = True
                Else
                    mblnBmsStatusCodeBattTempTooLow = False
                End If

                tempValue = Convert.ToInt32(&H4)     'Code Batt Temp High
                If (statusBytes And tempValue) = tempValue Then
                    mblnBmsStatusCodeBattTempHigh = True
                Else
                    mblnBmsStatusCodeBattTempHigh = False
                End If

                tempValue = Convert.ToInt32(&H2)     'Code Batt Temp Too High
                If (statusBytes And tempValue) = tempValue Then
                    mblnBmsStatusCodeImbalance = True
                Else
                    mblnBmsStatusCodeImbalance = False
                End If

                tempValue = Convert.ToInt32(&H1)     'Code Charger
                If (statusBytes And tempValue) = tempValue Then
                    mblnBmsStatusCodeCharger = True
                Else
                    mblnBmsStatusCodeCharger = False
                End If

                tempValue = Convert.ToInt32(&H0)     'Code Okay
                If (statusBytes And tempValue) = tempValue Then
                    mblnBmsStatusCodeOkay = True
                Else
                    mblnBmsStatusCodeOkay = False
                End If
            Catch ex As Exception
                MessageBox.Show(ex.ToString)
            End Try


        End Set
    End Property
    Property NumOfCharges() As Integer
        Get
            Return mintNumOfCharges
        End Get
        Set(ByVal value As Integer)
            mintNumOfCharges = value
        End Set
    End Property
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

    Property TimeStamp300() As Date
        Get
            Return mdateTimeStamp300
        End Get
        Set(ByVal value As Date)
            mdateTimeStamp300 = value
        End Set
    End Property

    Property TimeStamp380() As Date
        Get
            Return mdateTimeStamp380
        End Get
        Set(ByVal value As Date)
            mdateTimeStamp380 = value
        End Set
    End Property

    Property TimeStamp400() As Date
        Get
            Return mdateTimeStamp400
        End Get
        Set(ByVal value As Date)
            mdateTimeStamp400 = value
        End Set
    End Property

    Property TimeStamp480() As Date
        Get
            Return mdateTimeStamp480
        End Get
        Set(ByVal value As Date)
            mdateTimeStamp480 = value
        End Set
    End Property

    Property TimeStamp500() As Date
        Get
            Return mdateTimeStamp500
        End Get
        Set(ByVal value As Date)
            mdateTimeStamp500 = value
        End Set
    End Property
    Property PackVoltage() As Single
        Get
            Return msngPackVoltage
        End Get
        Set(ByVal value As Single)
            msngPackVoltage = value
        End Set
    End Property
    Property HighestPackTempC() As Integer
        Get
            Return mintHighestPackTempC
        End Get
        Set(ByVal value As Integer)
            mintHighestPackTempC = value
        End Set
    End Property
    Property LowestPackTempC() As Integer
        Get
            Return mintLowestPackTempC
        End Get
        Set(ByVal value As Integer)
            mintLowestPackTempC = value
        End Set
    End Property
    Property PackDischageAmps() As Single
        Get
            Return msngPackDischargeAmps
        End Get
        Set(ByVal value As Single)
            msngPackDischargeAmps = value
        End Set
    End Property
    Property PackCapacityRemainingAh() As Integer
        Get
            Return mintPackCapacityRemainingAh
        End Get
        Set(ByVal value As Integer)
            mintPackCapacityRemainingAh = value
        End Set
    End Property
    Property AllowedCurrentPercentage() As Integer
        Get
            Return mintAllowedCurrentPercentage
        End Get
        Set(ByVal value As Integer)
            mintAllowedCurrentPercentage = value
        End Set
    End Property
    Property PackCapacityAh() As Integer
        Get
            Return mintPackCapacityAh
        End Get
        Set(ByVal value As Integer)
            mintPackCapacityAh = value
        End Set
    End Property
    Property MaxDischargeCRate() As Single
        Get
            Return msngMaxDischargeCRate
        End Get
        Set(ByVal value As Single)
            msngMaxDischargeCRate = value
        End Set
    End Property
    Property MaxChargeCRate() As Single
        Get
            Return msngMaxChargeCRate
        End Get
        Set(ByVal value As Single)
            msngMaxChargeCRate = value
        End Set
    End Property
    ReadOnly Property StatusCodeOkay() As Boolean
        Get
            Return mblnBmsStatusCodeOkay
        End Get
    End Property
    ReadOnly Property StatusCodeCharger() As Boolean
        Get
            Return mblnBmsStatusCodeCharger
        End Get
    End Property
    ReadOnly Property StatusCodeBattTempTooHigh() As Boolean
        Get
            Return mblnBmsStatusCodeBattTempTooHigh
        End Get
    End Property
    ReadOnly Property StatusCodeBattTempHigh() As Boolean
        Get
            Return mblnBmsStatusCodeBattTempHigh
        End Get
    End Property
    ReadOnly Property StatusCodeBattTempTooLow() As Boolean
        Get
            Return mblnBmsStatusCodeBattTempTooLow
        End Get
    End Property
    ReadOnly Property StatusCodeLowBatt() As Boolean
        Get
            Return mblnBmsStatusCodeLowBatt
        End Get
    End Property
    ReadOnly Property StatusCodeCriticalBatt() As Boolean
        Get
            Return mblnBmsStatusCodeCriticalBatt
        End Get
    End Property
    ReadOnly Property StatusCodeImbalance() As Boolean
        Get
            Return mblnBmsStatusCodeImbalance
        End Get
    End Property
    ReadOnly Property StatusCodeInternalFault() As Boolean
        Get
            Return mblnBmsStatusCodeInternalFault
        End Get
    End Property
    ReadOnly Property StatusCode0x0100() As Boolean
        Get
            Return mblnBmsStatusCode0x0100
        End Get
    End Property
    ReadOnly Property StatusCodeContactorClosed() As Boolean
        Get
            Return mblnBmsStatusCodeContactorClosed
        End Get
    End Property
    ReadOnly Property StatusCodeIsolationFault() As Boolean
        Get
            Return mblnBmsStatusCodeIsolationFault
        End Get
    End Property
    ReadOnly Property StatusCodeCellTooHigh() As Boolean
        Get
            Return mblnBmsStatusCodeCellTooHigh
        End Get
    End Property
    ReadOnly Property StatusCodeCellTooLow() As Boolean
        Get
            Return mblnBmsStatusCodeCellTooLow
        End Get
    End Property
    ReadOnly Property StatusCodeChargeHalt() As Boolean
        Get
            Return mblnBmsStatusCodeChargeHalt
        End Get
    End Property
    ReadOnly Property StatusCodeFull() As Boolean
        Get
            Return mblnBmsStatusCodeFull
        End Get
    End Property
    ReadOnly Property StatusCodeInternalDisable() As Boolean
        Get
            Return mblnBmsStatusCodeInternalDisable
        End Get
    End Property

    Property PackSensorTempC(ByVal index As Integer) As Integer
        Get
            Return mintPackSensorTempC(index)
        End Get
        Set(value As Integer)
            mintPackSensorTempC(index) = value
        End Set
    End Property

    Property PackSensorStatus(ByVal index As Integer) As Integer
        Get
            Return mintPackSensorStatus(index)
        End Get
        Set(value As Integer)
            mintPackSensorStatus(index) = value
        End Set
    End Property
End Class

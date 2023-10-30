Public Class sevconClass
    Private mintVelocity As Integer
    Private msngThrottleInputVoltage As Single
    Private mintThrottleValue As Integer
    Private mintMotorTemp As Integer
    Private msngBatteryVoltage As Single
    Private mintControllerHeatsinkTemp As Integer
    Private msngBatteryCurrent As Single
    Private msngCapacitorVoltage As Single
    Private mbytDigitalInputs As Byte
    Private mblnReverseInput As Boolean
    Private mblnForwardInput As Boolean
    Private mblnFS1Input As Boolean
    Private mblnKeySwitchInput As Boolean
    Private mblnHandbrakeSwitchInput As Boolean
    Private mstrDescription As String
    Private mdateTimeStamp As Date
    Private mblnErrorState As Boolean
    Private mblnLineContactorClosed As Boolean
    Private mintTargetVelocity As Integer
    Private msngMaxCurrentAllowed As Single
    Private msngContactorVoltage As Single
    Private msngTorque As Single
    Private msngOdometerTenthMiles As Single
    Private msngTripTenthMiles As Single
    Private mintKeyOnHours As Integer
    Private msngKeyOnMinutes As Single
    Private mintTractionHours As Integer
    Private msngTractionMinutes As Single

    Property Velocity() As Integer
        Get
            Return mintVelocity
        End Get
        Set(ByVal value As Integer)
            mintVelocity = value
        End Set
    End Property

    Property ThrottleInputVoltage() As Single
        Get
            Return msngThrottleInputVoltage
        End Get
        Set(ByVal value As Single)
            msngThrottleInputVoltage = value
        End Set
    End Property
    Property ThrottleValue() As Integer
        Get
            Return mintThrottleValue
        End Get
        Set(ByVal value As Integer)
            If value <= 1000 Then
                mintThrottleValue = value
            End If
        End Set
    End Property

    Property MotorTemp() As Integer
        Get
            Return mintMotorTemp
        End Get
        Set(ByVal value As Integer)
            mintMotorTemp = value
        End Set
    End Property

    Property BatteryVoltage() As Single
        Get
            Return msngBatteryVoltage
        End Get
        Set(ByVal value As Single)
            msngBatteryVoltage = value
        End Set
    End Property

    Property ControllerHeatsinkTemp() As Integer
        Get
            Return mintControllerHeatsinkTemp
        End Get
        Set(ByVal value As Integer)
            mintControllerHeatsinkTemp = value
        End Set
    End Property

    Property BatteryCurrent() As Single
        Get
            Return msngBatteryCurrent
        End Get
        Set(ByVal value As Single)
            msngBatteryCurrent = value
        End Set
    End Property

    Property CapacitorVoltage() As Single
        Get
            Return msngCapacitorVoltage
        End Get
        Set(ByVal value As Single)
            msngCapacitorVoltage = value
        End Set
    End Property

    Property DigitalInputs() As Byte
        '//1-N/A
        '//2-Dummy
        '//4-Dummy
        '//8-Dummy
        '//16-Dummy
        '//32-FS1 Switch
        '//64-Reverse Switch
        '//128-Forward Switch


        Get
            Return mbytDigitalInputs
        End Get
        Set(ByVal value As Byte)
            mbytDigitalInputs = value
            Dim tempValue As Int32
            Dim statusBytes As Int32
            Try
                statusBytes = Convert.ToInt32(value)
                tempValue = Convert.ToInt32(&H1)        'Forward Switch
                If (statusBytes And tempValue) = tempValue Then
                    mblnForwardInput = True
                    'Debug.Print("FP Forward = True")
                Else
                    mblnForwardInput = False
                    'Debug.Print("FP Forward = False")
                End If

                tempValue = Convert.ToInt32(&H2)        'Reverse Switch
                If (statusBytes And tempValue) = tempValue Then
                    mblnReverseInput = True
                Else
                    mblnReverseInput = False
                End If

                tempValue = Convert.ToInt32(&H4)        'FS1 Switch
                If (statusBytes And tempValue) = tempValue Then
                    mblnFS1Input = True
                Else
                    mblnFS1Input = False
                End If

            Catch ex As Exception

            End Try
                
        End Set
    End Property

    ReadOnly Property ReverseInput As Boolean
        Get
            Return mblnReverseInput
        End Get
    End Property

    ReadOnly Property ForwardInput As Boolean
        Get
            Return mblnForwardInput
        End Get
    End Property

    ReadOnly Property FS1Input As Boolean
        Get
            Return mblnFS1Input
        End Get
    End Property

    ReadOnly Property KeySwitchInput As Boolean
        Get
            Return mblnKeySwitchInput
        End Get
    End Property

    ReadOnly Property HandbrakeInput As Boolean
        Get
            Return mblnHandbrakeSwitchInput
        End Get
    End Property

    Property Description As String
        Get
            Return mstrDescription
        End Get
        Set(ByVal value As String)
            mstrDescription = value
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

    ReadOnly Property ErrorState As Boolean
        Get
            Return mblnErrorState
        End Get
    End Property

    Property ContactorClosed As Boolean
        Get
            Return mblnLineContactorClosed
        End Get
        Set(value As Boolean)
            mblnLineContactorClosed = value
        End Set
    End Property

    Property TargetVelocity() As Integer
        Get
            Return mintTargetVelocity
        End Get
        Set(ByVal value As Integer)
            mintTargetVelocity = value
        End Set
    End Property

    Property MaximumCurrentAllowed() As Single
        Get
            Return msngMaxCurrentAllowed
        End Get
        Set(ByVal value As Single)
            msngMaxCurrentAllowed = value
        End Set
    End Property

    Property LineContactorVoltage() As Single
        Get
            Return msngContactorVoltage
        End Get
        Set(ByVal value As Single)
            msngContactorVoltage = value
        End Set
    End Property

    Property Torque() As Single
        Get
            Return msngTorque
        End Get
        Set(ByVal value As Single)
            msngTorque = value
        End Set
    End Property

    Property OdometerTenthMiles() As Single
        Get
            Return msngOdometerTenthMiles
        End Get
        Set(ByVal value As Single)
            msngOdometerTenthMiles = value
        End Set
    End Property

    Property TripTenthMiles() As Single
        Get
            Return msngTripTenthMiles
        End Get
        Set(ByVal value As Single)
            msngTripTenthMiles = value
        End Set
    End Property

    Property KeyOnHours() As Integer
        Get
            Return mintKeyOnHours
        End Get
        Set(value As Integer)
            mintKeyOnHours = value
        End Set
    End Property

    Property KeyOnMinutes() As Single
        Get
            Return msngKeyOnMinutes
        End Get
        Set(value As Single)
            msngKeyOnMinutes = value
        End Set
    End Property

    Property TractionHours() As Integer
        Get
            Return mintTractionHours
        End Get
        Set(value As Integer)
            mintTractionHours = value
        End Set
    End Property

    Property TractionMinutes() As Single
        Get
            Return msngTractionMinutes
        End Get
        Set(value As Single)
            msngTractionMinutes = value
        End Set
    End Property
End Class

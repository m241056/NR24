#include "Simulink_example_CAN_Serial1.h"
#include "Simulink_example_CAN_Serial1_private.h"

/* Block signals (default storage) */
B_Simulink_example_CAN_Serial_T Simulink_example_CAN_Serial1_B;

/* Block states (default storage) */
DW_Simulink_example_CAN_Seria_T Simulink_example_CAN_Serial1_DW;

/*
* Set which subrates need to run this base step (base rate always runs).
* This function must be called prior to calling the model step function
* in order to "remember" which rates need to run this base step.  The
* buffering of events allows for overlapping preemption.
*/
void Simulink_example_CAN_Serial1_SetEventsForThisBaseStep(boolean_T *eventFlags)
{
  /* Task runs when its counter is zero, computed via rtmStepTask macro */
  eventFlags[1] = ((boolean_T)rtmStepTask(Simulink_example_CAN_Serial1_M, 1));
  eventFlags[2] = ((boolean_T)rtmStepTask(Simulink_example_CAN_Serial1_M, 2));
  eventFlags[3] = ((boolean_T)rtmStepTask(Simulink_example_CAN_Serial1_M, 3));
}

/*
*   This function updates active task flag for each subrate
* and rate transition flags for tasks that exchange data.
* The function assumes rate-monotonic multitasking scheduler.
* The function must be called at model base rate so that
* the generated code self-manages all its subrates and rate
* transition flags.
*/
static void rate_monotonic_scheduler(void)
{
  /* Compute which subrates run during the next base time step.  Subrates
  * are an integer multiple of the base rate counter.  Therefore, the subtask
  * counter is reset when it reaches its limit (zero means run).
  */
  (Simulink_example_CAN_Serial1_M->Timing.TaskCounters.TID[1])++;
  if ((Simulink_example_CAN_Serial1_M->Timing.TaskCounters.TID[1]) > 99)
  {/* Sample time: [0.1s, 0.0s] */
    Simulink_example_CAN_Serial1_M->Timing.TaskCounters.TID[1] = 0;
  }
  (Simulink_example_CAN_Serial1_M->Timing.TaskCounters.TID[2])++;

  if ((Simulink_example_CAN_Serial1_M->Timing.TaskCounters.TID[2]) > 499)
  {/* Sample time: [0.5s, 0.0s] */
    Simulink_example_CAN_Serial1_M->Timing.TaskCounters.TID[2] = 0;
  }
  (Simulink_example_CAN_Serial1_M->Timing.TaskCounters.TID[3])++;

  if ((Simulink_example_CAN_Serial1_M->Timing.TaskCounters.TID[3]) > 999)
  {/* Sample time: [1.0s, 0.0s] */
    Simulink_example_CAN_Serial1_M->Timing.TaskCounters.TID[3] = 0;
  }
}

/* Model step function for TID0 */
void Simulink_example_CAN_Serial1_step0(void) /* Sample time: [0.001s, 0.0s] */
{
  MW_SPI_FirstBitTransfer_Type MsbFirstTransferLoc;
  MW_SPI_Mode_type ClockModeValue;
  uint16_T rtb_DutyCycleBuck;

  {                                    /* Sample time: [0.001s, 0.0s] */
    rate_monotonic_scheduler();
  }

  /* ManualSwitch: '<Root>/Manual Switch1' incorporates:
  *  Constant: '<Root>/ 1'
  *  Constant: '<Root>/src2'
  */
  if (Simulink_example_CAN_Serial1_P.ManualSwitch1_CurrentSetting == 1U)
  {
    rtb_DutyCycleBuck = Simulink_example_CAN_Serial1_P.src2_Value;
  } else
  {
    rtb_DutyCycleBuck = Simulink_example_CAN_Serial1_P.u_Value;
  }

  /* End of ManualSwitch: '<Root>/Manual Switch1' */

  /* S-Function (c2802xpwm): '<Root>/ePWM' */

  /*-- Update CMPA value for ePWM1 --*/
  {
    EPwm1Regs.CMPA.bit.CMPA = (uint16_T)((uint32_T)EPwm1Regs.TBPRD *
    rtb_DutyCycleBuck * 0.01);
  }

  /* MATLABSystem: '<Root>/SPI Transmit' incorporates:
  *  Constant: '<Root>/ 2'
  */
  MW_SPI_SetSlaveSelect(Simulink_example_CAN_Serial1_DW.obj_p.MW_SPI_HANDLE, 0U,
    true);
    ClockModeValue = MW_SPI_MODE_0;
    MsbFirstTransferLoc = MW_SPI_MOST_SIGNIFICANT_BIT_FIRST;
    rtb_DutyCycleBuck = MW_SPI_SetFormat
    (Simulink_example_CAN_Serial1_DW.obj_p.MW_SPI_HANDLE, 8U, ClockModeValue,
      MsbFirstTransferLoc);
    if (rtb_DutyCycleBuck == 0U)
    {
      MW_SPI_Write_16bits(Simulink_example_CAN_Serial1_DW.obj_p.MW_SPI_HANDLE,
        &Simulink_example_CAN_Serial1_P.u_Value_d, 1UL, 0U);
    }

    /* End of MATLABSystem: '<Root>/SPI Transmit' */

    /* S-Function (c28xsci_tx): '<Root>/SCI Transmit' incorporates:
    *  Constant: '<Root>/ 3'
    */
    {
      /* Send additional data header */
      {
        char *String = "S";
        scia_xmit(String, 1, 1);
      }

      scia_xmit((char*)&Simulink_example_CAN_Serial1_P.u_Value_n, 2, 2);

      /* Send additional data terminator */
      {
        char *String = "E";
        scia_xmit(String, 1, 1);
      }
    }

    /* S-Function (c280xcanxmt): '<Root>/eCAN Transmit' incorporates:
    *  Constant: '<Root>/ 4'
    */
    {
      tCANMsgObject sTXCANMessage;
      unsigned char ucTXMsgData[2];
      ucTXMsgData[0] = (Simulink_example_CAN_Serial1_P.u_Value_e & 0xFF);
      ucTXMsgData[1] = (Simulink_example_CAN_Serial1_P.u_Value_e >> 8);
      sTXCANMessage.ui32MsgIDMask = 0;   // no mask needed for TX
      sTXCANMessage.ui32MsgLen = 2;      // size of message
      sTXCANMessage.ui32MsgID = 455;     // CAN message ID - use 1
      sTXCANMessage.pucMsgData = ucTXMsgData;// ptr to message content
      sTXCANMessage.ui32Flags = MSG_OBJ_NO_FLAGS;
      CANMessageSet(CANB_BASE, 2, &sTXCANMessage, MSG_OBJ_TYPE_TX);
    }

    /* S-Function (c280xi2c_tx): '<Root>/I2C Transmit' incorporates:
    *  Constant: '<Root>/ 5'
    */
    {
      int unsigned tx_loop= 0;
      while (I2caRegs.I2CFFTX.bit.TXFFST!=0 && tx_loop<10000 )
      tx_loop++;
      if (tx_loop!=10000)
      {
        I2caRegs.I2CSAR.bit.SAR = 80;    /* Set slave address*/
        I2caRegs.I2CCNT= 2;              /* Set data length */

        /* mode:1 (1:master 0:slave)  Addressing mode:0 (1:10-bit 0:7-bit)
        free data mode:0 (1:enbaled 0:disabled) digital loopback mode:0 (1:enabled 0:disabled)
        bit count:0 (0:8bit) stop condition:0 (1:enabled 0: disabled)*/
        I2caRegs.I2CMDR.all = 26144;
        tx_loop= 0;
        while (I2caRegs.I2CFFTX.bit.TXFFST>14 && tx_loop<10000)
        tx_loop++;
        if (tx_loop!=10000)
        {
          I2caRegs.I2CDXR.bit.DATA = (uint8_T)
          (Simulink_example_CAN_Serial1_P.u_Value_n1&0xFF);
          I2caRegs.I2CDXR.bit.DATA = (uint8_T)
          ((Simulink_example_CAN_Serial1_P.u_Value_n1>>8&0xFF));
        }
      }
    }

    /* S-Function (c2802xadc): '<Root>/ADC' */
    {
      /*  Internal Reference Voltage : Fixed scale 0 to 3.3 V range.  */
      /*  External Reference Voltage : Allowable ranges of VREFHI(ADCINA0) = 3.3 and VREFLO(tied to ground) = 0  */
      Simulink_example_CAN_Serial1_B.ADC = (AdcaResultRegs.ADCRESULT0);
    }

    /* S-Function (c280xi2c_rx): '<Root>/I2C Receive' */
    {
      int rx_loop= 0;
      int8_T rx_output= 0;
      I2caRegs.I2CSAR.bit.SAR = 80;      /* Set slave address*/
      I2caRegs.I2CCNT= 1;                /* Set data length */

      /* mode:1 (1:master 0:slave)  Addressing mode:0 (1:10-bit 0:7-bit)
      free data mode:0 (1:enbaled 0:disabled) digital loopback mode:0 (1:enabled 0:disabled)
      bit count:0 (0:8bit) NACK mode:0 (1:enabled 0: disabled) stop condition:0 (1:enabled 0: disabled)*/
      I2caRegs.I2CMDR.all = 25632;
      rx_loop= 0;
      rx_output= 0;
      while (I2caRegs.I2CFFRX.bit.RXFFST==0 && rx_loop<10000)
      rx_loop++;
      if (rx_loop!=10000)
      {
        rx_output = I2caRegs.I2CDRR.bit.DATA;
        if (rx_output > 127)
        {
          Simulink_example_CAN_Serial1_B.I2CReceive = rx_output-256;
        } else
        {
          Simulink_example_CAN_Serial1_B.I2CReceive = rx_output;
        }
      }
    }
}

/* Model step function for TID1 */
void Simulink_example_CAN_Serial1_step1(void) /* Sample time: [0.1s, 0.0s] */
{
  MW_SPI_FirstBitTransfer_Type MsbFirstTransferLoc;
  MW_SPI_Mode_type ClockModeValue;
  uint16_T rdDataRaw;

  /* S-Function (c28xsci_rx): '<Root>/SCI Receive' */
  {
    int i;
    int errFlg = NOERROR;
    unsigned int recbuff[1];
    for (i = 0; i < 1; i++)
    recbuff[i] = 0;

    /* Getting Data Head */
    {
      unsigned int recHead;
      int cnt = 0;
      int i = 0;
      char *expHead = "S";
      while (i < 1)
      {
        scia_rcv(&recHead, 1, SHORTLOOP, 1);
        if (recHead == expHead[i])
        {
          i++;
        } else
        {
          i = 0;
        }

        if (cnt++ > 16)
        {
          errFlg = TIMEOUT;
          goto RXERRA;
        }
      }
    }

    /* End of Getting Data Head */

    /* Receiving data */
    errFlg = scia_rcv(recbuff, 1, LONGLOOP, 1);
    if (errFlg != NOERROR)
    goto RXERRA;

    /* Getting Data Tail */
    {
      int i;
      char *expTail = "E";
      unsigned int recTail[1];
      scia_rcv(recTail, 1, LONGLOOP, 1);
      for (i = 0; i< 1; i++)
      {
        if (expTail[i] != recTail[i])
        {
          errFlg = DATAERR;
          goto RXERRA;
        }
      }
    }

    /* End of Getting Data Tail */
    memcpy( &Simulink_example_CAN_Serial1_B.SCIReceive, recbuff, 1);
    RXERRA:
    asm(" NOP");
  }

  /* MATLABSystem: '<Root>/SPI Receive' */
  if (Simulink_example_CAN_Serial1_DW.obj.SampleTime !=
    Simulink_example_CAN_Serial1_P.SPIReceive_SampleTime)
    {
      Simulink_example_CAN_Serial1_DW.obj.SampleTime =
      Simulink_example_CAN_Serial1_P.SPIReceive_SampleTime;
    }

    ClockModeValue = MW_SPI_MODE_0;
    MsbFirstTransferLoc = MW_SPI_MOST_SIGNIFICANT_BIT_FIRST;
    rdDataRaw = MW_SPI_SetFormat(Simulink_example_CAN_Serial1_DW.obj.MW_SPI_HANDLE,
      8U, ClockModeValue, MsbFirstTransferLoc);
    if (rdDataRaw == 0U)
    {
        MW_SPI_Read_16bits(Simulink_example_CAN_Serial1_DW.obj.MW_SPI_HANDLE,
          &rdDataRaw, 1UL, 0U);
    }

    /* End of MATLABSystem: '<Root>/SPI Receive' */
}

/* Model step function for TID2 */
void Simulink_example_CAN_Serial1_step2(void) /* Sample time: [0.5s, 0.0s] */
{
  /* S-Function (c280xgpio_do): '<Root>/Blink Blue LED' incorporates:
  *  Constant: '<Root>/ '
  */
  {
    if (Simulink_example_CAN_Serial1_P._Value)
    GpioDataRegs.GPASET.bit.GPIO31 = 1;
    else
    GpioDataRegs.GPACLEAR.bit.GPIO31 = 1;
  }

  /* S-Function (c280xgpio_do): '<Root>/Red LED' incorporates:
  *  Constant: '<Root>/   '
  */
  {
    if (Simulink_example_CAN_Serial1_P._Value_a)
    GpioDataRegs.GPBSET.bit.GPIO34 = 1;
    else
    GpioDataRegs.GPBCLEAR.bit.GPIO34 = 1;
  }

  /* Logic: '<Root>/Logical Operator3' incorporates:
  *  Constant: '<Root>/ Enable'
  */
  Simulink_example_CAN_Serial1_B.LogicalOperator3 =
  (Simulink_example_CAN_Serial1_P.Enable_Value == 0U);

  /* S-Function (c280xgpio_do): '<Root>/Enable Pulses_Active Low' */
  {
    if (Simulink_example_CAN_Serial1_B.LogicalOperator3)
    GpioDataRegs.GPDSET.bit.GPIO124 = 1;
    else
    GpioDataRegs.GPDCLEAR.bit.GPIO124 = 1;
  }
}

/* Model step function for TID3 */
void Simulink_example_CAN_Serial1_step3(void) /* Sample time: [1.0s, 0.0s] */
{
  /* S-Function (c280xcanrcv): '<Root>/eCAN Receive' */
  {
    tCANMsgObject sRXCANMessage;
    unsigned char ucRXMsgData[8] = { 0, 0, 0, 0, 0, 0, 0, 0 };

    sRXCANMessage.ui32MsgID = 455;     // CAN message ID
    sRXCANMessage.ui32MsgIDMask = 0;   // no mask needed for TX
    sRXCANMessage.ui32Flags = MSG_OBJ_NO_FLAGS;
    sRXCANMessage.ui32MsgLen = sizeof(ucRXMsgData);// size of message
    sRXCANMessage.pucMsgData = ucRXMsgData;// ptr to message content

    // Get the receive message
    CANMessageGet(CANB_BASE, 1, &sRXCANMessage, false);
    if (sRXCANMessage.ui32MsgLen > 0) {
      Simulink_example_CAN_Serial1_B.eCANReceive_o2[0] = ucRXMsgData[0] |
      (ucRXMsgData[1] << 8);
      Simulink_example_CAN_Serial1_B.eCANReceive_o2[1] = ucRXMsgData[2] |
      (ucRXMsgData[3] << 8);
      Simulink_example_CAN_Serial1_B.eCANReceive_o2[2] = ucRXMsgData[4] |
      (ucRXMsgData[5] << 8);
      Simulink_example_CAN_Serial1_B.eCANReceive_o2[3] = ucRXMsgData[6] |
      (ucRXMsgData[7] << 8);

      /* -- Call CAN RX Fcn-Call_0 -- */
    }
  }
}

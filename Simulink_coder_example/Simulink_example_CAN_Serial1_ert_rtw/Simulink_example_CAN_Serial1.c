/*
 * Academic License - for use in teaching, academic research, and meeting
 * course requirements at degree granting institutions only.  Not for
 * government, commercial, or other organizational use.
 *
 * File: Simulink_example_CAN_Serial1.c
 *
 * Code generated for Simulink model 'Simulink_example_CAN_Serial1'.
 *
 * Model version                  : 4.4
 * Simulink Coder version         : 9.5 (R2021a) 14-Nov-2020
 * C/C++ source code generated on : Thu Feb  9 11:10:24 2023
 *
 * Target selection: ert.tlc
 * Embedded hardware selection: Texas Instruments->C2000
 * Code generation objectives: Unspecified
 * Validation result: Not run
 */

#include "Simulink_example_CAN_Serial1.h"
#include "Simulink_example_CAN_Serial1_private.h"

/* Block signals (default storage) */
B_Simulink_example_CAN_Serial_T Simulink_example_CAN_Serial1_B;

/* Block states (default storage) */
DW_Simulink_example_CAN_Seria_T Simulink_example_CAN_Serial1_DW;

/* Real-time model */
static RT_MODEL_Simulink_example_CAN_T Simulink_example_CAN_Serial1_M_;
RT_MODEL_Simulink_example_CAN_T *const Simulink_example_CAN_Serial1_M =
  &Simulink_example_CAN_Serial1_M_;
static void rate_monotonic_scheduler(void);
uint16_T MW_adcAInitFlag = 0;

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
  if ((Simulink_example_CAN_Serial1_M->Timing.TaskCounters.TID[1]) > 99) {/* Sample time: [0.1s, 0.0s] */
    Simulink_example_CAN_Serial1_M->Timing.TaskCounters.TID[1] = 0;
  }

  (Simulink_example_CAN_Serial1_M->Timing.TaskCounters.TID[2])++;
  if ((Simulink_example_CAN_Serial1_M->Timing.TaskCounters.TID[2]) > 499) {/* Sample time: [0.5s, 0.0s] */
    Simulink_example_CAN_Serial1_M->Timing.TaskCounters.TID[2] = 0;
  }

  (Simulink_example_CAN_Serial1_M->Timing.TaskCounters.TID[3])++;
  if ((Simulink_example_CAN_Serial1_M->Timing.TaskCounters.TID[3]) > 999) {/* Sample time: [1.0s, 0.0s] */
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
  if (Simulink_example_CAN_Serial1_P.ManualSwitch1_CurrentSetting == 1U) {
    rtb_DutyCycleBuck = Simulink_example_CAN_Serial1_P.src2_Value;
  } else {
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
  if (rtb_DutyCycleBuck == 0U) {
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
    if (tx_loop!=10000) {
      I2caRegs.I2CSAR.bit.SAR = 80;    /* Set slave address*/
      I2caRegs.I2CCNT= 2;              /* Set data length */

      /* mode:1 (1:master 0:slave)  Addressing mode:0 (1:10-bit 0:7-bit)
         free data mode:0 (1:enbaled 0:disabled) digital loopback mode:0 (1:enabled 0:disabled)
         bit count:0 (0:8bit) stop condition:0 (1:enabled 0: disabled)*/
      I2caRegs.I2CMDR.all = 26144;
      tx_loop= 0;
      while (I2caRegs.I2CFFTX.bit.TXFFST>14 && tx_loop<10000)
        tx_loop++;
      if (tx_loop!=10000) {
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
    if (rx_loop!=10000) {
      rx_output = I2caRegs.I2CDRR.bit.DATA;
      if (rx_output > 127) {
        Simulink_example_CAN_Serial1_B.I2CReceive = rx_output-256;
      } else {
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
      while (i < 1) {
        scia_rcv(&recHead, 1, SHORTLOOP, 1);
        if (recHead == expHead[i]) {
          i++;
        } else {
          i = 0;
        }

        if (cnt++ > 16) {
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
      for (i = 0; i< 1; i++) {
        if (expTail[i] != recTail[i]) {
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
      Simulink_example_CAN_Serial1_P.SPIReceive_SampleTime) {
    Simulink_example_CAN_Serial1_DW.obj.SampleTime =
      Simulink_example_CAN_Serial1_P.SPIReceive_SampleTime;
  }

  ClockModeValue = MW_SPI_MODE_0;
  MsbFirstTransferLoc = MW_SPI_MOST_SIGNIFICANT_BIT_FIRST;
  rdDataRaw = MW_SPI_SetFormat(Simulink_example_CAN_Serial1_DW.obj.MW_SPI_HANDLE,
    8U, ClockModeValue, MsbFirstTransferLoc);
  if (rdDataRaw == 0U) {
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

/* Model initialize function */
void Simulink_example_CAN_Serial1_initialize(void)
{
  /* Registration code */

  /* initialize real-time model */
  (void) memset((void *)Simulink_example_CAN_Serial1_M, 0,
                sizeof(RT_MODEL_Simulink_example_CAN_T));

  /* block I/O */
  (void) memset(((void *) &Simulink_example_CAN_Serial1_B), 0,
                sizeof(B_Simulink_example_CAN_Serial_T));

  /* states (dwork) */
  (void) memset((void *)&Simulink_example_CAN_Serial1_DW, 0,
                sizeof(DW_Simulink_example_CAN_Seria_T));

  {
    MW_SPI_FirstBitTransfer_Type MsbFirstTransferLoc;
    MW_SPI_Mode_type ClockModeValue;
    uint32_T SPIPinsLoc;
    uint32_T SSPinNameLoc;
    codertarget_tic2000_blocks_SP_T *obj_0;
    codertarget_tic2000_blocks__f_T *obj;

    /* Start for S-Function (c2802xpwm): '<Root>/ePWM' */

    /*** Initialize ePWM1 modules ***/
    {
      /*  // Time Base Control Register
         EPwm1Regs.TBCTL.bit.CTRMODE              = 2;          // Counter Mode
         EPwm1Regs.TBCTL.bit.SYNCOSEL             = 1;          // Sync Output Select

         EPwm1Regs.TBCTL.bit.PRDLD                = 0;          // Shadow select

         EPwm1Regs.TBCTL2.bit.PRDLDSYNC           = 0;          // Shadow select

         EPwm1Regs.TBCTL.bit.PHSEN                = 0;          // Phase Load Enable
         EPwm1Regs.TBCTL.bit.PHSDIR               = 0;          // Phase Direction Bit
         EPwm1Regs.TBCTL.bit.HSPCLKDIV            = 0;          // High Speed TBCLK Pre-scaler
         EPwm1Regs.TBCTL.bit.CLKDIV               = 2;          // Time Base Clock Pre-scaler
         EPwm1Regs.TBCTL.bit.SWFSYNC              = 0;          // Software Force Sync Pulse
       */
      EPwm1Regs.TBCTL.all = (EPwm1Regs.TBCTL.all & ~0x3FFF) | 0x812;
      EPwm1Regs.TBCTL2.all = (EPwm1Regs.TBCTL2.all & ~0xC000) | 0x0;

      /*-- Setup Time-Base (TB) Submodule --*/
      EPwm1Regs.TBPRD = 1250;          // Time Base Period Register

      /* // Time-Base Phase Register
         EPwm1Regs.TBPHS.bit.TBPHS               = 0;          // Phase offset register
       */
      EPwm1Regs.TBPHS.all = (EPwm1Regs.TBPHS.all & ~0xFFFF0000) | 0x0;

      // Time Base Counter Register
      EPwm1Regs.TBCTR = 0x0000;        /* Clear counter*/

      /*-- Setup Counter_Compare (CC) Submodule --*/
      /*	// Counter Compare Control Register

         EPwm1Regs.CMPCTL.bit.LOADASYNC           = 0U;          // Active Compare A Load SYNC Option
         EPwm1Regs.CMPCTL.bit.LOADBSYNC           = 1U;          // Active Compare B Load SYNC Option
         EPwm1Regs.CMPCTL.bit.LOADAMODE           = 0U;          // Active Compare A Load
         EPwm1Regs.CMPCTL.bit.LOADBMODE           = 0U;          // Active Compare B Load
         EPwm1Regs.CMPCTL.bit.SHDWAMODE           = 0;          // Compare A Register Block Operating Mode
         EPwm1Regs.CMPCTL.bit.SHDWBMODE           = 0;          // Compare B Register Block Operating Mode
       */
      EPwm1Regs.CMPCTL.all = (EPwm1Regs.CMPCTL.all & ~0x3C5F) | 0x1000;

      /* EPwm1Regs.CMPCTL2.bit.SHDWCMODE           = 0;          // Compare C Register Block Operating Mode
         EPwm1Regs.CMPCTL2.bit.SHDWDMODE           = 0;          // Compare D Register Block Operating Mode
         EPwm1Regs.CMPCTL2.bit.LOADCSYNC           = 0U;          // Active Compare C Load SYNC Option
         EPwm1Regs.CMPCTL2.bit.LOADDSYNC           = 0U;          // Active Compare D Load SYNC Option
         EPwm1Regs.CMPCTL2.bit.LOADCMODE           = 0U;          // Active Compare C Load
         EPwm1Regs.CMPCTL2.bit.LOADDMODE           = 2U;          // Active Compare D Load
       */
      EPwm1Regs.CMPCTL2.all = (EPwm1Regs.CMPCTL2.all & ~0x3C5F) | 0x8;
      EPwm1Regs.CMPA.bit.CMPA = 0;     // Counter Compare A Register
      EPwm1Regs.CMPB.bit.CMPB = 0;     // Counter Compare B Register
      EPwm1Regs.CMPC = 0;              // Counter Compare C Register
      EPwm1Regs.CMPD = 0;              // Counter Compare D Register

      /*-- Setup Action-Qualifier (AQ) Submodule --*/
      EPwm1Regs.AQCTLA.all = 150;
                               // Action Qualifier Control Register For Output A
      EPwm1Regs.AQCTLB.all = 2345;
                               // Action Qualifier Control Register For Output B

      /*	// Action Qualifier Software Force Register
         EPwm1Regs.AQSFRC.bit.RLDCSF              = 0;          // Reload from Shadow Options
       */
      EPwm1Regs.AQSFRC.all = (EPwm1Regs.AQSFRC.all & ~0xC0) | 0x0;

      /*	// Action Qualifier Continuous S/W Force Register
         EPwm1Regs.AQCSFRC.bit.CSFA               = 0;          // Continuous Software Force on output A
         EPwm1Regs.AQCSFRC.bit.CSFB               = 0;          // Continuous Software Force on output B
       */
      EPwm1Regs.AQCSFRC.all = (EPwm1Regs.AQCSFRC.all & ~0xF) | 0x0;

      /*-- Setup Dead-Band Generator (DB) Submodule --*/
      /*	// Dead-Band Generator Control Register
         EPwm1Regs.DBCTL.bit.OUT_MODE             = 3;          // Dead Band Output Mode Control
         EPwm1Regs.DBCTL.bit.IN_MODE              = 0;          // Dead Band Input Select Mode Control
         EPwm1Regs.DBCTL.bit.POLSEL               = 2;          // Polarity Select Control
         EPwm1Regs.DBCTL.bit.HALFCYCLE            = 0;          // Half Cycle Clocking Enable
         EPwm1Regs.DBCTL.bit.SHDWDBREDMODE        = 0;          // DBRED shadow mode
         EPwm1Regs.DBCTL.bit.SHDWDBFEDMODE        = 0;          // DBFED shadow mode
         EPwm1Regs.DBCTL.bit.LOADREDMODE          = 4U;        // DBRED load
         EPwm1Regs.DBCTL.bit.LOADFEDMODE          = 4U;        // DBFED load
       */
      EPwm1Regs.DBCTL.all = (EPwm1Regs.DBCTL.all & ~0x8FFF) | 0xB;
      EPwm1Regs.DBRED.bit.DBRED = (uint16_T)(0.0);
                         // Dead-Band Generator Rising Edge Delay Count Register
      EPwm1Regs.DBFED.bit.DBFED = (uint16_T)(0.0);
                        // Dead-Band Generator Falling Edge Delay Count Register

      /*-- Setup Event-Trigger (ET) Submodule --*/
      /*	// Event Trigger Selection and Pre-Scale Register
         EPwm1Regs.ETSEL.bit.SOCAEN               = 1;          // Start of Conversion A Enable
         EPwm1Regs.ETSEL.bit.SOCASELCMP           = 0;
         EPwm1Regs.ETSEL.bit.SOCASEL              = 2;          // Start of Conversion A Select
         EPwm1Regs.ETPS.bit.SOCPSSEL              = 1;          // EPWM1SOC Period Select
         EPwm1Regs.ETSOCPS.bit.SOCAPRD2           = 1;
         EPwm1Regs.ETSEL.bit.SOCBEN               = 0;          // Start of Conversion B Enable
         EPwm1Regs.ETSEL.bit.SOCBSELCMP           = 0;
         EPwm1Regs.ETSEL.bit.SOCBSEL              = 1;          // Start of Conversion A Select
         EPwm1Regs.ETPS.bit.SOCPSSEL              = 1;          // EPWM1SOCB Period Select
         EPwm1Regs.ETSOCPS.bit.SOCBPRD2           = 1;
         EPwm1Regs.ETSEL.bit.INTEN                = 0;          // EPWM1INTn Enable
         EPwm1Regs.ETSEL.bit.INTSELCMP            = 0;
         EPwm1Regs.ETSEL.bit.INTSEL               = 1;          // Start of Conversion A Select
         EPwm1Regs.ETPS.bit.INTPSSEL              = 1;          // EPWM1INTn Period Select
         EPwm1Regs.ETINTPS.bit.INTPRD2            = 1;
       */
      EPwm1Regs.ETSEL.all = (EPwm1Regs.ETSEL.all & ~0xFF7F) | 0x1A01;
      EPwm1Regs.ETPS.all = (EPwm1Regs.ETPS.all & ~0x30) | 0x30;
      EPwm1Regs.ETSOCPS.all = (EPwm1Regs.ETSOCPS.all & ~0xF0F) | 0x101;
      EPwm1Regs.ETINTPS.all = (EPwm1Regs.ETINTPS.all & ~0xF) | 0x1;

      /*-- Setup PWM-Chopper (PC) Submodule --*/
      /*	// PWM Chopper Control Register
         EPwm1Regs.PCCTL.bit.CHPEN                = 0;          // PWM chopping enable
         EPwm1Regs.PCCTL.bit.CHPFREQ              = 0;          // Chopping clock frequency
         EPwm1Regs.PCCTL.bit.OSHTWTH              = 0;          // One-shot pulse width
         EPwm1Regs.PCCTL.bit.CHPDUTY              = 0;          // Chopping clock Duty cycle
       */
      EPwm1Regs.PCCTL.all = (EPwm1Regs.PCCTL.all & ~0x7FF) | 0x0;

      /*-- Set up Trip-Zone (TZ) Submodule --*/
      EALLOW;
      EPwm1Regs.TZSEL.all = 0;         // Trip Zone Select Register

      /*	// Trip Zone Control Register
         EPwm1Regs.TZCTL.bit.TZA                  = 3;          // TZ1 to TZ6 Trip Action On EPWM1A
         EPwm1Regs.TZCTL.bit.TZB                  = 3;          // TZ1 to TZ6 Trip Action On EPWM1B
         EPwm1Regs.TZCTL.bit.DCAEVT1              = 3;          // EPWM1A action on DCAEVT1
         EPwm1Regs.TZCTL.bit.DCAEVT2              = 3;          // EPWM1A action on DCAEVT2
         EPwm1Regs.TZCTL.bit.DCBEVT1              = 3;          // EPWM1B action on DCBEVT1
         EPwm1Regs.TZCTL.bit.DCBEVT2              = 3;          // EPWM1B action on DCBEVT2
       */
      EPwm1Regs.TZCTL.all = (EPwm1Regs.TZCTL.all & ~0xFFF) | 0xFFF;

      /*	// Trip Zone Enable Interrupt Register
         EPwm1Regs.TZEINT.bit.OST                 = 0;          // Trip Zones One Shot Int Enable
         EPwm1Regs.TZEINT.bit.CBC                 = 0;          // Trip Zones Cycle By Cycle Int Enable
         EPwm1Regs.TZEINT.bit.DCAEVT1             = 0;          // Digital Compare A Event 1 Int Enable
         EPwm1Regs.TZEINT.bit.DCAEVT2             = 0;          // Digital Compare A Event 2 Int Enable
         EPwm1Regs.TZEINT.bit.DCBEVT1             = 0;          // Digital Compare B Event 1 Int Enable
         EPwm1Regs.TZEINT.bit.DCBEVT2             = 0;          // Digital Compare B Event 2 Int Enable
       */
      EPwm1Regs.TZEINT.all = (EPwm1Regs.TZEINT.all & ~0x7E) | 0x0;

      /*	// Digital Compare A Control Register
         EPwm1Regs.DCACTL.bit.EVT1SYNCE           = 0;          // DCAEVT1 SYNC Enable
         EPwm1Regs.DCACTL.bit.EVT1SOCE            = 1;          // DCAEVT1 SOC Enable
         EPwm1Regs.DCACTL.bit.EVT1FRCSYNCSEL      = 0;          // DCAEVT1 Force Sync Signal
         EPwm1Regs.DCACTL.bit.EVT1SRCSEL          = 0;          // DCAEVT1 Source Signal
         EPwm1Regs.DCACTL.bit.EVT2FRCSYNCSEL      = 0;          // DCAEVT2 Force Sync Signal
         EPwm1Regs.DCACTL.bit.EVT2SRCSEL          = 0;          // DCAEVT2 Source Signal
       */
      EPwm1Regs.DCACTL.all = (EPwm1Regs.DCACTL.all & ~0x30F) | 0x4;

      /*	// Digital Compare B Control Register
         EPwm1Regs.DCBCTL.bit.EVT1SYNCE           = 0;          // DCBEVT1 SYNC Enable
         EPwm1Regs.DCBCTL.bit.EVT1SOCE            = 0;          // DCBEVT1 SOC Enable
         EPwm1Regs.DCBCTL.bit.EVT1FRCSYNCSEL      = 0;          // DCBEVT1 Force Sync Signal
         EPwm1Regs.DCBCTL.bit.EVT1SRCSEL          = 0;          // DCBEVT1 Source Signal
         EPwm1Regs.DCBCTL.bit.EVT2FRCSYNCSEL      = 0;          // DCBEVT2 Force Sync Signal
         EPwm1Regs.DCBCTL.bit.EVT2SRCSEL          = 0;          // DCBEVT2 Source Signal
       */
      EPwm1Regs.DCBCTL.all = (EPwm1Regs.DCBCTL.all & ~0x30F) | 0x0;

      /*	// Digital Compare Trip Select Register
         EPwm1Regs.DCTRIPSEL.bit.DCAHCOMPSEL      = 0;          // Digital Compare A High COMP Input Select

         EPwm1Regs.DCTRIPSEL.bit.DCALCOMPSEL      = 1;          // Digital Compare A Low COMP Input Select
         EPwm1Regs.DCTRIPSEL.bit.DCBHCOMPSEL      = 0;          // Digital Compare B High COMP Input Select
         EPwm1Regs.DCTRIPSEL.bit.DCBLCOMPSEL      = 1;          // Digital Compare B Low COMP Input Select

       */
      EPwm1Regs.DCTRIPSEL.all = (EPwm1Regs.DCTRIPSEL.all & ~ 0xFFFF) | 0x1010;

      /*	// Trip Zone Digital Comparator Select Register
         EPwm1Regs.TZDCSEL.bit.DCAEVT1            = 0;          // Digital Compare Output A Event 1
         EPwm1Regs.TZDCSEL.bit.DCAEVT2            = 0;          // Digital Compare Output A Event 2
         EPwm1Regs.TZDCSEL.bit.DCBEVT1            = 0;          // Digital Compare Output B Event 1
         EPwm1Regs.TZDCSEL.bit.DCBEVT2            = 0;          // Digital Compare Output B Event 2
       */
      EPwm1Regs.TZDCSEL.all = (EPwm1Regs.TZDCSEL.all & ~0xFFF) | 0x0;

      /*	// Digital Compare Filter Control Register
         EPwm1Regs.DCFCTL.bit.BLANKE              = 0;          // Blanking Enable/Disable
         EPwm1Regs.DCFCTL.bit.PULSESEL            = 1;          // Pulse Select for Blanking & Capture Alignment
         EPwm1Regs.DCFCTL.bit.BLANKINV            = 0;          // Blanking Window Inversion
         EPwm1Regs.DCFCTL.bit.SRCSEL              = 0;          // Filter Block Signal Source Select
       */
      EPwm1Regs.DCFCTL.all = (EPwm1Regs.DCFCTL.all & ~0x3F) | 0x10;
      EPwm1Regs.DCFOFFSET = 0;         // Digital Compare Filter Offset Register
      EPwm1Regs.DCFWINDOW = 0;         // Digital Compare Filter Window Register

      /*	// Digital Compare Capture Control Register
         EPwm1Regs.DCCAPCTL.bit.CAPE              = 0;          // Counter Capture Enable
       */
      EPwm1Regs.DCCAPCTL.all = (EPwm1Regs.DCCAPCTL.all & ~0x1) | 0x0;

      /*	// HRPWM Configuration Register
         EPwm1Regs.HRCNFG.bit.SWAPAB              = 0;          // Swap EPWMA and EPWMB Outputs Bit
         EPwm1Regs.HRCNFG.bit.SELOUTB             = 1;          // EPWMB Output Selection Bit
       */
      EPwm1Regs.HRCNFG.all = (EPwm1Regs.HRCNFG.all & ~0xA0) | 0x20;

      /* Update the Link Registers with the link value for all the Compare values and TBPRD */
      /* No error is thrown if the ePWM register exists in the model or not */
      EPwm1Regs.EPWMXLINK.bit.TBPRDLINK = 0;
      EPwm1Regs.EPWMXLINK.bit.CMPALINK = 0;
      EPwm1Regs.EPWMXLINK.bit.CMPBLINK = 0;
      EPwm1Regs.EPWMXLINK.bit.CMPCLINK = 0;
      EPwm1Regs.EPWMXLINK.bit.CMPDLINK = 0;

      /* SYNCPER - Peripheral synchronization output event
         EPwm1Regs.HRPCTL.bit.PWMSYNCSEL            = 1;          // EPWMSYNCPER selection
         EPwm1Regs.HRPCTL.bit.PWMSYNCSELX           = 0;          //  EPWMSYNCPER selection
       */
      EPwm1Regs.HRPCTL.all = (EPwm1Regs.HRPCTL.all & ~0x72) | 0x2;
      EDIS;
      EALLOW;

      /* Enable TBCLK within the EPWM*/
      CpuSysRegs.PCLKCR0.bit.TBCLKSYNC = 1;
      EDIS;
    }

    /* Start for S-Function (c280xcanxmt): '<Root>/eCAN Transmit' incorporates:
     *  Constant: '<Root>/ 4'
     */
    {
    }

    /* Start for S-Function (c2802xadc): '<Root>/ADC' */
    if (MW_adcAInitFlag == 0) {
      InitAdcA();
      MW_adcAInitFlag = 1;
    }

    config_ADCA_SOC0 ();

    /* Start for S-Function (c280xi2c_rx): '<Root>/I2C Receive' */

    /* Initialize Simulink_example_CAN_Serial1_B.I2CReceive */
    {
      Simulink_example_CAN_Serial1_B.I2CReceive = (int8_T)0.0;
    }

    /* Start for S-Function (c28xsci_rx): '<Root>/SCI Receive' */

    /* Initialize Simulink_example_CAN_Serial1_B.SCIReceive */
    {
      Simulink_example_CAN_Serial1_B.SCIReceive = (uint8_T)0.0;
    }

    /* Start for S-Function (c280xgpio_do): '<Root>/Blink Blue LED' incorporates:
     *  Constant: '<Root>/ '
     */
    EALLOW;
    GpioCtrlRegs.GPAMUX2.all &= 0x3FFFFFFF;
    GpioCtrlRegs.GPADIR.all |= 0x80000000;
    EDIS;

    /* Start for S-Function (c280xgpio_do): '<Root>/Red LED' incorporates:
     *  Constant: '<Root>/   '
     */
    EALLOW;
    GpioCtrlRegs.GPBMUX1.all &= 0xFFFFFFCF;
    GpioCtrlRegs.GPBDIR.all |= 0x4;
    EDIS;

    /* Start for S-Function (c280xgpio_do): '<Root>/Enable Pulses_Active Low' */
    EALLOW;
    GpioCtrlRegs.GPDMUX2.all &= 0xFCFFFFFF;
    GpioCtrlRegs.GPDDIR.all |= 0x10000000;
    EDIS;

    /* Start for S-Function (c280xcanrcv): '<Root>/eCAN Receive' */
    {
      tCANMsgObject sRXCANMessage;
      unsigned char ucRXMsgData[8]= { 0, 0, 0, 0, 0, 0, 0, 0 };

      sRXCANMessage.ui32MsgID = 455;   // CAN message ID
      sRXCANMessage.ui32MsgIDMask = 0; // no mask needed for TX
      sRXCANMessage.ui32Flags = MSG_OBJ_NO_FLAGS;
      sRXCANMessage.ui32MsgLen = sizeof(ucRXMsgData);// size of message is 4
      sRXCANMessage.pucMsgData = ucRXMsgData;// ptr to message content

      // Setup the message object being used to receive messages
      CANMessageSet(CANB_BASE, 1, &sRXCANMessage, MSG_OBJ_TYPE_RX);
    }

    /* Initialize Simulink_example_CAN_Serial1_B.eCANReceive_o2[0] */
    {
      Simulink_example_CAN_Serial1_B.eCANReceive_o2[0] = (uint16_T)0.0;
      Simulink_example_CAN_Serial1_B.eCANReceive_o2[1] = (uint16_T)0.0;
      Simulink_example_CAN_Serial1_B.eCANReceive_o2[2] = (uint16_T)0.0;
      Simulink_example_CAN_Serial1_B.eCANReceive_o2[3] = (uint16_T)0.0;
    }

    /* Start for MATLABSystem: '<Root>/SPI Transmit' */
    Simulink_example_CAN_Serial1_DW.obj_p.matlabCodegenIsDeleted = true;
    Simulink_example_CAN_Serial1_DW.obj_p.isInitialized = 0L;
    Simulink_example_CAN_Serial1_DW.obj_p.matlabCodegenIsDeleted = false;
    obj = &Simulink_example_CAN_Serial1_DW.obj_p;
    Simulink_example_CAN_Serial1_DW.obj_p.isSetupComplete = false;
    Simulink_example_CAN_Serial1_DW.obj_p.isInitialized = 1L;
    SSPinNameLoc = MW_UNDEFINED_VALUE;
    SPIPinsLoc = MW_UNDEFINED_VALUE;
    obj->MW_SPI_HANDLE = MW_SPI_Open(0UL, SPIPinsLoc, SPIPinsLoc, SPIPinsLoc,
      SSPinNameLoc, true, 0U);
    ClockModeValue = MW_SPI_MODE_0;
    MsbFirstTransferLoc = MW_SPI_MOST_SIGNIFICANT_BIT_FIRST;
    MW_SPI_SetFormat(Simulink_example_CAN_Serial1_DW.obj_p.MW_SPI_HANDLE, 8U,
                     ClockModeValue, MsbFirstTransferLoc);
    Simulink_example_CAN_Serial1_DW.obj_p.isSetupComplete = true;

    /* Start for MATLABSystem: '<Root>/SPI Receive' */
    Simulink_example_CAN_Serial1_DW.obj.matlabCodegenIsDeleted = true;
    Simulink_example_CAN_Serial1_DW.obj.SampleTime = -1.0;
    Simulink_example_CAN_Serial1_DW.obj.isInitialized = 0L;
    Simulink_example_CAN_Serial1_DW.obj.matlabCodegenIsDeleted = false;
    Simulink_example_CAN_Serial1_DW.obj.SampleTime =
      Simulink_example_CAN_Serial1_P.SPIReceive_SampleTime;
    obj_0 = &Simulink_example_CAN_Serial1_DW.obj;
    Simulink_example_CAN_Serial1_DW.obj.isSetupComplete = false;
    Simulink_example_CAN_Serial1_DW.obj.isInitialized = 1L;
    SSPinNameLoc = MW_UNDEFINED_VALUE;
    SPIPinsLoc = MW_UNDEFINED_VALUE;
    obj_0->MW_SPI_HANDLE = MW_SPI_Open(0UL, SPIPinsLoc, SPIPinsLoc, SPIPinsLoc,
      SSPinNameLoc, true, 0U);
    ClockModeValue = MW_SPI_MODE_0;
    MsbFirstTransferLoc = MW_SPI_MOST_SIGNIFICANT_BIT_FIRST;
    MW_SPI_SetFormat(Simulink_example_CAN_Serial1_DW.obj.MW_SPI_HANDLE, 8U,
                     ClockModeValue, MsbFirstTransferLoc);
    Simulink_example_CAN_Serial1_DW.obj.isSetupComplete = true;
  }
}

/* Model terminate function */
void Simulink_example_CAN_Serial1_terminate(void)
{
  uint32_T PinNameLoc;
  uint32_T SPIPinsLoc;

  /* Terminate for MATLABSystem: '<Root>/SPI Transmit' */
  if (!Simulink_example_CAN_Serial1_DW.obj_p.matlabCodegenIsDeleted) {
    Simulink_example_CAN_Serial1_DW.obj_p.matlabCodegenIsDeleted = true;
    if ((Simulink_example_CAN_Serial1_DW.obj_p.isInitialized == 1L) &&
        Simulink_example_CAN_Serial1_DW.obj_p.isSetupComplete) {
      PinNameLoc = MW_UNDEFINED_VALUE;
      SPIPinsLoc = MW_UNDEFINED_VALUE;
      MW_SPI_Close(Simulink_example_CAN_Serial1_DW.obj_p.MW_SPI_HANDLE,
                   SPIPinsLoc, SPIPinsLoc, SPIPinsLoc, PinNameLoc);
    }
  }

  /* End of Terminate for MATLABSystem: '<Root>/SPI Transmit' */

  /* Terminate for MATLABSystem: '<Root>/SPI Receive' */
  if (!Simulink_example_CAN_Serial1_DW.obj.matlabCodegenIsDeleted) {
    Simulink_example_CAN_Serial1_DW.obj.matlabCodegenIsDeleted = true;
    if ((Simulink_example_CAN_Serial1_DW.obj.isInitialized == 1L) &&
        Simulink_example_CAN_Serial1_DW.obj.isSetupComplete) {
      PinNameLoc = MW_UNDEFINED_VALUE;
      SPIPinsLoc = MW_UNDEFINED_VALUE;
      MW_SPI_Close(Simulink_example_CAN_Serial1_DW.obj.MW_SPI_HANDLE, SPIPinsLoc,
                   SPIPinsLoc, SPIPinsLoc, PinNameLoc);
    }
  }

  /* End of Terminate for MATLABSystem: '<Root>/SPI Receive' */
}

/*
 * File trailer for generated code.
 *
 * [EOF]
 */

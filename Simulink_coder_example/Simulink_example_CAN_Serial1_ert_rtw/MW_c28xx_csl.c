#include "c2000BoardSupport.h"
#include "F2837xD_device.h"
#include "F2837xD_Examples.h"
#include "F2837xD_GlobalPrototypes.h"
#include "rtwtypes.h"
#include "Simulink_example_CAN_Serial1.h"
#include "Simulink_example_CAN_Serial1_private.h"

void enableExtInterrupt (void);
void disableWatchdog(void)
{
  int *WatchdogWDCR = (void *) 0x7029;
  asm(" EALLOW ");
  *WatchdogWDCR = 0x0068;
  asm(" EDIS ");
}

void init_SCI(void)
{                                      /* initialize SCI & FIFO registers */
  EALLOW;

  /*
   * Initialize SCI_A with following parameters:
   *    BaudRate              : 12500000
   *    CharacterLengthBits   : 8
   *    EnableLoopBack        : 0
   *    NumberOfStopBits      : 1
   *    ParityMode            : None
   *    SuspensionMode        : Free_run
   *    CommMode              : Raw_data
   */
  CpuSysRegs.PCLKCR7.bit.SCI_A = 1;
  asm(" NOP");
  SciaRegs.SCICCR.bit.STOPBITS = 0;
                    /*Number of stop bits. (0: One stop bit, 1: Two stop bits)*/
  SciaRegs.SCICCR.bit.PARITY = 0;/*Parity mode (0: Odd parity, 1: Even parity)*/
  SciaRegs.SCICCR.bit.PARITYENA = 0;   /*Enable Pary Mode */
  SciaRegs.SCICCR.bit.LOOPBKENA = 0;   /*Loop Back enable*/
  SciaRegs.SCICCR.bit.ADDRIDLE_MODE = 0;/*ADDR/IDLE Mode control*/
  SciaRegs.SCICCR.bit.SCICHAR = 7;     /*Character length*/
  SciaRegs.SCICTL1.bit.RXERRINTENA = 0;/*Disable receive error interrupt*/
  SciaRegs.SCICTL1.bit.SWRESET = 1;    /*Software reset*/
  SciaRegs.SCICTL1.bit.TXENA = 1;      /* SCI transmitter enable*/
  SciaRegs.SCICTL1.bit.RXENA = 1;      /* SCI receiver enable*/
  SciaRegs.SCIHBAUD.bit.BAUD = 0U;
  SciaRegs.SCILBAUD.bit.BAUD = 1U;

  /*Free run, continue SCI operation regardless of suspend*/
  SciaRegs.SCIPRI.bit.FREESOFT = 3;
  SciaRegs.SCIFFCT.bit.ABDCLR = 0;
  SciaRegs.SCIFFCT.bit.CDC = 0;
  SciaRegs.SCIFFTX.bit.SCIRST = 1;     /* SCI reset rx/tx channels*/
  SciaRegs.SCIFFTX.bit.SCIFFENA = 1;   /* SCI FIFO enhancements are enabled.*/
  SciaRegs.SCIFFTX.bit.TXFIFORESET = 1;/* Re-enable transmit FIFO operation.*/
  SciaRegs.SCIFFRX.bit.RXFIFORESET = 1;/* Re-enable receive FIFO operation.*/
  EDIS;
}

void init_SCI_GPIO(void)
{
  EALLOW;
  GpioCtrlRegs.GPBQSEL1.bit.GPIO43 = 3;/*Asynch input GPIO43 SCIRXDA*/
  GpioCtrlRegs.GPBPUD.bit.GPIO43 = 0;  /*Enable pull-up for GPIO43*/
  GpioCtrlRegs.GPBGMUX1.bit.GPIO43 = 3;
  GpioCtrlRegs.GPBMUX1.bit.GPIO43 = 3; /*Configure GPIO43 as SCIRXDA*/
  GpioCtrlRegs.GPBPUD.bit.GPIO42 = 0;  /*Enable pull-up for GPIO42*/
  GpioCtrlRegs.GPBGMUX1.bit.GPIO42 = 3;
  GpioCtrlRegs.GPBMUX1.bit.GPIO42 = 3; /*Configure GPIO42 as SCITXDA*/
  EDIS;
}

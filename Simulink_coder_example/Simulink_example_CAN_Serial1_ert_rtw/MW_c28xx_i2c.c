#include "c2000BoardSupport.h"
#include "F2837xD_device.h"
#include "F2837xD_Examples.h"
#include "F2837xD_GlobalPrototypes.h"
#include "rtwtypes.h"
#include "Simulink_example_CAN_Serial1.h"
#include "Simulink_example_CAN_Serial1_private.h"

void init_I2C_GPIO(void)
{
  EALLOW;                              /* Initial I2C GPIO pin*/
  GpioCtrlRegs.GPDPUD.bit.GPIO104 = 0; /* Enable pull-up on GPIO104 (SDAA)*/
  GpioCtrlRegs.GPDGMUX1.bit.GPIO104 = 0;/* Configure GPIO104 as SDAA*/
  GpioCtrlRegs.GPDMUX1.bit.GPIO104 = 1;/* Configure GPIO104 as SDAA*/
  GpioCtrlRegs.GPDPUD.bit.GPIO105 = 0; /* Enable pull-up on GPIO105 (SCLA)*/
  GpioCtrlRegs.GPDGMUX1.bit.GPIO105 = 0;/* Configure GPIO105 as SCLA*/
  GpioCtrlRegs.GPDMUX1.bit.GPIO105 = 1;/* Configure GPIO105 as SCLA*/
  EDIS;
}

void init_I2C_A(void)
{
  /* Initialize I2C*/
  EALLOW;
  CpuSysRegs.PCLKCR9.bit.I2C_A = 1;    /* Enable pheripheral clocks for I2C */
  EDIS;
  I2caRegs.I2CMDR.bit.MST = 1;         /* Select master or slave mode*/
  I2caRegs.I2CMDR.bit.DLB = 0;         /* Enable digital loopback bit */
  I2caRegs.I2CPSC.all = 9;          /* Prescaler - need 7-12 Mhz on module clk*/
  I2caRegs.I2CCLKL = 20;               /* NOTE: must be non zero*/
  I2caRegs.I2CCLKH = 20;               /* NOTE: must be non zero*/
  I2caRegs.I2CFFTX.all |= 0x6000;      /* Enable TxFIFO mode*/
  I2caRegs.I2CFFRX.all |= 0x2000;      /* Enable RxFIFO mode*/
  I2caRegs.I2CMDR.bit.IRS = 1;         /* Take I2C out of reset*/
}

#include "c2000BoardSupport.h"
#include "F2837xD_device.h"
#include "F2837xD_Examples.h"
#include "F2837xD_GlobalPrototypes.h"
#include "rtwtypes.h"
#include "Simulink_example_CAN_Serial1.h"
#include "Simulink_example_CAN_Serial1_private.h"
#include "MW_c28xGPIO.h"

void configureIXbar(void)
{
  /*--- Configuring GPIO set in Input Xbar---*/
  EALLOW;
  InputXbarRegs.INPUT7SELECT = 20;
  InputXbarRegs.INPUT8SELECT = 21;
  InputXbarRegs.INPUT9SELECT = 99;
  InputXbarRegs.INPUT10SELECT = 54;
  InputXbarRegs.INPUT11SELECT = 55;
  InputXbarRegs.INPUT12SELECT = 57;
  EDIS;
}

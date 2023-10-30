#include "c2000BoardSupport.h"
#include "F2837xD_device.h"
#include "F2837xD_Examples.h"
#include "F2837xD_GlobalPrototypes.h"
#include "rtwtypes.h"
#include "Simulink_example_CAN_Serial1.h"
#include "Simulink_example_CAN_Serial1_private.h"

void config_ADCA_SOC0()
{
  EALLOW;
  AdcaRegs.ADCSOC0CTL.bit.CHSEL = 3;   /* Set SOC0 channel select to ADCIN3*/
  AdcaRegs.ADCSOC0CTL.bit.TRIGSEL = 5;
  AdcaRegs.ADCSOC0CTL.bit.ACQPS = 14.0;
                               /* Set SOC0 S/H Window to 15.0 ADC Clock Cycles*/
  AdcaRegs.ADCINTSEL1N2.bit.INT1E = 1; /* Enabled/Disable ADCINT1 interrupt*/
  AdcaRegs.ADCINTSEL1N2.bit.INT1SEL = 0;/* Setup EOC0 to trigger ADCINT1*/
  AdcaRegs.ADCINTSEL1N2.bit.INT1CONT = 1;
                                     /* Enable/Disable ADCINT1 Continuous mode*/
  AdcaRegs.ADCINTSOCSEL1.bit.SOC0 = 0;
                                   /* SOCx No ADCINT Interrupt Trigger Select.*/
  AdcaRegs.ADCOFFTRIM.bit.OFFTRIM = AdcaRegs.ADCOFFTRIM.bit.OFFTRIM;/* Set Offset Error Correctino Value*/
  AdcaRegs.ADCCTL1.bit.INTPULSEPOS = 1;
                                /* Late interrupt pulse trips AdcResults latch*/
  AdcaRegs.ADCSOCPRICTL.bit.SOCPRIORITY = 0;/* All in round robin mode SOC Priority*/
  EDIS;
}

void InitAdcA()
{
  EALLOW;
  CpuSysRegs.PCLKCR13.bit.ADC_A = 1;
  AdcaRegs.ADCCTL2.bit.PRESCALE = 8;
  AdcSetMode(ADC_ADCA, ADC_RESOLUTION_12BIT, ADC_SIGNALMODE_SINGLE);

  //power up the ADC
  AdcaRegs.ADCCTL1.bit.ADCPWDNZ = 1;

  //delay for 1ms to allow ADC time to power up
  DELAY_US(1000);
  EDIS;
}

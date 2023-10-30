/*
 * Academic License - for use in teaching, academic research, and meeting
 * course requirements at degree granting institutions only.  Not for
 * government, commercial, or other organizational use.
 *
 * File: ert_main.c
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
#include "rtwtypes.h"

volatile int IsrOverrun = 0;
boolean_T isRateRunning[4] = { 0, 0, 0, 0 };

boolean_T need2runFlags[4] = { 0, 0, 0, 0 };

void rt_OneStep(void)
{
  boolean_T eventFlags[4];
  int_T i;

  /* Check base rate for overrun */
  if (isRateRunning[0]++) {
    IsrOverrun = 1;
    isRateRunning[0]--;                /* allow future iterations to succeed*/
    return;
  }

  /*
   * For a bare-board target (i.e., no operating system), the rates
   * that execute this base step are buffered locally to allow for
   * overlapping preemption.  The generated code includes function
   * writeCodeInfoFcn() which sets the rates
   * that need to run this time step.  The return values are 1 and 0
   * for true and false, respectively.
   */
  Simulink_example_CAN_Serial1_SetEventsForThisBaseStep(eventFlags);
  enableTimer0Interrupt();
  Simulink_example_CAN_Serial1_step0();

  /* Get model outputs here */
  disableTimer0Interrupt();
  isRateRunning[0]--;
  for (i = 1; i < 4; i++) {
    if (eventFlags[i]) {
      if (need2runFlags[i]++) {
        IsrOverrun = 1;
        need2runFlags[i]--;            /* allow future iterations to succeed*/
        break;
      }
    }
  }

  for (i = 1; i < 4; i++) {
    if (isRateRunning[i]) {
      /* Yield to higher priority*/
      return;
    }

    if (need2runFlags[i]) {
      isRateRunning[i]++;
      enableTimer0Interrupt();

      /* Step the model for subrate "i" */
      switch (i)
      {
       case 1 :
        Simulink_example_CAN_Serial1_step1();

        /* Get model outputs here */
        break;

       case 2 :
        Simulink_example_CAN_Serial1_step2();

        /* Get model outputs here */
        break;

       case 3 :
        Simulink_example_CAN_Serial1_step3();

        /* Get model outputs here */
        break;

       default :
        break;
      }

      disableTimer0Interrupt();
      need2runFlags[i]--;
      isRateRunning[i]--;
    }
  }
}

volatile boolean_T stopRequested;
volatile boolean_T runModel;
int main(void)
{
  float modelBaseRate = 0.001;
  float systemClock = 200;

  /* Initialize variables */
  stopRequested = false;
  runModel = false;
  c2000_flash_init();
  init_board();

#ifdef MW_EXEC_PROFILER_ON

  config_profilerTimer();

#endif

  ;
  rtmSetErrorStatus(Simulink_example_CAN_Serial1_M, 0);
  Simulink_example_CAN_Serial1_initialize();
  globalInterruptDisable();
  configureTimer0(modelBaseRate, systemClock);
  runModel =
    rtmGetErrorStatus(Simulink_example_CAN_Serial1_M) == (NULL);
  enableTimer0Interrupt();
  globalInterruptEnable();
  while (runModel) {
    stopRequested = !(
                      rtmGetErrorStatus(Simulink_example_CAN_Serial1_M) == (NULL));
  }

  /* Disable rt_OneStep() here */

  /* Terminate model */
  Simulink_example_CAN_Serial1_terminate();
  globalInterruptDisable();
  return 0;
}

/*
 * File trailer for generated code.
 *
 * [EOF]
 */

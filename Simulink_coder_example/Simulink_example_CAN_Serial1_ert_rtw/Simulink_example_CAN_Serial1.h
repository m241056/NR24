/*
 * Academic License - for use in teaching, academic research, and meeting
 * course requirements at degree granting institutions only.  Not for
 * government, commercial, or other organizational use.
 *
 * File: Simulink_example_CAN_Serial1.h
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

#ifndef RTW_HEADER_Simulink_example_CAN_Serial1_h_
#define RTW_HEADER_Simulink_example_CAN_Serial1_h_
#include <string.h>
#include <stddef.h>
#ifndef Simulink_example_CAN_Serial1_COMMON_INCLUDES_
#define Simulink_example_CAN_Serial1_COMMON_INCLUDES_
#include <string.h>
#include "rtwtypes.h"
#include "c2000BoardSupport.h"
#include "F2837xD_device.h"
#include "F2837xD_Examples.h"
#include "IQmathLib.h"
#include "DSP28xx_SciUtil.h"
#include "can_message.h"
#include "F2837xD_gpio.h"
#include "MW_SPI.h"
#include "MW_c2000SPI.h"
#endif                       /* Simulink_example_CAN_Serial1_COMMON_INCLUDES_ */

#include "Simulink_example_CAN_Serial1_types.h"
#include "MW_target_hardware_resources.h"

/* Macros for accessing real-time model data structure */
#ifndef rtmGetErrorStatus
#define rtmGetErrorStatus(rtm)         ((rtm)->errorStatus)
#endif

#ifndef rtmSetErrorStatus
#define rtmSetErrorStatus(rtm, val)    ((rtm)->errorStatus = (val))
#endif

#ifndef rtmStepTask
#define rtmStepTask(rtm, idx)          ((rtm)->Timing.TaskCounters.TID[(idx)] == 0)
#endif

#ifndef rtmTaskCounter
#define rtmTaskCounter(rtm, idx)       ((rtm)->Timing.TaskCounters.TID[(idx)])
#endif

extern void init_eCAN_B ( uint16_T bitRatePrescaler, uint16_T timeSeg1, uint16_T
  timeSeg2, uint16_T sbg, uint16_T sjw, uint16_T sam);
extern void init_SCI(void);
extern void init_SCI_GPIO(void);
extern void config_ePWM_GPIO (void);
extern void config_ePWM_XBAR(void);
extern void configureIXbar(void);
extern void init_I2C_GPIO(void);
extern void init_I2C_A(void);

/* user code (top of export header file) */
#include "can_message.h"
#include "can_message.h"

/* Block signals (default storage) */
typedef struct {
  uint16_T ADC;                        /* '<Root>/ADC' */
  uint16_T eCANReceive_o2[4];          /* '<Root>/eCAN Receive' */
  uint16_T SCIReceive;                 /* '<Root>/SCI Receive' */
  int16_T I2CReceive;                  /* '<Root>/I2C Receive' */
  boolean_T LogicalOperator3;          /* '<Root>/Logical Operator3' */
} B_Simulink_example_CAN_Serial_T;

/* Block states (default storage) for system '<Root>' */
typedef struct {
  codertarget_tic2000_blocks_SP_T obj; /* '<Root>/SPI Receive' */
  codertarget_tic2000_blocks__f_T obj_p;/* '<Root>/SPI Transmit' */
} DW_Simulink_example_CAN_Seria_T;

/* Parameters (default storage) */
struct P_Simulink_example_CAN_Serial_T_ {
  real_T SPIReceive_SampleTime;        /* Expression: 0.1
                                        * Referenced by: '<Root>/SPI Receive'
                                        */
  uint16_T src2_Value;                 /* Computed Parameter: src2_Value
                                        * Referenced by: '<Root>/src2'
                                        */
  uint16_T u_Value;                    /* Computed Parameter: u_Value
                                        * Referenced by: '<Root>/ 1'
                                        */
  uint16_T u_Value_d;                  /* Computed Parameter: u_Value_d
                                        * Referenced by: '<Root>/ 2'
                                        */
  uint16_T u_Value_n;                  /* Computed Parameter: u_Value_n
                                        * Referenced by: '<Root>/ 3'
                                        */
  uint16_T u_Value_e;                  /* Computed Parameter: u_Value_e
                                        * Referenced by: '<Root>/ 4'
                                        */
  uint16_T u_Value_n1;                 /* Computed Parameter: u_Value_n1
                                        * Referenced by: '<Root>/ 5'
                                        */
  uint16_T _Value;                     /* Computed Parameter: _Value
                                        * Referenced by: '<Root>/ '
                                        */
  uint16_T _Value_a;                   /* Computed Parameter: _Value_a
                                        * Referenced by: '<Root>/   '
                                        */
  uint16_T Enable_Value;               /* Computed Parameter: Enable_Value
                                        * Referenced by: '<Root>/ Enable'
                                        */
  uint16_T ManualSwitch1_CurrentSetting;
                             /* Computed Parameter: ManualSwitch1_CurrentSetting
                              * Referenced by: '<Root>/Manual Switch1'
                              */
};

/* Real-time Model Data Structure */
struct tag_RTM_Simulink_example_CAN__T {
  const char_T *errorStatus;

  /*
   * Timing:
   * The following substructure contains information regarding
   * the timing information for the model.
   */
  struct {
    struct {
      uint16_T TID[4];
    } TaskCounters;
  } Timing;
};

/* Block parameters (default storage) */
extern P_Simulink_example_CAN_Serial_T Simulink_example_CAN_Serial1_P;

/* Block signals (default storage) */
extern B_Simulink_example_CAN_Serial_T Simulink_example_CAN_Serial1_B;

/* Block states (default storage) */
extern DW_Simulink_example_CAN_Seria_T Simulink_example_CAN_Serial1_DW;

/* External function called from main */
extern void Simulink_example_CAN_Serial1_SetEventsForThisBaseStep(boolean_T
  *eventFlags);

/* Model entry point functions */
extern void Simulink_example_CAN_Serial1_SetEventsForThisBaseStep(boolean_T
  *eventFlags);
extern void Simulink_example_CAN_Serial1_initialize(void);
extern void Simulink_example_CAN_Serial1_step0(void);
extern void Simulink_example_CAN_Serial1_step1(void);
extern void Simulink_example_CAN_Serial1_step2(void);
extern void Simulink_example_CAN_Serial1_step3(void);
extern void Simulink_example_CAN_Serial1_terminate(void);

/* Real-time Model object */
extern RT_MODEL_Simulink_example_CAN_T *const Simulink_example_CAN_Serial1_M;
extern volatile boolean_T stopRequested;
extern volatile boolean_T runModel;

/*-
 * These blocks were eliminated from the model due to optimizations:
 *
 * Block '<Root>/Ph_C_Voltage' : Unused code path elimination
 */

/*-
 * The generated code includes comments that allow you to trace directly
 * back to the appropriate location in the model.  The basic format
 * is <system>/block_name, where system is the system number (uniquely
 * assigned by Simulink) and block_name is the name of the block.
 *
 * Use the MATLAB hilite_system command to trace the generated code back
 * to the model.  For example,
 *
 * hilite_system('<S3>')    - opens system 3
 * hilite_system('<S3>/Kp') - opens and selects block Kp which resides in S3
 *
 * Here is the system hierarchy for this model
 *
 * '<Root>' : 'Simulink_example_CAN_Serial1'
 */
#endif                          /* RTW_HEADER_Simulink_example_CAN_Serial1_h_ */

/*
 * File trailer for generated code.
 *
 * [EOF]
 */

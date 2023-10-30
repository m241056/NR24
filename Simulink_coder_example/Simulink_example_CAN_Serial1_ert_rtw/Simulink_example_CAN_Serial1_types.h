/*
 * Academic License - for use in teaching, academic research, and meeting
 * course requirements at degree granting institutions only.  Not for
 * government, commercial, or other organizational use.
 *
 * File: Simulink_example_CAN_Serial1_types.h
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

#ifndef RTW_HEADER_Simulink_example_CAN_Serial1_types_h_
#define RTW_HEADER_Simulink_example_CAN_Serial1_types_h_
#include "rtwtypes.h"

/* Model Code Variants */

/* Custom Type definition for MATLABSystem: '<Root>/SPI Transmit' */
#include "MW_SVD.h"
#ifndef struct_tag_ZScr3MJtZlmDJ2etniUAv
#define struct_tag_ZScr3MJtZlmDJ2etniUAv

struct tag_ZScr3MJtZlmDJ2etniUAv
{
  boolean_T matlabCodegenIsDeleted;
  int32_T isInitialized;
  boolean_T isSetupComplete;
  MW_Handle_Type MW_SPI_HANDLE;
  real_T SampleTime;
};

#endif                                 /* struct_tag_ZScr3MJtZlmDJ2etniUAv */

#ifndef typedef_codertarget_tic2000_blocks_SP_T
#define typedef_codertarget_tic2000_blocks_SP_T

typedef struct tag_ZScr3MJtZlmDJ2etniUAv codertarget_tic2000_blocks_SP_T;

#endif                             /* typedef_codertarget_tic2000_blocks_SP_T */

#ifndef struct_tag_olsjGCr0ajrmZ8ELSGk4HH
#define struct_tag_olsjGCr0ajrmZ8ELSGk4HH

struct tag_olsjGCr0ajrmZ8ELSGk4HH
{
  boolean_T matlabCodegenIsDeleted;
  int32_T isInitialized;
  boolean_T isSetupComplete;
  MW_Handle_Type MW_SPI_HANDLE;
};

#endif                                 /* struct_tag_olsjGCr0ajrmZ8ELSGk4HH */

#ifndef typedef_codertarget_tic2000_blocks__f_T
#define typedef_codertarget_tic2000_blocks__f_T

typedef struct tag_olsjGCr0ajrmZ8ELSGk4HH codertarget_tic2000_blocks__f_T;

#endif                             /* typedef_codertarget_tic2000_blocks__f_T */

/* Parameters (default storage) */
typedef struct P_Simulink_example_CAN_Serial_T_ P_Simulink_example_CAN_Serial_T;

/* Forward declaration for rtModel */
typedef struct tag_RTM_Simulink_example_CAN__T RT_MODEL_Simulink_example_CAN_T;

#endif                    /* RTW_HEADER_Simulink_example_CAN_Serial1_types_h_ */

/*
 * File trailer for generated code.
 *
 * [EOF]
 */

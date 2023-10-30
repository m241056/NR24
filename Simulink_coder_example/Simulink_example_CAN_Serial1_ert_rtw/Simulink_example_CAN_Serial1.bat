
cd .

if "%1"=="" ("C:\PROGRA~1\MATLAB\R2021a\bin\win64\gmake"  -B DEPRULES=0 -j3  -f Simulink_example_CAN_Serial1.mk all) else ("C:\PROGRA~1\MATLAB\R2021a\bin\win64\gmake"  -B DEPRULES=0 -j3  -f Simulink_example_CAN_Serial1.mk %1)
@if errorlevel 1 goto error_exit

exit /B 0

:error_exit
echo The make command returned an error of %errorlevel%
exit /B 1
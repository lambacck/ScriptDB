@echo off

REM
REM Copyright 2006 Jesse Hersch
REM
REM Permission to use, copy, modify, and distribute this software
REM and its documentation for any purpose is hereby granted without fee,
REM provided that the above copyright notice appears in all copies and that
REM both that copyright notice and this permission notice appear in
REM supporting documentation, and that the name of Jesse Hersch or
REM Elsasoft LLC not be used in advertising or publicity
REM pertaining to distribution of the software without specific, written
REM prior permission.  Jesse Hersch and Elsasoft LLC make no
REM representations about the suitability of this software for any
REM purpose.  It is provided "as is" without express or implied warranty.
REM
REM Jesse Hersch and Elsasoft LLC disclaim all warranties with
REM regard to this software, including all implied warranties of
REM merchantability and fitness, in no event shall Jesse Hersch or
REM Elsasoft LLC be liable for any special, indirect or
REM consequential damages or any damages whatsoever resulting from loss of
REM use, data or profits, whether in an action of contract, negligence or
REM other tortious action, arising out of or in connection with the use or
REM performance of this software.
REM
REM Author:
REM  Jesse Hersch
REM  Elsasoft LLC
REM 

set script_directory=%1

if "%script_directory%"=="" set root=%CD%

IF NOT EXIST "%script_directory%" echo Directory %script_directory% not found.  exiting. & goto Done

call :BuildDirectory "%script_directory%\Schemas\*.sql" || goto Done
call :BuildDirectory "%script_directory%\Programmability\Assemblies\*.sql" || goto Done
call :BuildDirectory "%script_directory%\Programmability\Assemblies\*.sql" || goto Done
call :BuildDirectory "%script_directory%\Programmability\Types\*.sql" || goto Done
call :BuildDirectory "%script_directory%\Programmability\Defaults\*.sql" || goto Done
call :BuildDirectory "%script_directory%\Programmability\Rules\*.sql" || goto Done
call :BuildDirectory "%script_directory%\Tables\*.sql" || goto Done
call :BuildDirectory "%script_directory%\Programmability\Triggers\*.sql" || goto Done
call :BuildDirectory "%script_directory%\Programmability\Functions\*.sql" || goto Done
call :BuildDirectory "%script_directory%\Views\*.sql" || goto Done
call :BuildDirectory "%script_directory%\Tables\Constraints\*.sql" || goto Done
call :BuildDirectory "%script_directory%\Tables\PrimaryKeys\*.sql" || goto Done
call :BuildDirectory "%script_directory%\Tables\UniqueKeys\*.sql" || goto Done
call :BuildDirectory "%script_directory%\Tables\ForeignKeys\*.sql" || goto Done
call :BuildDirectory "%script_directory%\Tables\Indexes\*.sql" || goto Done
call :BuildDirectory "%script_directory%\Tables\FullTextIndexes\*.sql" || goto Done
call :BuildDirectory "%script_directory%\Programmability\StoredProcedures\*.sql" || goto Done
goto Success

:BuildFile
ECHO -- executing: %1
REM sqlcmd.exe -b -i %1 -S %dbserver% -d %db% %auth% 	
type %1
IF ERRORLEVEL 1 ECHO failed to execute script %1
goto :EOF

:BuildDirectory
FOR %%i in ("%1") do call :BuildFile %%i || goto Done
goto :EOF

:Success
echo -- ==================================
echo -- all scripts executed successfully!
echo -- ==================================
goto Done

:Done

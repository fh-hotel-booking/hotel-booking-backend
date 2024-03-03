@echo off

IF [%1] EQU [] (
	echo USAGE: 
	EXIT /B 1
)

for /f "delims=" %%i in ('where podman') do set program=%%i
IF EXIST "%program%" (
  podman run -it --rm --network hotel-booking-backend_app-tier bitnami/kafka:latest %*
)
for /f "delims=" %%i in ('where docker') do set program=%%i
IF EXIST "%program%" (
  docker run -it --rm --network hotel-booking-backend_app-tier bitnami/kafka:latest %*
}

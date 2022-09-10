rem scp -r bin/Publish/Linux/ARM/ pi@192.168.1.11:~/Downloads/IDO_Server
rem scp -r bin/Publish/Linux/ARM/ pi@pi.hole:~/Downloads/IDO_Server
scp -r bin/Release/net6.0/linux-arm linaro@192.168.1.11:~/Downloads/IDO_Server
rem scp -r bin/Publish/Linux/ARM/Programs linaro@tinkerboard.lan:~/Downloads/ARM
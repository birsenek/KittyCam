# KittyCam

As you all know I'm cat caretaker. Recently one of our cats has became gravely sick and needed to be monitored constantly.

KittyCam is a .net application designed to stream and record video, likely from an IP camera, with a focus on monitoring and saving recordings. Built with .NET Framework, it provides a user-friendly interface for interacting with camera feeds and managing media files.

You'll need to create an environment variable called KITTY_CAM_PASS to store the cam password.
Many cameras use the RTSP (Real-Time Streaming Protocol), wich is a protocol designed or managing and delivering real-time media stream over networks.
Knowing this we can connect to it using the connection string: rtsp://username:password@camera-ip:554/stream1.

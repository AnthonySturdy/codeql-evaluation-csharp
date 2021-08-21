# 2D Multiplayer Racing Game
This project is a chat room application (Similar to Skype or Discord), made using C# and WinForms. The other half of the application is a 2D multiplayer racing game, which uses the chat application to send game invites and announce the winner of the game to other users. The objective of the game is to complete three laps before your opponent does.

There were a few challenges developing the game, as it was my first time using MonoGame and the first time I had created a multiplayer application. The biggest challenge was making sure the correct client recieves the correct information for the game (such as current checkpoint), and that there is persistance between the two clients (such as who is red the car and who is the blue car).

<img src="https://anthonysturdy.co.uk/images/projects/RacingMP_1.png">

### Features
<li>Race Car Controller</li>
<li>Radius Collision</li>
<li>Two player multiplayer game</li>
<li>TCP and UDP Packet Sending and Recieving</li>
<li>Client username</li>
<li>Profile Picture</li>
<li>Message Sending</li>
<li>Direct Messaging</li>
<li>Image Sending</li>
<li>Client List</li>

### What I Learned
Developing this application taught me about using C# sockets, networking and using threads for concurrency. I also learned about using MonoGame to create and render a game, and learned how to use Forms to create an application with GUI.

sphero-dev-kit
==============

A tiny GUI for interacting with Sphero from Windows.

 * Turn on Bluetooth on your development machine.
 * Run the app.
 * If your Sphero is not in the list, hit "Find Devices" to re-scan.
  * Note your Sphero must be awake before it can be connected.
 * Double-click your Sphero from the list to connect.
 * The background will turn blue on a successful connection.
 * Once connected, you can:
  * Enter color values in RGB bytes and click "Set Color" to update the Sphero's color.
  * Enter orbBasic code in the textbox on the right, and hit "Run!" to run that code.
    * A default example program will load in the textbox initially; this will make the Sphero move around and change color at random.
  * Load files from the list on the left by double-clicking them.
  * Save files into the default code directory by entering a name and hitting "Save Code".
  * Hit "Abort" to abort the currently-running program.

using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Timers;
using System.Text;

public class PingLogger
{
    private string _remoteHostAddress;
    private Timer _pingTimer;
    private Ping _ping;
    private StreamWriter _logFileWriter;

    public PingLogger(string remoteHostAddress)
    {
        this._remoteHostAddress = remoteHostAddress;

        // Configure a Timer for use
        this._pingTimer = new Timer();
        this._pingTimer.Interval = 300000;
        this._pingTimer.Elapsed += new ElapsedEventHandler(this.TimeElapsed);
        this._pingTimer.Enabled = true;

        this._ping = new Ping();

        // Gain write access to serverPingStatus.txt
        FileStream fileStream = File.Open("serverPingStatus.txt",
                FileMode.Append, FileAccess.Write);
        this._logFileWriter = new StreamWriter(fileStream);

    } // end public PingLogger()

    private void PingRemoteHost()
    {
        // Print the time that we try to ping the remote address
        this._logFileWriter.Write("[");
        this._logFileWriter.Write(System.DateTime.Now.ToString());
        this._logFileWriter.Write("] ");
        try
        {
            PingReply reply = this._ping.Send(this._remoteHostAddress, 3000);

            if (reply.Status == IPStatus.Success)
            {
                this._logFileWriter.Write("Successful ICMP response from ");
                this._logFileWriter.Write(this._remoteHostAddress);
                this._logFileWriter.Write(". Round Trip Time: ");
                this._logFileWriter.Write(reply.RoundtripTime);
                this._logFileWriter.Write(" milliseconds.");
            }
            else
            {
                this._logFileWriter.Write("Unsuccessful ICMP response from ");
                this._logFileWriter.Write(this._remoteHostAddress);
                this._logFileWriter.Write("Status of ICMP response: ");
                this._logFileWriter.Write(reply.Status);

            } // end if
        }
        catch (Exception ex)
        {
            this._logFileWriter.Write("Encountered problem while pinging ");
            this._logFileWriter.Write(this._remoteHostAddress);
            this._logFileWriter.Write(". Error message: ");
            this._logFileWriter.Write(ex.Message);
        } // end try-catch

        this._logFileWriter.WriteLine();
        this._logFileWriter.Flush();

    } // end private void PingRemoteHost()

    private void TimeElapsed(Object sender, ElapsedEventArgs eventArgs)
    {
        PingRemoteHost();
    } // end private void TimeElapsed()

} // end public class PingLogger
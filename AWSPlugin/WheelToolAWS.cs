using Loupedeck;
using Loupedeck.Devices.Loupedeck2Devices;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;

public class WheelToolAWS : WheelToolGeneric
{
	private Boolean _toggleState = false;
    private CancellationTokenSource cts;
    public WheelToolAWS()
		: base("Amazon", "Tools", 1)
	{
    }

    protected void StartThread()
    {
        this.cts = new CancellationTokenSource();
        ThreadPool.QueueUserWorkItem(new WaitCallback(this.ThreadChange), cts.Token);
    }

protected void CancelThread()
    {
        // Request cancellation.
        this.cts.Cancel();
        Console.WriteLine("Cancellation set in token source...");
        Thread.Sleep(2500);
        // Cancellation should have happened, so call Dispose.
        this.cts.Dispose();
    }

    public void ThreadChange(object obj)
    {
        CancellationToken token = (CancellationToken)obj;
        while (true)
        {
            if (token.IsCancellationRequested)
            {
                Console.WriteLine("Cancellation has been requested...");
                break;
            }

            this._toggleState = !this._toggleState;
            this.Draw();
            try
            {

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }
            Thread.Sleep(4000);
        }
    }

    protected override void OnStart()
    {
        base.OnStart();
        this.StartThread();
    }

    protected override void OnStop()
    {
        base.OnStop();
        this.CancelThread();
    }

    protected override void OnResume()
    {
        base.OnResume();
        this.StartThread();
    }

    protected override void OnPause()
    {
        base.OnPause();
        this.CancelThread();
    }

    protected override void OnEncoderEvent(DeviceEncoderEvent deviceEncoderEvent)
    {
        base.OnEncoderEvent(deviceEncoderEvent);
        Console.WriteLine("Encoder event.");
    }

    protected override void OnTouchEvent(DeviceTouchEvent deviceTouchEvent)
    {
        base.OnTouchEvent(deviceTouchEvent);
        Console.WriteLine("Touch event.");
    }

    protected override BitmapImage CreateImage()
    {
        Console.WriteLine("Image updated.");
        if (this._toggleState)
        {
            using (var bitmapBuilder = new BitmapBuilder(240, 240))
            {
                bitmapBuilder.FillCircle(120, 120, 120, BitmapColor.White);
                return bitmapBuilder.ToImage();
            }
        }
        else
        {
            using (var bitmapBuilder = new BitmapBuilder(240, 240))
            {
                bitmapBuilder.FillCircle(120, 120, 60, BitmapColor.White);
                return bitmapBuilder.ToImage();
            }
        }
    }

    protected override BitmapImage CreateDemoImage()
    {
        Console.WriteLine("Demo image updated.");
        if (this._toggleState)
        {
            using (var bitmapBuilder = new BitmapBuilder(240, 240))
            {
                bitmapBuilder.FillCircle(120, 120, 120, BitmapColor.White);
                return bitmapBuilder.ToImage();
            }
        }
        else
        {
            using (var bitmapBuilder = new BitmapBuilder(240, 240))
            {
                bitmapBuilder.FillCircle(120, 120, 60, BitmapColor.White);
                return bitmapBuilder.ToImage();
            }
        }
    }
}

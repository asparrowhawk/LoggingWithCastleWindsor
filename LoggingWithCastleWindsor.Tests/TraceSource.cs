using System;
using System.Diagnostics;

class Program
{
    static void ShowAllTraceMethods(TraceSource trc)
    {
        // The simplest method. Notice it takes parameters, which is a 
        // big improvement over the Trace object. 
        trc.TraceInformation("The simplest trace method with {0}{1}", "params", "!");
        
        // The method to trace an event based on the TraceEventType enum.
        trc.TraceEvent(TraceEventType.Error, 100, "Pretend error!");
        
        // The method to make dumping out data easy.
        trc.TraceData(TraceEventType.Information, 50, "Some", "pretend", "data.");
        
        // The method to record a transfer. This method is primarily for 
        // the XmlWriterTraceListener. See the column text for more 
        // discussion. 
        trc.TraceTransfer(75, "What was transferred?", new Guid("7b5fcdbc-913e-43bd-8e39-ee13c062ecc3"));
    }

    static void Main(string[] args)
    {
        // Create the TraceSource for this program. Like the Trace 
        // object, the TraceSource.Listeners collection starts out with 
        // the DefaultTraceListener. 
        TraceSource trc = new TraceSource("HappySource");
        
        // Set the switch level for this TraceSource instance so 
        // everything is shown. The default for TraceSource is to *not* 
        // trace. The default name of the switch is the same as the 
        // TraceSource. You'll probably want to be sharing Switches across 
        // TraceSources in your development.
        trc.Switch.Level = SourceLevels.All; 
        
        // Trace to show the default output. 
        ShowAllTraceMethods(trc);
        
        // The TraceListener class has a very interesting new property, 
        // TraceOutputOptions, which tells the TraceListener the additional
        // data to automatically display. 
        trc.Listeners["Default"].TraceOutputOptions = TraceOptions.Callstack | TraceOptions.DateTime | TraceOptions.ProcessId | TraceOptions.ThreadId | TraceOptions.Timestamp;
        
        // Now all the trace calls in the Debug Output window will show 
        // all the data included in the TraceOutputOptions.
        ShowAllTraceMethods(trc);
        
        // Filtering allows you to apply a limiter to the TraceListener 
        // directly. That way you can turn on tracing, but apply more 
        // smarts to the actual output so you can better separate the 
        // wheat from the chaff on a production system. 
        EventTypeFilter evtFilt = new EventTypeFilter(SourceLevels.Error);
        
        // Apply the filter to the DefaultTraceListener.
        trc.Listeners["Default"].Filter = evtFilt;
        
        // The only output in the Debug Output window will be from the 
        // TraceEvent method call in ShowAllTraceMethods. 
        ShowAllTraceMethods(trc); 
        
        trc.Flush(); 
        
        trc.Close();
    }
}
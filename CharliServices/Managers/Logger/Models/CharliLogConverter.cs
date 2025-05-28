using log4net.Core;
using log4net.Util;

namespace McpNetwork.Charli.Server.Managers.Logger.Models
{
    public class CharliLogConverter : PatternConverter
    {
        public override void Convert(TextWriter writer, object? state)
        {
            if (state == null)
            {
                writer.Write(SystemInfo.NullText);
                return;
            }

            var loggingEvent = (LoggingEvent)state;
            var requestInfo = loggingEvent.MessageObject;

            if (requestInfo == null)
            {
                writer.Write(SystemInfo.NullText);
            }
            else
            {
                if (this.Option != null)
                {
                    var members = requestInfo.GetType().GetProperty(this.Option);
                    if (members == null)
                    {
                        if (System.Diagnostics.Debugger.IsAttached)
                        {
                            System.Diagnostics.Debugger.Break();
                        }
                        var message = string.Format("Trying to get property {0} of object {1} which does not exists", this.Option, requestInfo.GetType().Name);
                        System.Diagnostics.Debug.WriteLine(message);
                        writer.Write(SystemInfo.NullText);
                    }
                    else
                    {
                        var value = members.GetValue(requestInfo);
                        if (value != null)
                        {
                            if (value.ToString() == string.Empty)
                            {
                                value = null;
                            }
                            else
                            {
                                value = value.ToString().Replace(System.Environment.NewLine, "");
                            }
                        }
                        writer.Write(value);
                    }
                }
                else
                {
                    writer.Write(SystemInfo.NullText);
                }
            }
        }
    }
}

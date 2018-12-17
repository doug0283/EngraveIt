using System;
using TcHmiSrv;
using TcHmiSrv.Management;

namespace EngraveItServer
{
    // The default class of the server extension must inherit from the IExtension interface.
    // The path of the server extension is available via the ExtensionPath property if the default class of the server extension inherits from the ExtensionWithPath base class instead.
    public class EngraveItServer : IExtension
    {
        private ITcHmiSrvExtHost _host = null;

        private object _shutdownLock = new object();
        private bool _isShuttingDown = false;

        private TcHmiSrvRequestListener _requestListener = new TcHmiSrvRequestListener();
        private TcHmiSrvShutdownListener _shutdownListener = new TcHmiSrvShutdownListener();

        private Log _logger = null;
        private Data _data = new Data();

        private Random _rand = new Random("EngraveItServer".GetHashCode());

        // Initializes the server extension.
        public ErrorValue Init(ITcHmiSrvExtHost host, Context context)
        {
            _logger = new Log(host);

            try
            {
                _host = host;

                // Add event handlers
                _requestListener.OnRequest += OnRequest;
                _shutdownListener.OnShutdown += OnShutdown;

                // Register listeners
                _host.register_listener(context, _requestListener);
                _host.register_listener(context, _shutdownListener);

                _logger.Send(context, "MESSAGE_INIT");
                return ErrorValue.HMI_SUCCESS;
            }
            catch (Exception ex)
            {
                _logger.Send(context, "ERROR_INIT", ex.Message, Severity.Severity_Error);
                return ErrorValue.HMI_E_EXTENSION_LOAD;
            }
        }

        // Called when a client requests a symbol from the extension domain.
        private ErrorValue OnRequest(object sender, TcHmiSrvRequestListener.OnRequestEventArgs e)
        {
            ErrorValue ret = ErrorValue.HMI_SUCCESS;
            Context context = e.Context;
            CommandGroup commands = e.Commands;

            try
            {
                commands.Result = ExtensionErrorValue.HMI_EXT_SUCCESS;
                string mapping = "";

                foreach (Command command in commands)
                {
                    mapping = command.Mapping;

                    try
                    {
                        // Use the mapping to check which command is requested
                        switch (mapping)
                        {
                            case "RandomValue":
                                ret = RandomValue(command);
                                break;

                            case "MaxRandom":
                                ret = MaxRandom(command);
                                break;

                            default:
                                ret = ErrorValue.HMI_E_EXTENSION;
                                break;
                        }

                        //if (ret != ErrorValue.HMI_SUCCESS)
                        // Do something on error
                    }
                    catch (Exception ex)
                    {
                        command.ExtensionResult = ExtensionErrorValue.HMI_EXT_FAIL;
                        command.ResultString = _logger.Localize(context, "ERROR_CALL_COMMAND", new string[] { mapping, ex.Message });
                    }
                }
            }
            catch
            {
                commands.Result = ExtensionErrorValue.HMI_EXT_FAIL;
            }
            finally
            {
                if (commands.Result != ExtensionErrorValue.HMI_EXT_SUCCESS)
                {
                    // Reset the read value of the commands to prevent the server from sending invalid data
                    foreach (Command command in commands)
                    {
                        command.ReadValue = null;
                    }

                    ret = ErrorValue.HMI_E_EXTENSION;
                }
            }

            return ret;
        }

        // Called when the server is shutting down. After exiting this function the extension dll will be unloaded.
        private ErrorValue OnShutdown(object sender, TcHmiSrvShutdownListener.OnShutdownEventArgs e)
        {
            // If the extension does not shutdown after 10 seconds (blocking threads) OnShutdown will be called again
            lock (_shutdownLock)
            {
                if (_isShuttingDown)
                {
                    return ErrorValue.HMI_SUCCESS;
                }

                _isShuttingDown = true;

                Context context = e.Context;

                try
                {
                    // Unregister listeners
                    _host.unregister_listener(context, _requestListener);
                    _host.unregister_listener(context, _shutdownListener);

                    _logger.Send(context, "MESSAGE_SHUTDOWN");
                    return ErrorValue.HMI_SUCCESS;
                }
                catch (Exception ex)
                {
                    _logger.Send(context, "ERROR_SHUTDOWN", ex.Message, Severity.Severity_Error);
                    return ErrorValue.HMI_E_EXTENSION;
                }
            }
        }

        // Generates a random number and writes it to the read value of the given command.
        private ErrorValue RandomValue(Command command)
        {
            command.ReadValue = _rand.Next(_data.MaxRandom) + 1;

            command.ExtensionResult = ExtensionErrorValue.HMI_EXT_SUCCESS;
            return ErrorValue.HMI_SUCCESS;
        }

        // Gets or sets the maximum random value.
        private ErrorValue MaxRandom(Command command)
        {
            if (command.WriteValue.IsSet && command.WriteValue.Type == TcHmiSrv.ValueType.ValueType_Int)
            {
                _data.MaxRandom = command.WriteValue;
            }

            command.ReadValue = _data.MaxRandom;

            command.ExtensionResult = ExtensionErrorValue.HMI_EXT_SUCCESS;
            return ErrorValue.HMI_SUCCESS;
        }
    }
}

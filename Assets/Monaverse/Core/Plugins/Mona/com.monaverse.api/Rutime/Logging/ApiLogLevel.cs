namespace Monaverse.Api.Logging
{
    public enum ApiLogLevel
    {
        /// <summary>
        /// Do not use it in logging. Only in config to disable logging.
        /// </summary>
        Off,
        /// <summary>For errors that must be shown in Exception Browser</summary>
        Error,
        /// <summary>Suspicious situations but not errors</summary>
        Warn,
        /// <summary>Regular level for important events</summary>
        Info,
    }
}
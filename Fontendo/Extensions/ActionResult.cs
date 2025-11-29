public class ActionResult
{
    public bool Success { get; set; }
    public string Message { get; set; }

    public ActionResult(bool success, string message)
    {
        Message = message;
        Success = success;
    }
}
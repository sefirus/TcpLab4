using Newtonsoft.Json;

namespace Core.Helpers;

public static class JsonHelper
{
    public static TObject? ReadObject<TObject>(string? filePath, bool needToThrow = false)
    {
        try
        {
            if (filePath is null || !File.Exists(filePath))
            {
                throw new Exception("File does not exist or path is null!");
            }

            var readText = File.ReadAllText(filePath);
            if (readText is null)
            {
                throw new Exception("File is empty!");
            }

            var obj = JsonConvert.DeserializeObject<TObject>(readText)
                      ?? throw new Exception("Cant deserialize an object!");
            return obj;
        }
        catch (Exception e)
        {
            if (needToThrow)
            {
                throw new InvalidOperationException("File you provided can`t be accessed!", e);
            }

            return default;
        }
    }
}
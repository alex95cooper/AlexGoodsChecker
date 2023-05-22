namespace AlexGoodsChecker;

public static class DotNetEnv
{
    public static void Load(string filePath)
    {
        if (File.Exists(filePath))
        {
            foreach (var line in File.ReadAllLines(filePath))
            {
                string[] variableParts = line.Split('=', StringSplitOptions.RemoveEmptyEntries);
                if (variableParts.Length == 2)
                {
                    Environment.SetEnvironmentVariable(variableParts[0], variableParts[1]);
                }
            }
        }
    }
}
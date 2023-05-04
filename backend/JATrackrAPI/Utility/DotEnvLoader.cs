namespace JATrackrAPI.Utility;
/*
 * Utility class to load env vars from a .env file and inject them into current environment
 * Source: https://dusted.codes/dotenv-in-dotnet
 */
public static class DotEnvLoader
{
    public static void Load(string filePath)
    {
        if (!File.Exists(filePath))
            return;

        foreach (var line in File.ReadAllLines(filePath))
        {
            var parts = line.Split('=', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
                continue;
            
            // key = value
            Environment.SetEnvironmentVariable(parts[0], parts[1]);
        }
    }
}
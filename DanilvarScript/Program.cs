if (args.Length > 1)
{
    Console.WriteLine("Usage: Danilvar [script]");
    Environment.Exit(64);
}
else if (args.Length == 1)
{
    DVScript.RunFile(args[0]);
}
else
{
    DVScript.RunPrompt();
}

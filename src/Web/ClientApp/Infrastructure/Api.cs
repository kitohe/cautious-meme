namespace ClientApp.Infrastructure
{
    public static class Api
    {
        public static class Messaging
        {
            public static string AdminTestApiEndpoint(string baseUri) => $"{baseUri}/TestApi";
        }
    }
}
